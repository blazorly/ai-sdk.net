namespace AiSdk.Providers.Fal;

/// <summary>
/// Configuration for Fal AI API client.
/// </summary>
public record FalConfiguration
{
    /// <summary>
    /// Gets or sets the Fal AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Fal AI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://fal.run";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
