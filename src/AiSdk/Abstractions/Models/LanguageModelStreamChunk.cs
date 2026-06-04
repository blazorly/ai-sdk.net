namespace AiSdk.Abstractions;

/// <summary>
/// A chunk of data from a streaming language model response.
/// </summary>
public record LanguageModelStreamChunk
{
    /// <summary>
    /// The type of this chunk.
    /// </summary>
    public required ChunkType Type { get; init; }

    /// <summary>
    /// Optional stable identifier for the chunk (e.g. the response id from the
    /// upstream provider, or a synthetic id minted for a reasoning block).
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Partial text content (for <see cref="ChunkType.TextDelta"/> chunks).
    /// </summary>
    public string? Delta { get; init; }

    /// <summary>
    /// Partial reasoning / "thinking" content (for
    /// <see cref="ChunkType.ReasoningDelta"/> chunks). This is the model's
    /// chain-of-thought text emitted before the final answer — surfaced
    /// separately from <see cref="Delta"/> so the UI can render it as a
    /// distinct, collapsible block instead of mixing it into the response.
    ///
    /// Populated by providers that surface reasoning (OpenRouter, ZAI,
    /// DeepSeek, Anthropic extended thinking, etc.). Providers that don't
    /// support reasoning will simply leave this null and continue to emit
    /// answer deltas as usual.
    /// </summary>
    public string? ReasoningContent { get; init; }

    /// <summary>
    /// Tool call information (for <see cref="ChunkType.ToolCallDelta"/> chunks).
    /// </summary>
    public ToolCall? ToolCall { get; init; }

    /// <summary>
    /// Finish reason (for completion chunks).
    /// </summary>
    public FinishReason? FinishReason { get; init; }

    /// <summary>
    /// Usage statistics (typically in the final chunk).
    /// </summary>
    public Usage? Usage { get; init; }
}

/// <summary>
/// The type of content in a stream chunk.
/// </summary>
public enum ChunkType
{
    /// <summary>
    /// Partial text content.
    /// </summary>
    TextDelta,

    /// <summary>
    /// Partial reasoning / "thinking" content. Providers that support
    /// chain-of-thought reasoning (OpenRouter, ZAI, DeepSeek, Anthropic
    /// extended thinking, etc.) emit the model's internal monologue as
    /// one or more <see cref="ReasoningDelta"/> chunks, typically before
    /// the corresponding <see cref="TextDelta"/> chunks. Consumers can
    /// choose to surface this to the user, fold it into a "thinking…"
    /// indicator, or ignore it entirely.
    /// </summary>
    ReasoningDelta,

    /// <summary>
    /// Tool/function call delta.
    /// </summary>
    ToolCallDelta,

    /// <summary>
    /// Stream completion marker.
    /// </summary>
    Finish,

    /// <summary>
    /// Error occurred.
    /// </summary>
    Error
}
