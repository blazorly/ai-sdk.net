using System.Text.Json;

namespace AiSdk.Abstractions;

/// <summary>
/// Non-generic interface for executing tool calls within an agent loop.
/// Implemented by ToolWithExecution&lt;TInput, TOutput&gt; to allow the agent loop
/// to execute tools without knowing their generic type parameters.
/// </summary>
public interface IToolExecutor
{
    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    string ToolName { get; }

    /// <summary>
    /// Gets the tool definition (name, description, parameters schema).
    /// </summary>
    ToolDefinition Definition { get; }

    /// <summary>
    /// Executes the tool with the provided JSON arguments and returns the result as a string.
    /// </summary>
    /// <param name="arguments">The JSON arguments for the tool call.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tool result serialized as a string.</returns>
    Task<string> ExecuteAsync(JsonDocument arguments, CancellationToken cancellationToken = default);
}
