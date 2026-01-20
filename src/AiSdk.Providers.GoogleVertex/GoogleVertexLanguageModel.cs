using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.GoogleVertex.Exceptions;
using AiSdk.Providers.GoogleVertex.Models;

namespace AiSdk.Providers.GoogleVertex;

/// <summary>
/// Google Vertex AI implementation of ILanguageModel.
/// Supports both Gemini (Google) and Claude (Anthropic) models through Vertex AI.
/// </summary>
public class GoogleVertexLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly GoogleVertexConfiguration _config;
    private readonly string _modelId;
    private readonly bool _ownsHttpClient;
    private readonly ModelType _modelType;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "google-vertex";

    /// <summary>
    /// Gets the provider-specific model identifier.
    /// </summary>
    public string ModelId => _modelId;

    private enum ModelType
    {
        Gemini,
        Claude
    }

    /// <summary>
    /// Gets the supported URL patterns by media type for this provider.
    /// </summary>
    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        // Both Gemini and Claude models support images
        var supported = new Dictionary<string, IReadOnlyList<string>>
        {
            ["image/*"] = new List<string> { ".*" }.AsReadOnly()
        };

        // Gemini also supports audio and video
        if (_modelType == ModelType.Gemini)
        {
            supported["audio/*"] = new List<string> { ".*" }.AsReadOnly();
            supported["video/*"] = new List<string> { ".*" }.AsReadOnly();
        }

        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleVertexLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "gemini-1.5-pro", "claude-3-5-sonnet").</param>
    /// <param name="config">The Google Vertex AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public GoogleVertexLanguageModel(
        string modelId,
        GoogleVertexConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

        _modelId = modelId;
        _config = config;
        _ownsHttpClient = httpClient == null;
        _httpClient = httpClient ?? new HttpClient();

        // Detect model type from model ID
        _modelType = DetectModelType(modelId);

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

        if (_modelType == ModelType.Gemini)
        {
            return await GenerateGeminiAsync(options, cancellationToken);
        }
        else
        {
            return await GenerateClaudeAsync(options, cancellationToken);
        }
    }

    /// <summary>
    /// Streams text generation from the language model.
    /// </summary>
    public IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (_modelType == ModelType.Gemini)
        {
            return StreamGeminiAsync(options, cancellationToken);
        }
        else
        {
            return StreamClaudeAsync(options, cancellationToken);
        }
    }

    private static ModelType DetectModelType(string modelId)
    {
        var lowerModelId = modelId.ToLowerInvariant();

        if (lowerModelId.Contains("gemini"))
        {
            return ModelType.Gemini;
        }
        else if (lowerModelId.Contains("claude"))
        {
            return ModelType.Claude;
        }
        else
        {
            throw new GoogleVertexException($"Unable to detect model type from model ID: {modelId}. Model ID must contain 'gemini' or 'claude'.");
        }
    }

    private void ConfigureHttpClient()
    {
        var baseUrl = _config.BaseUrl ?? $"https://{_config.Location}-aiplatform.googleapis.com/v1";
        _httpClient.BaseAddress = new Uri(baseUrl);

        // Set authorization header
        if (!string.IsNullOrWhiteSpace(_config.AccessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.AccessToken);
        }

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    #region Gemini Implementation

    private async Task<LanguageModelGenerateResult> GenerateGeminiAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken)
    {
        var request = BuildGeminiRequest(options);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"projects/{_config.ProjectId}/locations/{_config.Location}/publishers/google/models/{_modelId}:generateContent";
        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<VertexGeminiResponse>(cancellationToken)
            ?? throw new GoogleVertexException("Failed to deserialize Vertex AI Gemini response");

        return MapGeminiToGenerateResult(result);
    }

    private async IAsyncEnumerable<LanguageModelStreamChunk> StreamGeminiAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = BuildGeminiRequest(options);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"projects/{_config.ProjectId}/locations/{_config.Location}/publishers/google/models/{_modelId}:streamGenerateContent";
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            // Vertex AI streams JSON objects
            var jsonLine = line.Trim();

            if (string.IsNullOrWhiteSpace(jsonLine) || jsonLine == "[DONE]")
                continue;

            VertexGeminiStreamResponse? chunk;
            try
            {
                chunk = JsonSerializer.Deserialize<VertexGeminiStreamResponse>(jsonLine);
            }
            catch (JsonException)
            {
                // Skip malformed JSON chunks
                continue;
            }

            if (chunk?.Candidates == null || chunk.Candidates.Count == 0)
                continue;

            var candidate = chunk.Candidates[0];
            var candidateContent = candidate.Content;

            if (candidateContent?.Parts != null)
            {
                foreach (var part in candidateContent.Parts)
                {
                    if (!string.IsNullOrEmpty(part.Text))
                    {
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.TextDelta,
                            Delta = part.Text
                        };
                    }

                    if (part.FunctionCall != null)
                    {
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.ToolCallDelta,
                            ToolCall = new ToolCall(
                                ToolCallId: Guid.NewGuid().ToString(),
                                ToolName: part.FunctionCall.Name,
                                Arguments: JsonDocument.Parse(JsonSerializer.Serialize(part.FunctionCall.Args))
                            )
                        };
                    }
                }
            }

            if (candidate.FinishReason != null)
            {
                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.Finish,
                    FinishReason = MapGeminiFinishReason(candidate.FinishReason),
                    Usage = chunk.UsageMetadata != null ? MapGeminiUsage(chunk.UsageMetadata) : null
                };
            }
        }
    }

    private VertexGeminiRequest BuildGeminiRequest(LanguageModelCallOptions options)
    {
        var contents = new List<VertexGeminiContent>();

        foreach (var message in options.Messages)
        {
            var role = MapGeminiRole(message.Role);

            if (message.Role == MessageRole.System)
                continue;

            var parts = new List<VertexGeminiPart>();

            if (message.Role == MessageRole.Tool)
            {
                parts.Add(new VertexGeminiPart
                {
                    FunctionResponse = new VertexGeminiFunctionResponse
                    {
                        Name = message.Name ?? "unknown",
                        Response = new { result = message.Content }
                    }
                });
            }
            else
            {
                if (!string.IsNullOrEmpty(message.Content))
                {
                    parts.Add(new VertexGeminiPart
                    {
                        Text = message.Content
                    });
                }
            }

            contents.Add(new VertexGeminiContent
            {
                Role = role,
                Parts = parts
            });
        }

        var request = new VertexGeminiRequest
        {
            Contents = contents,
            GenerationConfig = new VertexGeminiGenerationConfig
            {
                MaxOutputTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences?.ToList()
            }
        };

        if (options.Tools?.Count > 0)
        {
            var functionDeclarations = options.Tools.Select(t => new VertexGeminiFunctionDeclaration
            {
                Name = t.Name,
                Description = t.Description,
                Parameters = t.Parameters != null ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement : new { }
            }).ToList();

            request = request with
            {
                Tools = new List<VertexGeminiTool>
                {
                    new VertexGeminiTool { FunctionDeclarations = functionDeclarations }
                }
            };

            if (options.ToolChoice != null)
            {
                request = request with
                {
                    ToolConfig = new VertexGeminiToolConfig
                    {
                        FunctionCallingConfig = new VertexGeminiFunctionCallingConfig
                        {
                            Mode = "ANY",
                            AllowedFunctionNames = new List<string> { options.ToolChoice }
                        }
                    }
                };
            }
        }

        return request;
    }

    private static LanguageModelGenerateResult MapGeminiToGenerateResult(VertexGeminiResponse response)
    {
        if (response.Candidates == null || response.Candidates.Count == 0)
        {
            throw new GoogleVertexException("No candidates in Gemini response");
        }

        var candidate = response.Candidates[0];
        var content = candidate.Content;

        string? text = null;
        List<ToolCall>? toolCalls = null;

        if (content?.Parts != null)
        {
            var textParts = new List<string>();
            var calls = new List<ToolCall>();

            foreach (var part in content.Parts)
            {
                if (!string.IsNullOrEmpty(part.Text))
                {
                    textParts.Add(part.Text);
                }

                if (part.FunctionCall != null)
                {
                    calls.Add(new ToolCall(
                        ToolCallId: Guid.NewGuid().ToString(),
                        ToolName: part.FunctionCall.Name,
                        Arguments: JsonDocument.Parse(JsonSerializer.Serialize(part.FunctionCall.Args))
                    ));
                }
            }

            text = textParts.Count > 0 ? string.Join("", textParts) : null;
            toolCalls = calls.Count > 0 ? calls : null;
        }

        return new LanguageModelGenerateResult
        {
            Text = text,
            FinishReason = MapGeminiFinishReason(candidate.FinishReason),
            Usage = response.UsageMetadata != null ? MapGeminiUsage(response.UsageMetadata) : new Usage(),
            ToolCalls = toolCalls
        };
    }

    private static string MapGeminiRole(MessageRole role) => role switch
    {
        MessageRole.System => "user",
        MessageRole.User => "user",
        MessageRole.Assistant => "model",
        MessageRole.Tool => "function",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static FinishReason MapGeminiFinishReason(string? reason) => reason switch
    {
        "STOP" => FinishReason.Stop,
        "MAX_TOKENS" => FinishReason.Length,
        "SAFETY" => FinishReason.ContentFilter,
        "RECITATION" => FinishReason.ContentFilter,
        _ => FinishReason.Other
    };

    private static Usage MapGeminiUsage(VertexGeminiUsageMetadata usage) => new Usage(
        InputTokens: usage.PromptTokenCount,
        OutputTokens: usage.CandidatesTokenCount,
        TotalTokens: usage.TotalTokenCount
    );

    #endregion

    #region Claude Implementation

    private async Task<LanguageModelGenerateResult> GenerateClaudeAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken)
    {
        var request = BuildClaudeRequest(options, stream: false);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"projects/{_config.ProjectId}/locations/{_config.Location}/publishers/anthropic/models/{_modelId}:streamRawPredict";
        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<VertexClaudeResponse>(cancellationToken)
            ?? throw new GoogleVertexException("Failed to deserialize Vertex AI Claude response");

        return MapClaudeToGenerateResult(result);
    }

    private async IAsyncEnumerable<LanguageModelStreamChunk> StreamClaudeAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var request = BuildClaudeRequest(options, stream: true);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"projects/{_config.ProjectId}/locations/{_config.Location}/publishers/anthropic/models/{_modelId}:streamRawPredict";
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var toolCallsInProgress = new Dictionary<int, ToolCallBuilder>();
        VertexClaudeUsage? finalUsage = null;
        string? finishReason = null;

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                var streamEvent = JsonSerializer.Deserialize<VertexClaudeStreamChunk>(data);

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
                        else if (streamEvent.Delta?.Type == "input_json_delta" && !string.IsNullOrEmpty(streamEvent.Delta.PartialJson) && streamEvent.Index.HasValue)
                        {
                            if (toolCallsInProgress.TryGetValue(streamEvent.Index.Value, out var builder))
                            {
                                builder.JsonArguments.Append(streamEvent.Delta.PartialJson);
                            }
                        }
                        break;

                    case "content_block_stop":
                        if (streamEvent.Index.HasValue && toolCallsInProgress.TryGetValue(streamEvent.Index.Value, out var completedBuilder))
                        {
                            var argsJson = completedBuilder.JsonArguments.ToString();
                            var argsDoc = string.IsNullOrWhiteSpace(argsJson) ? JsonDocument.Parse("{}") : JsonDocument.Parse(argsJson);

                            yield return new LanguageModelStreamChunk
                            {
                                Type = ChunkType.ToolCallDelta,
                                ToolCall = new ToolCall(
                                    ToolCallId: completedBuilder.Id,
                                    ToolName: completedBuilder.Name,
                                    Arguments: argsDoc
                                )
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
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.Finish,
                            FinishReason = MapClaudeFinishReason(finishReason),
                            Usage = finalUsage != null ? MapClaudeUsage(finalUsage) : null
                        };
                        break;
                }
            }
        }
    }

    private VertexClaudeRequest BuildClaudeRequest(LanguageModelCallOptions options, bool stream)
    {
        var messages = new List<VertexClaudeMessage>();
        string? systemMessage = null;

        foreach (var message in options.Messages)
        {
            if (message.Role == MessageRole.System)
            {
                systemMessage = message.Content;
                continue;
            }

            var role = MapClaudeRole(message.Role);
            object content;

            if (message.Role == MessageRole.Tool)
            {
                content = new List<VertexClaudeContentBlock>
                {
                    new VertexClaudeContentBlock
                    {
                        Type = "tool_result",
                        ToolUseId = message.Name ?? "unknown",
                        Content = message.Content
                    }
                };
            }
            else
            {
                content = message.Content ?? "";
            }

            messages.Add(new VertexClaudeMessage
            {
                Role = role,
                Content = content
            });
        }

        var request = new VertexClaudeRequest
        {
            Messages = messages,
            MaxTokens = options.MaxTokens ?? 4096,
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
                Tools = options.Tools.Select(t => new VertexClaudeTool
                {
                    Name = t.Name,
                    Description = t.Description,
                    InputSchema = t.Parameters != null ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement : new { }
                }).ToList()
            };

            if (options.ToolChoice != null)
            {
                request = request with
                {
                    ToolChoice = new { type = "tool", name = options.ToolChoice }
                };
            }
        }

        return request;
    }

    private static LanguageModelGenerateResult MapClaudeToGenerateResult(VertexClaudeResponse response)
    {
        string? text = null;
        List<ToolCall>? toolCalls = null;

        if (response.Content != null)
        {
            var textParts = new List<string>();
            var calls = new List<ToolCall>();

            foreach (var block in response.Content)
            {
                if (block.Type == "text" && !string.IsNullOrEmpty(block.Text))
                {
                    textParts.Add(block.Text);
                }
                else if (block.Type == "tool_use")
                {
                    calls.Add(new ToolCall(
                        ToolCallId: block.Id ?? Guid.NewGuid().ToString(),
                        ToolName: block.Name ?? "unknown",
                        Arguments: block.Input != null ? JsonDocument.Parse(JsonSerializer.Serialize(block.Input)) : JsonDocument.Parse("{}")
                    ));
                }
            }

            text = textParts.Count > 0 ? string.Join("", textParts) : null;
            toolCalls = calls.Count > 0 ? calls : null;
        }

        return new LanguageModelGenerateResult
        {
            Text = text,
            FinishReason = MapClaudeFinishReason(response.StopReason),
            Usage = MapClaudeUsage(response.Usage),
            ToolCalls = toolCalls
        };
    }

    private static string MapClaudeRole(MessageRole role) => role switch
    {
        MessageRole.User => "user",
        MessageRole.Assistant => "assistant",
        MessageRole.Tool => "user",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static FinishReason MapClaudeFinishReason(string? reason) => reason switch
    {
        "end_turn" => FinishReason.Stop,
        "max_tokens" => FinishReason.Length,
        "stop_sequence" => FinishReason.Stop,
        "tool_use" => FinishReason.ToolCalls,
        _ => FinishReason.Other
    };

    private static Usage MapClaudeUsage(VertexClaudeUsage usage) => new Usage(
        InputTokens: usage.InputTokens,
        OutputTokens: usage.OutputTokens,
        TotalTokens: usage.InputTokens + usage.OutputTokens
    );

    #endregion

    #region Helper Methods

    private async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<VertexErrorResponse>(content);
                throw new GoogleVertexException(
                    errorResponse?.Error.Message ?? "Vertex AI error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Status,
                    _modelType.ToString());
            }
            catch (JsonException)
            {
                throw new GoogleVertexException($"Vertex AI error: {content}", (int)response.StatusCode, null, _modelType.ToString());
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

    #endregion
}
