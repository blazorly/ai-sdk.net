using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenAI.Models;

/// <summary>
/// OpenAI embedding API request.
/// </summary>
internal record OpenAIEmbeddingRequest
{
    [JsonPropertyName("input")]
    public required object Input { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("dimensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Dimensions { get; init; }

    [JsonPropertyName("encoding_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncodingFormat { get; init; }
}

/// <summary>
/// OpenAI embedding API response.
/// </summary>
internal record OpenAIEmbeddingResponse
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "list";

    [JsonPropertyName("data")]
    public required List<OpenAIEmbeddingData> Data { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("usage")]
    public required OpenAIEmbeddingUsage Usage { get; init; }
}

/// <summary>
/// A single embedding in the response.
/// </summary>
internal record OpenAIEmbeddingData
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "embedding";

    [JsonPropertyName("embedding")]
    public required float[] Embedding { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }
}

/// <summary>
/// Usage statistics for embedding requests.
/// </summary>
internal record OpenAIEmbeddingUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}
