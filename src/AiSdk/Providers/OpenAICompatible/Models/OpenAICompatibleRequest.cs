using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenAICompatible.Models;

/// <summary>
/// OpenAI-compatible chat completion request.
/// </summary>
internal record OpenAICompatibleRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("messages")]
    public required List<OpenAICompatibleMessage> Messages { get; init; }

    [JsonPropertyName("max_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("top_p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Stop { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OpenAICompatibleTool>? Tools { get; init; }

    [JsonPropertyName("tool_choice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ToolChoice { get; init; }

    /// <summary>
    /// Reasoning request config for reasoning-capable models via OpenAI-compatible
    /// aggregators (e.g. OpenRouter expects <c>{ "effort": "low|medium|high" }</c>).
    /// Omitted when null so non-reasoning requests are byte-for-byte unchanged.
    /// </summary>
    [JsonPropertyName("reasoning")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Reasoning { get; init; }
}

/// <summary>
/// OpenAI-compatible message in a request.
/// </summary>
internal record OpenAICompatibleMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; init; }

    /// <summary>
    /// Reasoning / "thinking" content surfaced by reasoning-capable models on
    /// non-streaming responses via OpenAI-compatible aggregators (e.g. OpenRouter
    /// carrying the <c>reasoning</c> field). Null on the request and on responses
    /// from providers/models that don't emit reasoning.
    /// </summary>
    [JsonPropertyName("reasoning")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Reasoning { get; init; }

    [JsonPropertyName("tool_calls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OpenAICompatibleToolCall>? ToolCalls { get; init; }

    [JsonPropertyName("tool_call_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ToolCallId { get; init; }
}

/// <summary>
/// OpenAI-compatible tool definition.
/// </summary>
internal record OpenAICompatibleTool
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    [JsonPropertyName("function")]
    public required OpenAICompatibleFunction Function { get; init; }
}

/// <summary>
/// OpenAI-compatible function definition.
/// </summary>
internal record OpenAICompatibleFunction
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    [JsonPropertyName("parameters")]
    public required object Parameters { get; init; }
}

/// <summary>
/// OpenAI-compatible tool call in a message.
/// </summary>
internal record OpenAICompatibleToolCall
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; } = "function";

    [JsonPropertyName("function")]
    public required OpenAICompatibleFunctionCall Function { get; init; }
}

/// <summary>
/// OpenAI-compatible function call.
/// </summary>
internal record OpenAICompatibleFunctionCall
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("arguments")]
    public required string Arguments { get; init; }
}
