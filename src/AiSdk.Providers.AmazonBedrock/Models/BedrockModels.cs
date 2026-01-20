using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiSdk.Providers.AmazonBedrock.Models;

/// <summary>
/// Anthropic Claude request for Bedrock.
/// </summary>
internal record ClaudeRequest
{
    [JsonPropertyName("anthropic_version")]
    public string AnthropicVersion { get; init; } = "bedrock-2023-05-31";

    [JsonPropertyName("messages")]
    public required List<ClaudeMessage> Messages { get; init; }

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

    [JsonPropertyName("system")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? System { get; init; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ClaudeTool>? Tools { get; init; }

    [JsonPropertyName("tool_choice")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? ToolChoice { get; init; }
}

/// <summary>
/// Claude message.
/// </summary>
internal record ClaudeMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required object Content { get; init; }
}

/// <summary>
/// Claude content block.
/// </summary>
internal record ClaudeContentBlock
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
/// Claude tool definition.
/// </summary>
internal record ClaudeTool
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
/// Claude response from Bedrock.
/// </summary>
internal record ClaudeResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("content")]
    public required List<ClaudeContentBlock> Content { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; init; }

    [JsonPropertyName("usage")]
    public required ClaudeUsage Usage { get; init; }
}

/// <summary>
/// Claude usage statistics.
/// </summary>
internal record ClaudeUsage
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; init; }
}

/// <summary>
/// Amazon Titan request.
/// </summary>
internal record TitanRequest
{
    [JsonPropertyName("inputText")]
    public required string InputText { get; init; }

    [JsonPropertyName("textGenerationConfig")]
    public TitanTextGenerationConfig? TextGenerationConfig { get; init; }
}

/// <summary>
/// Titan text generation configuration.
/// </summary>
internal record TitanTextGenerationConfig
{
    [JsonPropertyName("maxTokenCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxTokenCount { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("topP")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stopSequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? StopSequences { get; init; }
}

/// <summary>
/// Titan response from Bedrock.
/// </summary>
internal record TitanResponse
{
    [JsonPropertyName("inputTextTokenCount")]
    public int InputTextTokenCount { get; init; }

    [JsonPropertyName("results")]
    public required List<TitanResult> Results { get; init; }
}

/// <summary>
/// Titan result item.
/// </summary>
internal record TitanResult
{
    [JsonPropertyName("tokenCount")]
    public int TokenCount { get; init; }

    [JsonPropertyName("outputText")]
    public required string OutputText { get; init; }

    [JsonPropertyName("completionReason")]
    public string? CompletionReason { get; init; }
}

/// <summary>
/// Meta Llama request.
/// </summary>
internal record LlamaRequest
{
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    [JsonPropertyName("max_gen_len")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxGenLen { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("top_p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }
}

/// <summary>
/// Llama response from Bedrock.
/// </summary>
internal record LlamaResponse
{
    [JsonPropertyName("generation")]
    public required string Generation { get; init; }

    [JsonPropertyName("prompt_token_count")]
    public int PromptTokenCount { get; init; }

    [JsonPropertyName("generation_token_count")]
    public int GenerationTokenCount { get; init; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }
}

/// <summary>
/// Claude streaming response chunk.
/// </summary>
internal record ClaudeStreamChunk
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("index")]
    public int? Index { get; init; }

    [JsonPropertyName("delta")]
    public ClaudeDelta? Delta { get; init; }

    [JsonPropertyName("content_block")]
    public ClaudeContentBlock? ContentBlock { get; init; }

    [JsonPropertyName("usage")]
    public ClaudeUsage? Usage { get; init; }
}

/// <summary>
/// Claude delta for streaming.
/// </summary>
internal record ClaudeDelta
{
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonPropertyName("partial_json")]
    public string? PartialJson { get; init; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }
}
