namespace AiSdk.Providers.TogetherAI;

/// <summary>
/// Configuration for Together AI API client.
/// </summary>
public record TogetherAIConfiguration
{
    /// <summary>
    /// Gets or sets the Together AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Together AI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.together.xyz/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
