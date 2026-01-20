namespace AiSdk.Providers.Novita;

/// <summary>
/// Configuration for Novita API client.
/// </summary>
public record NovitaConfiguration
{
    /// <summary>
    /// Gets or sets the Novita API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Novita API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.novita.ai/v3/openai/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
