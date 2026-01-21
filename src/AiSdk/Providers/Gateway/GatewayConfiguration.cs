namespace AiSdk.Providers.Gateway;

/// <summary>
/// Configuration for Vercel AI Gateway API client.
/// </summary>
public record GatewayConfiguration
{
    /// <summary>
    /// Gets or sets the base URL prefix for API calls.
    /// Defaults to "https://ai-gateway.vercel.sh/v3/ai".
    /// </summary>
    public string BaseUrl { get; init; } = "https://ai-gateway.vercel.sh/v3/ai";

    /// <summary>
    /// Gets or sets the API key that is being sent using the Authorization header.
    /// Can also be set via AI_GATEWAY_API_KEY environment variable.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gets or sets custom headers to include in the requests.
    /// </summary>
    public Dictionary<string, string>? Headers { get; init; }

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }

    /// <summary>
    /// Gets or sets how frequently to refresh the metadata cache in milliseconds.
    /// Defaults to 5 minutes (300,000ms).
    /// </summary>
    public int MetadataCacheRefreshMillis { get; init; } = 1000 * 60 * 5;

    /// <summary>
    /// Gets or sets internal testing options (for internal use only).
    /// </summary>
    public GatewayInternalSettings? Internal { get; init; }
}

/// <summary>
/// Internal settings for testing purposes.
/// </summary>
public record GatewayInternalSettings
{
    /// <summary>
    /// Gets or sets a function to get the current date (for testing).
    /// </summary>
    public Func<DateTime>? CurrentDate { get; init; }
}
