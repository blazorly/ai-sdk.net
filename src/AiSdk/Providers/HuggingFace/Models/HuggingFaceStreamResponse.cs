using System.Text.Json.Serialization;

namespace AiSdk.Providers.HuggingFace.Models;

/// <summary>
/// Hugging Face streaming response chunk.
/// </summary>
internal record HuggingFaceStreamResponse
{
    [JsonPropertyName("token")]
    public HuggingFaceToken? Token { get; init; }

    [JsonPropertyName("generated_text")]
    public string? GeneratedText { get; init; }

    [JsonPropertyName("details")]
    public HuggingFaceDetails? Details { get; init; }
}

/// <summary>
/// Hugging Face token information.
/// </summary>
internal record HuggingFaceToken
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("logprob")]
    public double? Logprob { get; init; }

    [JsonPropertyName("special")]
    public bool Special { get; init; }
}

/// <summary>
/// Hugging Face completion details.
/// </summary>
internal record HuggingFaceDetails
{
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }

    [JsonPropertyName("generated_tokens")]
    public int? GeneratedTokens { get; init; }

    [JsonPropertyName("seed")]
    public long? Seed { get; init; }
}
