using System.Text.Json.Serialization;

namespace AiSdk.Providers.HuggingFace.Models;

/// <summary>
/// Hugging Face Inference API response.
/// </summary>
internal record HuggingFaceResponse
{
    [JsonPropertyName("generated_text")]
    public string? GeneratedText { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}

/// <summary>
/// Hugging Face error response.
/// </summary>
internal record HuggingFaceErrorResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; init; }

    [JsonPropertyName("error_type")]
    public string? ErrorType { get; init; }
}
