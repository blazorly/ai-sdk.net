namespace AiSdk.Providers.Mistral;

/// <summary>
/// Configuration for Mistral AI API client.
/// </summary>
public record MistralConfiguration
{
    /// <summary>
    /// Gets or sets the Mistral AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Mistral AI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.mistral.ai/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
