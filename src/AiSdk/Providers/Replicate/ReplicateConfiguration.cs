namespace AiSdk.Providers.Replicate;

/// <summary>
/// Configuration for Replicate API client.
/// </summary>
public record ReplicateConfiguration
{
    /// <summary>
    /// Gets or sets the Replicate API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Replicate API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.replicate.com/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
