using AiSdk.Abstractions;
using AiSdk.Registry;

namespace AiSdk.Providers.Cohere;

/// <summary>
/// Factory for creating Cohere models from the provider registry.
/// </summary>
public class CohereProviderFactory : IProviderFactory
{
    private readonly CohereConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <inheritdoc/>
    public string ProviderId => "cohere";

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereProviderFactory"/> class.
    /// </summary>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="baseUrl">Optional base URL.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public CohereProviderFactory(
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        _config = new CohereConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.cohere.ai/v1"
        };
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereProviderFactory"/> class.
    /// </summary>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public CohereProviderFactory(CohereConfiguration config, HttpClient? httpClient = null)
    {
        _config = config;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public ILanguageModel CreateLanguageModel(string modelId)
    {
        return new CohereChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <inheritdoc/>
    public IEmbeddingModel CreateEmbeddingModel(string modelId)
    {
        return new CohereEmbeddingModel(modelId, _config, _httpClient);
    }
}
