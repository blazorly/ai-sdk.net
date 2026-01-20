namespace AiSdk.Providers.DeepSeek;

/// <summary>
/// Configuration for DeepSeek API client.
/// </summary>
public record DeepSeekConfiguration
{
    /// <summary>
    /// Gets or sets the DeepSeek API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the DeepSeek API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.deepseek.com/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
