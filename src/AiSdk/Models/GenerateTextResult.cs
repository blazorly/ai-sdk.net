using AiSdk.Abstractions;

namespace AiSdk.Models;

/// <summary>
/// Result from text generation, supporting multi-step agent execution.
/// When MaxSteps is used, this wraps the results from all steps.
/// </summary>
public record GenerateTextResult
{
    /// <summary>
    /// The final generated text from the last step.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// The finish reason from the last step.
    /// </summary>
    public required FinishReason FinishReason { get; init; }

    /// <summary>
    /// Aggregated token usage across all steps.
    /// </summary>
    public required Usage Usage { get; init; }

    /// <summary>
    /// Tool calls from the last step (if any).
    /// </summary>
    public IReadOnlyList<ToolCall>? ToolCalls { get; init; }

    /// <summary>
    /// Results from each step in the agent loop.
    /// Contains a single step when MaxSteps is not used.
    /// </summary>
    public required IReadOnlyList<StepResult> Steps { get; init; }

    /// <summary>
    /// All messages in the conversation, including tool call/result messages
    /// appended during multi-step execution.
    /// </summary>
    public required IReadOnlyList<Message> Messages { get; init; }

    /// <summary>
    /// Provider-specific metadata from the last step.
    /// </summary>
    public object? ProviderMetadata { get; init; }

    /// <summary>
    /// Warnings from the provider.
    /// </summary>
    public IReadOnlyList<string>? Warnings { get; init; }
}
