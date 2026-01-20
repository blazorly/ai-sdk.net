namespace AiSdk.Providers.Stability;

/// <summary>
/// Configuration for Stability AI API client.
/// </summary>
public record StabilityConfiguration
{
    /// <summary>
    /// Gets or sets the Stability AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Stability AI API base URL.
    /// Default is "https://api.stability.ai/v2beta" for image generation.
    /// For self-hosted StableLM models, use your deployment URL (e.g., "http://localhost:8000/v1").
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.stability.ai/v2beta/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
