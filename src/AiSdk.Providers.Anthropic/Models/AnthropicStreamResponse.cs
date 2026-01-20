using System.Text.Json.Serialization;

namespace AiSdk.Providers.Anthropic.Models;

/// <summary>
/// Anthropic streaming response event.
/// </summary>
internal record AnthropicStreamResponse
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AnthropicResponse? Message { get; init; }

    [JsonPropertyName("index")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Index { get; init; }

    [JsonPropertyName("content_block")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AnthropicContentBlock? ContentBlock { get; init; }

    [JsonPropertyName("delta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AnthropicDelta? Delta { get; init; }

    [JsonPropertyName("usage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AnthropicUsage? Usage { get; init; }
}

/// <summary>
/// Anthropic delta for streaming content.
/// </summary>
internal record AnthropicDelta
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    [JsonPropertyName("stop_reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StopSequence { get; init; }

    [JsonPropertyName("partial_json")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? PartialJson { get; init; }
}
