using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Google.Exceptions;
using AiSdk.Providers.Google.Models;

namespace AiSdk.Providers.Google;

/// <summary>
/// Google Gemini implementation of ILanguageModel.
/// </summary>
public class GoogleChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly GoogleConfiguration _config;
    private readonly string _modelId;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "google";

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
        // Google Gemini supports various media types natively
        var supported = new Dictionary<string, IReadOnlyList<string>>
        {
            ["image/*"] = new List<string> { ".*" }.AsReadOnly(),
            ["audio/*"] = new List<string> { ".*" }.AsReadOnly(),
            ["video/*"] = new List<string> { ".*" }.AsReadOnly()
        };

        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Google model ID (e.g., "gemini-1.5-pro", "gemini-1.5-flash").</param>
    /// <param name="config">The Google configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public GoogleChatLanguageModel(
        string modelId,
        GoogleConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        _modelId = modelId;
        _config = config;
        _ownsHttpClient = httpClient == null;
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

        var request = BuildRequest(options);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"models/{_modelId}:generateContent?key={_config.ApiKey}";
        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<GoogleResponse>(cancellationToken)
            ?? throw new GoogleException("Failed to deserialize Google response");

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

        var request = BuildRequest(options);
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var endpoint = $"models/{_modelId}:streamGenerateContent?key={_config.ApiKey}";
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

            // Google streams JSON objects, sometimes with "data: " prefix
            var jsonLine = line.StartsWith("data: ") ? line.Substring(6) : line;

            if (string.IsNullOrWhiteSpace(jsonLine) || jsonLine == "[DONE]")
                continue;

            GoogleStreamResponse? chunk;
            try
            {
                chunk = JsonSerializer.Deserialize<GoogleStreamResponse>(jsonLine);
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
                    FinishReason = MapFinishReason(candidate.FinishReason),
                    Usage = chunk.UsageMetadata != null ? MapUsage(chunk.UsageMetadata) : null
                };
            }
        }
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private GoogleRequest BuildRequest(LanguageModelCallOptions options)
    {
        var contents = new List<GoogleContent>();

        // Group messages by role to create contents
        // Google requires alternating user/model messages, system messages go in systemInstruction
        foreach (var message in options.Messages)
        {
            var role = MapRole(message.Role);

            // Skip system messages for now (they should be handled separately in production)
            if (message.Role == MessageRole.System)
                continue;

            var parts = new List<GooglePart>();

            if (message.Role == MessageRole.Tool)
            {
                // Tool results
                parts.Add(new GooglePart
                {
                    FunctionResponse = new GoogleFunctionResponse
                    {
                        Name = message.Name ?? "unknown",
                        Response = new { result = message.Content }
                    }
                });
            }
            else if (message.Parts != null && message.Parts.Count > 0)
            {
                // Multi-modal content parts
                foreach (var part in message.Parts)
                {
                    switch (part)
                    {
                        case TextPart textPart:
                            parts.Add(new GooglePart { Text = textPart.Text });
                            break;

                        case ImagePart imagePart:
                            if (imagePart.Data != null)
                            {
                                parts.Add(new GooglePart
                                {
                                    InlineData = new GoogleInlineData
                                    {
                                        MimeType = imagePart.MimeType ?? "image/png",
                                        Data = Convert.ToBase64String(imagePart.Data)
                                    }
                                });
                            }
                            else if (imagePart.Url != null)
                            {
                                parts.Add(new GooglePart
                                {
                                    FileData = new GoogleFileData
                                    {
                                        MimeType = imagePart.MimeType ?? "image/png",
                                        FileUri = imagePart.Url
                                    }
                                });
                            }
                            break;

                        case AudioPart audioPart:
                            if (audioPart.Data != null)
                            {
                                parts.Add(new GooglePart
                                {
                                    InlineData = new GoogleInlineData
                                    {
                                        MimeType = audioPart.MimeType ?? "audio/wav",
                                        Data = Convert.ToBase64String(audioPart.Data)
                                    }
                                });
                            }
                            else if (audioPart.Url != null)
                            {
                                parts.Add(new GooglePart
                                {
                                    FileData = new GoogleFileData
                                    {
                                        MimeType = audioPart.MimeType ?? "audio/wav",
                                        FileUri = audioPart.Url
                                    }
                                });
                            }
                            break;

                        case FilePart filePart:
                            if (filePart.Data != null)
                            {
                                parts.Add(new GooglePart
                                {
                                    InlineData = new GoogleInlineData
                                    {
                                        MimeType = filePart.MimeType,
                                        Data = Convert.ToBase64String(filePart.Data)
                                    }
                                });
                            }
                            else
                            {
                                parts.Add(new GooglePart
                                {
                                    FileData = new GoogleFileData
                                    {
                                        MimeType = filePart.MimeType,
                                        FileUri = filePart.Url
                                    }
                                });
                            }
                            break;
                    }
                }
            }
            else
            {
                // Regular text content
                if (!string.IsNullOrEmpty(message.Content))
                {
                    parts.Add(new GooglePart
                    {
                        Text = message.Content
                    });
                }
            }

            contents.Add(new GoogleContent
            {
                Role = role,
                Parts = parts
            });
        }

        var request = new GoogleRequest
        {
            Contents = contents,
            GenerationConfig = new GoogleGenerationConfig
            {
                MaxOutputTokens = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences?.ToList()
            }
        };

        if (options.Tools?.Count > 0)
        {
            var functionDeclarations = options.Tools.Select(t => new GoogleFunctionDeclaration
            {
                Name = t.Name,
                Description = t.Description,
                Parameters = t.Parameters != null ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement : new { }
            }).ToList();

            request = request with
            {
                Tools = new List<GoogleTool>
                {
                    new GoogleTool { FunctionDeclarations = functionDeclarations }
                }
            };

            if (options.ToolChoice != null)
            {
                request = request with
                {
                    ToolConfig = new GoogleToolConfig
                    {
                        FunctionCallingConfig = new GoogleFunctionCallingConfig
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

    private static LanguageModelGenerateResult MapToGenerateResult(GoogleResponse response)
    {
        if (response.Candidates == null || response.Candidates.Count == 0)
        {
            throw new GoogleException("No candidates in response");
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
            FinishReason = MapFinishReason(candidate.FinishReason),
            Usage = response.UsageMetadata != null ? MapUsage(response.UsageMetadata) : new Usage(),
            ToolCalls = toolCalls
        };
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "STOP" => FinishReason.Stop,
        "MAX_TOKENS" => FinishReason.Length,
        "SAFETY" => FinishReason.ContentFilter,
        "RECITATION" => FinishReason.ContentFilter,
        _ => FinishReason.Other
    };

    private static Usage MapUsage(GoogleUsageMetadata usage) => new Usage(
        InputTokens: usage.PromptTokenCount,
        OutputTokens: usage.CandidatesTokenCount,
        TotalTokens: usage.TotalTokenCount
    );

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.System => "user", // Google doesn't have system role, treat as user
        MessageRole.User => "user",
        MessageRole.Assistant => "model",
        MessageRole.Tool => "function", // Tool responses use function role
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<GoogleErrorResponse>(content);
                throw new GoogleException(
                    errorResponse?.Error.Message ?? "Google API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Status);
            }
            catch (JsonException)
            {
                throw new GoogleException($"Google API error: {content}", (int)response.StatusCode, null);
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
