namespace AiSdk.Providers.Baseten;

/// <summary>
/// Configuration for Baseten API client.
/// </summary>
public record BasetenConfiguration
{
    /// <summary>
    /// Gets or sets the Baseten API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Baseten API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.baseten.co/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
