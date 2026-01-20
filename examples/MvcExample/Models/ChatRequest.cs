namespace MvcExample.Models;

/// <summary>
/// Represents a chat message request from the client.
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// The user's message text.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Optional system prompt to guide the AI's behavior.
    /// </summary>
    public string? SystemPrompt { get; set; }

    /// <summary>
    /// Maximum number of tokens to generate.
    /// </summary>
    public int? MaxTokens { get; set; }

    /// <summary>
    /// Temperature for response generation (0.0 to 2.0).
    /// Lower values make output more focused and deterministic.
    /// </summary>
    public double? Temperature { get; set; }

    /// <summary>
    /// Whether to stream the response (Server-Sent Events).
    /// </summary>
    public bool Stream { get; set; }
}
