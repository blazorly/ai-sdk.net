using System.Text.Json.Serialization;

namespace AiSdk.Providers.Google.Models;

/// <summary>
/// Google embeddings API request.
/// </summary>
internal record GoogleEmbeddingRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("content")]
    public required GoogleEmbeddingContent Content { get; init; }

    [JsonPropertyName("outputDimensionality")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? OutputDimensionality { get; init; }
}

/// <summary>
/// Google batch embeddings API request.
/// </summary>
internal record GoogleBatchEmbeddingRequest
{
    [JsonPropertyName("requests")]
    public required List<GoogleEmbeddingRequest> Requests { get; init; }
}

/// <summary>
/// Content for an embedding request.
/// </summary>
internal record GoogleEmbeddingContent
{
    [JsonPropertyName("parts")]
    public required List<GoogleEmbeddingPart> Parts { get; init; }
}

/// <summary>
/// A part in an embedding content.
/// </summary>
internal record GoogleEmbeddingPart
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }
}

/// <summary>
/// Google embedding API response.
/// </summary>
internal record GoogleEmbeddingResponse
{
    [JsonPropertyName("embedding")]
    public required GoogleEmbeddingValues Embedding { get; init; }
}

/// <summary>
/// Google batch embedding API response.
/// </summary>
internal record GoogleBatchEmbeddingResponse
{
    [JsonPropertyName("embeddings")]
    public required List<GoogleEmbeddingValues> Embeddings { get; init; }
}

/// <summary>
/// Embedding values in a Google response.
/// </summary>
internal record GoogleEmbeddingValues
{
    [JsonPropertyName("values")]
    public required float[] Values { get; init; }
}
