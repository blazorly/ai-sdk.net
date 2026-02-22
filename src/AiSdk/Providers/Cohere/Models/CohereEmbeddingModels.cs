using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cohere.Models;

/// <summary>
/// Cohere embed API request.
/// </summary>
internal record CohereEmbeddingRequest
{
    [JsonPropertyName("texts")]
    public required List<string> Texts { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("input_type")]
    public string InputType { get; init; } = "search_document";

    [JsonPropertyName("truncate")]
    public string Truncate { get; init; } = "END";

    [JsonPropertyName("embedding_types")]
    public List<string> EmbeddingTypes { get; init; } = new() { "float" };
}

/// <summary>
/// Cohere embed API response.
/// </summary>
internal record CohereEmbeddingResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("embeddings")]
    public required CohereEmbeddings Embeddings { get; init; }

    [JsonPropertyName("texts")]
    public List<string>? Texts { get; init; }

    [JsonPropertyName("meta")]
    public CohereEmbeddingMeta? Meta { get; init; }
}

/// <summary>
/// Cohere embeddings container (supports multiple types).
/// </summary>
internal record CohereEmbeddings
{
    [JsonPropertyName("float")]
    public List<float[]>? Float { get; init; }
}

/// <summary>
/// Cohere embedding response metadata.
/// </summary>
internal record CohereEmbeddingMeta
{
    [JsonPropertyName("api_version")]
    public CohereApiVersion? ApiVersion { get; init; }

    [JsonPropertyName("billed_units")]
    public CohereBilledUnits? BilledUnits { get; init; }
}

// CohereApiVersion and CohereBilledUnits are defined in CohereResponse.cs
