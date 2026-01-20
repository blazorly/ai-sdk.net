using System.Text;
using AiSdk.Core.Streaming;
using FluentAssertions;
using Xunit;

namespace AiSdk.Core.Tests;

public class ServerSentEventsParserTests
{
    #region Basic SSE Parsing Tests

    [Fact]
    public async Task ParseAsync_WithSingleEvent_ShouldParseCorrectly()
    {
        var input = "data: Hello World\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Hello World");
        events[0].Event.Should().BeNull();
        events[0].Id.Should().BeNull();
    }

    [Fact]
    public async Task ParseAsync_WithMultipleEvents_ShouldParseAll()
    {
        var input = """
            data: First event

            data: Second event

            data: Third event


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(3);
        events[0].Data.Should().Be("First event");
        events[1].Data.Should().Be("Second event");
        events[2].Data.Should().Be("Third event");
    }

    [Fact]
    public async Task ParseAsync_WithEmptyData_ShouldNotEmitEvent()
    {
        var input = "data:\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        // Empty data (data:) doesn't emit an event since HasData checks Length > 0
        events.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_WithDataOnly_ShouldHaveNullEventAndId()
    {
        var input = "data: test data\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("test data");
        events[0].Event.Should().BeNull();
        events[0].Id.Should().BeNull();
    }

    #endregion

    #region Multi-line Data Tests

    [Fact]
    public async Task ParseAsync_WithMultiLineData_ShouldJoinWithNewlines()
    {
        var input = """
            data: Line 1
            data: Line 2
            data: Line 3


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Line 1\nLine 2\nLine 3");
    }

    [Fact]
    public async Task ParseAsync_WithMultiLineDataAndEmptyLines_ShouldPreserveStructure()
    {
        var input = """
            data: First line
            data:
            data: Third line


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("First line\n\nThird line");
    }

    [Fact]
    public async Task ParseAsync_WithJsonInMultiLineData_ShouldParseCorrectly()
    {
        var input = """
            data: {
            data:   "name": "test",
            data:   "value": 123
            data: }


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("{\n  \"name\": \"test\",\n  \"value\": 123\n}");
    }

    #endregion

    #region Event Type Tests

    [Fact]
    public async Task ParseAsync_WithEventType_ShouldParseEventField()
    {
        var input = """
            event: custom-event
            data: Event data


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Event.Should().Be("custom-event");
        events[0].Data.Should().Be("Event data");
    }

    [Fact]
    public async Task ParseAsync_WithMultipleEventTypes_ShouldParseEachCorrectly()
    {
        var input = """
            event: type1
            data: Data 1

            event: type2
            data: Data 2

            data: Data 3


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(3);
        events[0].Event.Should().Be("type1");
        events[0].Data.Should().Be("Data 1");
        events[1].Event.Should().Be("type2");
        events[1].Data.Should().Be("Data 2");
        events[2].Event.Should().BeNull();
        events[2].Data.Should().Be("Data 3");
    }

    [Fact]
    public async Task ParseAsync_WithEmptyEventType_ShouldParseAsEmptyString()
    {
        var input = """
            event:
            data: Test


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Event.Should().Be("");
    }

    #endregion

    #region ID Handling Tests

    [Fact]
    public async Task ParseAsync_WithId_ShouldParseIdField()
    {
        var input = """
            id: 12345
            data: Test data


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Id.Should().Be("12345");
        events[0].Data.Should().Be("Test data");
    }

    [Fact]
    public async Task ParseAsync_WithEventTypeAndId_ShouldParseBoth()
    {
        var input = """
            event: update
            id: msg-001
            data: Update message


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Event.Should().Be("update");
        events[0].Id.Should().Be("msg-001");
        events[0].Data.Should().Be("Update message");
    }

    [Fact]
    public async Task ParseAsync_WithAllFields_ShouldParseAllCorrectly()
    {
        var input = """
            event: message
            id: 42
            data: Complete event


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Event.Should().Be("message");
        events[0].Id.Should().Be("42");
        events[0].Data.Should().Be("Complete event");
    }

    [Fact]
    public async Task ParseAsync_WithDifferentFieldOrder_ShouldParseCorrectly()
    {
        var input = """
            data: First field
            id: 100
            event: reordered


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Event.Should().Be("reordered");
        events[0].Id.Should().Be("100");
        events[0].Data.Should().Be("First field");
    }

    #endregion

    #region Empty Lines Tests

    [Fact]
    public async Task ParseAsync_WithMultipleEmptyLines_ShouldNotCreateExtraEvents()
    {
        var input = """
            data: Event 1



            data: Event 2


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(2);
        events[0].Data.Should().Be("Event 1");
        events[1].Data.Should().Be("Event 2");
    }

    [Fact]
    public async Task ParseAsync_WithLeadingEmptyLines_ShouldIgnoreThem()
    {
        var input = """


            data: First event


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("First event");
    }

    [Fact]
    public async Task ParseAsync_WithTrailingEmptyLines_ShouldIgnoreThem()
    {
        var input = """
            data: Last event



            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Last event");
    }

    #endregion

    #region Comments Tests

    [Fact]
    public async Task ParseAsync_WithComments_ShouldIgnoreComments()
    {
        var input = """
            : This is a comment
            data: Real data


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Real data");
    }

    [Fact]
    public async Task ParseAsync_WithMultipleComments_ShouldIgnoreAll()
    {
        var input = """
            : Comment 1
            : Comment 2
            data: Data
            : Comment 3


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Data");
    }

    [Fact]
    public async Task ParseAsync_WithOnlyComments_ShouldReturnNoEvents()
    {
        var input = """
            : Comment 1
            : Comment 2
            : Comment 3
            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_WithCommentBetweenEvents_ShouldNotAffectParsing()
    {
        var input = """
            data: Event 1

            : Separator comment

            data: Event 2


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(2);
        events[0].Data.Should().Be("Event 1");
        events[1].Data.Should().Be("Event 2");
    }

    #endregion

    #region Malformed Input Tests

    [Fact]
    public async Task ParseAsync_WithNoColon_ShouldTreatAsFieldWithEmptyValue()
    {
        var input = """
            data
            data: Real data


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        // When field has no colon, it gets empty value, then next data field appends
        // The first empty data doesn't add a newline, second data adds content
        events[0].Data.Should().Be("Real data");
    }

    [Fact]
    public async Task ParseAsync_WithUnknownField_ShouldIgnoreField()
    {
        var input = """
            unknown: field
            data: Known data


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Known data");
    }

    [Fact]
    public async Task ParseAsync_WithEventWithoutData_ShouldNotEmitEvent()
    {
        var input = """
            event: orphan-event

            data: Valid event


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Valid event");
    }

    [Fact]
    public async Task ParseAsync_WithIdWithoutData_ShouldNotEmitEvent()
    {
        var input = """
            id: orphan-id

            data: Valid event


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Valid event");
    }

    [Fact]
    public async Task ParseAsync_WithColonAndSpace_ShouldStripLeadingSpace()
    {
        var input = "data: value with space\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("value with space");
    }

    [Fact]
    public async Task ParseAsync_WithColonNoSpace_ShouldNotStripValue()
    {
        var input = "data:value without space\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("value without space");
    }

    [Fact]
    public async Task ParseAsync_WithMultipleColons_ShouldTreatRestAsValue()
    {
        var input = "data: value:with:colons\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("value:with:colons");
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task ParseAsync_WithCancellation_ShouldStopParsing()
    {
        var input = """
            data: Event 1

            data: Event 2

            data: Event 3


            """;
        var stream = CreateStream(input);
        using var cts = new CancellationTokenSource();
        var events = new List<ServerSentEvent>();

        try
        {
            await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, cts.Token))
            {
                events.Add(evt);
                if (events.Count == 1)
                {
                    cts.Cancel();
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation occurs
        }

        events.Should().HaveCountGreaterOrEqualTo(1);
        events[0].Data.Should().Be("Event 1");
    }

    [Fact]
    public async Task ParseAsync_WithPreCancelledToken_ShouldHandleCancellation()
    {
        var input = "data: Event\n\n";
        var stream = CreateStream(input);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // With a pre-cancelled token, we should either get an empty result or an exception
        var events = new List<ServerSentEvent>();
        try
        {
            await foreach (var evt in ServerSentEventsParser.ParseAsync(stream, cts.Token))
            {
                events.Add(evt);
            }
        }
        catch (OperationCanceledException)
        {
            // This is expected
        }

        // Either way, we shouldn't have processed any events
        events.Should().BeEmpty();
    }

    #endregion

    #region Real-world Scenarios

    [Fact]
    public async Task ParseAsync_WithOpenAIStyleChunk_ShouldParseCorrectly()
    {
        var input = """
            event: completion
            data: {"id":"chatcmpl-123","object":"chat.completion.chunk","choices":[{"delta":{"content":"Hello"}}]}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Event.Should().Be("completion");
        events[0].Data.Should().Contain("Hello");
    }

    [Fact]
    public async Task ParseAsync_WithAnthropicStyleStream_ShouldParseCorrectly()
    {
        var input = """
            event: message_start
            data: {"type":"message_start","message":{"id":"msg_123"}}

            event: content_block_delta
            data: {"type":"content_block_delta","delta":{"text":"Hi"}}

            event: message_stop
            data: {"type":"message_stop"}


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(3);
        events[0].Event.Should().Be("message_start");
        events[1].Event.Should().Be("content_block_delta");
        events[2].Event.Should().Be("message_stop");
    }

    [Fact]
    public async Task ParseAsync_WithKeepAliveComments_ShouldIgnoreAndContinue()
    {
        var input = """
            data: Event 1

            : keep-alive

            data: Event 2


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(2);
        events[0].Data.Should().Be("Event 1");
        events[1].Data.Should().Be("Event 2");
    }

    [Fact]
    public async Task ParseAsync_WithDoneSignal_ShouldParseAsNormalEvent()
    {
        var input = """
            data: Regular event

            data: [DONE]


            """;
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(2);
        events[0].Data.Should().Be("Regular event");
        events[1].Data.Should().Be("[DONE]");
    }

    [Fact]
    public async Task ParseAsync_WithLongStreamingSession_ShouldHandleManyEvents()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 100; i++)
        {
            sb.AppendLine($"data: Event {i}");
            sb.AppendLine();
        }
        var stream = CreateStream(sb.ToString());

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(100);
        events[0].Data.Should().Be("Event 0");
        events[99].Data.Should().Be("Event 99");
    }

    [Fact]
    public async Task ParseAsync_WithEmptyStream_ShouldReturnNoEvents()
    {
        var stream = CreateStream("");

        var events = await ParseAllEventsAsync(stream);

        events.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_WithOnlyWhitespace_ShouldReturnNoEvents()
    {
        var stream = CreateStream("\n\n\n\n");

        var events = await ParseAllEventsAsync(stream);

        events.Should().BeEmpty();
    }

    [Fact]
    public async Task ParseAsync_WithUnicodeContent_ShouldPreserveUnicode()
    {
        var input = "data: Hello 世界 🌍\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Hello 世界 🌍");
    }

    [Fact]
    public async Task ParseAsync_WithSpecialCharacters_ShouldPreserveCharacters()
    {
        var input = "data: Special chars: !@#$%^&*()_+-=[]{}|;:',.<>?/~`\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().Be("Special chars: !@#$%^&*()_+-=[]{}|;:',.<>?/~`");
    }

    [Fact]
    public async Task ParseAsync_WithVeryLongLine_ShouldHandleCorrectly()
    {
        var longData = new string('x', 10000);
        var input = $"data: {longData}\n\n";
        var stream = CreateStream(input);

        var events = await ParseAllEventsAsync(stream);

        events.Should().HaveCount(1);
        events[0].Data.Should().HaveLength(10000);
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
