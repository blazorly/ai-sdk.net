using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.Luma.Exceptions;
using AiSdk.Providers.Luma.Models;

namespace AiSdk.Providers.Luma;

/// <summary>
/// Luma AI implementation of ILanguageModel.
/// Note: This is a placeholder implementation. Luma AI specializes in video generation (Dream Machine),
/// not language models. This structure is designed for future expansion to support video generation features.
/// </summary>
public class LumaChatLanguageModel : ILanguageModel
{
    private readonly HttpClient _httpClient;
    private readonly LumaConfiguration _config;
    private readonly string _modelId;

    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    public string SpecificationVersion => "v1";

    /// <summary>
    /// Gets the provider identifier.
    /// </summary>
    public string Provider => "luma";

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
        // Luma AI specializes in video generation, not image URLs in chat
        // Future: Could support image-to-video URLs
        var supported = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(supported);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LumaChatLanguageModel"/> class.
    /// </summary>
    /// <param name="modelId">The Luma model ID (e.g., "dream-machine-1.0").</param>
    /// <param name="config">The Luma configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public LumaChatLanguageModel(
        string modelId,
        LumaConfiguration config,
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
    /// Note: Luma AI does not currently support traditional chat completions.
    /// This is a placeholder implementation for future expansion.
    /// </summary>
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Placeholder implementation - Luma AI is primarily for video generation
        throw new LumaException(
            "Luma AI is a video generation service. Traditional chat completion is not supported. " +
            "This provider is a placeholder structure for future video generation features.",
            501,
            "not_implemented");
    }

    /// <summary>
    /// Streams text generation from the language model.
    /// Note: Luma AI does not currently support traditional chat completions.
    /// This is a placeholder implementation for future expansion.
    /// </summary>
    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Placeholder implementation - Luma AI is primarily for video generation
        throw new LumaException(
            "Luma AI is a video generation service. Traditional chat completion streaming is not supported. " +
            "This provider is a placeholder structure for future video generation features.",
            501,
            "not_implemented");

        // Required to satisfy async enumerable signature
        #pragma warning disable CS0162 // Unreachable code detected
        yield break;
        #pragma warning restore CS0162
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

    private LumaRequest BuildRequest(LanguageModelCallOptions options, bool stream)
    {
        // Placeholder - would be used for video generation requests in the future
        var messages = options.Messages.Select(m => new LumaMessage
        {
            Role = MapRole(m.Role),
            Content = m.Content
        }).ToList();

        var request = new LumaRequest
        {
            Model = _modelId,
            Messages = messages,
            Stream = stream
        };

        return request;
    }

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
                var errorResponse = JsonSerializer.Deserialize<LumaErrorResponse>(content);
                throw new LumaException(
                    errorResponse?.Error.Message ?? "Luma AI API error",
                    (int)response.StatusCode,
                    errorResponse?.Error.Code);
            }
            catch (JsonException)
            {
                throw new LumaException($"Luma AI API error: {content}", (int)response.StatusCode, null);
            }
        }
    }
}
