namespace AiSdk.Providers.ZAI;

/// <summary>
/// Configuration for Z.AI API client.
/// </summary>
public record ZAIConfiguration
{
    /// <summary>
    /// Gets or sets the Z.AI API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Z.AI API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.z.ai/api/paas/v4/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
