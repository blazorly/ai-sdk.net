using AiSdk.Abstractions;
using AiSdk.Registry;

namespace AiSdk.Providers.Google;

/// <summary>
/// Factory for creating Google Gemini models from the provider registry.
/// </summary>
public class GoogleProviderFactory : IProviderFactory
{
    private readonly GoogleConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <inheritdoc/>
    public string ProviderId => "google";

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleProviderFactory"/> class.
    /// </summary>
    /// <param name="apiKey">The Google API key.</param>
    /// <param name="baseUrl">Optional base URL.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public GoogleProviderFactory(
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        _config = new GoogleConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://generativelanguage.googleapis.com/v1beta/"
        };
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleProviderFactory"/> class.
    /// </summary>
    /// <param name="config">The Google configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public GoogleProviderFactory(GoogleConfiguration config, HttpClient? httpClient = null)
    {
        _config = config;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public ILanguageModel CreateLanguageModel(string modelId)
    {
        return new GoogleChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <inheritdoc/>
    public IEmbeddingModel CreateEmbeddingModel(string modelId)
    {
        return new GoogleEmbeddingModel(modelId, _config, _httpClient);
    }
}
