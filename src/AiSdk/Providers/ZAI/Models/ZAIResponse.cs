using System.Text.Json.Serialization;

namespace AiSdk.Providers.ZAI.Models;

/// <summary>
/// Z.AI chat completion response.
/// </summary>
internal record ZAIResponse
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
    public required List<ZAIChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public ZAIUsage? Usage { get; init; }
}

/// <summary>
/// Z.AI choice in a response.
/// </summary>
internal record ZAIChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public required ZAIResponseMessage Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Z.AI response message.
/// </summary>
internal record ZAIResponseMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("reasoning_content")]
    public string? ReasoningContent { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<ZAIToolCall>? ToolCalls { get; init; }
}

/// <summary>
/// Z.AI token usage information.
/// </summary>
internal record ZAIUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}
