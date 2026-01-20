using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Cohere.Exceptions;
using AiSdk.Providers.Cohere.Models;

namespace AiSdk.Providers.Cohere;

/// <summary>
/// Cohere implementation of ILanguageModel.
/// </summary>
public class CohereChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly CohereConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "cohere";

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
        // Cohere does not support image URLs directly in chat API
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Cohere model ID (e.g., "command-r-plus", "command-r", "command").</param>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public CohereChatLanguageModel(
        string modelId,
        CohereConfiguration config,
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

        var response = await _httpClient.PostAsync("chat", content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<CohereResponse>(cancellationToken)
            ?? throw new CohereException("Failed to deserialize Cohere response");

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

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        CohereMeta? finalMeta = null;
        string? finishReason = null;

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var streamEvent = JsonSerializer.Deserialize<CohereStreamResponse>(line);

            if (streamEvent == null)
                continue;

            switch (streamEvent.EventType)
            {
                case "text-generation":
                    if (!string.IsNullOrEmpty(streamEvent.Text))
                    {
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.TextDelta,
                            Delta = streamEvent.Text
                        };
                    }
                    break;

                case "tool-calls-generation":
                    if (streamEvent.ToolCalls != null && streamEvent.ToolCalls.Count > 0)
                    {
                        foreach (var toolCall in streamEvent.ToolCalls)
                        {
                            var toolCallObj = new ToolCall(
                                ToolCallId: toolCall.GenerationId ?? Guid.NewGuid().ToString(),
                                ToolName: toolCall.Name,
                                Arguments: JsonDocument.Parse(JsonSerializer.Serialize(toolCall.Parameters))
                            );

                            yield return new LanguageModelStreamChunk
                            {
                                Type = ChunkType.ToolCallDelta,
                                ToolCall = toolCallObj
                            };
                        }
                    }
                    break;

                case "stream-end":
                    if (streamEvent.Response != null)
                    {
                        finishReason = streamEvent.Response.FinishReason;
                        finalMeta = streamEvent.Response.Meta;
                    }

                    // Send final chunk with finish reason and usage
                    yield return new LanguageModelStreamChunk
                    {
                        Type = ChunkType.Finish,
                        FinishReason = MapFinishReason(finishReason),
                        Usage = finalMeta != null ? MapUsage(finalMeta) : null
                    };
                    break;
            }
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.ApiKey);

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private CohereRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        // Cohere requires separating the latest user message from chat history
        var chatHistory = new List<CohereMessage>();
        string latestMessage = string.Empty;
        List<CohereToolResult>? toolResults = null;

        for (int i = 0; i < options.Messages.Count; i++)
        {
            var msg = options.Messages[i];

            if (msg.Role == MessageRole.System)
            {
                // Skip system messages - Cohere doesn't have dedicated system message support in chat endpoint
                // Could be prepended to first user message if needed
                continue;
            }
            else if (msg.Role == MessageRole.Tool)
            {
                // Tool results need special handling
                toolResults ??= new List<CohereToolResult>();

                // Find the corresponding tool call from the previous assistant message
                var toolCallName = msg.Name ?? "unknown";
                var toolCallParams = new { result = msg.Content };

                toolResults.Add(new CohereToolResult
                {
                    Call = new CohereToolCall
                    {
                        Name = toolCallName,
                        Parameters = toolCallParams
                    },
                    Outputs = new List<object> { new { output = msg.Content } }
                });
            }
            else if (i == options.Messages.Count - 1 && msg.Role == MessageRole.User)
            {
                // Latest message must be from user
                latestMessage = msg.Content ?? string.Empty;
            }
            else
            {
                // Add to chat history
                var cohereMsg = new CohereMessage
                {
                    Role = MapRole(msg.Role),
                    Message = msg.Content ?? string.Empty
                };

                // Check metadata for tool calls (stored from previous assistant response)
                if (msg.Role == MessageRole.Assistant &&
                    msg.Metadata?.TryGetValue("tool_calls", out var toolCallsObj) == true &&
                    toolCallsObj is IReadOnlyList<ToolCall> toolCallsList)
                {
                    cohereMsg = cohereMsg with
                    {
                        ToolCalls = toolCallsList.Select(tc => new CohereToolCall
                        {
                            Name = tc.ToolName,
                            Parameters = JsonSerializer.Deserialize<object>(tc.Arguments.RootElement.GetRawText())!
                        }).ToList()
                    };
                }

                chatHistory.Add(cohereMsg);
            }
        }

        var request = new CohereRequest
        {
            Model = _modelId,
            Message = latestMessage,
            ChatHistory = chatHistory.Count > 0 ? chatHistory : null,
            Temperature = options.Temperature,
            MaxTokens = options.MaxTokens,
            P = options.TopP,
            StopSequences = options.StopSequences,
            Stream = stream,
            ToolResults = toolResults
        };

        if (options.Tools?.Count > 0)
        {
            request = request with
            {
                Tools = options.Tools.Select(t => new CohereTool
                {
                    Name = t.Name,
                    Description = t.Description,
                    ParameterDefinitions = t.Parameters != null
                        ? ConvertJsonSchemaToParameterDefinitions(t.Parameters)
                        : new { }
                }).ToList()
            };
        }

        return request;
    }

    private static object ConvertJsonSchemaToParameterDefinitions(JsonDocument schema)
    {
        // Cohere uses a different format for parameter definitions
        // Extract properties from JSON schema and convert to Cohere format
        var root = schema.RootElement;

        if (root.TryGetProperty("properties", out var properties))
        {
            var definitions = new Dictionary<string, object>();

            foreach (var prop in properties.EnumerateObject())
            {
                var propDef = new Dictionary<string, object>();

                if (prop.Value.TryGetProperty("type", out var type))
                {
                    propDef["type"] = type.GetString() ?? "string";
                }

                if (prop.Value.TryGetProperty("description", out var desc))
                {
                    propDef["description"] = desc.GetString() ?? "";
                }

                if (prop.Value.TryGetProperty("required", out var required) && required.GetBoolean())
                {
                    propDef["required"] = true;
                }

                definitions[prop.Name] = propDef;
            }

            return definitions;
        }

        return new { };
    }

    private static LanguageModelGenerateResult MapToGenerateResult(CohereResponse response)
    {
        List<ToolCall>? toolCalls = null;

        if (response.ToolCalls != null && response.ToolCalls.Count > 0)
        {
            toolCalls = response.ToolCalls.Select(tc => new ToolCall(
                ToolCallId: tc.GenerationId ?? Guid.NewGuid().ToString(),
                ToolName: tc.Name,
                Arguments: JsonDocument.Parse(JsonSerializer.Serialize(tc.Parameters))
            )).ToList();
        }

        return new LanguageModelGenerateResult
        {
            Text = response.Text,
            FinishReason = MapFinishReason(response.FinishReason),
            Usage = response.Meta != null ? MapUsage(response.Meta) : new Usage(0, 0, 0),
            ToolCalls = toolCalls
        };
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "COMPLETE" => FinishReason.Stop,
        "MAX_TOKENS" => FinishReason.Length,
        "ERROR" => FinishReason.Other,
        "ERROR_TOXIC" => FinishReason.ContentFilter,
        "ERROR_LIMIT" => FinishReason.Length,
        "USER_CANCEL" => FinishReason.Stop,
        _ => FinishReason.Other
    };

    private static Usage MapUsage(CohereMeta meta)
    {
        var inputTokens = meta.BilledUnits?.InputTokens ?? meta.Tokens?.InputTokens ?? 0;
        var outputTokens = meta.BilledUnits?.OutputTokens ?? meta.Tokens?.OutputTokens ?? 0;

        return new Usage(
            InputTokens: inputTokens,
            OutputTokens: outputTokens,
            TotalTokens: inputTokens + outputTokens
        );
    }

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.User => "USER",
        MessageRole.Assistant => "CHATBOT",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<CohereErrorResponse>(content);
                throw new CohereException(
                    errorResponse?.Message ?? "Cohere API error",
                    (int)response.StatusCode);
            }
            catch (JsonException)
            {
                throw new CohereException($"Cohere API error: {content}", (int)response.StatusCode);
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
}
