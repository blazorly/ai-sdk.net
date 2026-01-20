using System.Text.Json.Serialization;

namespace AiSdk.Providers.TogetherAI.Models;

/// <summary>
/// Together AI streaming chat completion response chunk.
/// </summary>
internal record TogetherAIStreamResponse
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
    public required List<TogetherAIStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public TogetherAIUsage? Usage { get; init; }
}

/// <summary>
/// Together AI streaming choice.
/// </summary>
internal record TogetherAIStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required TogetherAIDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Together AI delta (partial content in streaming).
/// </summary>
internal record TogetherAIDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<TogetherAIToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Together AI tool call delta.
/// </summary>
internal record TogetherAIToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public TogetherAIFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Together AI function call delta.
/// </summary>
internal record TogetherAIFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
