using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Utilities;

namespace AiSdk.Tools;

/// <summary>
/// Factory class for creating tool definitions with strongly-typed input and output.
/// </summary>
public static class Tool
{
    /// <summary>
    /// Creates a tool definition with strongly-typed input and output types.
    /// </summary>
    /// <typeparam name="TInput">The input parameter type for the tool.</typeparam>
    /// <typeparam name="TOutput">The output/return type for the tool.</typeparam>
    /// <param name="name">The name of the tool.</param>
    /// <param name="description">A description of what the tool does.</param>
    /// <param name="execute">The function to execute when the tool is called.</param>
    /// <returns>A tool definition with execution context.</returns>
    public static ToolWithExecution<TInput, TOutput> Create<TInput, TOutput>(
        string name,
        string description,
        Func<TInput, CancellationToken, Task<TOutput>> execute)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(execute);

        var schema = JsonSchemaGenerator.GenerateSchema<TInput>();

        var definition = new ToolDefinition(
            Name: name,
            Description: description,
            Parameters: schema
        );

        return new ToolWithExecution<TInput, TOutput>(definition, execute);
    }

    /// <summary>
    /// Creates a tool definition with strongly-typed input and output types (synchronous version).
    /// </summary>
    /// <typeparam name="TInput">The input parameter type for the tool.</typeparam>
    /// <typeparam name="TOutput">The output/return type for the tool.</typeparam>
    /// <param name="name">The name of the tool.</param>
    /// <param name="description">A description of what the tool does.</param>
    /// <param name="execute">The function to execute when the tool is called.</param>
    /// <returns>A tool definition with execution context.</returns>
    public static ToolWithExecution<TInput, TOutput> Create<TInput, TOutput>(
        string name,
        string description,
        Func<TInput, TOutput> execute)
    {
        ArgumentNullException.ThrowIfNull(execute);

        return Create<TInput, TOutput>(
            name,
            description,
            (input, ct) => Task.FromResult(execute(input))
        );
    }
}

/// <summary>
/// Represents a tool definition with execution capability.
/// </summary>
/// <typeparam name="TInput">The input parameter type for the tool.</typeparam>
/// <typeparam name="TOutput">The output/return type for the tool.</typeparam>
public class ToolWithExecution<TInput, TOutput> : IToolExecutor
{
    /// <summary>
    /// Gets the tool definition.
    /// </summary>
    public ToolDefinition Definition { get; }

    /// <summary>
    /// Gets the tool name.
    /// </summary>
    public string ToolName => Definition.Name;

    /// <summary>
    /// Gets the execution function.
    /// </summary>
    public Func<TInput, CancellationToken, Task<TOutput>> Execute { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolWithExecution{TInput, TOutput}"/> class.
    /// </summary>
    /// <param name="definition">The tool definition.</param>
    /// <param name="execute">The execution function.</param>
    public ToolWithExecution(
        ToolDefinition definition,
        Func<TInput, CancellationToken, Task<TOutput>> execute)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        Execute = execute ?? throw new ArgumentNullException(nameof(execute));
    }

    /// <summary>
    /// Executes the tool with the provided arguments and returns the typed result.
    /// </summary>
    /// <param name="argumentsJson">The JSON arguments to parse.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tool execution result.</returns>
    public async Task<TOutput> ExecuteAsync(
        JsonDocument argumentsJson,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(argumentsJson);

        var input = JsonSerializer.Deserialize<TInput>(argumentsJson.RootElement);
        if (input == null)
        {
            throw new InvalidOperationException($"Failed to deserialize arguments for tool {Definition.Name}");
        }

        return await Execute(input, cancellationToken);
    }

    /// <summary>
    /// Executes the tool with raw JSON string arguments.
    /// </summary>
    /// <param name="argumentsJson">The JSON string arguments.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tool execution result.</returns>
    public async Task<TOutput> ExecuteAsync(
        string argumentsJson,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(argumentsJson);

        using var doc = JsonDocument.Parse(argumentsJson);
        return await ExecuteAsync(doc, cancellationToken);
    }

    /// <summary>
    /// IToolExecutor implementation: executes the tool and returns the result as a serialized string.
    /// </summary>
    async Task<string> IToolExecutor.ExecuteAsync(
        JsonDocument arguments,
        CancellationToken cancellationToken)
    {
        var result = await ExecuteAsync(arguments, cancellationToken);
        return JsonSerializer.Serialize(result);
    }
}
