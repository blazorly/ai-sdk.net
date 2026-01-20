using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenAI.Models;

/// <summary>
/// OpenAI streaming chat completion response chunk.
/// </summary>
internal record OpenAIStreamResponse
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
    public required List<OpenAIStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public OpenAIUsage? Usage { get; init; }
}

/// <summary>
/// OpenAI streaming choice.
/// </summary>
internal record OpenAIStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required OpenAIDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// OpenAI delta (partial content in streaming).
/// </summary>
internal record OpenAIDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<OpenAIToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// OpenAI tool call delta.
/// </summary>
internal record OpenAIToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public OpenAIFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// OpenAI function call delta.
/// </summary>
internal record OpenAIFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
