namespace AiSdk.Providers.Friendli;

/// <summary>
/// Factory for creating Friendli language models.
/// </summary>
public static class FriendliProvider
{
    /// <summary>
    /// Creates a Friendli chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Friendli model ID (e.g., "mixtral-8x7b-instruct-v0-1", "meta-llama-3-1-70b-instruct").</param>
    /// <param name="apiKey">The Friendli API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://inference.friendli.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Friendli chat language model.</returns>
    public static FriendliChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new FriendliConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://inference.friendli.ai/v1"
        };

        return new FriendliChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Friendli chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Friendli model ID.</param>
    /// <param name="config">The Friendli configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Friendli chat language model.</returns>
    public static FriendliChatLanguageModel CreateChatModel(
        string modelId,
        FriendliConfiguration config,
        HttpClient? httpClient = null)
    {
        return new FriendliChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct model.
    /// High-performance mixture of experts model with excellent reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Friendli API key.</param>
    /// <returns>A Mixtral 8x7B Instruct chat model.</returns>
    public static FriendliChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mixtral-8x7b-instruct-v0-1", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 70B Instruct model.
    /// Large-scale model with excellent performance across diverse tasks.
    /// </summary>
    /// <param name="apiKey">The Friendli API key.</param>
    /// <returns>A Meta Llama 3.1 70B Instruct chat model.</returns>
    public static FriendliChatLanguageModel Llama3_70B(string apiKey)
    {
        return CreateChatModel("meta-llama-3-1-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 8B Instruct model.
    /// Optimized for speed with lower latency, suitable for real-time applications.
    /// </summary>
    /// <param name="apiKey">The Friendli API key.</param>
    /// <returns>A Meta Llama 3.1 8B Instruct chat model.</returns>
    public static FriendliChatLanguageModel Llama3_8B(string apiKey)
    {
        return CreateChatModel("meta-llama-3-1-8b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any Friendli model.
    /// </summary>
    /// <param name="modelId">The Friendli model ID.</param>
    /// <param name="apiKey">The Friendli API key.</param>
    /// <returns>A Friendli chat language model.</returns>
    public static FriendliChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
