namespace BlazorServerExample.Models;

/// <summary>
/// Represents a chat message in the conversation.
/// </summary>
public class ChatMessage
{
    /// <summary>
    /// Gets or sets the role of the message sender (user or assistant).
    /// </summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content of the message.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets whether this message is from the user.
    /// </summary>
    public bool IsUser => Role.Equals("user", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this message is from the assistant.
    /// </summary>
    public bool IsAssistant => Role.Equals("assistant", StringComparison.OrdinalIgnoreCase);
}
