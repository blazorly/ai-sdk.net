using System.Text.Json.Serialization;

namespace AiSdk.Providers.Portkey.Models;

/// <summary>
/// Portkey AI Gateway streaming chat completion response chunk.
/// </summary>
internal record PortkeyStreamResponse
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
    public required List<PortkeyStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public PortkeyUsage? Usage { get; init; }
}

/// <summary>
/// Portkey streaming choice.
/// </summary>
internal record PortkeyStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required PortkeyDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Portkey delta (partial content in streaming).
/// </summary>
internal record PortkeyDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<PortkeyToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Portkey tool call delta.
/// </summary>
internal record PortkeyToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public PortkeyFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Portkey function call delta.
/// </summary>
internal record PortkeyFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
