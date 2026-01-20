using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cohere.Models;

/// <summary>
/// Cohere streaming response event.
/// </summary>
internal record CohereStreamResponse
{
    [JsonPropertyName("event_type")]
    public required string EventType { get; init; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    [JsonPropertyName("finish_reason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? FinishReason { get; init; }

    [JsonPropertyName("generation_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? GenerationId { get; init; }

    [JsonPropertyName("tool_calls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereToolCallResponse>? ToolCalls { get; init; }

    [JsonPropertyName("response")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CohereStreamEndResponse? Response { get; init; }
}

/// <summary>
/// Cohere stream end response with metadata.
/// </summary>
internal record CohereStreamEndResponse
{
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("generation_id")]
    public string? GenerationId { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }

    [JsonPropertyName("meta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CohereMeta? Meta { get; init; }

    [JsonPropertyName("citations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereCitation>? Citations { get; init; }
}
