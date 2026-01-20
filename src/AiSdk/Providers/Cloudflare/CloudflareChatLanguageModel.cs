using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Cloudflare.Exceptions;
using AiSdk.Providers.Cloudflare.Models;

namespace AiSdk.Providers.Cloudflare;

/// <summary>
/// Cloudflare Workers AI implementation of ILanguageModel.
/// </summary>
public class CloudflareChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly CloudflareConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "cloudflare";

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
        // Cloudflare Workers AI currently does not support image URLs
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudflareChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Cloudflare Workers AI model ID (e.g., "@cf/meta/llama-3-8b-instruct").</param>
    /// <param name="config">The Cloudflare configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public CloudflareChatLanguageModel(
        string modelId,
        CloudflareConfiguration config,
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

        var endpoint = $"accounts/{_config.AccountId}/ai/run/{_modelId}";
        var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
        await EnsureSuccessStatusCode(response);

        var result = await response.Content.ReadFromJsonAsync<CloudflareResponse>(cancellationToken)
            ?? throw new CloudflareException("Failed to deserialize Cloudflare response");

        if (!result.Success)
        {
            var errorMessage = result.Errors.Count > 0
                ? result.Errors[0].Message
                : "Unknown Cloudflare error";
            throw new CloudflareException(errorMessage);
        }

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

        var endpoint = $"accounts/{_config.AccountId}/ai/run/{_modelId}";
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

            if (line.StartsWith("data: "))
            {
                var data = line.Substring(6).Trim();

                if (data == "[DONE]")
                {
                    yield return new LanguageModelStreamChunk
                    {
                        Type = ChunkType.Finish,
                        FinishReason = FinishReason.Stop,
                        Usage = new Usage()
                    };
                    continue;
                }

                CloudflareStreamResponse? chunk = null;
                try
                {
                    chunk = JsonSerializer.Deserialize<CloudflareStreamResponse>(data);
                }
                catch (JsonException)
                {
                    // Skip malformed chunks
                    continue;
                }

                if (chunk?.Response != null)
                {
                    yield return new LanguageModelStreamChunk
                    {
                        Type = ChunkType.TextDelta,
                        Delta = chunk.Response
                    };
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

    private CloudflareRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        var messages = options.Messages.Select(m => new CloudflareMessage
        {
            Role = MapRole(m.Role),
            Content = m.Content ?? string.Empty
        }).ToList();

        var request = new CloudflareRequest
        {
            Messages = messages,
            MaxTokens = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP,
            Stream = stream ? true : null
        };

        return request;
    }

    private static LanguageModelGenerateResult MapToGenerateResult(CloudflareResponse response)
    {
        return new LanguageModelGenerateResult
        {
            Text = response.Result.Response,
            FinishReason = FinishReason.Stop,
            Usage = new Usage()
        };
    }

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.System => "system",
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
                var errorResponse = JsonSerializer.Deserialize<CloudflareErrorResponse>(content);

                if (errorResponse?.Errors.Count > 0)
                {
                    var error = errorResponse.Errors[0];
                    throw new CloudflareException(
                        error.Message,
                        (int)response.StatusCode,
                        error.Code.ToString());
                }
            }
            catch (JsonException)
            {
                // Fall through to generic error
            }

            throw new CloudflareException($"Cloudflare Workers AI error: {content}", (int)response.StatusCode, null);
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
