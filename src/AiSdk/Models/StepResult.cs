using AiSdk.Abstractions;

namespace AiSdk.Models;

/// <summary>
/// Result from a single step in a multi-step agent loop.
/// </summary>
public record StepResult
{
    /// <summary>
    /// The zero-based step number.
    /// </summary>
    public required int StepNumber { get; init; }

    /// <summary>
    /// The generated text from this step (may be null if the model only made tool calls).
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// The finish reason for this step.
    /// </summary>
    public required FinishReason FinishReason { get; init; }

    /// <summary>
    /// Token usage for this step.
    /// </summary>
    public required Usage Usage { get; init; }

    /// <summary>
    /// Tool calls made in this step (if any).
    /// </summary>
    public IReadOnlyList<ToolCall>? ToolCalls { get; init; }

    /// <summary>
    /// Tool results from executing tool calls in this step (if any).
    /// </summary>
    public IReadOnlyList<ToolResult>? ToolResults { get; init; }
}

/// <summary>
/// Result from executing a single tool call.
/// </summary>
public record ToolResult
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
