namespace AiSdk.Providers.Google;

/// <summary>
/// Factory for creating Google Gemini language models.
/// </summary>
public static class GoogleProvider
{
    /// <summary>
    /// Creates a Google Gemini chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Google model ID (e.g., "gemini-1.5-pro", "gemini-1.5-flash", "gemini-1.0-pro").</param>
    /// <param name="apiKey">The Google API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://generativelanguage.googleapis.com/v1beta).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Google Gemini chat language model.</returns>
    public static GoogleChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new GoogleConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://generativelanguage.googleapis.com/v1beta"
        };

        return new GoogleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Google Gemini chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Google model ID.</param>
    /// <param name="config">The Google configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Google Gemini chat language model.</returns>
    public static GoogleChatLanguageModel CreateChatModel(
        string modelId,
        GoogleConfiguration config,
        HttpClient? httpClient = null)
    {
        return new GoogleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Gemini 1.5 Pro model.
    /// </summary>
    /// <param name="apiKey">The Google API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Gemini 1.5 Pro chat model.</returns>
    public static GoogleChatLanguageModel Gemini15Pro(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("gemini-1.5-pro", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Gemini 1.5 Flash model.
    /// </summary>
    /// <param name="apiKey">The Google API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Gemini 1.5 Flash chat model.</returns>
    public static GoogleChatLanguageModel Gemini15Flash(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("gemini-1.5-flash", apiKey, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Gemini 1.0 Pro model.
    /// </summary>
    /// <param name="apiKey">The Google API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Gemini 1.0 Pro chat model.</returns>
    public static GoogleChatLanguageModel Gemini10Pro(
        string apiKey,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("gemini-1.0-pro", apiKey, httpClient: httpClient);
    }
}
