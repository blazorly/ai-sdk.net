namespace AiSdk.Providers.HuggingFace;

/// <summary>
/// Configuration for Hugging Face Inference API client.
/// </summary>
public record HuggingFaceConfiguration
{
    /// <summary>
    /// Gets or sets the Hugging Face API key (required).
    /// </summary>
    public required string ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Hugging Face API base URL.
    /// Defaults to https://api-inference.huggingface.co/models/
    /// </summary>
    public string BaseUrl { get; init; } = "https://api-inference.huggingface.co/models/";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
