using System.Text.Json.Serialization;

namespace AiSdk.Providers.Vercel.Models;

/// <summary>
/// Vercel AI Gateway streaming chat completion response chunk.
/// </summary>
internal record VercelStreamResponse
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
    public required List<VercelStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public VercelUsage? Usage { get; init; }
}

/// <summary>
/// Vercel streaming choice.
/// </summary>
internal record VercelStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required VercelDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Vercel delta (partial content in streaming).
/// </summary>
internal record VercelDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<VercelToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Vercel tool call delta.
/// </summary>
internal record VercelToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public VercelFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Vercel function call delta.
/// </summary>
internal record VercelFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
