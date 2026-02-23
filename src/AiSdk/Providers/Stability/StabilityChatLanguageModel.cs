using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Stability.Exceptions;
using AiSdk.Providers.Stability.Models;

namespace AiSdk.Providers.Stability;

/// <summary>
/// Stability AI implementation of ILanguageModel.
/// Supports StableLM language models via OpenAI-compatible API format.
///
/// Note: Stability AI is primarily known for image generation (Stable Diffusion).
/// This implementation supports their language models which can be:
/// - Self-hosted using frameworks like vLLM with OpenAI-compatible endpoints
/// - Accessed through third-party platforms
/// - Future Stability AI hosted text generation API endpoints
/// </summary>
public class StabilityChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly StabilityConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "stability";

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
        // StableLM models currently don't support URL-based content natively
        // Future versions may support image URLs if multimodal capabilities are added
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StabilityChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Stability AI model ID (e.g., "stablelm-2-12b", "stablelm-2-zephyr-1_6b").</param>
    /// <param name="config">The Stability AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public StabilityChatLanguageModel(
        string modelId,
        StabilityConfiguration config,
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

        var response = await _httpClient.PostAsync("chat/completions", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<StabilityResponse>(cancellationToken)
            ?? throw new StabilityException("Failed to deserialize Stability AI response");

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

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        // Track tool calls being built across chunks
        var toolCallsInProgress = new Dictionary<int, ToolCallBuilder>();
        StabilityUsage? finalUsage = null;
        string? finishReason = null;

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);

                // Check for end of stream
                if (data.Trim() == "[DONE]")
                {
                    break;
                }

                var streamChunk = JsonSerializer.Deserialize<StabilityStreamResponse>(data);
                if (streamChunk?.Choices == null || streamChunk.Choices.Count == 0)
                    continue;

                var choice = streamChunk.Choices[0];
                var delta = choice.Delta;

                // Handle text delta
                if (!string.IsNullOrEmpty(delta.Content))
                {
                    yield return new LanguageModelStreamChunk
                    {
                        Type = ChunkType.TextDelta,
                        Delta = delta.Content
                    };
                }

                // Handle tool call deltas
                if (delta.ToolCalls != null)
                {
                    foreach (var toolCallDelta in delta.ToolCalls)
                    {
                        if (!toolCallsInProgress.TryGetValue(toolCallDelta.Index, out var builder))
                        {
                            builder = new ToolCallBuilder
                            {
                                Id = toolCallDelta.Id ?? string.Empty,
                                Name = toolCallDelta.Function?.Name ?? string.Empty,
                                JsonArguments = new StringBuilder()
                            };
                            toolCallsInProgress[toolCallDelta.Index] = builder;
                        }

                        if (toolCallDelta.Id != null)
                        {
                            builder.Id = toolCallDelta.Id;
                        }

                        if (toolCallDelta.Function?.Name != null)
                        {
                            builder.Name = toolCallDelta.Function.Name;
                        }

                        if (toolCallDelta.Function?.Arguments != null)
                        {
                            builder.JsonArguments.Append(toolCallDelta.Function.Arguments);
                        }
                    }
                }

                // Handle finish reason
                if (choice.FinishReason != null)
                {
                    finishReason = choice.FinishReason;
                }

                // Handle usage
                if (streamChunk.Usage != null)
                {
                    finalUsage = streamChunk.Usage;
                }
            }
        }

        // Emit completed tool calls
        foreach (var toolCall in toolCallsInProgress.Values)
        {
            if (!string.IsNullOrEmpty(toolCall.Id) && !string.IsNullOrEmpty(toolCall.Name))
            {
                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.ToolCallDelta,
                    ToolCall = new ToolCall(
                        ToolCallId: toolCall.Id,
                        ToolName: toolCall.Name,
                        Arguments: JsonDocument.Parse(toolCall.JsonArguments.ToString())
                    )
                };
            }
        }

        // Send final chunk
        yield return new LanguageModelStreamChunk
        {
            Type = ChunkType.Finish,
            FinishReason = MapFinishReason(finishReason),
            Usage = finalUsage != null ? MapUsage(finalUsage) : null
        };
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private StabilityRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        var messages = options.Messages.Select(m => new StabilityMessage
        {
            Role = MapRole(m.Role),
            Content = m.Content ?? string.Empty,
            // Tool role messages use Name as ToolCallId
            ToolCallId = m.Role == MessageRole.Tool ? m.Name : null
        }).ToList();

        var request = new StabilityRequest
        {
            Model = _modelId,
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            Stop = options.StopSequences,
            Stream = stream
        };

        if (options.Tools?.Count > 0)
        {
            request = request with
            {
                Tools = options.Tools.Select(t => new StabilityTool
                {
                    Function = new StabilityFunctionDefinition
                    {
                        Name = t.Name,
                        Description = t.Description,
                        Parameters = t.Parameters != null ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement : new { }
                    }
                }).ToList(),
                ToolChoice = options.ToolChoice != null
                    ? new { type = "function", function = new { name = options.ToolChoice } }
                    : null
            };
        }

        return request;
    }

    private static LanguageModelGenerateResult MapToGenerateResult(StabilityResponse response)
    {
        if (response.Choices.Count == 0)
        {
            throw new StabilityException("Stability AI response contained no choices");
        }

        var choice = response.Choices[0];
        var message = choice.Message;

        List<ToolCall>? toolCalls = null;
        if (message.ToolCalls?.Count > 0)
        {
            toolCalls = message.ToolCalls.Select(tc => new ToolCall(
                ToolCallId: tc.Id,
                ToolName: tc.Function.Name,
                Arguments: JsonDocument.Parse(tc.Function.Arguments)
            )).ToList();
        }

        return new LanguageModelGenerateResult
        {
            Text = message.Content,
            FinishReason = MapFinishReason(choice.FinishReason),
            Usage = response.Usage != null ? MapUsage(response.Usage) : new Usage(),
            ToolCalls = toolCalls
        };
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "stop" => FinishReason.Stop,
        "length" => FinishReason.Length,
        "tool_calls" => FinishReason.ToolCalls,
        "content_filter" => FinishReason.ContentFilter,
        _ => FinishReason.Other
    };

    private static Usage MapUsage(StabilityUsage usage) => new Usage(
        InputTokens: usage.PromptTokens,
        OutputTokens: usage.CompletionTokens,
        TotalTokens: usage.TotalTokens
    );

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.System => "system",
        MessageRole.User => "user",
        MessageRole.Assistant => "assistant",
        MessageRole.Tool => "tool",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<StabilityErrorResponse>(content);
                throw new StabilityException(
                    errorResponse?.Error.Message ?? "Stability AI API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Type,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new StabilityException($"Stability AI API error: {content}", (int)response.StatusCode, null, null);
            }
        }
    }

    private static async IAsyncEnumerable<string> ReadLinesAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            yield return line;
        }
    }

    private class ToolCallBuilder
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required StringBuilder JsonArguments { get; init; }
    }
}
