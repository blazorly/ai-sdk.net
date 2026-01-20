using System.Text.Json.Serialization;

namespace AiSdk.Providers.Fireworks.Models;

/// <summary>
/// Fireworks AI streaming chat completion response chunk.
/// </summary>
internal record FireworksStreamResponse
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
    public required List<FireworksStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public FireworksUsage? Usage { get; init; }
}

/// <summary>
/// Fireworks AI streaming choice.
/// </summary>
internal record FireworksStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required FireworksDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Fireworks AI delta (partial content in streaming).
/// </summary>
internal record FireworksDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<FireworksToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Fireworks AI tool call delta.
/// </summary>
internal record FireworksToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public FireworksFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Fireworks AI function call delta.
/// </summary>
internal record FireworksFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
