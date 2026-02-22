namespace AiSdk.Abstractions;

/// <summary>
/// Represents a message in a conversation with an AI model.
/// </summary>
/// <param name="Role">The role of the message sender.</param>
/// <param name="Content">The text content of the message.</param>
/// <param name="Name">Optional name of the message sender (e.g., function/tool name).</param>
/// <param name="Metadata">Optional metadata associated with the message.</param>
public record Message(
    MessageRole Role,
    string Content,
    string? Name = null,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Multi-modal content parts (text, images, files, audio).
    /// When set, providers that support multi-modal input will use these parts
    /// instead of the plain text Content property.
    /// </summary>
    public IReadOnlyList<ContentPart>? Parts { get; init; }
}

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
