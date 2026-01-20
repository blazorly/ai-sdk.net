namespace AiSdk.Providers.Perplexity;

/// <summary>
/// Factory for creating Perplexity language models.
/// </summary>
public static class PerplexityProvider
{
    /// <summary>
    /// Creates a Perplexity chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Perplexity model ID (e.g., "llama-3.1-sonar-small-128k-online", "llama-3.1-70b-instruct").</param>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.perplexity.ai).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Perplexity chat language model.</returns>
    public static PerplexityChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new PerplexityConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.perplexity.ai"
        };

        return new PerplexityChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Perplexity chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Perplexity model ID.</param>
    /// <param name="config">The Perplexity configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Perplexity chat language model.</returns>
    public static PerplexityChatLanguageModel CreateChatModel(
        string modelId,
        PerplexityConfiguration config,
        HttpClient? httpClient = null)
    {
        return new PerplexityChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.1 Sonar Small 128K Online model.
    /// Optimized for speed with real-time web search and 128K context window.
    /// </summary>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <returns>A Llama 3.1 Sonar Small Online chat model.</returns>
    public static PerplexityChatLanguageModel SonarSmallOnline(string apiKey)
    {
        return CreateChatModel("llama-3.1-sonar-small-128k-online", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 Sonar Large 128K Online model.
    /// Balanced performance with real-time web search and 128K context window.
    /// </summary>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <returns>A Llama 3.1 Sonar Large Online chat model.</returns>
    public static PerplexityChatLanguageModel SonarLargeOnline(string apiKey)
    {
        return CreateChatModel("llama-3.1-sonar-large-128k-online", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 Sonar Huge 128K Online model.
    /// Maximum performance with real-time web search and 128K context window.
    /// </summary>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <returns>A Llama 3.1 Sonar Huge Online chat model.</returns>
    public static PerplexityChatLanguageModel SonarHugeOnline(string apiKey)
    {
        return CreateChatModel("llama-3.1-sonar-huge-128k-online", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 8B Instruct model.
    /// Fast and efficient instruction-following model without web search.
    /// </summary>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <returns>A Llama 3.1 8B Instruct chat model.</returns>
    public static PerplexityChatLanguageModel Llama31_8B(string apiKey)
    {
        return CreateChatModel("llama-3.1-8b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 70B Instruct model.
    /// High-performance instruction-following model without web search.
    /// </summary>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <returns>A Llama 3.1 70B Instruct chat model.</returns>
    public static PerplexityChatLanguageModel Llama31_70B(string apiKey)
    {
        return CreateChatModel("llama-3.1-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any Perplexity model.
    /// </summary>
    /// <param name="modelId">The Perplexity model ID.</param>
    /// <param name="apiKey">The Perplexity API key.</param>
    /// <returns>A Perplexity chat language model.</returns>
    public static PerplexityChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
