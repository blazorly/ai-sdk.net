using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Friendli.Exceptions;
using AiSdk.Providers.Friendli.Models;

namespace AiSdk.Providers.Friendli;

/// <summary>
/// Friendli implementation of ILanguageModel.
/// </summary>
public class FriendliChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly FriendliConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "friendli";

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
        // Friendli currently does not support image URLs
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FriendliChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Friendli model ID (e.g., "mixtral-8x7b-instruct-v0-1", "meta-llama-3-1-70b-instruct").</param>
    /// <param name="config">The Friendli configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public FriendliChatLanguageModel(
        string modelId,
        FriendliConfiguration config,
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

        var result = await response.Content.ReadFromJsonAsync<FriendliResponse>(cancellationToken)
            ?? throw new FriendliException("Failed to deserialize Friendli response");

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

        await foreach (var line in ReadLinesAsync(stream, cancellationToken))
        {
            if (string.IsNullOrWhiteSpace(line) || line == "data: [DONE]")
                continue;

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                var chunk = JsonSerializer.Deserialize<FriendliStreamResponse>(data);

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
        _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);

        // Enforce timeout if configured
        if (_config.TimeoutSeconds.HasValue)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds.Value);
        }
    }

    private FriendliRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        var messages = options.Messages.Select(m => new FriendliMessage
        {
            Role = MapRole(m.Role),
            Content = m.Content,
            // Tool role messages use Name as ToolCallId
            ToolCallId = m.Role == MessageRole.Tool ? m.Name : null
        }).ToList();

        var request = new FriendliRequest
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
                Tools = options.Tools.Select(t => new FriendliTool
                {
                    Function = new FriendliFunction
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

    private static LanguageModelGenerateResult MapToGenerateResult(FriendliResponse response)
    {
        var choice = response.Choices[0];
        var message = choice.Message;

        return new LanguageModelGenerateResult
        {
            Text = message?.Content,
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

    private static Usage MapUsage(FriendliUsage usage) => new Usage(
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
                var errorResponse = JsonSerializer.Deserialize<FriendliErrorResponse>(content);
                throw new FriendliException(
                    errorResponse?.Error.Message ?? "Friendli API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new FriendliException($"Friendli API error: {content}", (int)response.StatusCode, null);
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
