namespace MvcExample.Models;

/// <summary>
/// Represents a chat response from the AI model.
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// The generated response text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The reason the generation finished.
    /// </summary>
    public string FinishReason { get; set; } = string.Empty;

    /// <summary>
    /// Token usage statistics.
    /// </summary>
    public UsageInfo? Usage { get; set; }

    /// <summary>
    /// Indicates if an error occurred.
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// Error message if IsError is true.
    /// </summary>
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Token usage information.
/// </summary>
public class UsageInfo
{
    /// <summary>
    /// Number of input tokens.
    /// </summary>
    public int InputTokens { get; set; }

    /// <summary>
    /// Number of output tokens.
    /// </summary>
    public int OutputTokens { get; set; }

    /// <summary>
    /// Total tokens used.
    /// </summary>
    public int TotalTokens { get; set; }
}
