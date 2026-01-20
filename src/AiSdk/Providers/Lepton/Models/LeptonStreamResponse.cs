using System.Text.Json.Serialization;

namespace AiSdk.Providers.Lepton.Models;

/// <summary>
/// Lepton AI streaming chat completion response chunk.
/// </summary>
internal record LeptonStreamResponse
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
    public required List<LeptonStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public LeptonUsage? Usage { get; init; }
}

/// <summary>
/// Lepton AI streaming choice.
/// </summary>
internal record LeptonStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required LeptonDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Lepton AI delta (partial content in streaming).
/// </summary>
internal record LeptonDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<LeptonToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Lepton AI tool call delta.
/// </summary>
internal record LeptonToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public LeptonFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Lepton AI function call delta.
/// </summary>
internal record LeptonFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
