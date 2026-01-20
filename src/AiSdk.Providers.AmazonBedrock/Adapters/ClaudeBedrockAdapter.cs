using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.AmazonBedrock.Models;

namespace AiSdk.Providers.AmazonBedrock.Adapters;

/// <summary>
/// Adapter for Anthropic Claude models on Bedrock.
/// </summary>
internal class ClaudeBedrockAdapter : IBedrockModelAdapter
{
    // Track tool calls being built across chunks
    private readonly Dictionary<int, ToolCallBuilder> _toolCallsInProgress = new();
    private ClaudeUsage? _finalUsage;
    private string? _finishReason;

    public bool SupportsStreaming => true;

    public string BuildRequest(LanguageModelCallOptions options)
    {
        // Extract system message if present
        string? systemMessage = null;
        var messages = new List<ClaudeMessage>();

        foreach (var msg in options.Messages)
        {
            if (msg.Role == MessageRole.System)
            {
                systemMessage = msg.Content;
            }
            else if (msg.Role == MessageRole.User || msg.Role == MessageRole.Assistant)
            {
                messages.Add(new ClaudeMessage
                {
                    Role = MapRole(msg.Role),
                    Content = msg.Content ?? string.Empty
                });
            }
            else if (msg.Role == MessageRole.Tool)
            {
                // Claude uses tool_result content blocks
                messages.Add(new ClaudeMessage
                {
                    Role = "user",
                    Content = new List<ClaudeContentBlock>
                    {
                        new ClaudeContentBlock
                        {
                            Type = "tool_result",
                            ToolUseId = msg.Name,
                            Content = msg.Content
                        }
                    }
                });
            }
        }

        var request = new ClaudeRequest
        {
            Messages = messages,
            MaxTokens = options.MaxTokens ?? 4096,
            Temperature = options.Temperature,
            TopP = options.TopP,
            StopSequences = options.StopSequences,
            System = systemMessage
        };

        if (options.Tools?.Count > 0)
        {
            var tools = options.Tools.Select(t => new ClaudeTool
            {
                Name = t.Name,
                Description = t.Description,
                InputSchema = t.Parameters != null
                    ? JsonDocument.Parse(t.Parameters.RootElement.GetRawText()).RootElement
                    : new { }
            }).ToList();

            request = request with
            {
                Tools = tools,
                ToolChoice = options.ToolChoice != null
                    ? new { type = "tool", name = options.ToolChoice }
                    : null
            };
        }

        return JsonSerializer.Serialize(request);
    }

    public LanguageModelGenerateResult ParseResponse(string responseJson)
    {
        var response = JsonSerializer.Deserialize<ClaudeResponse>(responseJson)
            ?? throw new InvalidOperationException("Failed to deserialize Claude response");

        string? text = null;
        List<ToolCall>? toolCalls = null;

        foreach (var block in response.Content)
        {
            if (block.Type == "text" && block.Text != null)
            {
                text = (text ?? "") + block.Text;
            }
            else if (block.Type == "tool_use")
            {
                toolCalls ??= new List<ToolCall>();
                toolCalls.Add(new ToolCall(
                    ToolCallId: block.Id!,
                    ToolName: block.Name!,
                    Arguments: JsonDocument.Parse(JsonSerializer.Serialize(block.Input))
                ));
            }
        }

        return new LanguageModelGenerateResult
        {
            Text = text,
            FinishReason = MapFinishReason(response.StopReason),
            Usage = MapUsage(response.Usage),
            ToolCalls = toolCalls
        };
    }

    public LanguageModelStreamChunk? ParseStreamChunk(string chunkJson)
    {
        var streamEvent = JsonSerializer.Deserialize<ClaudeStreamChunk>(chunkJson);
        if (streamEvent == null)
            return null;

        switch (streamEvent.Type)
        {
            case "content_block_start":
                if (streamEvent.ContentBlock?.Type == "tool_use" && streamEvent.Index.HasValue)
                {
                    _toolCallsInProgress[streamEvent.Index.Value] = new ToolCallBuilder
                    {
                        Id = streamEvent.ContentBlock.Id!,
                        Name = streamEvent.ContentBlock.Name!,
                        JsonArguments = new StringBuilder()
                    };
                }
                return null;

            case "content_block_delta":
                if (streamEvent.Delta?.Type == "text_delta" && !string.IsNullOrEmpty(streamEvent.Delta.Text))
                {
                    return new LanguageModelStreamChunk
                    {
                        Type = ChunkType.TextDelta,
                        Delta = streamEvent.Delta.Text
                    };
                }
                else if (streamEvent.Delta?.Type == "input_json_delta" &&
                         streamEvent.Index.HasValue &&
                         _toolCallsInProgress.TryGetValue(streamEvent.Index.Value, out var builder))
                {
                    builder.JsonArguments.Append(streamEvent.Delta.PartialJson);
                }
                return null;

            case "content_block_stop":
                // Tool call complete - emit it
                if (streamEvent.Index.HasValue &&
                    _toolCallsInProgress.TryGetValue(streamEvent.Index.Value, out var completedTool))
                {
                    var toolCall = new ToolCall(
                        ToolCallId: completedTool.Id,
                        ToolName: completedTool.Name,
                        Arguments: JsonDocument.Parse(completedTool.JsonArguments.ToString())
                    );

                    _toolCallsInProgress.Remove(streamEvent.Index.Value);

                    return new LanguageModelStreamChunk
                    {
                        Type = ChunkType.ToolCallDelta,
                        ToolCall = toolCall
                    };
                }
                return null;

            case "message_delta":
                if (streamEvent.Delta?.StopReason != null)
                {
                    _finishReason = streamEvent.Delta.StopReason;
                }
                if (streamEvent.Usage != null)
                {
                    _finalUsage = streamEvent.Usage;
                }
                return null;

            case "message_stop":
                // Send final chunk with finish reason and usage
                return new LanguageModelStreamChunk
                {
                    Type = ChunkType.Finish,
                    FinishReason = MapFinishReason(_finishReason),
                    Usage = _finalUsage != null ? MapUsage(_finalUsage) : null
                };

            default:
                return null;
        }
    }

    private static string MapRole(MessageRole role) => role switch
    {
        MessageRole.User => "user",
        MessageRole.Assistant => "assistant",
        _ => throw new ArgumentException($"Unsupported message role: {role}")
    };

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "end_turn" => FinishReason.Stop,
        "max_tokens" => FinishReason.Length,
        "tool_use" => FinishReason.ToolCalls,
        "stop_sequence" => FinishReason.Stop,
        _ => FinishReason.Other
    };

    private static Usage MapUsage(ClaudeUsage usage) => new Usage(
        InputTokens: usage.InputTokens,
        OutputTokens: usage.OutputTokens,
        TotalTokens: usage.InputTokens + usage.OutputTokens
    );

    private class ToolCallBuilder
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required StringBuilder JsonArguments { get; init; }
    }
}
