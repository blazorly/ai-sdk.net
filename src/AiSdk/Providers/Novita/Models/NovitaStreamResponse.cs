using System.Text.Json.Serialization;

namespace AiSdk.Providers.Novita.Models;

/// <summary>
/// Novita streaming chat completion response chunk.
/// </summary>
internal record NovitaStreamResponse
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
    public required List<NovitaStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public NovitaUsage? Usage { get; init; }
}

/// <summary>
/// Novita streaming choice.
/// </summary>
internal record NovitaStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required NovitaDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Novita delta (partial content in streaming).
/// </summary>
internal record NovitaDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<NovitaToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Novita tool call delta.
/// </summary>
internal record NovitaToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public NovitaFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Novita function call delta.
/// </summary>
internal record NovitaFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
