using System.Text.Json.Serialization;

namespace AiSdk.Providers.ZAI.Models;

/// <summary>
/// Z.AI streaming chat completion response chunk.
/// </summary>
internal record ZAIStreamResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("object")]
    public string? Object { get; init; }

    [JsonPropertyName("created")]
    public long? Created { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("choices")]
    public required List<ZAIStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public ZAIUsage? Usage { get; init; }
}

/// <summary>
/// Z.AI streaming choice.
/// </summary>
internal record ZAIStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required ZAIStreamDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Z.AI streaming delta content.
/// </summary>
internal record ZAIStreamDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("reasoning_content")]
    public string? ReasoningContent { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<ZAIStreamToolCall>? ToolCalls { get; init; }
}

/// <summary>
/// Z.AI streaming tool call.
/// </summary>
internal record ZAIStreamToolCall
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public ZAIStreamToolCallFunction? Function { get; init; }
}

/// <summary>
/// Z.AI streaming function call details.
/// </summary>
internal record ZAIStreamToolCallFunction
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
