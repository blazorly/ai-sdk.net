using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.AmazonBedrock.Models;

namespace AiSdk.Providers.AmazonBedrock.Adapters;

/// <summary>
/// Adapter for Meta Llama models on Bedrock.
/// Llama models use a prompt-based format with Llama-specific formatting.
/// </summary>
internal class LlamaBedrockAdapter : IBedrockModelAdapter
{
    public bool SupportsStreaming => false;

    public string BuildRequest(LanguageModelCallOptions options)
    {
        // Llama 3.1 uses the following format:
        // <|begin_of_text|><|start_header_id|>system<|end_header_id|>
        // {system_message}<|eot_id|><|start_header_id|>user<|end_header_id|>
        // {user_message}<|eot_id|><|start_header_id|>assistant<|end_header_id|>

        var promptBuilder = new StringBuilder();
        promptBuilder.Append("<|begin_of_text|>");

        foreach (var msg in options.Messages)
        {
            string roleTag = msg.Role switch
            {
                MessageRole.System => "system",
                MessageRole.User => "user",
                MessageRole.Assistant => "assistant",
                _ => "user"
            };

            promptBuilder.Append($"<|start_header_id|>{roleTag}<|end_header_id|>\n\n");
            promptBuilder.Append(msg.Content);
            promptBuilder.Append("<|eot_id|>");
        }

        // Add assistant header to prompt the model to respond
        if (options.Messages.Count > 0 && options.Messages[^1].Role == MessageRole.User)
        {
            promptBuilder.Append("<|start_header_id|>assistant<|end_header_id|>\n\n");
        }

        var request = new LlamaRequest
        {
            Prompt = promptBuilder.ToString(),
            MaxGenLen = options.MaxTokens,
            Temperature = options.Temperature,
            TopP = options.TopP
        };

        return JsonSerializer.Serialize(request);
    }

    public LanguageModelGenerateResult ParseResponse(string responseJson)
    {
        var response = JsonSerializer.Deserialize<LlamaResponse>(responseJson)
            ?? throw new InvalidOperationException("Failed to deserialize Llama response");

        return new LanguageModelGenerateResult
        {
            Text = response.Generation.Trim(),
            FinishReason = MapFinishReason(response.StopReason),
            Usage = new Usage(
                InputTokens: response.PromptTokenCount,
                OutputTokens: response.GenerationTokenCount,
                TotalTokens: response.PromptTokenCount + response.GenerationTokenCount
            ),
            ToolCalls = null
        };
    }

    public LanguageModelStreamChunk? ParseStreamChunk(string chunkJson)
    {
        // Llama doesn't support streaming on Bedrock (yet)
        throw new NotSupportedException("Meta Llama models on Bedrock do not support streaming");
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "stop" => FinishReason.Stop,
        "length" => FinishReason.Length,
        _ => FinishReason.Other
    };
}
