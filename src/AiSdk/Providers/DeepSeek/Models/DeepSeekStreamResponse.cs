using System.Text.Json.Serialization;

namespace AiSdk.Providers.DeepSeek.Models;

/// <summary>
/// DeepSeek streaming chat completion response chunk.
/// </summary>
internal record DeepSeekStreamResponse
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
    public required List<DeepSeekStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public DeepSeekUsage? Usage { get; init; }
}

/// <summary>
/// DeepSeek streaming choice.
/// </summary>
internal record DeepSeekStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required DeepSeekDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// DeepSeek delta (partial content in streaming).
/// </summary>
internal record DeepSeekDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<DeepSeekToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// DeepSeek tool call delta.
/// </summary>
internal record DeepSeekToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public DeepSeekFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// DeepSeek function call delta.
/// </summary>
internal record DeepSeekFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
