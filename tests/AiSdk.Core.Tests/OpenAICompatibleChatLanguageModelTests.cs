using System.Net;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAICompatible;
using FluentAssertions;
using Xunit;

namespace AiSdk.Core.Tests;

public class OpenAICompatibleChatLanguageModelTests
{
    [Fact]
    public async Task GenerateAsync_uses_chat_completions_under_v1_base_urls_without_trailing_slash()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var model = new OpenAICompatibleChatLanguageModel(
            "deepseek/deepseek-v4-flash",
            new OpenAICompatibleConfiguration
            {
                BaseUrl = "https://openrouter.ai/api/v1",
                ApiKey = "test-key"
            },
            httpClient);

        await model.GenerateAsync(
            new LanguageModelCallOptions
            {
                Messages = new[]
                {
                    new Message(MessageRole.User, "hello")
                }
            },
            TestContext.Current.CancellationToken);

        handler.RequestUri.Should().Be(new Uri("https://openrouter.ai/api/v1/chat/completions"));
    }

    [Fact]
    public async Task StreamAsync_emits_reasoning_delta_when_provider_carries_reasoning_field()
    {
        // OpenRouter (and other OpenAI-compatible aggregators) carry a top-level
        // `reasoning` field on streaming deltas for reasoning-capable models
        // like DeepSeek R1 and Grok-4. The model must surface it as a
        // ReasoningDelta chunk — NOT fold it into the TextDelta — so the UI
        // can render it as a distinct thinking block.
        var handler = new RecordingHandler(
            """
            data: {"id":"chatcmpl-rs","object":"chat.completion.chunk","created":1,"model":"x-ai/grok-4","choices":[{"index":0,"delta":{"role":"assistant"},"finish_reason":null}]}

            data: {"id":"chatcmpl-rs","object":"chat.completion.chunk","created":1,"model":"x-ai/grok-4","choices":[{"index":0,"delta":{"reasoning":"The user wants a thinking block."},"finish_reason":null}]}

            data: {"id":"chatcmpl-rs","object":"chat.completion.chunk","created":1,"model":"x-ai/grok-4","choices":[{"index":0,"delta":{"reasoning":" Let me think more."},"finish_reason":null}]}

            data: {"id":"chatcmpl-rs","object":"chat.completion.chunk","created":1,"model":"x-ai/grok-4","choices":[{"index":0,"delta":{"content":"Here is the answer."},"finish_reason":null}]}

            data: {"id":"chatcmpl-rs","object":"chat.completion.chunk","created":1,"model":"x-ai/grok-4","choices":[{"index":0,"delta":{},"finish_reason":"stop"}]}

            data: [DONE]

            """,
            "text/event-stream");
        using var httpClient = new HttpClient(handler);
        var model = new OpenAICompatibleChatLanguageModel(
            "x-ai/grok-4",
            new OpenAICompatibleConfiguration
            {
                BaseUrl = "https://openrouter.ai/api/v1",
                ApiKey = "test-key"
            },
            httpClient);

        var chunks = new List<LanguageModelStreamChunk>();
        await foreach (var chunk in model.StreamAsync(
                           new LanguageModelCallOptions
                           {
                               Messages = new[] { new Message(MessageRole.User, "hello") }
                           },
                           TestContext.Current.CancellationToken))
        {
            chunks.Add(chunk);
        }

        // Two reasoning deltas — they should stay separate from the text delta.
        var reasoningChunks = chunks.Where(c => c.Type == ChunkType.ReasoningDelta).ToList();
        Assert.Equal(2, reasoningChunks.Count);
        Assert.Equal("The user wants a thinking block.", reasoningChunks[0].ReasoningContent);
        Assert.Equal(" Let me think more.", reasoningChunks[1].ReasoningContent);
        // Chunk id from the SSE event is carried through so the UI can route
        // the deltas back to the same reasoning part.
        Assert.Equal("chatcmpl-rs", reasoningChunks[0].Id);

        var textChunk = Assert.Single(chunks, c => c.Type == ChunkType.TextDelta);
        Assert.Equal("Here is the answer.", textChunk.Delta);
        // And the reasoning is NOT in the text delta.
        Assert.DoesNotContain("thinking block", textChunk.Delta);

        Assert.Single(chunks, c => c.Type == ChunkType.Finish);
    }

    [Fact]
    public async Task StreamAsync_does_not_emit_reasoning_delta_when_field_is_absent()
    {
        // For providers/models that don't emit reasoning, the stream should
        // never produce a ReasoningDelta chunk — only TextDelta / ToolCall /
        // Finish.
        var handler = new RecordingHandler(
            """
            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1,"model":"gpt-4o","choices":[{"index":0,"delta":{"role":"assistant"},"finish_reason":null}]}

            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1,"model":"gpt-4o","choices":[{"index":0,"delta":{"content":"hi"},"finish_reason":null}]}

            data: {"id":"chatcmpl-1","object":"chat.completion.chunk","created":1,"model":"gpt-4o","choices":[{"index":0,"delta":{},"finish_reason":"stop"}]}

            data: [DONE]

            """,
            "text/event-stream");
        using var httpClient = new HttpClient(handler);
        var model = new OpenAICompatibleChatLanguageModel(
            "gpt-4o",
            new OpenAICompatibleConfiguration
            {
                BaseUrl = "https://api.openai.com/v1",
                ApiKey = "test-key"
            },
            httpClient);

        var chunks = new List<LanguageModelStreamChunk>();
        await foreach (var chunk in model.StreamAsync(
                           new LanguageModelCallOptions
                           {
                               Messages = new[] { new Message(MessageRole.User, "hi") }
                           },
                           TestContext.Current.CancellationToken))
        {
            chunks.Add(chunk);
        }

        Assert.DoesNotContain(chunks, c => c.Type == ChunkType.ReasoningDelta);
    }

    [Fact]
    public async Task GenerateAsync_surfaces_reasoning_on_non_streaming_response()
    {
        var handler = new RecordingHandler(
            """
            {
              "id": "chatcmpl-r",
              "object": "chat.completion",
              "created": 1,
              "model": "deepseek/deepseek-v4-flash",
              "choices": [
                {
                  "index": 0,
                  "message": {
                    "role": "assistant",
                    "content": "Here is the answer.",
                    "reasoning": "The user wants a thinking block."
                  },
                  "finish_reason": "stop"
                }
              ],
              "usage": { "prompt_tokens": 1, "completion_tokens": 1, "total_tokens": 2 }
            }
            """,
            "application/json");
        using var httpClient = new HttpClient(handler);
        var model = new OpenAICompatibleChatLanguageModel(
            "deepseek/deepseek-v4-flash",
            new OpenAICompatibleConfiguration
            {
                BaseUrl = "https://openrouter.ai/api/v1",
                ApiKey = "test-key"
            },
            httpClient);

        var result = await model.GenerateAsync(
            new LanguageModelCallOptions
            {
                Messages = new[] { new Message(MessageRole.User, "hello") }
            },
            TestContext.Current.CancellationToken);

        Assert.Equal("Here is the answer.", result.Text);
        Assert.Equal("The user wants a thinking block.", result.ReasoningContent);
    }

    [Fact]
    public async Task StreamAsync_emits_tool_call_delta_from_openai_compatible_stream()
    {
        var handler = new RecordingHandler(
            """
            data: {"id":"chatcmpl-test","object":"chat.completion.chunk","created":1,"model":"deepseek/deepseek-v4-flash","choices":[{"index":0,"delta":{"role":"assistant"},"finish_reason":null}]}

            data: {"id":"chatcmpl-test","object":"chat.completion.chunk","created":1,"model":"deepseek/deepseek-v4-flash","choices":[{"index":0,"delta":{"tool_calls":[{"index":0,"id":"call_1","type":"function","function":{"name":"ls","arguments":"{\"path\":\".\"}"}}]},"finish_reason":null}]}

            data: {"id":"chatcmpl-test","object":"chat.completion.chunk","created":1,"model":"deepseek/deepseek-v4-flash","choices":[{"index":0,"delta":{},"finish_reason":"tool_calls"}]}

            data: [DONE]

            """,
            "text/event-stream");
        using var httpClient = new HttpClient(handler);
        var model = new OpenAICompatibleChatLanguageModel(
            "deepseek/deepseek-v4-flash",
            new OpenAICompatibleConfiguration
            {
                BaseUrl = "https://openrouter.ai/api/v1",
                ApiKey = "test-key"
            },
            httpClient);

        var chunks = new List<LanguageModelStreamChunk>();
        await foreach (var chunk in model.StreamAsync(
                           new LanguageModelCallOptions
                           {
                               Messages = new[]
                               {
                                   new Message(MessageRole.User, "list files")
                               }
                           },
                           TestContext.Current.CancellationToken))
        {
            chunks.Add(chunk);
        }

        var toolChunk = Assert.Single(chunks, chunk => chunk.Type == ChunkType.ToolCallDelta);
        Assert.NotNull(toolChunk.ToolCall);
        Assert.Equal("call_1", toolChunk.ToolCall.ToolCallId);
        Assert.Equal("ls", toolChunk.ToolCall.ToolName);
        Assert.Equal(".", toolChunk.ToolCall.Arguments.RootElement.GetProperty("path").GetString());

        var finishChunk = Assert.Single(chunks, chunk => chunk.Type == ChunkType.Finish);
        Assert.Equal(FinishReason.ToolCalls, finishChunk.FinishReason);
    }

    [Fact]
    public async Task GenerateAsync_serializes_assistant_tool_calls_and_tool_results()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        using var arguments = JsonDocument.Parse("""{"path":"."}""");
        var model = new OpenAICompatibleChatLanguageModel(
            "deepseek/deepseek-v4-flash",
            new OpenAICompatibleConfiguration
            {
                BaseUrl = "https://openrouter.ai/api/v1",
                ApiKey = "test-key"
            },
            httpClient);

        await model.GenerateAsync(
            new LanguageModelCallOptions
            {
                Messages = new[]
                {
                    new Message(MessageRole.Assistant, "")
                    {
                        ToolCalls = new[]
                        {
                            new ToolCall("call_1", "ls", arguments)
                        }
                    },
                    new Message(MessageRole.Tool, "bin\nsrc", "call_1")
                }
            },
            TestContext.Current.CancellationToken);

        using var request = JsonDocument.Parse(handler.RequestBody!);
        var messages = request.RootElement.GetProperty("messages");

        var assistant = messages[0];
        Assert.Equal("assistant", assistant.GetProperty("role").GetString());
        var toolCall = Assert.Single(assistant.GetProperty("tool_calls").EnumerateArray());
        Assert.Equal("call_1", toolCall.GetProperty("id").GetString());
        Assert.Equal("function", toolCall.GetProperty("type").GetString());
        Assert.Equal("ls", toolCall.GetProperty("function").GetProperty("name").GetString());
        Assert.Equal("""{"path":"."}""", toolCall.GetProperty("function").GetProperty("arguments").GetString());

        var tool = messages[1];
        Assert.Equal("tool", tool.GetProperty("role").GetString());
        Assert.Equal("call_1", tool.GetProperty("tool_call_id").GetString());
        Assert.Equal("bin\nsrc", tool.GetProperty("content").GetString());
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly string _responseBody;
        private readonly string _mediaType;

        public Uri? RequestUri { get; private set; }
        public string? RequestBody { get; private set; }

        public RecordingHandler(
            string responseBody = """
            {
              "id": "chatcmpl-test",
              "object": "chat.completion",
              "created": 1,
              "model": "deepseek/deepseek-v4-flash",
              "choices": [
                {
                  "index": 0,
                  "message": {
                    "role": "assistant",
                    "content": "hi"
                  },
                  "finish_reason": "stop"
                }
              ],
              "usage": {
                "prompt_tokens": 1,
                "completion_tokens": 1,
                "total_tokens": 2
              }
            }
            """,
            string mediaType = "application/json")
        {
            _responseBody = responseBody;
            _mediaType = mediaType;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            RequestBody = request.Content == null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseBody, Encoding.UTF8, _mediaType)
            };
        }
    }
}
