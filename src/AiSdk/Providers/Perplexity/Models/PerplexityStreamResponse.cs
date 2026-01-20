using System.Text.Json.Serialization;

namespace AiSdk.Providers.Perplexity.Models;

/// <summary>
/// Perplexity streaming chat completion response chunk.
/// </summary>
internal record PerplexityStreamResponse
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
    public required List<PerplexityStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public PerplexityUsage? Usage { get; init; }
}

/// <summary>
/// Perplexity streaming choice.
/// </summary>
internal record PerplexityStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required PerplexityDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Perplexity delta (partial content in streaming).
/// </summary>
internal record PerplexityDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<PerplexityToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Perplexity tool call delta.
/// </summary>
internal record PerplexityToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public PerplexityFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Perplexity function call delta.
/// </summary>
internal record PerplexityFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
