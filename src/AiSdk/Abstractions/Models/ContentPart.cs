namespace AiSdk.Abstractions;

/// <summary>
/// Base type for multi-modal content parts within a message.
/// </summary>
public abstract record ContentPart;

/// <summary>
/// A text content part.
/// </summary>
/// <param name="Text">The text content.</param>
public record TextPart(string Text) : ContentPart;

/// <summary>
/// An image content part, either as a URL or inline data.
/// </summary>
public record ImagePart : ContentPart
{
    /// <summary>
    /// URL of the image (e.g., https://... or data:image/...).
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Raw image data as bytes.
    /// </summary>
    public byte[]? Data { get; init; }

    /// <summary>
    /// MIME type of the image (e.g., "image/png", "image/jpeg").
    /// </summary>
    public string? MimeType { get; init; }
}

/// <summary>
/// A file content part (e.g., PDFs, documents).
/// </summary>
public record FilePart : ContentPart
{
    /// <summary>
    /// URL of the file.
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// MIME type of the file (e.g., "application/pdf").
    /// </summary>
    public required string MimeType { get; init; }

    /// <summary>
    /// Raw file data as bytes (alternative to URL).
    /// </summary>
    public byte[]? Data { get; init; }
}

/// <summary>
/// An audio content part.
/// </summary>
public record AudioPart : ContentPart
{
    /// <summary>
    /// URL of the audio file.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Raw audio data as bytes.
    /// </summary>
    public byte[]? Data { get; init; }

    /// <summary>
    /// MIME type of the audio (e.g., "audio/mp3", "audio/wav").
    /// </summary>
    public string? MimeType { get; init; }
}

/// <summary>
/// A tool result content part, used in tool response messages.
/// </summary>
public record ToolResultPart : ContentPart
{
    /// <summary>
    /// The tool call ID this result corresponds to.
    /// </summary>
    public required string ToolCallId { get; init; }

    /// <summary>
    /// The tool name.
    /// </summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// The serialized result from the tool execution.
    /// </summary>
    public required string Result { get; init; }
}
