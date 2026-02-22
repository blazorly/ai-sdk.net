using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Anthropic.Exceptions;
using AiSdk.Providers.Anthropic.Models;

namespace AiSdk.Providers.Anthropic;

/// <summary>
/// Anthropic implementation of ILanguageModel.
/// </summary>
public class AnthropicChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly AnthropicConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "anthropic";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    /// <summary>
    /// Gets the supported URL patterns by media type for this provider.
    /// </summary>
    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        // Anthropic supports image URLs natively for vision-enabled Claude models
        var supported = new Dictionary<string, IReadOnlyList<string>>
        {
            ["image/*"] = new List<string> { ".*" }.AsReadOnly()
        };

        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Anthropic model ID (e.g., "claude-3-5-sonnet-20241022", "claude-3-opus-20240229").</param>
    /// <param name="config">The Anthropic configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public AnthropicChatLanguageModel(
        string modelId,
        AnthropicConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        _modelId = modelId;
        _config = config;
        _httpClient = httpClient ?? new HttpClient();

        ConfigureHttpClient();
    }

    /// <summary>
    /// Generates text from the language model (non-streaming).
    /// </summary>
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var request = BuildRequest(options, stream: false);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("messages", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>(cancellationToken)
            ?? throw new AnthropicException("Failed to deserialize Anthropic response");

        return MapToGenerateResult(result);
    }

    /// <summary>
    /// Streams text generation from the language model.
    /// </summary>
    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        var request = BuildRequest(options, stream: true);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "messages")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        // Track tool calls being built across chunks
        var toolCallsInProgress = new Dictionary<int, ToolCallBuilder>();
        AnthropicUsage? finalUsage = null;
        string? finishReason = null;

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                var streamEvent = JsonSerializer.Deserialize<AnthropicStreamResponse>(data);

                if (streamEvent == null)
                    continue;

                switch (streamEvent.Type)
                {
                    case "content_block_start":
                        if (streamEvent.ContentBlock?.Type == "tool_use" && streamEvent.Index.HasValue)
                        {
                            toolCallsInProgress[streamEvent.Index.Value] = new ToolCallBuilder
                            {
                                Id = streamEvent.ContentBlock.Id!,
                                Name = streamEvent.ContentBlock.Name!,
                                JsonArguments = new StringBuilder()
                            };
                        }
                        break;

                    case "content_block_delta":
                        if (streamEvent.Delta?.Type == "text_delta" && !string.IsNullOrEmpty(streamEvent.Delta.Text))
                        {
                            yield return new LanguageModelStreamChunk
                            {
                                Type = ChunkType.TextDelta,
                                Delta = streamEvent.Delta.Text
                            };
                        }
                        else if (streamEvent.Delta?.Type == "input_json_delta" &&
                                 streamEvent.Index.HasValue &&
                                 toolCallsInProgress.TryGetValue(streamEvent.Index.Value, out var builder))
                        {
                            builder.JsonArguments.Append(streamEvent.Delta.PartialJson);
                        }
                        break;

                    case "content_block_stop":
                        // Tool call complete - emit it
                        if (streamEvent.Index.HasValue &&
                            toolCallsInProgress.TryGetValue(streamEvent.Index.Value, out var completedTool))
                        {
                            var toolCall = new ToolCall(
                                ToolCallId: completedTool.Id,
                                ToolName: completedTool.Name,
                                Arguments: JsonDocument.Parse(completedTool.JsonArguments.ToString())
                            );

                            yield return new LanguageModelStreamChunk
                            {
                                Type = ChunkType.ToolCallDelta,
                                ToolCall = toolCall
                            };

                            toolCallsInProgress.Remove(streamEvent.Index.Value);
                        }
                        break;

                    case "message_delta":
                        if (streamEvent.Delta?.StopReason != null)
                        {
                            finishReason = streamEvent.Delta.StopReason;
                        }
                        if (streamEvent.Usage != null)
                        {
                            finalUsage = streamEvent.Usage;
                        }
                        break;

                    case "message_stop":
                        // Send final chunk with finish reason and usage
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.Finish,
                            FinishReason = MapFinishReason(finishReason),
                            Usage = finalUsage != null ? MapUsage(finalUsage) : null
                        };
                        break;
                }
            }
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", _config.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", _config.ApiVersion);

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private AnthropicRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        // Extract system message if present
        string? systemMessage = null;
        var messages = new List<AnthropicMessage>();

        foreach (var msg in options.Messages)
        {
            if (msg.Role == MessageRole.System)
            {
                systemMessage = msg.Content;
            }
            else if (msg.Role == MessageRole.User || msg.Role == MessageRole.Assistant)
            {
                messages.Add(new AnthropicMessage
                {
                    Role = MapRole(msg.Role),
                    Content = BuildAnthropicContent(msg)
                });
            }
            else if (msg.Role == MessageRole.Tool)
            {
                // Anthropic uses tool_result content blocks
                messages.Add(new AnthropicMessage
                {
                    Role = "user",
                    Content = new List<AnthropicContentBlock>
                    {
                        new AnthropicContentBlock
                        {
                            Type = "tool_result",
                            ToolUseId = msg.Name,
                            Content = msg.Content
                        }
                    }
                });
            }
        }

        var request = new AnthropicRequest
        {
            Model = _modelId,
            Messages = messages,
            MaxTokens = options.MaxTokens ?? 4096, // Anthropic requires max_tokens
            Temperature = options.Temperature,
            TopP = options.TopP,
            StopSequences = options.StopSequences,
            Stream = stream,
            System = systemMessage
        };

        if (options.Tools?.Count > 0)
        {
            request = request with
            {
                Tools = options.Tools.Select(t => new AnthropicTool
                {
                    Name = t.Name,
                    Description = t.Description,
                    InputSchema = t.Parameters != null ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement : new { }
                }).ToList(),
                ToolChoice = options.ToolChoice != null
                    ? new { type = "tool", name = options.ToolChoice }
                    : null
            };
        }

        return request;
    }

    /// <summary>
    /// Builds the content for an Anthropic message, supporting both simple text and multi-modal content parts.
    /// Anthropic uses content blocks natively: text, image (base64/url), and document.
    /// </summary>
    private static object BuildAnthropicContent(Message message)
    {
        // If no multi-modal parts, use plain text content
        if (message.Parts == null || message.Parts.Count == 0)
        {
            return message.Content ?? string.Empty;
        }

        // Build a list of Anthropic content blocks
        var blocks = new List<AnthropicContentBlock>();

        foreach (var part in message.Parts)
        {
            switch (part)
            {
                case TextPart textPart:
                    blocks.Add(new AnthropicContentBlock
                    {
                        Type = "text",
                        Text = textPart.Text
                    });
                    break;

                case ImagePart imagePart:
                    if (imagePart.Data != null)
                    {
                        var mimeType = imagePart.MimeType ?? "image/png";
                        blocks.Add(new AnthropicContentBlock
                        {
                            Type = "image",
                            Source = new AnthropicSource
                            {
                                Type = "base64",
                                MediaType = mimeType,
                                Data = Convert.ToBase64String(imagePart.Data)
                            }
                        });
                    }
                    else if (imagePart.Url != null)
                    {
                        blocks.Add(new AnthropicContentBlock
                        {
                            Type = "image",
                            Source = new AnthropicSource
                            {
                                Type = "url",
                                Url = imagePart.Url
                            }
                        });
                    }
                    break;

                case FilePart filePart:
                    if (filePart.MimeType == "application/pdf")
                    {
                        if (filePart.Data != null)
                        {
                            blocks.Add(new AnthropicContentBlock
                            {
                                Type = "document",
                                Source = new AnthropicSource
                                {
                                    Type = "base64",
                                    MediaType = filePart.MimeType,
                                    Data = Convert.ToBase64String(filePart.Data)
                                }
                            });
                        }
                        else
                        {
                            blocks.Add(new AnthropicContentBlock
                            {
                                Type = "document",
                                Source = new AnthropicSource
                                {
                                    Type = "url",
                                    Url = filePart.Url
                                }
                            });
                        }
                    }
                    break;
            }
        }

        return blocks;
    }

    private static LanguageModelGenerateResult MapToGenerateResult(AnthropicResponse response)
    {
        string? text = null;
        List<ToolCall>? toolCalls = null;

        foreach (var block in response.Content)
        {
            if (block.Type == "text" && block.Text != null)
            {
                text = (text ?? "") + block.Text;
            }
            else if (block.Type == "tool_use")
            {
                toolCalls ??= new List<ToolCall>();
                toolCalls.Add(new ToolCall(
                    ToolCallId: block.Id!,
                    ToolName: block.Name!,
                    Arguments: JsonDocument.Parse(JsonSerializer.Serialize(block.Input))
                ));
            }
        }

        return new LanguageModelGenerateResult
        {
            Text = text,
            FinishReason = MapFinishReason(response.StopReason),
            Usage = MapUsage(response.Usage),
            ToolCalls = toolCalls
        };
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "end_turn" => FinishReason.Stop,
        "max_tokens" => FinishReason.Length,
        "tool_use" => FinishReason.ToolCalls,
        "stop_sequence" => FinishReason.Stop,
        _ => FinishReason.Other
    };

    private static Usage MapUsage(AnthropicUsage usage) => new Usage(
        InputTokens: usage.InputTokens,
        OutputTokens: usage.OutputTokens,
        TotalTokens: usage.InputTokens + usage.OutputTokens
    );

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.User => "user",
        MessageRole.Assistant => "assistant",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<AnthropicErrorResponse>(content);
                throw new AnthropicException(
                    errorResponse?.Error.Message ?? "Anthropic API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Type);
            }
            catch (JsonException)
            {
                throw new AnthropicException($"Anthropic API error: {content}", (int)response.StatusCode, null);
            }
        }
    }

    private static async IAsyncEnumerable<string> ReadLinesAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line != null)
            {
                yield return line;
            }
        }
    }

    private class ToolCallBuilder
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required StringBuilder JsonArguments { get; init; }
    }
}
