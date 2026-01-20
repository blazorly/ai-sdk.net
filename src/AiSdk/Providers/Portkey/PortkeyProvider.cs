namespace AiSdk.Providers.Portkey;

/// <summary>
/// Factory for creating Portkey AI Gateway language models.
/// Provides access to models from multiple providers through Portkey's unified API gateway.
/// </summary>
public static class PortkeyProvider
{
    /// <summary>
    /// Creates a Portkey AI Gateway chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "gpt-4", "claude-3-5-sonnet-20241022").</param>
    /// <param name="apiKey">The Portkey API key.</param>
    /// <param name="provider">Optional provider name (e.g., "openai", "anthropic").</param>
    /// <param name="virtualKey">Optional virtual key for the target provider.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.portkey.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Portkey AI Gateway chat language model.</returns>
    public static PortkeyChatLanguageModel ChatModel(
        string modelId,
        string apiKey,
        string? provider = null,
        string? virtualKey = null,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new PortkeyConfiguration
        {
            ApiKey = apiKey,
            Provider = provider,
            VirtualKey = virtualKey,
            BaseUrl = baseUrl ?? "https://api.portkey.ai/v1"
        };

        return new PortkeyChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Portkey AI Gateway chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="config">The Portkey AI Gateway configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Portkey AI Gateway chat language model.</returns>
    public static PortkeyChatLanguageModel CreateChatModel(
        string modelId,
        PortkeyConfiguration config,
        HttpClient? httpClient = null)
    {
        return new PortkeyChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Portkey AI Gateway chat model with provider-specific settings.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="apiKey">The Portkey API key.</param>
    /// <param name="provider">The target provider name.</param>
    /// <param name="virtualKey">Optional virtual key for the target provider.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Portkey AI Gateway chat language model.</returns>
    public static PortkeyChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string provider,
        string? virtualKey = null,
        HttpClient? httpClient = null)
    {
        var config = new PortkeyConfiguration
        {
            ApiKey = apiKey,
            Provider = provider,
            VirtualKey = virtualKey
        };

        return new PortkeyChatLanguageModel(modelId, config, httpClient);
    }
}
