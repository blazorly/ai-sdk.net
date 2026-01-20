using System.Diagnostics;
using System.Text;
using AiSdk.Core.Http;
using AiSdk.Core.Streaming;
using FluentAssertions;
using Xunit;

namespace AiSdk.Core.Tests;

/// <summary>
/// Integration tests combining SSE parsing with JSON serialization.
/// </summary>
public class IntegrationTests
{
    #region Test Models

    private record ChatCompletionChunk(
        string Id,
        string Object,
        long Created,
        string Model,
        List<ChunkChoice> Choices
    );

    private record ChunkChoice(
        int Index,
        ChunkDelta Delta,
        string? FinishReason = null
    );

    private record ChunkDelta(
        string? Role = null,
        string? Content = null
    );

    private record StreamEvent(
        string Type,
        string? MessageId = null,
        string? Text = null,
        object? Data = null
    );

    #endregion

    #region SSE with JSON Payload Tests

    [Fact]
    public async Task SseWithJson_SingleEvent_ShouldParseAndDeserialize()
    {
        var input = """
            data: {"id":"chatcmpl-123","object":"chat.completion.chunk","created":1234567890,"model":"gpt-4","choices":[{"index":0,"delta":{"content":"Hello"}}]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(1);

        var chunk = SafeJsonSerializer.Deserialize<ChatCompletionChunk>(events[0].Data);
        chunk.Id.Should().Be("chatcmpl-123");
        chunk.Choices.Should().HaveCount(1);
        chunk.Choices[0].Delta.Content.Should().Be("Hello");
    }

    [Fact]
    public async Task SseWithJson_MultipleEvents_ShouldParseAndDeserializeAll()
    {
        var input = """
            data: {"id":"msg-1","object":"chat.completion.chunk","created":1234567890,"model":"gpt-4","choices":[{"index":0,"delta":{"content":"Hello"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1234567890,"model":"gpt-4","choices":[{"index":0,"delta":{"content":" world"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1234567890,"model":"gpt-4","choices":[{"index":0,"delta":{},"finishReason":"stop"}]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(3);

        var chunks = events.Select(e => SafeJsonSerializer.Deserialize<ChatCompletionChunk>(e.Data)).ToList();
        chunks[0].Choices[0].Delta.Content.Should().Be("Hello");
        chunks[1].Choices[0].Delta.Content.Should().Be(" world");
        chunks[2].Choices[0].FinishReason.Should().Be("stop");
    }

    [Fact]
    public async Task SseWithJson_WithEventTypes_ShouldParseCorrectly()
    {
        var input = """
            event: message_start
            data: {"type":"message_start","messageId":"msg_001"}

            event: content_delta
            data: {"type":"content_delta","text":"Hello"}

            event: message_stop
            data: {"type":"message_stop"}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(3);

        var startEvent = SafeJsonSerializer.Deserialize<StreamEvent>(events[0].Data);
        startEvent.Type.Should().Be("message_start");
        startEvent.MessageId.Should().Be("msg_001");

        var deltaEvent = SafeJsonSerializer.Deserialize<StreamEvent>(events[1].Data);
        deltaEvent.Type.Should().Be("content_delta");
        deltaEvent.Text.Should().Be("Hello");

        var stopEvent = SafeJsonSerializer.Deserialize<StreamEvent>(events[2].Data);
        stopEvent.Type.Should().Be("message_stop");
    }

    [Fact]
    public async Task SseWithJson_WithNestedObjects_ShouldDeserializeCorrectly()
    {
        var input = """
            data: {"id":"1","object":"chunk","created":1000,"model":"test","choices":[{"index":0,"delta":{"role":"assistant","content":"Hi"}},{"index":1,"delta":{"role":"assistant","content":"Hello"}}]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(1);

        var chunk = SafeJsonSerializer.Deserialize<ChatCompletionChunk>(events[0].Data);
        chunk.Choices.Should().HaveCount(2);
        chunk.Choices[0].Delta.Role.Should().Be("assistant");
        chunk.Choices[0].Delta.Content.Should().Be("Hi");
        chunk.Choices[1].Delta.Content.Should().Be("Hello");
    }

    #endregion

    #region Streaming Pipeline Tests

    [Fact]
    public async Task StreamingPipeline_BuildCompletionFromChunks_ShouldCombineCorrectly()
    {
        var input = """
            data: {"id":"msg-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"role":"assistant"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":"The"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":" quick"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":" brown"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":" fox"}}]}

            data: {"id":"msg-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{},"finishReason":"stop"}]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        var chunks = events.Select(e => SafeJsonSerializer.Deserialize<ChatCompletionChunk>(e.Data)).ToList();

        var fullContent = string.Concat(chunks
            .Select(c => c.Choices.FirstOrDefault()?.Delta.Content ?? ""));

        fullContent.Should().Be("The quick brown fox");
        chunks.Last().Choices[0].FinishReason.Should().Be("stop");
    }

    [Fact]
    public async Task StreamingPipeline_WithKeepAliveComments_ShouldIgnoreAndContinue()
    {
        var input = """
            data: {"id":"1","object":"chunk","created":1000,"model":"test","choices":[{"index":0,"delta":{"content":"Hello"}}]}

            : keep-alive

            data: {"id":"1","object":"chunk","created":1000,"model":"test","choices":[{"index":0,"delta":{"content":" World"}}]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(2);

        var chunks = events.Select(e => SafeJsonSerializer.Deserialize<ChatCompletionChunk>(e.Data)).ToList();
        var fullContent = string.Concat(chunks.Select(c => c.Choices[0].Delta.Content));
        fullContent.Should().Be("Hello World");
    }

    [Fact]
    public async Task StreamingPipeline_ProcessEventsAsTheyArrive_ShouldStreamCorrectly()
    {
        var input = """
            data: {"type":"content_delta","text":"A"}

            data: {"type":"content_delta","text":"B"}

            data: {"type":"content_delta","text":"C"}


            """;
        var stream = CreateStream(input);

        var processedTexts = new List<string>();
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, TestContext.Current.CancellationToken))
        {
            var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(evt.Data);
            if (streamEvent.Text != null)
            {
                processedTexts.Add(streamEvent.Text);
            }
        }

        processedTexts.Should().Equal("A", "B", "C");
    }

    [Fact]
    public async Task StreamingPipeline_WithDoneMarker_ShouldStopProcessing()
    {
        var input = """
            data: {"type":"content_delta","text":"Part 1"}

            data: {"type":"content_delta","text":"Part 2"}

            data: [DONE]


            """;
        var stream = CreateStream(input);

        var processedEvents = new List<StreamEvent>();
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, TestContext.Current.CancellationToken))
        {
            if (evt.Data == "[DONE]")
            {
                break;
            }
            var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(evt.Data);
            processedEvents.Add(streamEvent);
        }

        processedEvents.Should().HaveCount(2);
        processedEvents[0].Text.Should().Be("Part 1");
        processedEvents[1].Text.Should().Be("Part 2");
    }

    #endregion

    #region Error Handling Integration Tests

    [Fact]
    public async Task ErrorHandling_InvalidJsonInSse_ShouldThrowJsonException()
    {
        var input = """
            data: {invalid json}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(1);

        var act = () => SafeJsonSerializer.Deserialize<ChatCompletionChunk>(events[0].Data);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public async Task ErrorHandling_MissingRequiredFields_ShouldUseDefaults()
    {
        var input = """
            data: {"id":"1","object":"chunk"}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        // Records with missing properties use default values
        var result = SafeJsonSerializer.Deserialize<ChatCompletionChunk>(events[0].Data);
        result.Id.Should().Be("1");
        result.Object.Should().Be("chunk");
        // Missing properties get default values
        result.Created.Should().Be(0);
        result.Model.Should().BeNull();
        result.Choices.Should().BeNull();
    }

    [Fact]
    public async Task ErrorHandling_TypeMismatch_ShouldThrowJsonException()
    {
        var input = """
            data: {"id":"1","object":"chunk","created":"not-a-number","model":"test","choices":[]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        var act = () => SafeJsonSerializer.Deserialize<ChatCompletionChunk>(events[0].Data);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public async Task ErrorHandling_PartialStreamWithError_ShouldProcessValidEventsBeforeError()
    {
        var input = """
            data: {"type":"content_delta","text":"Valid 1"}

            data: {"type":"content_delta","text":"Valid 2"}

            data: {invalid}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(3);

        var validEvent1 = SafeJsonSerializer.Deserialize<StreamEvent>(events[0].Data);
        validEvent1.Text.Should().Be("Valid 1");

        var validEvent2 = SafeJsonSerializer.Deserialize<StreamEvent>(events[1].Data);
        validEvent2.Text.Should().Be("Valid 2");

        var act = () => SafeJsonSerializer.Deserialize<StreamEvent>(events[2].Data);
        act.Should().Throw<System.Text.Json.JsonException>();
    }

    [Fact]
    public async Task ErrorHandling_EmptyDataField_ShouldHandleGracefully()
    {
        var input = """
            data:


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        // Empty data fields don't emit events
        events.Should().BeEmpty();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public async Task Performance_LargeNumberOfEvents_ShouldProcessEfficiently()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            sb.AppendLine($"data: {{\"type\":\"content_delta\",\"text\":\"Chunk {i}\"}}");
            sb.AppendLine();
        }
        var stream = CreateStream(sb.ToString());

        var stopwatch = Stopwatch.StartNew();
        var count = 0;
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, TestContext.Current.CancellationToken))
        {
            var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(evt.Data);
            streamEvent.Type.Should().Be("content_delta");
            count++;
        }
        stopwatch.Stop();

        count.Should().Be(1000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should process 1000 events in less than 5 seconds
    }

    [Fact]
    public async Task Performance_LargeJsonPayload_ShouldHandleEfficiently()
    {
        var largeText = new string('x', 10000);
        var input = $$"""
            data: {"type":"content_delta","text":"{{largeText}}"}


            """;
        var stream = CreateStream(input);

        var stopwatch = Stopwatch.StartNew();
        var events = await ParseAllEventsAsync(stream);
        var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(events[0].Data);
        stopwatch.Stop();

        streamEvent.Text.Should().HaveLength(10000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000);
    }

    [Fact]
    public async Task Performance_StreamingWithBackpressure_ShouldHandleCorrectly()
    {
        var input = """
            data: {"type":"content_delta","text":"1"}

            data: {"type":"content_delta","text":"2"}

            data: {"type":"content_delta","text":"3"}


            """;
        var stream = CreateStream(input);

        var processedCount = 0;
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, TestContext.Current.CancellationToken))
        {
            // Simulate slow processing
            await Task.Delay(10, TestContext.Current.CancellationToken);
            var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(evt.Data);
            streamEvent.Type.Should().Be("content_delta");
            processedCount++;
        }

        processedCount.Should().Be(3);
    }

    [Fact]
    public async Task Performance_MultiLineJsonInSse_ShouldHandleCorrectly()
    {
        var input = """
            data: {
            data:   "type": "content_delta",
            data:   "text": "Multi-line JSON"
            data: }


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(1);

        var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(events[0].Data);
        streamEvent.Type.Should().Be("content_delta");
        streamEvent.Text.Should().Be("Multi-line JSON");
    }

    #endregion

    #region Real-World Scenarios

    [Fact]
    public async Task RealWorld_OpenAICompletionStream_ShouldProcessCorrectly()
    {
        var input = """
            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"role":"assistant","content":""}}]}

            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":"I"}}]}

            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":"'m"}}]}

            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{"content":" Claude"}}]}

            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1000,"model":"gpt-4","choices":[{"index":0,"delta":{},"finishReason":"stop"}]}

            data: [DONE]


            """;
        var stream = CreateStream(input);

        var content = new StringBuilder();
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, TestContext.Current.CancellationToken))
        {
            if (evt.Data == "[DONE]")
                break;

            var chunk = SafeJsonSerializer.Deserialize<ChatCompletionChunk>(evt.Data);
            var delta = chunk.Choices.FirstOrDefault()?.Delta.Content;
            if (delta != null)
            {
                content.Append(delta);
            }
        }

        content.ToString().Should().Be("I'm Claude");
    }

    [Fact]
    public async Task RealWorld_AnthropicMessageStream_ShouldProcessCorrectly()
    {
        var input = """
            event: message_start
            data: {"type":"message_start","messageId":"msg_01"}

            event: content_block_start
            data: {"type":"content_block_start"}

            event: content_block_delta
            data: {"type":"content_delta","text":"Hello"}

            event: content_block_delta
            data: {"type":"content_delta","text":" from"}

            event: content_block_delta
            data: {"type":"content_delta","text":" Anthropic"}

            event: content_block_stop
            data: {"type":"content_block_stop"}

            event: message_stop
            data: {"type":"message_stop"}


            """;
        var stream = CreateStream(input);

        var content = new StringBuilder();
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, TestContext.Current.CancellationToken))
        {
            var streamEvent = SafeJsonSerializer.Deserialize<StreamEvent>(evt.Data);
            if (streamEvent.Type == "content_delta" && streamEvent.Text != null)
            {
                content.Append(streamEvent.Text);
            }
        }

        content.ToString().Should().Be("Hello from Anthropic");
    }

    [Fact]
    public async Task RealWorld_ErrorInStream_ShouldPropagateCorrectly()
    {
        var input = """
            data: {"type":"content_delta","text":"Before error"}

            event: error
            data: {"type":"error","message":"Rate limit exceeded"}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);
        events.Should().HaveCount(2);

        var validEvent = SafeJsonSerializer.Deserialize<StreamEvent>(events[0].Data);
        validEvent.Text.Should().Be("Before error");

        events[1].Event.Should().Be("error");
    }

    #endregion

    #region Helper Methods

    private static MemoryStream CreateStream(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }

    private static async Task<List<ServerSentEvent>> ParseAllEventsAsync(
        Stream stream,
        CancellationToken? cancellationToken = null)
    {
        var events = new List<ServerSentEvent>();
        var token = cancellationToken ?? TestContext.Current.CancellationToken;
        await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, token))
        {
            events.Add(evt);
        }
        return events;
    }

    #endregion
}
