using System.Runtime.CompilerServices;
using System.Text.Json;
using AiSdk.Abstractions;

namespace FunctionCallingExample;

/// <summary>
/// A mock language model that simulates tool calling.
/// This allows the example to run without requiring an API key.
/// </summary>
public class MockLanguageModel : ILanguageModel
{
    public string SpecificationVersion => "v3";
    public string Provider => "mock";
    public string ModelId => "mock-function-calling-model";

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        var empty = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(empty);
    }

    public Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        // Extract the user prompt
        var userMessage = options.Messages?.FirstOrDefault(m => m.Role == MessageRole.User);
        var promptText = userMessage?.Content?.ToLowerInvariant() ?? "";

        // Simulate intelligent tool selection based on the prompt
        List<ToolCall>? toolCalls = null;
        string? responseText = null;

        // Check if weather-related
        if (promptText.Contains("weather") || promptText.Contains("temperature"))
        {
            // Extract city from prompt (very simple extraction)
            var city = ExtractCity(promptText);

            toolCalls = new List<ToolCall>
            {
                new ToolCall(
                    ToolCallId: "call_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                    ToolName: "get_weather",
                    Arguments: JsonDocument.Parse($"{{\"city\":\"{city}\",\"unit\":\"celsius\"}}")
                )
            };
        }
        // Check if calculation-related
        else if (promptText.Contains("calculate") || promptText.Contains("add") ||
                 promptText.Contains("multiply") || promptText.Contains("divide") ||
                 ContainsNumbers(promptText))
        {
            var (operation, a, b) = ExtractCalculation(promptText);

            toolCalls = new List<ToolCall>
            {
                new ToolCall(
                    ToolCallId: "call_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                    ToolName: "calculate",
                    Arguments: JsonDocument.Parse($"{{\"operation\":\"{operation}\",\"a\":{a},\"b\":{b}}}")
                )
            };
        }
        else
        {
            // No tool needed, just respond with text
            responseText = "I can help you with weather information or calculations. " +
                          "Try asking 'What's the weather in London?' or 'Calculate 15 times 23'.";
        }

        var result = new LanguageModelGenerateResult
        {
            Text = responseText,
            ToolCalls = toolCalls,
            FinishReason = toolCalls != null ? FinishReason.ToolCalls : FinishReason.Stop,
            Usage = new Usage(
                InputTokens: 20,
                OutputTokens: 10,
                TotalTokens: 30
            )
        };

        return Task.FromResult(result);
    }

    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // For simplicity, convert the non-streaming result to chunks
        var result = await GenerateAsync(options, cancellationToken);

        if (result.ToolCalls != null)
        {
            foreach (var toolCall in result.ToolCalls)
            {
                yield return new LanguageModelStreamChunk
                {
                    Type = ChunkType.ToolCallDelta,
                    ToolCall = toolCall
                };
            }
        }

        if (result.Text != null)
        {
            yield return new LanguageModelStreamChunk
            {
                Type = ChunkType.TextDelta,
                Delta = result.Text
            };
        }

        yield return new LanguageModelStreamChunk
        {
            Type = ChunkType.Finish,
            FinishReason = result.FinishReason,
            Usage = result.Usage
        };
    }

    private string ExtractCity(string text)
    {
        // Simple city extraction - look for common city names
        var cities = new[] { "london", "paris", "new york", "tokyo", "sydney", "san francisco", "berlin", "mumbai" };
        foreach (var city in cities)
        {
            if (text.Contains(city))
            {
                return city;
            }
        }
        return "London"; // Default city
    }

    private bool ContainsNumbers(string text)
    {
        return text.Any(char.IsDigit);
    }

    private (string operation, double a, double b) ExtractCalculation(string text)
    {
        // Very simple number and operation extraction
        var numbers = new List<double>();
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (double.TryParse(part.TrimEnd('.', ',', '?', '!'), out var num))
            {
                numbers.Add(num);
            }
        }

        // Default to 0 if not enough numbers found
        var a = numbers.Count > 0 ? numbers[0] : 10;
        var b = numbers.Count > 1 ? numbers[1] : 5;

        // Detect operation
        var operation = "add";
        if (text.Contains("multiply") || text.Contains("times") || text.Contains("×") || text.Contains("*"))
            operation = "multiply";
        else if (text.Contains("divide") || text.Contains("÷") || text.Contains("/"))
            operation = "divide";
        else if (text.Contains("subtract") || text.Contains("minus") || text.Contains("-"))
            operation = "subtract";
        else if (text.Contains("power") || text.Contains("^"))
            operation = "power";
        else if (text.Contains("modulo") || text.Contains("%"))
            operation = "modulo";

        return (operation, a, b);
    }
}
