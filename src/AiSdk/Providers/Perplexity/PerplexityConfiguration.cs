namespace AiSdk.Providers.Perplexity;

/// <summary>
/// Configuration for Perplexity API client.
/// </summary>
public record PerplexityConfiguration
{
    /// <summary>
    /// Gets or sets the Perplexity API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Perplexity API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.perplexity.ai";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
