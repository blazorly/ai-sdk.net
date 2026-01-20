using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cohere.Models;

/// <summary>
/// Cohere chat API response.
/// </summary>
internal record CohereResponse
{
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    [JsonPropertyName("generation_id")]
    public string? GenerationId { get; init; }

    [JsonPropertyName("chat_history")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereMessage>? ChatHistory { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }

    [JsonPropertyName("meta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CohereMeta? Meta { get; init; }

    [JsonPropertyName("tool_calls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereToolCallResponse>? ToolCalls { get; init; }

    [JsonPropertyName("citations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereCitation>? Citations { get; init; }
}

/// <summary>
/// Cohere tool call in response.
/// </summary>
internal record CohereToolCallResponse
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("parameters")]
    public required object Parameters { get; init; }

    [JsonPropertyName("generation_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? GenerationId { get; init; }
}

/// <summary>
/// Cohere response metadata.
/// </summary>
internal record CohereMeta
{
    [JsonPropertyName("api_version")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CohereApiVersion? ApiVersion { get; init; }

    [JsonPropertyName("billed_units")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CohereBilledUnits? BilledUnits { get; init; }

    [JsonPropertyName("tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CohereTokens? Tokens { get; init; }
}

/// <summary>
/// Cohere API version info.
/// </summary>
internal record CohereApiVersion
{
    [JsonPropertyName("version")]
    public string? Version { get; init; }
}

/// <summary>
/// Cohere billed units.
/// </summary>
internal record CohereBilledUnits
{
    [JsonPropertyName("input_tokens")]
    public int? InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public int? OutputTokens { get; init; }
}

/// <summary>
/// Cohere token usage.
/// </summary>
internal record CohereTokens
{
    [JsonPropertyName("input_tokens")]
    public int? InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public int? OutputTokens { get; init; }
}

/// <summary>
/// Cohere citation.
/// </summary>
internal record CohereCitation
{
    [JsonPropertyName("start")]
    public int? Start { get; init; }

    [JsonPropertyName("end")]
    public int? End { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("document_ids")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? DocumentIds { get; init; }
}

/// <summary>
/// Cohere error response.
/// </summary>
internal record CohereErrorResponse
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
