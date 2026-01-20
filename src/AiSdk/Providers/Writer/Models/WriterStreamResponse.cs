using System.Text.Json.Serialization;

namespace AiSdk.Providers.Writer.Models;

/// <summary>
/// Writer streaming chat completion response chunk.
/// </summary>
internal record WriterStreamResponse
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
    public required List<WriterStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public WriterUsage? Usage { get; init; }
}

/// <summary>
/// Writer streaming choice.
/// </summary>
internal record WriterStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required WriterDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Writer delta (partial content in streaming).
/// </summary>
internal record WriterDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<WriterToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Writer tool call delta.
/// </summary>
internal record WriterToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public WriterFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Writer function call delta.
/// </summary>
internal record WriterFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
