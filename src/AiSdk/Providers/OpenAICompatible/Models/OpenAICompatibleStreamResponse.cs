using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenAICompatible.Models;

/// <summary>
/// OpenAI-compatible streaming chat completion response chunk.
/// </summary>
internal record OpenAICompatibleStreamResponse
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
    public required List<OpenAICompatibleStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public OpenAICompatibleUsage? Usage { get; init; }
}

/// <summary>
/// OpenAI-compatible streaming choice.
/// </summary>
internal record OpenAICompatibleStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required OpenAICompatibleDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// OpenAI-compatible delta (partial content in streaming).
/// </summary>
internal record OpenAICompatibleDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>
    /// Reasoning / "thinking" content emitted by reasoning-capable models
    /// surfaced through OpenAI-compatible aggregators (OpenRouter, etc.).
    ///
    /// OpenRouter carries this as a top-level <c>reasoning</c> field on the
    /// streaming delta. We surface it as a separate chunk type so the UI
    /// can render it as a distinct thinking block. Providers that don't
    /// support reasoning simply omit this field, in which case it stays null.
    /// </summary>
    [JsonPropertyName("reasoning")]
    public string? Reasoning { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<OpenAICompatibleToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// OpenAI-compatible tool call delta.
/// </summary>
internal record OpenAICompatibleToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public OpenAICompatibleFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// OpenAI-compatible function call delta.
/// </summary>
internal record OpenAICompatibleFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
