namespace AiSdk.Providers.Vercel;

/// <summary>
/// Configuration for Vercel AI Gateway client.
/// </summary>
public record VercelConfiguration
{
    /// <summary>
    /// Gets or sets the Vercel AI Gateway API key.
    /// This can be a Vercel API token or a custom API key configured in your AI Gateway settings.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Vercel AI Gateway base URL.
    /// Defaults to the official Vercel AI Gateway endpoint.
    /// </summary>
    public string BaseUrl { get; init; } = "https://ai-gateway.vercel.sh/v3/ai";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
