namespace AiSdk.Providers.AI21;

/// <summary>
/// Factory for creating AI21 Labs language models.
/// AI21 Labs offers powerful language models including Jamba and Jurassic families with OpenAI-compatible API.
/// </summary>
public static class AI21Provider
{
    /// <summary>
    /// Creates an AI21 Labs chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The AI21 Labs model ID (e.g., "jamba-1.5-large").</param>
    /// <param name="apiKey">The AI21 Labs API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.ai21.com/studio/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An AI21 Labs chat language model.</returns>
    public static AI21ChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new AI21Configuration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.ai21.com/studio/v1"
        };

        return new AI21ChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an AI21 Labs chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The AI21 Labs model ID.</param>
    /// <param name="config">The AI21 Labs configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An AI21 Labs chat language model.</returns>
    public static AI21ChatLanguageModel CreateChatModel(
        string modelId,
        AI21Configuration config,
        HttpClient? httpClient = null)
    {
        return new AI21ChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Jamba 1.5 Large model.
    /// AI21's latest flagship model with 256K context window, strong reasoning, and multilingual capabilities.
    /// Optimized for long-context tasks and complex reasoning.
    /// </summary>
    /// <param name="apiKey">The AI21 Labs API key.</param>
    /// <returns>A Jamba 1.5 Large chat model.</returns>
    public static AI21ChatLanguageModel Jamba15Large(string apiKey)
    {
        return CreateChatModel("jamba-1.5-large", apiKey);
    }

    /// <summary>
    /// Creates a Jamba 1.5 Mini model.
    /// Compact version of Jamba 1.5 with 256K context window, optimized for speed and efficiency.
    /// Ideal for cost-effective applications requiring long-context understanding.
    /// </summary>
    /// <param name="apiKey">The AI21 Labs API key.</param>
    /// <returns>A Jamba 1.5 Mini chat model.</returns>
    public static AI21ChatLanguageModel Jamba15Mini(string apiKey)
    {
        return CreateChatModel("jamba-1.5-mini", apiKey);
    }

    /// <summary>
    /// Creates a Jurassic-2 Ultra model.
    /// AI21's powerful foundation model with excellent instruction following and generation quality.
    /// Suitable for complex tasks requiring nuanced understanding.
    /// </summary>
    /// <param name="apiKey">The AI21 Labs API key.</param>
    /// <returns>A Jurassic-2 Ultra chat model.</returns>
    public static AI21ChatLanguageModel Jurassic2Ultra(string apiKey)
    {
        return CreateChatModel("jurassic-2-ultra", apiKey);
    }

    /// <summary>
    /// Creates a Jurassic-2 Mid model.
    /// Balanced model offering strong performance with efficient resource usage.
    /// Good for general-purpose applications.
    /// </summary>
    /// <param name="apiKey">The AI21 Labs API key.</param>
    /// <returns>A Jurassic-2 Mid chat model.</returns>
    public static AI21ChatLanguageModel Jurassic2Mid(string apiKey)
    {
        return CreateChatModel("jurassic-2-mid", apiKey);
    }

    /// <summary>
    /// Creates a chat model with a custom model ID.
    /// Use this for any AI21 Labs model not listed in the convenience methods.
    /// </summary>
    /// <param name="modelId">The AI21 Labs model ID.</param>
    /// <param name="apiKey">The AI21 Labs API key.</param>
    /// <returns>An AI21 Labs chat language model.</returns>
    public static AI21ChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
