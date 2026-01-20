using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cohere.Models;

/// <summary>
/// Cohere chat API request.
/// </summary>
internal record CohereRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("chat_history")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereMessage>? ChatHistory { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("max_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? P { get; init; }

    [JsonPropertyName("stop_sequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? StopSequences { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereTool>? Tools { get; init; }

    [JsonPropertyName("tool_results")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereToolResult>? ToolResults { get; init; }
}

/// <summary>
/// Cohere message in chat history.
/// </summary>
internal record CohereMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("tool_calls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<CohereToolCall>? ToolCalls { get; init; }
}

/// <summary>
/// Cohere tool definition.
/// </summary>
internal record CohereTool
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    [JsonPropertyName("parameter_definitions")]
    public required object ParameterDefinitions { get; init; }
}

/// <summary>
/// Cohere tool call in a message.
/// </summary>
internal record CohereToolCall
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("parameters")]
    public required object Parameters { get; init; }
}

/// <summary>
/// Cohere tool result.
/// </summary>
internal record CohereToolResult
{
    [JsonPropertyName("call")]
    public required CohereToolCall Call { get; init; }

    [JsonPropertyName("outputs")]
    public required List<object> Outputs { get; init; }
}
