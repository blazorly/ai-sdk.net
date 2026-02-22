using AiSdk.Abstractions;
using AiSdk.Registry;

namespace AiSdk.Providers.Mistral;

/// <summary>
/// Factory for creating Mistral AI models from the provider registry.
/// </summary>
public class MistralProviderFactory : IProviderFactory
{
    private readonly MistralConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <inheritdoc/>
    public string ProviderId => "mistral";

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralProviderFactory"/> class.
    /// </summary>
    /// <param name="apiKey">The Mistral AI API key.</param>
    /// <param name="baseUrl">Optional base URL.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public MistralProviderFactory(
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        _config = new MistralConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.mistral.ai/v1"
        };
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MistralProviderFactory"/> class.
    /// </summary>
    /// <param name="config">The Mistral configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public MistralProviderFactory(MistralConfiguration config, HttpClient? httpClient = null)
    {
        _config = config;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public ILanguageModel CreateLanguageModel(string modelId)
    {
        return new MistralChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <inheritdoc/>
    public IEmbeddingModel CreateEmbeddingModel(string modelId)
    {
        return new MistralEmbeddingModel(modelId, _config, _httpClient);
    }
}
