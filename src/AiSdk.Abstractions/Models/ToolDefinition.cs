using System.Text.Json;

namespace AiSdk.Abstractions;

/// <summary>
/// Represents a tool/function that can be called by the AI model.
/// </summary>
/// <param name="Name">The name of the tool.</param>
/// <param name="Description">A description of what the tool does.</param>
/// <param name="Parameters">JSON schema defining the tool's parameters.</param>
public record ToolDefinition(
    string Name,
    string? Description = null,
    JsonDocument? Parameters = null);

/// <summary>
/// Represents a tool call made by the AI model.
/// </summary>
/// <param name="ToolCallId">Unique identifier for this tool call.</param>
/// <param name="ToolName">Name of the tool being called.</param>
/// <param name="Arguments">Arguments for the tool call as JSON.</param>
public record ToolCall(
    string ToolCallId,
    string ToolName,
    JsonDocument Arguments);
