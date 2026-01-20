using System.Text.Json.Serialization;

namespace AiSdk.Providers.Friendli.Models;

/// <summary>
/// Friendli streaming chat completion response chunk.
/// </summary>
internal record FriendliStreamResponse
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
    public required List<FriendliStreamChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public FriendliUsage? Usage { get; init; }
}

/// <summary>
/// Friendli streaming choice.
/// </summary>
internal record FriendliStreamChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("delta")]
    public required FriendliDelta Delta { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Friendli delta (partial content in streaming).
/// </summary>
internal record FriendliDelta
{
    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public string? Content { get; init; }

    [JsonPropertyName("tool_calls")]
    public List<FriendliToolCallDelta>? ToolCalls { get; init; }
}

/// <summary>
/// Friendli tool call delta.
/// </summary>
internal record FriendliToolCallDelta
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("function")]
    public FriendliFunctionCallDelta? Function { get; init; }
}

/// <summary>
/// Friendli function call delta.
/// </summary>
internal record FriendliFunctionCallDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("arguments")]
    public string? Arguments { get; init; }
}
