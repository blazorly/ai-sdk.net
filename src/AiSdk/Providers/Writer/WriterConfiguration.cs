namespace AiSdk.Providers.Writer;

/// <summary>
/// Configuration for Writer API client.
/// </summary>
public record WriterConfiguration
{
    /// <summary>
    /// Gets or sets the Writer API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Writer API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.writer.com/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
