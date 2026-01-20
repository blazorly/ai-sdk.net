namespace AiSdk.Providers.Google;

/// <summary>
/// Configuration for Google Gemini API client.
/// </summary>
public record GoogleConfiguration
{
    /// <summary>
    /// Gets or sets the Google API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Google API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://generativelanguage.googleapis.com/v1beta";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
