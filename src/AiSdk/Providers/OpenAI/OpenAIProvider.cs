namespace AiSdk.Providers.OpenAI;

/// <summary>
/// Factory for creating OpenAI language models.
/// </summary>
public static class OpenAIProvider
{
    /// <summary>
    /// Creates an OpenAI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The OpenAI model ID (e.g., "gpt-4", "gpt-4-turbo", "gpt-3.5-turbo").</param>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.openai.com/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenAI chat language model.</returns>
    public static OpenAIChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? organization = null,
        string? project = null,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAIConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.openai.com/v1",
            Organization = organization,
            Project = project
        };

        return new OpenAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an OpenAI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The OpenAI model ID.</param>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenAI chat language model.</returns>
    public static OpenAIChatLanguageModel CreateChatModel(
        string modelId,
        OpenAIConfiguration config,
        HttpClient? httpClient = null)
    {
        return new OpenAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a GPT-4 model.
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <returns>A GPT-4 chat model.</returns>
    public static OpenAIChatLanguageModel GPT4(
        string apiKey,
        string? organization = null,
        string? project = null)
    {
        return CreateChatModel("gpt-4", apiKey, organization, project);
    }

    /// <summary>
    /// Creates a GPT-4 Turbo model.
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <returns>A GPT-4 Turbo chat model.</returns>
    public static OpenAIChatLanguageModel GPT4Turbo(
        string apiKey,
        string? organization = null,
        string? project = null)
    {
        return CreateChatModel("gpt-4-turbo", apiKey, organization, project);
    }

    /// <summary>
    /// Creates a GPT-3.5 Turbo model.
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <returns>A GPT-3.5 Turbo chat model.</returns>
    public static OpenAIChatLanguageModel GPT35Turbo(
        string apiKey,
        string? organization = null,
        string? project = null)
    {
        return CreateChatModel("gpt-3.5-turbo", apiKey, organization, project);
    }

    /// <summary>
    /// Creates an OpenAI embedding model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The embedding model ID (e.g., "text-embedding-3-small").</param>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <param name="baseUrl">Optional base URL.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenAI embedding model.</returns>
    public static OpenAIEmbeddingModel CreateEmbeddingModel(
        string modelId,
        string apiKey,
        string? organization = null,
        string? project = null,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAIConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.openai.com/v1",
            Organization = organization,
            Project = project
        };

        return new OpenAIEmbeddingModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an OpenAI embedding model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The embedding model ID.</param>
    /// <param name="config">The OpenAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenAI embedding model.</returns>
    public static OpenAIEmbeddingModel CreateEmbeddingModel(
        string modelId,
        OpenAIConfiguration config,
        HttpClient? httpClient = null)
    {
        return new OpenAIEmbeddingModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a text-embedding-3-small model (best value).
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <returns>A text-embedding-3-small model.</returns>
    public static OpenAIEmbeddingModel TextEmbedding3Small(
        string apiKey,
        string? organization = null,
        string? project = null)
    {
        return CreateEmbeddingModel("text-embedding-3-small", apiKey, organization, project);
    }

    /// <summary>
    /// Creates a text-embedding-3-large model (highest performance).
    /// </summary>
    /// <param name="apiKey">The OpenAI API key.</param>
    /// <param name="organization">Optional organization ID.</param>
    /// <param name="project">Optional project ID.</param>
    /// <returns>A text-embedding-3-large model.</returns>
    public static OpenAIEmbeddingModel TextEmbedding3Large(
        string apiKey,
        string? organization = null,
        string? project = null)
    {
        return CreateEmbeddingModel("text-embedding-3-large", apiKey, organization, project);
    }
}
