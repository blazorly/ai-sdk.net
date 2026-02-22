using AiSdk.Abstractions;
using AiSdk.Registry;

namespace AiSdk.Providers.Azure;

/// <summary>
/// Factory for creating Azure OpenAI models from the provider registry.
/// Note: Azure OpenAI uses deployment names rather than model IDs.
/// The model ID passed here should be the deployment name.
/// </summary>
public class AzureOpenAIProviderFactory : IProviderFactory
{
    private readonly string _endpoint;
    private readonly string? _apiKey;
    private readonly string? _azureAdToken;
    private readonly string _apiVersion;
    private readonly HttpClient? _httpClient;

    /// <inheritdoc/>
    public string ProviderId => "azure-openai";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIProviderFactory"/> class with API key authentication.
    /// </summary>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="apiVersion">Optional API version.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    public AzureOpenAIProviderFactory(
        string endpoint,
        string apiKey,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        _endpoint = endpoint;
        _apiKey = apiKey;
        _azureAdToken = null;
        _apiVersion = apiVersion ?? "2024-02-15-preview";
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates an Azure OpenAI provider factory with Azure AD token authentication.
    /// </summary>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="azureAdToken">The Azure AD token for authentication.</param>
    /// <param name="apiVersion">Optional API version.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An AzureOpenAIProviderFactory using Azure AD auth.</returns>
    public static AzureOpenAIProviderFactory WithAzureAd(
        string endpoint,
        string azureAdToken,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        return new AzureOpenAIProviderFactory(endpoint, azureAdToken, apiVersion, httpClient, useAzureAd: true);
    }

    private AzureOpenAIProviderFactory(
        string endpoint,
        string token,
        string? apiVersion,
        HttpClient? httpClient,
        bool useAzureAd)
    {
        _endpoint = endpoint;
        if (useAzureAd)
        {
            _azureAdToken = token;
            _apiKey = null;
        }
        else
        {
            _apiKey = token;
            _azureAdToken = null;
        }
        _apiVersion = apiVersion ?? "2024-02-15-preview";
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public ILanguageModel CreateLanguageModel(string modelId)
    {
        var config = BuildConfig(modelId);
        return new AzureOpenAIChatLanguageModel(modelId, config, _httpClient);
    }

    /// <inheritdoc/>
    public IEmbeddingModel CreateEmbeddingModel(string modelId)
    {
        var config = BuildConfig(modelId);
        return new AzureOpenAIEmbeddingModel(modelId, config, _httpClient);
    }

    private AzureOpenAIConfiguration BuildConfig(string deploymentName)
    {
        return new AzureOpenAIConfiguration
        {
            Endpoint = _endpoint,
            DeploymentName = deploymentName,
            ApiKey = _apiKey,
            AzureAdToken = _azureAdToken,
            ApiVersion = _apiVersion
        };
    }
}
