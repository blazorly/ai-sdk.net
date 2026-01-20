namespace AiSdk.Providers.Portkey;

/// <summary>
/// Configuration for Portkey AI Gateway client.
/// </summary>
public record PortkeyConfiguration
{
    /// <summary>
    /// Gets or sets the Portkey API key.
    /// This is your Portkey account API key used to authenticate with the gateway.
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the target provider name (optional).
    /// Examples: "openai", "anthropic", "cohere", "azure-openai", etc.
    /// If not set, you can specify it per-request or use virtual keys.
    /// </summary>
    public string? Provider { get; init; }

    /// <summary>
    /// Gets or sets the virtual key for the target provider (optional).
    /// Virtual keys allow you to store provider API keys securely in Portkey's vault.
    /// If set, this will be used instead of passing provider API keys directly.
    /// </summary>
    public string? VirtualKey { get; init; }

    /// <summary>
    /// Gets or sets the Portkey AI Gateway base URL.
    /// Defaults to the official Portkey API endpoint.
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.portkey.ai/v1";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
