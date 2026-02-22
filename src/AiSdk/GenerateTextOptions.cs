using AiSdk.Abstractions;
using AiSdk.Models;

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

    /// <summary>
    /// Maximum number of steps in the agent loop.
    /// When set to a value greater than 1, the SDK will automatically execute tool calls
    /// and feed results back to the model until it stops calling tools or the limit is reached.
    /// Default is 1 (single step, no automatic tool execution).
    /// </summary>
    public int? MaxSteps { get; init; }

    /// <summary>
    /// Tool executors keyed by tool name. Required when MaxSteps > 1.
    /// These are used to automatically execute tool calls within the agent loop.
    /// </summary>
    public IReadOnlyDictionary<string, IToolExecutor>? ToolExecutors { get; init; }

    /// <summary>
    /// Optional callback invoked after each step completes.
    /// </summary>
    public Action<StepResult>? OnStepFinish { get; init; }
}
