namespace AiSdk.Providers.Anthropic;

/// <summary>
/// Factory for creating Anthropic language models.
/// </summary>
public static class AnthropicProvider
{
    /// <summary>
    /// Creates an Anthropic chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Anthropic model ID (e.g., "claude-3-5-sonnet-20241022", "claude-3-opus-20240229").</param>
    /// <param name="apiKey">The Anthropic API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.anthropic.com/v1).</param>
    /// <param name="apiVersion">Optional API version (defaults to 2023-06-01).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Anthropic chat language model.</returns>
    public static AnthropicChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        string? apiVersion = null,
        HttpClient? httpClient = null)
    {
        var config = new AnthropicConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.anthropic.com/v1",
            ApiVersion = apiVersion ?? "2023-06-01"
        };

        return new AnthropicChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an Anthropic chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Anthropic model ID.</param>
    /// <param name="config">The Anthropic configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Anthropic chat language model.</returns>
    public static AnthropicChatLanguageModel CreateChatModel(
        string modelId,
        AnthropicConfiguration config,
        HttpClient? httpClient = null)
    {
        return new AnthropicChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Claude 3.5 Sonnet model (latest version).
    /// </summary>
    /// <param name="apiKey">The Anthropic API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3.5 Sonnet chat model.</returns>
    public static AnthropicChatLanguageModel Claude35Sonnet(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("claude-3-5-sonnet-20241022", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Claude 3 Opus model.
    /// </summary>
    /// <param name="apiKey">The Anthropic API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3 Opus chat model.</returns>
    public static AnthropicChatLanguageModel Claude3Opus(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("claude-3-opus-20240229", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Claude 3 Sonnet model.
    /// </summary>
    /// <param name="apiKey">The Anthropic API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3 Sonnet chat model.</returns>
    public static AnthropicChatLanguageModel Claude3Sonnet(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("claude-3-sonnet-20240229", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Claude 3 Haiku model.
    /// </summary>
    /// <param name="apiKey">The Anthropic API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3 Haiku chat model.</returns>
    public static AnthropicChatLanguageModel Claude3Haiku(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("claude-3-haiku-20240307", apiKey, httpClient: httpClient);
    }
}
