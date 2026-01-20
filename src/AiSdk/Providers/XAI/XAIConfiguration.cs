namespace AiSdk.Providers.XAI;

/// <summary>
/// Configuration for xAI (Grok) API client.
/// </summary>
public record XAIConfiguration
{
    /// <summary>
    /// Gets or sets the xAI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the xAI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.x.ai/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
