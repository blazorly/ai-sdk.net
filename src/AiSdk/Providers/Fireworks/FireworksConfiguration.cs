namespace AiSdk.Providers.Fireworks;

/// <summary>
/// Configuration for Fireworks AI API client.
/// </summary>
public record FireworksConfiguration
{
    /// <summary>
    /// Gets or sets the Fireworks AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Fireworks AI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.fireworks.ai/inference/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
