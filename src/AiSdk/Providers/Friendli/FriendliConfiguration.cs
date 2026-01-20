namespace AiSdk.Providers.Friendli;

/// <summary>
/// Configuration for Friendli API client.
/// </summary>
public record FriendliConfiguration
{
    /// <summary>
    /// Gets or sets the Friendli API key.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Friendli API base URL.
    /// </summary>
    public string BaseUrl { get; init; } = "https://inference.friendli.ai/v1/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
