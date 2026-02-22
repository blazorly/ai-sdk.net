using System.Text.Json.Serialization;

namespace AiSdk.Providers.Azure.Models;

/// <summary>
/// Azure OpenAI embedding API request.
/// Uses the same format as OpenAI but deployed through Azure.
/// </summary>
internal record AzureOpenAIEmbeddingRequest
{
    [JsonPropertyName("input")]
    public required object Input { get; init; }

    [JsonPropertyName("dimensions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Dimensions { get; init; }

    [JsonPropertyName("encoding_format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EncodingFormat { get; init; }
}

/// <summary>
/// Azure OpenAI embedding API response.
/// </summary>
internal record AzureOpenAIEmbeddingResponse
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "list";

    [JsonPropertyName("data")]
    public required List<AzureOpenAIEmbeddingData> Data { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("usage")]
    public required AzureOpenAIEmbeddingUsage Usage { get; init; }
}

/// <summary>
/// A single embedding in the Azure OpenAI response.
/// </summary>
internal record AzureOpenAIEmbeddingData
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = "embedding";

    [JsonPropertyName("embedding")]
    public required float[] Embedding { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }
}

/// <summary>
/// Usage statistics for Azure OpenAI embedding requests.
/// </summary>
internal record AzureOpenAIEmbeddingUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}
