namespace AiSdk.Mcp;

/// <summary>
/// Client interface for connecting to MCP (Model Context Protocol) servers.
/// MCP servers provide tools, resources, and prompts that can be used by AI models.
/// </summary>
public interface IMcpClient : IAsyncDisposable
{
    /// <summary>
    /// Connects to the MCP server.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists available tools from the MCP server.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of available tool definitions.</returns>
    Task<IReadOnlyList<McpToolDefinition>> ListToolsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls a tool on the MCP server.
    /// </summary>
    /// <param name="toolName">The name of the tool to call.</param>
    /// <param name="arguments">The JSON arguments for the tool call.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The tool result as a string.</returns>
    Task<string> CallToolAsync(
        string toolName,
        string arguments,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets whether the client is connected.
    /// </summary>
    bool IsConnected { get; }
}
