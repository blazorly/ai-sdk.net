using System.Text.Json;
using AiSdk.Abstractions;

namespace AiSdk.Mcp;

/// <summary>
/// Converts MCP tools into IToolExecutor instances that can be used in the agent loop.
/// This bridges MCP servers with the AI SDK's tool execution system.
/// </summary>
public class McpToolProvider
{
    private readonly IMcpClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="McpToolProvider"/> class.
    /// </summary>
    /// <param name="client">The MCP client to use for tool discovery and execution.</param>
    public McpToolProvider(IMcpClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        _client = client;
    }

    /// <summary>
    /// Gets all tools from the MCP server as IToolExecutor instances.
    /// These can be passed directly to GenerateTextOptions.ToolExecutors.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary of tool name to IToolExecutor mappings.</returns>
    public async Task<IReadOnlyDictionary<string, IToolExecutor>> GetToolExecutorsAsync(
        CancellationToken cancellationToken = default)
    {
        var tools = await _client.ListToolsAsync(cancellationToken);
        var executors = new Dictionary<string, IToolExecutor>();

        foreach (var tool in tools)
        {
            executors[tool.Name] = new McpToolExecutor(_client, tool);
        }

        return executors;
    }

    /// <summary>
    /// Gets all tool definitions from the MCP server as ToolDefinition instances.
    /// These can be passed directly to GenerateTextOptions.Tools.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of tool definitions.</returns>
    public async Task<IReadOnlyList<ToolDefinition>> GetToolDefinitionsAsync(
        CancellationToken cancellationToken = default)
    {
        var tools = await _client.ListToolsAsync(cancellationToken);

        return tools.Select(t => new ToolDefinition(
            Name: t.Name,
            Description: t.Description ?? string.Empty,
            Parameters: t.InputSchema
        )).ToList().AsReadOnly();
    }
}

/// <summary>
/// IToolExecutor implementation that delegates to an MCP client.
/// </summary>
internal class McpToolExecutor : IToolExecutor
{
    private readonly IMcpClient _client;
    private readonly McpToolDefinition _toolDefinition;

    public McpToolExecutor(IMcpClient client, McpToolDefinition toolDefinition)
    {
        _client = client;
        _toolDefinition = toolDefinition;
    }

    /// <inheritdoc/>
    public string ToolName => _toolDefinition.Name;

    /// <inheritdoc/>
    public ToolDefinition Definition => new(
        Name: _toolDefinition.Name,
        Description: _toolDefinition.Description ?? string.Empty,
        Parameters: _toolDefinition.InputSchema
    );

    /// <inheritdoc/>
    public async Task<string> ExecuteAsync(
        JsonDocument arguments,
        CancellationToken cancellationToken = default)
    {
        var argsJson = arguments.RootElement.GetRawText();
        return await _client.CallToolAsync(ToolName, argsJson, cancellationToken);
    }
}
