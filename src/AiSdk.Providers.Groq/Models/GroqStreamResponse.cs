using System.Text.Json.Serialization;

namespace AiSdk.Providers.Groq.Models;

/// <summary>
/// Groq streaming chat completion response chunk.
/// </summary>
internal record GroqStreamResponse
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
    public required List<GroqStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public GroqUsage? Usage { get; init; }
}

/// <summary>
/// Groq streaming choice.
/// </summary>
internal record GroqStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required GroqDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Groq delta (partial content in streaming).
/// </summary>
internal record GroqDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<GroqToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Groq tool call delta.
/// </summary>
internal record GroqToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public GroqFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Groq function call delta.
/// </summary>
internal record GroqFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
