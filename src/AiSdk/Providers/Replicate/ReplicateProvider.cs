namespace AiSdk.Providers.Replicate;

/// <summary>
/// Factory for creating Replicate language models.
/// </summary>
public static class ReplicateProvider
{
    /// <summary>
    /// Creates a Replicate chat model with the specified model version.
    /// </summary>
    /// <param name="modelVersion">The Replicate model version (e.g., "meta/llama-2-70b-chat").</param>
    /// <param name="apiKey">The Replicate API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.replicate.com/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Replicate chat language model.</returns>
    public static ReplicateChatLanguageModel CreateChatModel(
        string modelVersion,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new ReplicateConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.replicate.com/v1"
        };

        return new ReplicateChatLanguageModel(modelVersion, config, httpClient);
    }

    /// <summary>
    /// Creates a Replicate chat model with the specified configuration.
    /// </summary>
    /// <param name="modelVersion">The Replicate model version.</param>
    /// <param name="config">The Replicate configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Replicate chat language model.</returns>
    public static ReplicateChatLanguageModel CreateChatModel(
        string modelVersion,
        ReplicateConfiguration config,
        HttpClient? httpClient = null)
    {
        return new ReplicateChatLanguageModel(modelVersion, config, httpClient);
    }

    /// <summary>
    /// Creates a Llama 2 70B Chat model.
    /// Meta's largest Llama 2 chat model with excellent performance.
    /// </summary>
    /// <param name="apiKey">The Replicate API key.</param>
    /// <returns>A Llama 2 70B chat model.</returns>
    public static ReplicateChatLanguageModel Llama2_70B(string apiKey)
    {
        return CreateChatModel("meta/llama-2-70b-chat", apiKey);
    }

    /// <summary>
    /// Creates a Llama 2 13B Chat model.
    /// Meta's medium-sized Llama 2 chat model, balanced performance and speed.
    /// </summary>
    /// <param name="apiKey">The Replicate API key.</param>
    /// <returns>A Llama 2 13B chat model.</returns>
    public static ReplicateChatLanguageModel Llama2_13B(string apiKey)
    {
        return CreateChatModel("meta/llama-2-13b-chat", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct model.
    /// Mistral AI's mixture of experts model with excellent reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Replicate API key.</param>
    /// <returns>A Mixtral 8x7B instruct model.</returns>
    public static ReplicateChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mistralai/mixtral-8x7b-instruct-v0.1", apiKey);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct model.
    /// Mistral AI's efficient 7B parameter instruction-following model.
    /// </summary>
    /// <param name="apiKey">The Replicate API key.</param>
    /// <returns>A Mistral 7B instruct model.</returns>
    public static ReplicateChatLanguageModel Mistral7B(string apiKey)
    {
        return CreateChatModel("mistralai/mistral-7b-instruct-v0.2", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model version.
    /// Convenience method for creating any Replicate model.
    /// </summary>
    /// <param name="modelVersion">The Replicate model version (e.g., "meta/llama-2-70b-chat").</param>
    /// <param name="apiKey">The Replicate API key.</param>
    /// <returns>A Replicate chat language model.</returns>
    public static ReplicateChatLanguageModel ChatModel(string modelVersion, string apiKey)
    {
        return CreateChatModel(modelVersion, apiKey);
    }
}
