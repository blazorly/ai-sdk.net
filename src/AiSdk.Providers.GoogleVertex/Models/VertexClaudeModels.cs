using System.Text.Json.Serialization;

namespace AiSdk.Providers.GoogleVertex.Models;

/// <summary>
/// Vertex AI Claude API request.
/// </summary>
internal record VertexClaudeRequest
{
    [JsonPropertyName("anthropic_version")]
    public string AnthropicVersion { get; init; } = "vertex-2023-10-16";

    [JsonPropertyName("messages")]
    public required List<VertexClaudeMessage> Messages { get; init; }

    [JsonPropertyName("max_tokens")]
    public required int MaxTokens { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("top_p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stop_sequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? StopSequences { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }

    [JsonPropertyName("system")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; init; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<VertexClaudeTool>? Tools { get; init; }

    [JsonPropertyName("tool_choice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ToolChoice { get; init; }
}

/// <summary>
/// Vertex AI Claude message in a request.
/// </summary>
internal record VertexClaudeMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required object Content { get; init; }
}

/// <summary>
/// Vertex AI Claude content block (text or tool use).
/// </summary>
internal record VertexClaudeContentBlock
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    [JsonPropertyName("input")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Input { get; init; }

    [JsonPropertyName("tool_use_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ToolUseId { get; init; }

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; init; }
}

/// <summary>
/// Vertex AI Claude tool definition.
/// </summary>
internal record VertexClaudeTool
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    [JsonPropertyName("input_schema")]
    public required object InputSchema { get; init; }
}

/// <summary>
/// Vertex AI Claude API response.
/// </summary>
internal record VertexClaudeResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required List<VertexClaudeContentBlock> Content { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; init; }

    [JsonPropertyName("usage")]
    public required VertexClaudeUsage Usage { get; init; }
}

/// <summary>
/// Vertex AI Claude usage statistics.
/// </summary>
internal record VertexClaudeUsage
{
    [JsonPropertyName("input_tokens")]
    public required int InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public required int OutputTokens { get; init; }
}

/// <summary>
/// Vertex AI Claude stream response chunk.
/// </summary>
internal record VertexClaudeStreamChunk
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("index")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Index { get; init; }

    [JsonPropertyName("delta")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexClaudeDelta? Delta { get; init; }

    [JsonPropertyName("content_block")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexClaudeContentBlock? ContentBlock { get; init; }

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexClaudeStreamMessage? Message { get; init; }

    [JsonPropertyName("usage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexClaudeUsage? Usage { get; init; }
}

/// <summary>
/// Vertex AI Claude delta in stream chunk.
/// </summary>
internal record VertexClaudeDelta
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

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

/// <summary>
/// Vertex AI Claude stream message.
/// </summary>
internal record VertexClaudeStreamMessage
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("usage")]
    public VertexClaudeUsage? Usage { get; init; }
}

/// <summary>
/// Vertex AI error response.
/// </summary>
internal record VertexErrorResponse
{
    [JsonPropertyName("error")]
    public required VertexError Error { get; init; }
}

/// <summary>
/// Vertex AI error details.
/// </summary>
internal record VertexError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("details")]
    public List<object>? Details { get; init; }
}
