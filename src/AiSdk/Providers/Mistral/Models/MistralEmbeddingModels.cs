using System.Text.Json.Serialization;

namespace AiSdk.Providers.Mistral.Models;

/// <summary>
/// Mistral embedding API request.
/// </summary>
internal record MistralEmbeddingRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("input")]
    public required List<string> Input { get; init; }

    [JsonPropertyName("encoding_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncodingFormat { get; init; }
}

/// <summary>
/// Mistral embedding API response.
/// </summary>
internal record MistralEmbeddingResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("object")]
    public string Object { get; init; } = "list";

    [JsonPropertyName("data")]
    public required List<MistralEmbeddingData> Data { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("usage")]
    public required MistralEmbeddingUsage Usage { get; init; }
}

/// <summary>
/// A single embedding in the Mistral response.
/// </summary>
internal record MistralEmbeddingData
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "embedding";

    [JsonPropertyName("embedding")]
    public required float[] Embedding { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }
}

/// <summary>
/// Usage statistics for Mistral embedding requests.
/// </summary>
internal record MistralEmbeddingUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}
