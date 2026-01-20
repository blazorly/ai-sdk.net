using System.Text.Json.Serialization;

namespace AiSdk.Providers.Mistral.Models;

/// <summary>
/// Mistral AI streaming chat completion response chunk.
/// </summary>
internal record MistralStreamResponse
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
    public required List<MistralStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public MistralUsage? Usage { get; init; }
}

/// <summary>
/// Mistral AI streaming choice.
/// </summary>
internal record MistralStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required MistralDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Mistral AI delta (partial content in streaming).
/// </summary>
internal record MistralDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<MistralToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Mistral AI tool call delta.
/// </summary>
internal record MistralToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public MistralFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Mistral AI function call delta.
/// </summary>
internal record MistralFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
