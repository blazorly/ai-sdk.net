namespace AiSdk.Providers.Mistral;

/// <summary>
/// Factory for creating Mistral AI language models.
/// </summary>
public static class MistralProvider
{
    /// <summary>
    /// Creates a Mistral AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Mistral AI model ID (e.g., "mistral-large-latest", "mistral-medium-latest").</param>
    /// <param name="apiKey">The Mistral AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.mistral.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Mistral AI chat language model.</returns>
    public static MistralChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new MistralConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.mistral.ai/v1"
        };

        return new MistralChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Mistral AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Mistral AI model ID.</param>
    /// <param name="config">The Mistral AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Mistral AI chat language model.</returns>
    public static MistralChatLanguageModel CreateChatModel(
        string modelId,
        MistralConfiguration config,
        HttpClient? httpClient = null)
    {
        return new MistralChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Mistral Large model (most capable).
    /// </summary>
    /// <param name="apiKey">The Mistral AI API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Mistral Large chat model.</returns>
    public static MistralChatLanguageModel MistralLarge(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("mistral-large-latest", apiKey, null, httpClient);
    }

    /// <summary>
    /// Creates a Mistral Medium model (balanced performance).
    /// </summary>
    /// <param name="apiKey">The Mistral AI API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Mistral Medium chat model.</returns>
    public static MistralChatLanguageModel MistralMedium(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("mistral-medium-latest", apiKey, null, httpClient);
    }

    /// <summary>
    /// Creates a Mistral Small model (fast and efficient).
    /// </summary>
    /// <param name="apiKey">The Mistral AI API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Mistral Small chat model.</returns>
    public static MistralChatLanguageModel MistralSmall(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("mistral-small-latest", apiKey, null, httpClient);
    }

    /// <summary>
    /// Creates a generic chat model with the specified model ID (for any Mistral model).
    /// </summary>
    /// <param name="modelId">The Mistral AI model ID.</param>
    /// <param name="apiKey">The Mistral AI API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Mistral AI chat model.</returns>
    public static MistralChatLanguageModel ChatModel(
        string modelId,
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel(modelId, apiKey, null, httpClient);
    }
}
