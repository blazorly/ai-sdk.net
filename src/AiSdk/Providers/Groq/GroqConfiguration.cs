namespace AiSdk.Providers.Groq;

/// <summary>
/// Configuration for Groq API client.
/// </summary>
public record GroqConfiguration
{
    /// <summary>
    /// Gets or sets the Groq API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Groq API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.groq.com/openai/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
