using AiSdk.Abstractions;
using AiSdk.Registry;

namespace AiSdk.Providers.OpenAI;

/// <summary>
/// Factory for creating OpenAI models from the provider registry.
/// </summary>
public class OpenAIProviderFactory : IProviderFactory
{
    private readonly OpenAIConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <inheritdoc/>
    public string ProviderId => "openai";

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIProviderFactory"/> class.
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <param name="baseUrl">Optional base URL.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public OpenAIProviderFactory(
        string apiKey,
        string? organization = null,
        string? project = null,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        _config = new OpenAIConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.openai.com/v1",
            Organization = organization,
            Project = project
        };
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIProviderFactory"/> class.
    /// </summary>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public OpenAIProviderFactory(OpenAIConfiguration config, HttpClient? httpClient = null)
    {
        _config = config;
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public ILanguageModel CreateLanguageModel(string modelId)
    {
        return new OpenAIChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <inheritdoc/>
    public IEmbeddingModel CreateEmbeddingModel(string modelId)
    {
        return new OpenAIEmbeddingModel(modelId, _config, _httpClient);
    }
}
