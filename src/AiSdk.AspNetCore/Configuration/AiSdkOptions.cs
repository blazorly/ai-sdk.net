namespace AiSdk.AspNetCore.Configuration;

/// <summary>
/// Configuration options for AI SDK ASP.NET Core integration.
/// </summary>
public class AiSdkOptions
{
    /// <summary>
    /// The default provider to use when not explicitly specified.
    /// </summary>
    /// <example>openai, anthropic, google</example>
    public string? DefaultProvider { get; set; }

    /// <summary>
    /// Provider-specific configuration settings.
    /// </summary>
    /// <remarks>
    /// Dictionary keys are provider names (e.g., "openai", "anthropic").
    /// Values contain provider-specific configuration such as API keys, base URLs, etc.
    /// </remarks>
    public Dictionary<string, ProviderConfiguration> Providers { get; set; } = new();

    /// <summary>
    /// Global timeout for all AI provider calls in seconds.
    /// </summary>
    /// <remarks>
    /// Can be overridden by provider-specific timeout settings.
    /// Default is null (no timeout).
    /// </remarks>
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// Enable health checks for AI providers.
    /// </summary>
    public bool EnableHealthChecks { get; set; } = true;

    /// <summary>
    /// Enable telemetry and observability features.
    /// </summary>
    public bool EnableTelemetry { get; set; } = true;
}

/// <summary>
/// Configuration for a specific AI provider.
/// </summary>
public class ProviderConfiguration
{
    /// <summary>
    /// API key for the provider.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Base URL for the provider's API.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Default model to use for this provider.
    /// </summary>
    public string? DefaultModel { get; set; }

    /// <summary>
    /// Organization ID (if applicable).
    /// </summary>
    public string? OrganizationId { get; set; }

    /// <summary>
    /// Provider-specific timeout in seconds.
    /// </summary>
    public int? TimeoutSeconds { get; set; }

    /// <summary>
    /// Additional provider-specific settings.
    /// </summary>
    public Dictionary<string, string> AdditionalSettings { get; set; } = new();

    /// <summary>
    /// Whether this provider is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
