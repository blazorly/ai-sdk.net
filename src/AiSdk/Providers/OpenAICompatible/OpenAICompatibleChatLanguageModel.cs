using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAICompatible.Exceptions;
using AiSdk.Providers.OpenAICompatible.Models;

namespace AiSdk.Providers.OpenAICompatible;

/// <summary>
/// OpenAI-compatible implementation of ILanguageModel.
/// Supports any OpenAI-compatible API such as Ollama, LocalAI, vLLM, LM Studio, etc.
/// </summary>
public class OpenAICompatibleChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly OpenAICompatibleConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "openai-compatible";

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
        // OpenAI-compatible endpoints support image URLs natively for vision models
        var supported = new Dictionary<string, IReadOnlyList<string>>
        {
            ["image/*"] = new List<string> { ".*" }.AsReadOnly()
        };

        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAICompatibleChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "llama2", "mistral", "gpt-3.5-turbo").</param>
    /// <param name="config">The OpenAI-compatible configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public OpenAICompatibleChatLanguageModel(
        string modelId,
        OpenAICompatibleConfiguration config,
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

        var result = await response.Content.ReadFromJsonAsync<OpenAICompatibleResponse>(cancellationToken)
            ?? throw new OpenAICompatibleException("Failed to deserialize OpenAI-compatible response");

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
        var toolCallsInProgress = new Dictionary<int, ToolCallBuilder>();

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line) || line == "data: [DONE]")
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                var chunk = JsonSerializer.Deserialize<OpenAICompatibleStreamResponse>(data);

                if (chunk?.Choices.Count > 0)
                {
                    var choice = chunk.Choices[0];
                    var delta = choice.Delta;

// Reasoning arrives as a separate field on the streaming
                    // delta (carried by aggregators like OpenRouter for
                    // reasoning-capable models — DeepSeek R1, Grok, etc.).
                    // Some providers put it at the choice level instead of (or
                    // in addition to) the delta. We check both so reasoning is
                    // never missed.
                    // Emit it as ReasoningDelta so the consumer can render
                    // it as a distinct thinking block instead of mixing it
                    // into the answer.
                    var reasoningText = delta.Reasoning ?? choice.Reasoning;
                    if (!string.IsNullOrEmpty(reasoningText))
                    {
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.ReasoningDelta,
                            Id = chunk.Id,
                            ReasoningContent = reasoningText
                        };
                    }

                    if (!string.IsNullOrEmpty(delta.Content))
                    {
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.TextDelta,
                            Delta = delta.Content
                        };
                    }

                    if (delta.ToolCalls != null)
                    {
                        foreach (var toolCallDelta in delta.ToolCalls)
                        {
                            if (!toolCallsInProgress.TryGetValue(toolCallDelta.Index, out var builder))
                            {
                                builder = new ToolCallBuilder();
                                toolCallsInProgress[toolCallDelta.Index] = builder;
                            }

                            if (!string.IsNullOrEmpty(toolCallDelta.Id))
                            {
                                builder.Id = toolCallDelta.Id;
                            }

                            if (!string.IsNullOrEmpty(toolCallDelta.Function?.Name))
                            {
                                builder.Name = toolCallDelta.Function.Name;
                            }

                            if (!string.IsNullOrEmpty(toolCallDelta.Function?.Arguments))
                            {
                                builder.Arguments.Append(toolCallDelta.Function.Arguments);
                            }
                        }
                    }

                    if (choice.FinishReason != null)
                    {
                        foreach (var toolCall in toolCallsInProgress.OrderBy(pair => pair.Key).Select(pair => pair.Value))
                        {
                            if (string.IsNullOrWhiteSpace(toolCall.Id))
                            {
                                throw new OpenAICompatibleException("OpenAI-compatible stream emitted a tool call without an id");
                            }

                            if (string.IsNullOrWhiteSpace(toolCall.Name))
                            {
                                throw new OpenAICompatibleException("OpenAI-compatible stream emitted a tool call without a function name");
                            }

                            yield return new LanguageModelStreamChunk
                            {
                                Type = ChunkType.ToolCallDelta,
                                ToolCall = new ToolCall(
                                    toolCall.Id,
                                    toolCall.Name,
                                    JsonDocument.Parse(toolCall.Arguments.Length > 0 ? toolCall.Arguments.ToString() : "{}"))
                            };
                        }
                        toolCallsInProgress.Clear();

                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.Finish,
                            FinishReason = MapFinishReason(choice.FinishReason),
                            Usage = chunk.Usage != null ? MapUsage(chunk.Usage) : null
                        };
                    }
                }
            }
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(NormalizeBaseUrl(_config.BaseUrl));

        // Only add Authorization header if API key is provided
        if (!string.IsNullOrEmpty(_config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);
        }

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private static string NormalizeBaseUrl(string baseUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        return baseUrl.EndsWith("/", StringComparison.Ordinal) ? baseUrl : $"{baseUrl}/";
    }

    private OpenAICompatibleRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        var messages = options.Messages.Select(m => new OpenAICompatibleMessage
        {
            Role = MapRole(m.Role),
            Content = m.Content,
            ToolCalls = m.ToolCalls?.Select(tc => new OpenAICompatibleToolCall
            {
                Id = tc.ToolCallId,
                Function = new OpenAICompatibleFunctionCall
                {
                    Name = tc.ToolName,
                    Arguments = tc.Arguments.RootElement.GetRawText()
                }
            }).ToList(),
            // Tool role messages use Name as ToolCallId
            ToolCallId = m.Role == MessageRole.Tool ? m.Name : null
        }).ToList();

        var request = new OpenAICompatibleRequest
        {
            Model = _modelId,
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            Stop = options.StopSequences,
            Stream = stream
        };

// Ask reasoning-capable models (via OpenRouter etc.) to emit thinking tokens.
        // OpenRouter requires enabled:true (not just effort) to activate reasoning:
        //   https://openrouter.ai/docs/features/reasoning
        // Only sent when an effort is configured, so default requests are unchanged.
        if (!string.IsNullOrWhiteSpace(_config.ReasoningEffort))
        {
            request = request with { Reasoning = new { enabled = true, effort = _config.ReasoningEffort } };
        }

        if (options.Tools?.Count > 0)
        {
            request = request with
            {
                Tools = options.Tools.Select(t => new OpenAICompatibleTool
                {
                    Function = new OpenAICompatibleFunction
                    {
                        Name = t.Name,
                        Description = t.Description,
                        Parameters = t.Parameters != null ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement : new { }
                    }
                }).ToList(),
                ToolChoice = options.ToolChoice != null ? new { type = "function", function = new { name = options.ToolChoice } } : null
            };
        }

        return request;
    }

    private sealed class ToolCallBuilder
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public StringBuilder Arguments { get; } = new();
    }

    private static LanguageModelGenerateResult MapToGenerateResult(OpenAICompatibleResponse response)
    {
        var choice = response.Choices[0];
        var message = choice.Message;

        return new LanguageModelGenerateResult
        {
            Text = message?.Content,
            // Reasoning is a separate field on the result, not concatenated
            // into Text — the consumer renders it as a distinct thinking
            // block instead of letting chain-of-thought leak into the answer.
            ReasoningContent = message?.Reasoning,
            FinishReason = MapFinishReason(choice.FinishReason),
            Usage = response.Usage != null ? MapUsage(response.Usage) : new Usage(),
            ToolCalls = message?.ToolCalls?.Select(tc => new ToolCall(
                ToolCallId: tc.Id,
                ToolName: tc.Function.Name,
                Arguments: JsonDocument.Parse(tc.Function.Arguments)
            )).ToList()
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

    private static Usage MapUsage(OpenAICompatibleUsage usage) => new Usage(
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
                var errorResponse = JsonSerializer.Deserialize<OpenAICompatibleErrorResponse>(content);
                throw new OpenAICompatibleException(
                    errorResponse?.Error.Message ?? "OpenAI-compatible API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new OpenAICompatibleException($"OpenAI-compatible API error: {content}", (int)response.StatusCode, null);
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
}
