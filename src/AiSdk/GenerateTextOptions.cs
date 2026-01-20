using AiSdk.Abstractions;

namespace AiSdk;

/// <summary>
/// Options for text generation.
/// </summary>
public record GenerateTextOptions
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
    /// Available tools that the model can call.
    /// </summary>
    public IReadOnlyList<ToolDefinition>? Tools { get; init; }

    /// <summary>
    /// Tool choice strategy ("auto", "none", "required", or specific tool name).
    /// </summary>
    public string? ToolChoice { get; init; }
}
