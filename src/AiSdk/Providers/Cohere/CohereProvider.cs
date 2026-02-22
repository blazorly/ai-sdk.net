namespace AiSdk.Providers.Cohere;

/// <summary>
/// Factory for creating Cohere language models.
/// </summary>
public static class CohereProvider
{
    /// <summary>
    /// Creates a Cohere chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Cohere model ID (e.g., "command-r-plus", "command-r", "command").</param>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.cohere.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere chat language model.</returns>
    public static CohereChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new CohereConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.cohere.ai/v1"
        };

        return new CohereChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Cohere chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Cohere model ID.</param>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere chat language model.</returns>
    public static CohereChatLanguageModel CreateChatModel(
        string modelId,
        CohereConfiguration config,
        HttpClient? httpClient = null)
    {
        return new CohereChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Command R+ model (best for complex tasks).
    /// </summary>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere Command R+ chat model.</returns>
    public static CohereChatLanguageModel CommandRPlus(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("command-r-plus", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Command R model (balanced performance).
    /// </summary>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere Command R chat model.</returns>
    public static CohereChatLanguageModel CommandR(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("command-r", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Command model.
    /// </summary>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere Command chat model.</returns>
    public static CohereChatLanguageModel Command(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("command", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID using the provided configuration.
    /// </summary>
    /// <param name="modelId">The Cohere model ID.</param>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere chat language model.</returns>
    public static CohereChatLanguageModel ChatModel(
        string modelId,
        CohereConfiguration config,
        HttpClient? httpClient = null)
    {
        return new CohereChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Cohere embedding model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Cohere embedding model ID (e.g., "embed-english-v3.0").</param>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="baseUrl">Optional base URL.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere embedding model.</returns>
    public static CohereEmbeddingModel CreateEmbeddingModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new CohereConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.cohere.ai/v1"
        };

        return new CohereEmbeddingModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Cohere embedding model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Cohere embedding model ID.</param>
    /// <param name="config">The Cohere configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere embedding model.</returns>
    public static CohereEmbeddingModel CreateEmbeddingModel(
        string modelId,
        CohereConfiguration config,
        HttpClient? httpClient = null)
    {
        return new CohereEmbeddingModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an Embed English v3.0 model.
    /// </summary>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere embed-english-v3.0 model.</returns>
    public static CohereEmbeddingModel EmbedEnglishV3(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateEmbeddingModel("embed-english-v3.0", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates an Embed Multilingual v3.0 model.
    /// </summary>
    /// <param name="apiKey">The Cohere API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cohere embed-multilingual-v3.0 model.</returns>
    public static CohereEmbeddingModel EmbedMultilingualV3(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateEmbeddingModel("embed-multilingual-v3.0", apiKey, httpClient: httpClient);
    }
}
