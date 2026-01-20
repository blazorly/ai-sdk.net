namespace AiSdk.Providers.LlamaFile;

/// <summary>
/// Configuration for LlamaFile local server client.
/// </summary>
public record LlamaFileConfiguration
{
    /// <summary>
    /// Gets or sets the API key (optional for local use, defaults to empty string).
    /// Most local llamafile instances don't require authentication.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the LlamaFile server base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "http://localhost:8080/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// Local models may need longer timeouts for initial processing.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
