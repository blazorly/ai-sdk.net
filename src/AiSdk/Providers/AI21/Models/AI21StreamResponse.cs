using System.Text.Json.Serialization;

namespace AiSdk.Providers.AI21.Models;

/// <summary>
/// AI21 Labs streaming chat completion response chunk.
/// </summary>
internal record AI21StreamResponse
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
    public required List<AI21StreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public AI21Usage? Usage { get; init; }
}

/// <summary>
/// AI21 Labs streaming choice.
/// </summary>
internal record AI21StreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required AI21Delta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// AI21 Labs delta (partial content in streaming).
/// </summary>
internal record AI21Delta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<AI21ToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// AI21 Labs tool call delta.
/// </summary>
internal record AI21ToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public AI21FunctionCallDelta? Function { get; init; }
}

/// <summary>
/// AI21 Labs function call delta.
/// </summary>
internal record AI21FunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
