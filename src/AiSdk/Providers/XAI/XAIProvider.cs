namespace AiSdk.Providers.XAI;

/// <summary>
/// Factory for creating xAI (Grok) language models.
/// </summary>
public static class XAIProvider
{
    /// <summary>
    /// Creates an xAI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The xAI model ID (e.g., "grok-4", "grok-3", "grok-2-vision-1212").</param>
    /// <param name="apiKey">The xAI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.x.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An xAI chat language model.</returns>
    public static XAIChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new XAIConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.x.ai/v1"
        };

        return new XAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an xAI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The xAI model ID.</param>
    /// <param name="config">The xAI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An xAI chat language model.</returns>
    public static XAIChatLanguageModel CreateChatModel(
        string modelId,
        XAIConfiguration config,
        HttpClient? httpClient = null)
    {
        return new XAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Grok 4 model.
    /// The latest and most capable Grok model with advanced reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 4 chat model.</returns>
    public static XAIChatLanguageModel Grok4(string apiKey)
    {
        return CreateChatModel("grok-4", apiKey);
    }

    /// <summary>
    /// Creates a Grok 4.1 Fast Reasoning model.
    /// Ultra-fast reasoning with optimized performance for complex tasks.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 4.1 Fast Reasoning chat model.</returns>
    public static XAIChatLanguageModel Grok4_1FastReasoning(string apiKey)
    {
        return CreateChatModel("grok-4-1-fast-reasoning", apiKey);
    }

    /// <summary>
    /// Creates a Grok 4.1 Fast Non-Reasoning model.
    /// Ultra-fast responses without extended reasoning for simple queries.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 4.1 Fast Non-Reasoning chat model.</returns>
    public static XAIChatLanguageModel Grok4_1FastNonReasoning(string apiKey)
    {
        return CreateChatModel("grok-4-1-fast-non-reasoning", apiKey);
    }

    /// <summary>
    /// Creates a Grok 4 Fast Reasoning model.
    /// Fast reasoning capabilities with balanced performance.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 4 Fast Reasoning chat model.</returns>
    public static XAIChatLanguageModel Grok4FastReasoning(string apiKey)
    {
        return CreateChatModel("grok-4-fast-reasoning", apiKey);
    }

    /// <summary>
    /// Creates a Grok 4 Fast Non-Reasoning model.
    /// Fast responses without extended reasoning for simple queries.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 4 Fast Non-Reasoning chat model.</returns>
    public static XAIChatLanguageModel Grok4FastNonReasoning(string apiKey)
    {
        return CreateChatModel("grok-4-fast-non-reasoning", apiKey);
    }

    /// <summary>
    /// Creates a Grok Code Fast 1 model.
    /// Specialized for code generation and analysis tasks.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok Code Fast 1 chat model.</returns>
    public static XAIChatLanguageModel GrokCodeFast1(string apiKey)
    {
        return CreateChatModel("grok-code-fast-1", apiKey);
    }

    /// <summary>
    /// Creates a Grok 3 model.
    /// Previous generation high-performance model with strong capabilities.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 3 chat model.</returns>
    public static XAIChatLanguageModel Grok3(string apiKey)
    {
        return CreateChatModel("grok-3", apiKey);
    }

    /// <summary>
    /// Creates a Grok 3 Mini model.
    /// Compact version optimized for speed and efficiency.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 3 Mini chat model.</returns>
    public static XAIChatLanguageModel Grok3Mini(string apiKey)
    {
        return CreateChatModel("grok-3-mini", apiKey);
    }

    /// <summary>
    /// Creates a Grok 2 Vision 1212 model.
    /// Vision-enabled model for image understanding and analysis.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 2 Vision chat model.</returns>
    public static XAIChatLanguageModel Grok2Vision(string apiKey)
    {
        return CreateChatModel("grok-2-vision-1212", apiKey);
    }

    /// <summary>
    /// Creates a Grok 2 Image 1212 model.
    /// Image generation and manipulation capabilities.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 2 Image chat model.</returns>
    public static XAIChatLanguageModel Grok2Image(string apiKey)
    {
        return CreateChatModel("grok-2-image-1212", apiKey);
    }

    /// <summary>
    /// Creates a Grok 2 1212 model.
    /// Standard Grok 2 model with reliable performance.
    /// </summary>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>A Grok 2 chat model.</returns>
    public static XAIChatLanguageModel Grok2(string apiKey)
    {
        return CreateChatModel("grok-2-1212", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any xAI model.
    /// </summary>
    /// <param name="modelId">The xAI model ID.</param>
    /// <param name="apiKey">The xAI API key.</param>
    /// <returns>An xAI chat language model.</returns>
    public static XAIChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
