using System.Text.Json.Serialization;

namespace AiSdk.Providers.Baseten.Models;

/// <summary>
/// Baseten streaming chat completion response chunk.
/// </summary>
internal record BasetenStreamResponse
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
    public required List<BasetenStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public BasetenUsage? Usage { get; init; }
}

/// <summary>
/// Baseten streaming choice.
/// </summary>
internal record BasetenStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required BasetenDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Baseten delta (partial content in streaming).
/// </summary>
internal record BasetenDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<BasetenToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Baseten tool call delta.
/// </summary>
internal record BasetenToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public BasetenFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Baseten function call delta.
/// </summary>
internal record BasetenFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
