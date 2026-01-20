namespace AiSdk.Providers.OpenAICompatible;

/// <summary>
/// Configuration for OpenAI-compatible API client.
/// This allows connecting to any OpenAI-compatible endpoint such as Ollama, LocalAI, vLLM, LM Studio, etc.
/// </summary>
public record OpenAICompatibleConfiguration
{
    /// <summary>
    /// Gets or sets the API key (optional for some local endpoints).
    /// Many local servers like Ollama don't require an API key.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the base URL for the OpenAI-compatible API.
    /// Examples:
    /// - Ollama: http://localhost:11434/v1
    /// - LocalAI: http://localhost:8080/v1
    /// - vLLM: http://localhost:8000/v1
    /// - LM Studio: http://localhost:1234/v1
    /// - Text Generation WebUI: http://localhost:5000/v1
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
