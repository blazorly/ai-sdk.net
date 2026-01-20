using System.Text.Json.Serialization;

namespace AiSdk.Providers.Fal.Models;

/// <summary>
/// Fal AI streaming chat completion response chunk.
/// </summary>
internal record FalStreamResponse
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
    public required List<FalStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public FalUsage? Usage { get; init; }
}

/// <summary>
/// Fal AI streaming choice.
/// </summary>
internal record FalStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required FalDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Fal AI delta (partial content in streaming).
/// </summary>
internal record FalDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<FalToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Fal AI tool call delta.
/// </summary>
internal record FalToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public FalFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Fal AI function call delta.
/// </summary>
internal record FalFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
