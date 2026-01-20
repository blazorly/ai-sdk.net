namespace AiSdk.Providers.Lepton;

/// <summary>
/// Configuration for Lepton AI API client.
/// </summary>
public record LeptonConfiguration
{
    /// <summary>
    /// Gets or sets the Lepton AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Lepton AI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.lepton.ai/api/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
