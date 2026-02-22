namespace AiSdk.Providers.Azure;

/// <summary>
/// Factory for creating Azure OpenAI language models.
/// </summary>
public static class AzureOpenAIProvider
{
    /// <summary>
    /// Creates an Azure OpenAI chat model with the specified deployment.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI deployment name.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL (e.g., https://your-resource.openai.azure.com).</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="modelId">Optional model ID for display purposes (defaults to deployment name).</param>
    /// <param name="apiVersion">Optional API version (defaults to 2024-02-15-preview).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Azure OpenAI chat language model.</returns>
    public static AzureOpenAIChatLanguageModel CreateChatModel(
        string deploymentName,
        string endpoint,
        string apiKey,
        string? modelId = null,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        var config = new AzureOpenAIConfiguration
        {
            Endpoint = endpoint,
            DeploymentName = deploymentName,
            ApiKey = apiKey,
            ApiVersion = apiVersion ?? "2024-02-15-preview"
        };

        return new AzureOpenAIChatLanguageModel(modelId ?? deploymentName, config, httpClient);
    }

    /// <summary>
    /// Creates an Azure OpenAI chat model with Azure AD authentication.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI deployment name.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="azureAdToken">The Azure AD token for authentication.</param>
    /// <param name="modelId">Optional model ID for display purposes (defaults to deployment name).</param>
    /// <param name="apiVersion">Optional API version (defaults to 2024-02-15-preview).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Azure OpenAI chat language model.</returns>
    public static AzureOpenAIChatLanguageModel CreateChatModelWithAzureAd(
        string deploymentName,
        string endpoint,
        string azureAdToken,
        string? modelId = null,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        var config = new AzureOpenAIConfiguration
        {
            Endpoint = endpoint,
            DeploymentName = deploymentName,
            AzureAdToken = azureAdToken,
            ApiVersion = apiVersion ?? "2024-02-15-preview"
        };

        return new AzureOpenAIChatLanguageModel(modelId ?? deploymentName, config, httpClient);
    }

    /// <summary>
    /// Creates an Azure OpenAI chat model with the specified configuration.
    /// </summary>
    /// <param name="config">The Azure OpenAI configuration.</param>
    /// <param name="modelId">Optional model ID for display purposes (defaults to deployment name).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Azure OpenAI chat language model.</returns>
    public static AzureOpenAIChatLanguageModel CreateChatModel(
        AzureOpenAIConfiguration config,
        string? modelId = null,
        HttpClient? httpClient = null)
    {
        return new AzureOpenAIChatLanguageModel(modelId ?? config.DeploymentName, config, httpClient);
    }

    /// <summary>
    /// Creates an Azure OpenAI GPT-4 model.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI deployment name for GPT-4.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="apiVersion">Optional API version (defaults to 2024-02-15-preview).</param>
    /// <returns>An Azure OpenAI GPT-4 chat model.</returns>
    public static AzureOpenAIChatLanguageModel GPT4(
        string deploymentName,
        string endpoint,
        string apiKey,
        string? apiVersion = null)
    {
        return CreateChatModel(deploymentName, endpoint, apiKey, "gpt-4", apiVersion);
    }

    /// <summary>
    /// Creates an Azure OpenAI GPT-4 Turbo model.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI deployment name for GPT-4 Turbo.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="apiVersion">Optional API version (defaults to 2024-02-15-preview).</param>
    /// <returns>An Azure OpenAI GPT-4 Turbo chat model.</returns>
    public static AzureOpenAIChatLanguageModel GPT4Turbo(
        string deploymentName,
        string endpoint,
        string apiKey,
        string? apiVersion = null)
    {
        return CreateChatModel(deploymentName, endpoint, apiKey, "gpt-4-turbo", apiVersion);
    }

    /// <summary>
    /// Creates an Azure OpenAI GPT-3.5 Turbo model.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI deployment name for GPT-3.5 Turbo.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="apiVersion">Optional API version (defaults to 2024-02-15-preview).</param>
    /// <returns>An Azure OpenAI GPT-3.5 Turbo chat model.</returns>
    public static AzureOpenAIChatLanguageModel GPT35Turbo(
        string deploymentName,
        string endpoint,
        string apiKey,
        string? apiVersion = null)
    {
        return CreateChatModel(deploymentName, endpoint, apiKey, "gpt-3.5-turbo", apiVersion);
    }

    /// <summary>
    /// Creates an Azure OpenAI embedding model.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI embedding deployment name.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <param name="modelId">Optional model ID for display purposes (defaults to deployment name).</param>
    /// <param name="apiVersion">Optional API version.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Azure OpenAI embedding model.</returns>
    public static AzureOpenAIEmbeddingModel CreateEmbeddingModel(
        string deploymentName,
        string endpoint,
        string apiKey,
        string? modelId = null,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        var config = new AzureOpenAIConfiguration
        {
            Endpoint = endpoint,
            DeploymentName = deploymentName,
            ApiKey = apiKey,
            ApiVersion = apiVersion ?? "2024-02-15-preview"
        };

        return new AzureOpenAIEmbeddingModel(modelId ?? deploymentName, config, httpClient);
    }

    /// <summary>
    /// Creates an Azure OpenAI embedding model with Azure AD authentication.
    /// </summary>
    /// <param name="deploymentName">The Azure OpenAI embedding deployment name.</param>
    /// <param name="endpoint">The Azure OpenAI endpoint URL.</param>
    /// <param name="azureAdToken">The Azure AD token for authentication.</param>
    /// <param name="modelId">Optional model ID for display purposes.</param>
    /// <param name="apiVersion">Optional API version.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Azure OpenAI embedding model.</returns>
    public static AzureOpenAIEmbeddingModel CreateEmbeddingModelWithAzureAd(
        string deploymentName,
        string endpoint,
        string azureAdToken,
        string? modelId = null,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        var config = new AzureOpenAIConfiguration
        {
            Endpoint = endpoint,
            DeploymentName = deploymentName,
            AzureAdToken = azureAdToken,
            ApiVersion = apiVersion ?? "2024-02-15-preview"
        };

        return new AzureOpenAIEmbeddingModel(modelId ?? deploymentName, config, httpClient);
    }

    /// <summary>
    /// Creates an Azure OpenAI embedding model with the specified configuration.
    /// </summary>
    /// <param name="config">The Azure OpenAI configuration.</param>
    /// <param name="modelId">Optional model ID for display purposes.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Azure OpenAI embedding model.</returns>
    public static AzureOpenAIEmbeddingModel CreateEmbeddingModel(
        AzureOpenAIConfiguration config,
        string? modelId = null,
        HttpClient? httpClient = null)
    {
        return new AzureOpenAIEmbeddingModel(modelId ?? config.DeploymentName, config, httpClient);
    }
}
