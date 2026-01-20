using AiSdk.Abstractions;

namespace AiSdk.Models;

/// <summary>
/// Options for streaming structured objects from a language model.
/// </summary>
public record StreamObjectOptions
{
    /// <summary>
    /// System message defining behavior and context.
    /// </summary>
    public string? System { get; init; }

    /// <summary>
    /// Simple text prompt (alternative to Messages).
    /// </summary>
    public string? Prompt { get; init; }

    /// <summary>
    /// List of messages (alternative to Prompt).
    /// </summary>
    public IReadOnlyList<Message>? Messages { get; init; }

    /// <summary>
    /// Maximum number of tokens to generate.
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// Temperature for sampling (typically 0.0 to 2.0).
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Top-p nucleus sampling parameter.
    /// </summary>
    public double? TopP { get; init; }

    /// <summary>
    /// Stop sequences where the model will stop generating.
    /// </summary>
    public IReadOnlyList<string>? StopSequences { get; init; }

    /// <summary>
    /// The mode for structured output generation ("auto", "json", or "tool").
    /// </summary>
    public string? Mode { get; init; }

    /// <summary>
    /// Name for the generated object (used in tool mode).
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Description for the schema (used in tool mode).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Callback invoked for each chunk of the streaming response.
    /// </summary>
    public Action<string>? OnChunk { get; init; }
}
