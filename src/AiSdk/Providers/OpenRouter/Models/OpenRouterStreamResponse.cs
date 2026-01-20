using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenRouter.Models;

/// <summary>
/// OpenRouter streaming chat completion response chunk.
/// </summary>
internal record OpenRouterStreamResponse
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
    public required List<OpenRouterStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public OpenRouterUsage? Usage { get; init; }
}

/// <summary>
/// OpenRouter streaming choice.
/// </summary>
internal record OpenRouterStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required OpenRouterDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// OpenRouter delta (partial content in streaming).
/// </summary>
internal record OpenRouterDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<OpenRouterToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// OpenRouter tool call delta.
/// </summary>
internal record OpenRouterToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public OpenRouterFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// OpenRouter function call delta.
/// </summary>
internal record OpenRouterFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
