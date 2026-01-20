namespace AiSdk.Providers.Cohere;

/// <summary>
/// Configuration for Cohere API client.
/// </summary>
public record CohereConfiguration
{
    /// <summary>
    /// Gets or sets the Cohere API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Cohere API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.cohere.ai/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
