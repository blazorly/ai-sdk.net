namespace AiSdk.Abstractions;

/// <summary>
/// Represents a message in a conversation with an AI model.
/// </summary>
/// <param name="Role">The role of the message sender.</param>
/// <param name="Content">The content of the message.</param>
/// <param name="Name">Optional name of the message sender (e.g., function/tool name).</param>
/// <param name="Metadata">Optional metadata associated with the message.</param>
public record Message(
    MessageRole Role,
    string Content,
    string? Name = null,
    IReadOnlyDictionary<string, object>? Metadata = null);

/// <summary>
/// Defines the role of a message sender in a conversation.
/// </summary>
public enum MessageRole
{
    /// <summary>
    /// System message defining behavior and context.
    /// </summary>
    System,

    /// <summary>
    /// Message from the user.
    /// </summary>
    User,

    /// <summary>
    /// Message from the AI assistant.
    /// </summary>
    Assistant,

    /// <summary>
    /// Message from a tool/function execution.
    /// </summary>
    Tool
}
