namespace AiSdk.Providers.Anthropic;

/// <summary>
/// Configuration for Anthropic API client.
/// </summary>
public record AnthropicConfiguration
{
    /// <summary>
    /// Gets or sets the Anthropic API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Anthropic API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.anthropic.com/v1";

    /// <summary>
    /// Gets or sets the Anthropic API version header.
    /// </summary>
    public string ApiVersion { get; init; } = "2023-06-01";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
