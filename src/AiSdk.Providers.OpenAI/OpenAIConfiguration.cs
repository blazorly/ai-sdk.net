namespace AiSdk.Providers.OpenAI;

/// <summary>
/// Configuration for OpenAI API client.
/// </summary>
public record OpenAIConfiguration
{
    /// <summary>
    /// Gets or sets the OpenAI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the OpenAI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.openai.com/v1";

    /// <summary>
    /// Gets or sets the organization ID (optional).
    /// </summary>
    public string? Organization { get; init; }

    /// <summary>
    /// Gets or sets the project ID (optional).
    /// </summary>
    public string? Project { get; init; }

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
