namespace AiSdk.Providers.Cerebras;

/// <summary>
/// Configuration for Cerebras API client.
/// </summary>
public record CerebrasConfiguration
{
    /// <summary>
    /// Gets or sets the Cerebras API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Cerebras API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.cerebras.ai/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
