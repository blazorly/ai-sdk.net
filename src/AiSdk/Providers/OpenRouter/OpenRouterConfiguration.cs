namespace AiSdk.Providers.OpenRouter;

/// <summary>
/// Configuration for OpenRouter AI client.
/// </summary>
public record OpenRouterConfiguration
{
    /// <summary>
    /// Gets or sets the OpenRouter API key.
    /// Required for authenticating API requests to OpenRouter.
    /// Get your API key from https://openrouter.ai/keys
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the OpenRouter base URL.
    /// Defaults to the official OpenRouter API endpoint.
    /// </summary>
    public string BaseUrl { get; init; } = "https://openrouter.ai/api/v1/";

    /// <summary>
    /// Gets or sets the site URL for HTTP-Referer header (optional).
    /// This helps OpenRouter attribute requests to your site and can be required for some models.
    /// </summary>
    public string? SiteUrl { get; init; }

    /// <summary>
    /// Gets or sets the app name for X-Title header (optional).
    /// This helps identify your application in OpenRouter's analytics.
    /// </summary>
    public string? AppName { get; init; }

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
