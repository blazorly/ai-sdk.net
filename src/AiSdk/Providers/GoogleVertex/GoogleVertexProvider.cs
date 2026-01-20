namespace AiSdk.Providers.GoogleVertex;

/// <summary>
/// Factory for creating Google Vertex AI language models.
/// Supports both Google Gemini and Anthropic Claude models through Vertex AI.
/// </summary>
public static class GoogleVertexProvider
{
    /// <summary>
    /// Creates a Google Vertex AI model with the specified model ID.
    /// Automatically detects whether to use Gemini or Claude based on the model ID.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "gemini-1.5-pro", "claude-3-5-sonnet-20241022").</param>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region (e.g., "us-central1").</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://{location}-aiplatform.googleapis.com/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Google Vertex AI language model.</returns>
    public static GoogleVertexLanguageModel CreateModel(
        string modelId,
        string projectId,
        string location,
        string accessToken,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new GoogleVertexConfiguration
        {
            ProjectId = projectId,
            Location = location,
            AccessToken = accessToken,
            BaseUrl = baseUrl
        };

        return new GoogleVertexLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Google Vertex AI model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="config">The Google Vertex AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Google Vertex AI language model.</returns>
    public static GoogleVertexLanguageModel CreateModel(
        string modelId,
        GoogleVertexConfiguration config,
        HttpClient? httpClient = null)
    {
        return new GoogleVertexLanguageModel(modelId, config, httpClient);
    }

    #region Gemini Models

    /// <summary>
    /// Creates a Gemini 1.5 Pro model.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Gemini 1.5 Pro model.</returns>
    public static GoogleVertexLanguageModel Gemini15Pro(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("gemini-1.5-pro", projectId, location, accessToken, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Gemini 1.5 Flash model.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Gemini 1.5 Flash model.</returns>
    public static GoogleVertexLanguageModel Gemini15Flash(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("gemini-1.5-flash", projectId, location, accessToken, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Gemini 1.0 Pro model.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Gemini 1.0 Pro model.</returns>
    public static GoogleVertexLanguageModel Gemini10Pro(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("gemini-1.0-pro", projectId, location, accessToken, httpClient: httpClient);
    }

    #endregion

    #region Claude Models

    /// <summary>
    /// Creates a Claude 3.5 Sonnet model (latest version).
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3.5 Sonnet model.</returns>
    public static GoogleVertexLanguageModel Claude35Sonnet(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("claude-3-5-sonnet-20241022", projectId, location, accessToken, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Claude 3 Opus model.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3 Opus model.</returns>
    public static GoogleVertexLanguageModel Claude3Opus(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("claude-3-opus-20240229", projectId, location, accessToken, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Claude 3 Sonnet model.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3 Sonnet model.</returns>
    public static GoogleVertexLanguageModel Claude3Sonnet(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("claude-3-sonnet-20240229", projectId, location, accessToken, httpClient: httpClient);
    }

    /// <summary>
    /// Creates a Claude 3 Haiku model.
    /// </summary>
    /// <param name="projectId">The Google Cloud project ID.</param>
    /// <param name="location">The Google Cloud location/region.</param>
    /// <param name="accessToken">The Google Cloud OAuth2 access token.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Claude 3 Haiku model.</returns>
    public static GoogleVertexLanguageModel Claude3Haiku(
        string projectId,
        string location,
        string accessToken,
        HttpClient? httpClient = null)
    {
        return CreateModel("claude-3-haiku-20240307", projectId, location, accessToken, httpClient: httpClient);
    }

    #endregion
}
