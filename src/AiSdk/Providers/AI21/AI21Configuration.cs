namespace AiSdk.Providers.AI21;

/// <summary>
/// Configuration for AI21 Labs API client.
/// </summary>
public record AI21Configuration
{
    /// <summary>
    /// Gets or sets the AI21 Labs API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the AI21 Labs API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.ai21.com/studio/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
