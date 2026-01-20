namespace AiSdk.Providers.Stability;

/// <summary>
/// Factory for creating Stability AI language models.
///
/// Note: Stability AI is primarily known for image generation (Stable Diffusion).
/// This provider supports their language models (StableLM) which can be:
/// - Self-hosted using frameworks like vLLM with OpenAI-compatible endpoints
/// - Accessed through third-party platforms
/// - Future Stability AI hosted text generation API endpoints
///
/// For image generation features, future extensions will be added.
/// </summary>
public static class StabilityProvider
{
    /// <summary>
    /// Creates a Stability AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Stability AI model ID (e.g., "stablelm-2-12b", "stablelm-2-zephyr-1_6b").</param>
    /// <param name="apiKey">The Stability AI API key or bearer token.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.stability.ai/v2beta, or use your self-hosted endpoint).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Stability AI chat language model.</returns>
    public static StabilityChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new StabilityConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.stability.ai/v2beta"
        };

        return new StabilityChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Stability AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Stability AI model ID.</param>
    /// <param name="config">The Stability AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Stability AI chat language model.</returns>
    public static StabilityChatLanguageModel CreateChatModel(
        string modelId,
        StabilityConfiguration config,
        HttpClient? httpClient = null)
    {
        return new StabilityChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a StableLM 2 12B model (recommended for general use).
    /// Note: Requires self-hosted deployment or compatible API endpoint.
    /// </summary>
    /// <param name="apiKey">The API key or bearer token.</param>
    /// <param name="baseUrl">The base URL of your StableLM deployment.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A StableLM 2 12B chat model.</returns>
    public static StabilityChatLanguageModel StableLM2_12B(
        string apiKey,
        string baseUrl,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("stablelm-2-12b", apiKey, baseUrl, httpClient);
    }

    /// <summary>
    /// Creates a StableLM 2 Zephyr 1.6B model (lightweight, fast).
    /// Note: Requires self-hosted deployment or compatible API endpoint.
    /// </summary>
    /// <param name="apiKey">The API key or bearer token.</param>
    /// <param name="baseUrl">The base URL of your StableLM deployment.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A StableLM 2 Zephyr 1.6B chat model.</returns>
    public static StabilityChatLanguageModel StableLM2Zephyr_1_6B(
        string apiKey,
        string baseUrl,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("stablelm-2-zephyr-1_6b", apiKey, baseUrl, httpClient);
    }

    /// <summary>
    /// Creates a StableLM 3B model.
    /// Note: Requires self-hosted deployment or compatible API endpoint.
    /// </summary>
    /// <param name="apiKey">The API key or bearer token.</param>
    /// <param name="baseUrl">The base URL of your StableLM deployment.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A StableLM 3B chat model.</returns>
    public static StabilityChatLanguageModel StableLM_3B(
        string apiKey,
        string baseUrl,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("stablelm-base-alpha-3b", apiKey, baseUrl, httpClient);
    }

    /// <summary>
    /// Creates a generic chat model with a custom model ID.
    /// Use this for any StableLM variant or custom fine-tuned models.
    /// </summary>
    /// <param name="modelId">The model identifier.</param>
    /// <param name="apiKey">The API key or bearer token.</param>
    /// <param name="baseUrl">The base URL of your deployment.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Stability AI chat language model.</returns>
    public static StabilityChatLanguageModel ChatModel(
        string modelId,
        string apiKey,
        string baseUrl,
        HttpClient? httpClient = null)
    {
        return CreateChatModel(modelId, apiKey, baseUrl, httpClient);
    }
}
