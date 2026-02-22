using System.Text.Json;

namespace AiSdk.Mcp;

/// <summary>
/// Represents a tool definition from an MCP server.
/// </summary>
public record McpToolDefinition
{
    /// <summary>
    /// The tool name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The tool description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// The JSON schema for the tool's input parameters.
    /// </summary>
    public JsonDocument? InputSchema { get; init; }
}
