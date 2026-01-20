using System.Text.Json.Serialization;

namespace AiSdk.Providers.Luma.Models;

/// <summary>
/// Luma AI streaming response chunk.
/// Note: This is a placeholder for future video generation features.
/// </summary>
internal record LumaStreamResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("created")]
    public required long Created { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("choices")]
    public required List<LumaStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public LumaUsage? Usage { get; init; }
}

/// <summary>
/// Luma streaming choice.
/// </summary>
internal record LumaStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required LumaDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Luma delta (partial content in streaming).
/// </summary>
internal record LumaDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }
}
