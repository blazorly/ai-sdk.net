using System.Text;
using System.Text.Json;
using AiSdk.Abstractions;
using AiSdk.Providers.AmazonBedrock.Models;

namespace AiSdk.Providers.AmazonBedrock.Adapters;

/// <summary>
/// Adapter for Amazon Titan models on Bedrock.
/// Titan models use a simpler request/response format and don't support tools or streaming.
/// </summary>
internal class TitanBedrockAdapter : IBedrockModelAdapter
{
    public bool SupportsStreaming => false;

    public string BuildRequest(LanguageModelCallOptions options)
    {
        // Titan expects a simple text prompt, so we need to convert messages to a single string
        var promptBuilder = new StringBuilder();

        foreach (var msg in options.Messages)
        {
            switch (msg.Role)
            {
                case MessageRole.System:
                    promptBuilder.AppendLine($"System: {msg.Content}");
                    promptBuilder.AppendLine();
                    break;
                case MessageRole.User:
                    promptBuilder.AppendLine($"User: {msg.Content}");
                    break;
                case MessageRole.Assistant:
                    promptBuilder.AppendLine($"Assistant: {msg.Content}");
                    break;
            }
        }

        // Add final "Assistant:" to prompt the model to respond
        if (options.Messages.Count > 0 && options.Messages[^1].Role == MessageRole.User)
        {
            promptBuilder.Append("Assistant:");
        }

        var request = new TitanRequest
        {
            InputText = promptBuilder.ToString(),
            TextGenerationConfig = new TitanTextGenerationConfig
            {
                MaxTokenCount = options.MaxTokens,
                Temperature = options.Temperature,
                TopP = options.TopP,
                StopSequences = options.StopSequences
            }
        };

        return JsonSerializer.Serialize(request);
    }

    public LanguageModelGenerateResult ParseResponse(string responseJson)
    {
        var response = JsonSerializer.Deserialize<TitanResponse>(responseJson)
            ?? throw new InvalidOperationException("Failed to deserialize Titan response");

        if (response.Results.Count == 0)
        {
            throw new InvalidOperationException("Titan response contained no results");
        }

        var result = response.Results[0];

        return new LanguageModelGenerateResult
        {
            Text = result.OutputText,
            FinishReason = MapFinishReason(result.CompletionReason),
            Usage = new Usage(
                InputTokens: response.InputTextTokenCount,
                OutputTokens: result.TokenCount,
                TotalTokens: response.InputTextTokenCount + result.TokenCount
            ),
            ToolCalls = null
        };
    }

    public LanguageModelStreamChunk? ParseStreamChunk(string chunkJson)
    {
        // Titan doesn't support streaming
        throw new NotSupportedException("Amazon Titan models do not support streaming");
    }

    private static FinishReason MapFinishReason(string? reason) => reason switch
    {
        "FINISH" => FinishReason.Stop,
        "LENGTH" => FinishReason.Length,
        _ => FinishReason.Other
    };
}
