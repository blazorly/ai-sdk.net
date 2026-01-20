using System.Text.Json.Serialization;

namespace AiSdk.Providers.LlamaFile.Models;

/// <summary>
/// LlamaFile streaming chat completion response chunk.
/// </summary>
internal record LlamaFileStreamResponse
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
    public required List<LlamaFileStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public LlamaFileUsage? Usage { get; init; }
}

/// <summary>
/// LlamaFile streaming choice.
/// </summary>
internal record LlamaFileStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required LlamaFileDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// LlamaFile delta (partial content in streaming).
/// </summary>
internal record LlamaFileDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<LlamaFileToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// LlamaFile tool call delta.
/// </summary>
internal record LlamaFileToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public LlamaFileFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// LlamaFile function call delta.
/// </summary>
internal record LlamaFileFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
