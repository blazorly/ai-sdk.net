namespace AiSdk.Providers.Cloudflare;

/// <summary>
/// Configuration for Cloudflare Workers AI client.
/// </summary>
public record CloudflareConfiguration
{
    /// <summary>
    /// Gets or sets the Cloudflare API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Cloudflare account ID.
    /// </summary>
    public required string AccountId { get; init; }

    /// <summary>
    /// Gets or sets the Cloudflare API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.cloudflare.com/client/v4/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
