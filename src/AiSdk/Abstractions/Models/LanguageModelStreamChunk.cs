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
    /// Partial text content (for TextDelta chunks).
    /// </summary>
    public string? Delta { get; init; }

    /// <summary>
    /// Tool call information (for ToolCallDelta chunks).
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
