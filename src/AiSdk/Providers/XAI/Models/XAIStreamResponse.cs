using System.Text.Json.Serialization;

namespace AiSdk.Providers.XAI.Models;

/// <summary>
/// xAI streaming chat completion response chunk.
/// </summary>
internal record XAIStreamResponse
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
    public required List<XAIStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public XAIUsage? Usage { get; init; }
}

/// <summary>
/// xAI streaming choice.
/// </summary>
internal record XAIStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required XAIDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// xAI delta (partial content in streaming).
/// </summary>
internal record XAIDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<XAIToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// xAI tool call delta.
/// </summary>
internal record XAIToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public XAIFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// xAI function call delta.
/// </summary>
internal record XAIFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
