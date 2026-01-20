using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cerebras.Models;

/// <summary>
/// Cerebras streaming chat completion response chunk.
/// </summary>
internal record CerebrasStreamResponse
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
    public required List<CerebrasStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public CerebrasUsage? Usage { get; init; }
}

/// <summary>
/// Cerebras streaming choice.
/// </summary>
internal record CerebrasStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required CerebrasDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Cerebras delta (partial content in streaming).
/// </summary>
internal record CerebrasDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<CerebrasToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Cerebras tool call delta.
/// </summary>
internal record CerebrasToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public CerebrasFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Cerebras function call delta.
/// </summary>
internal record CerebrasFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
