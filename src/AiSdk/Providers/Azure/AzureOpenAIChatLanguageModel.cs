using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Azure.Exceptions;
using AiSdk.Providers.Azure.Models;

namespace AiSdk.Providers.Azure;

/// <summary>
/// Azure OpenAI implementation of ILanguageModel.
/// </summary>
public class AzureOpenAIChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly AzureOpenAIConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "azure-openai";

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
        // Azure OpenAI supports image URLs natively for vision models
        var supported = new Dictionary<string, IReadOnlyList<string>>
        {
            ["image/*"] = new List<string> { ".*" }.AsReadOnly()
        };

        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The model identifier for display purposes.</param>
    /// <param name="config">The Azure OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public AzureOpenAIChatLanguageModel(
        string modelId,
        AzureOpenAIConfiguration config,
        HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(config);

        config.Validate();

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

        var endpoint = BuildEndpoint("chat/completions");
        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<AzureOpenAIResponse>(cancellationToken)
            ?? throw new AzureOpenAIException("Failed to deserialize Azure OpenAI response");

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

        var endpoint = BuildEndpoint("chat/completions");
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line) || line == "data: [DONE]")
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                var chunk = JsonSerializer.Deserialize<AzureOpenAIStreamResponse>(data);

                if (chunk?.Choices.Count > 0)
                {
                    var choice = chunk.Choices[0];
                    var delta = choice.Delta;

                    if (!string.IsNullOrEmpty(delta.Content))
                    {
                        yield return new LanguageModelStreamChunk
                        {
                            Type = ChunkType.TextDelta,
                            Delta = delta.Content
                        };
                    }

                    if (choice.FinishReason != null)
                    {
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
        // Azure OpenAI uses api-key header instead of Bearer token
        if (!string.IsNullOrWhiteSpace(_config.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Remove("api-key");
            _httpClient.DefaultRequestHeaders.Add("api-key", _config.ApiKey);
        }
        else if (!string.IsNullOrWhiteSpace(_config.AzureAdToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config.AzureAdToken);
        }

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private string BuildEndpoint(string path)
    {
        // Azure OpenAI endpoint format:
        // https://{resource}.openai.azure.com/openai/deployments/{deployment-id}/chat/completions?api-version={version}
        var endpoint = _config.Endpoint.TrimEnd('/');
        var deploymentName = _config.DeploymentName;
        var apiVersion = _config.ApiVersion;

        return $"{endpoint}/openai/deployments/{deploymentName}/{path}?api-version={apiVersion}";
    }

    private AzureOpenAIRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        var messages = options.Messages.Select(m => new AzureOpenAIMessage
        {
            Role = MapRole(m.Role),
            Content = BuildMessageContent(m),
            // Tool role messages use Name as ToolCallId
            ToolCallId = m.Role == MessageRole.Tool ? m.Name : null
        }).ToList();

        var request = new AzureOpenAIRequest
        {
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
                Tools = options.Tools.Select(t => new AzureOpenAITool
                {
                    Function = new AzureOpenAIFunction
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

    /// <summary>
    /// Builds the content for an Azure OpenAI message, supporting multi-modal content parts.
    /// Azure OpenAI uses the same format as OpenAI for vision support.
    /// </summary>
    private static object? BuildMessageContent(Message message)
    {
        if (message.Parts == null || message.Parts.Count == 0)
        {
            return message.Content;
        }

        var parts = new List<AzureOpenAIContentPart>();

        foreach (var part in message.Parts)
        {
            switch (part)
            {
                case TextPart textPart:
                    parts.Add(new AzureOpenAIContentPart
                    {
                        Type = "text",
                        Text = textPart.Text
                    });
                    break;

                case ImagePart imagePart:
                    var imageUrl = imagePart.Url;
                    if (imageUrl == null && imagePart.Data != null)
                    {
                        var mimeType = imagePart.MimeType ?? "image/png";
                        imageUrl = $"data:{mimeType};base64,{Convert.ToBase64String(imagePart.Data)}";
                    }

                    if (imageUrl != null)
                    {
                        parts.Add(new AzureOpenAIContentPart
                        {
                            Type = "image_url",
                            ImageUrl = new AzureOpenAIImageUrl { Url = imageUrl }
                        });
                    }
                    break;

                case FilePart filePart:
                    parts.Add(new AzureOpenAIContentPart
                    {
                        Type = "image_url",
                        ImageUrl = new AzureOpenAIImageUrl { Url = filePart.Url }
                    });
                    break;
            }
        }

        return parts;
    }

    private static LanguageModelGenerateResult MapToGenerateResult(AzureOpenAIResponse response)
    {
        var choice = response.Choices[0];
        var message = choice.Message;

        return new LanguageModelGenerateResult
        {
            Text = message?.Content?.ToString(),
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

    private static Usage MapUsage(AzureOpenAIUsage usage) => new Usage(
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
                var errorResponse = JsonSerializer.Deserialize<AzureOpenAIErrorResponse>(content);
                throw new AzureOpenAIException(
                    errorResponse?.Error.Message ?? "Azure OpenAI API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new AzureOpenAIException($"Azure OpenAI API error: {content}", (int)response.StatusCode, null);
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
