using System.Runtime.CompilerServices;
using AiSdk.Abstractions;

namespace MinimalApiExample;

/// <summary>
/// A mock language model that simulates AI responses without requiring API keys.
/// Perfect for testing, development, and demonstration purposes.
/// </summary>
public class MockLanguageModel : ILanguageModel
{
    private readonly Random _random = new();

    public string SpecificationVersion => "v3";
    public string Provider => "mock";
    public string ModelId => "mock-api-model";

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
        // Extract the user's question from messages
        var userMessage = options.Messages?.LastOrDefault(m => m.Role == MessageRole.User);
        var prompt = userMessage?.Content ?? "Hello";

        // Generate a contextual response based on the prompt
        var responseText = GenerateContextualResponse(prompt);

        var result = new LanguageModelGenerateResult
        {
            Text = responseText,
            FinishReason = FinishReason.Stop,
            Usage = new Usage(
                InputTokens: CountTokens(prompt),
                OutputTokens: CountTokens(responseText),
                TotalTokens: CountTokens(prompt) + CountTokens(responseText)
            )
        };

        return Task.FromResult(result);
    }

    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Extract the user's question from messages
        var userMessage = options.Messages?.LastOrDefault(m => m.Role == MessageRole.User);
        var prompt = userMessage?.Content ?? "Hello";

        // Generate a contextual response
        var responseText = GenerateContextualResponse(prompt);

        // Stream the response word by word
        var words = responseText.Split(' ');
        var inputTokens = CountTokens(prompt);

        for (var i = 0; i < words.Length; i++)
        {
            // Simulate realistic streaming delay (30-100ms per word)
            var delay = _random.Next(30, 100);
            await Task.Delay(delay, cancellationToken);

            var word = words[i];
            var isLastWord = i == words.Length - 1;

            yield return new LanguageModelStreamChunk
            {
                Type = ChunkType.TextDelta,
                Delta = word + (isLastWord ? "" : " ")
            };
        }

        // Final chunk with usage information
        yield return new LanguageModelStreamChunk
        {
            Type = ChunkType.Finish,
            FinishReason = FinishReason.Stop,
            Usage = new Usage(
                InputTokens: inputTokens,
                OutputTokens: words.Length,
                TotalTokens: inputTokens + words.Length
            )
        };
    }

    private string GenerateContextualResponse(string prompt)
    {
        var lowerPrompt = prompt.ToLowerInvariant();

        // Generate contextual responses based on keywords
        if (lowerPrompt.Contains("weather"))
        {
            return "I'm a mock AI model, so I can't provide real weather data. However, in a production environment with a real AI provider like OpenAI or Anthropic, I could help you get accurate weather information for any location.";
        }

        if (lowerPrompt.Contains("hello") || lowerPrompt.Contains("hi"))
        {
            return "Hello! I'm a mock language model designed for testing the AI SDK. I can demonstrate streaming responses, chat completions, and API integration without requiring actual API keys. How can I help you today?";
        }

        if (lowerPrompt.Contains("streaming"))
        {
            return "Streaming is a powerful feature that allows AI responses to be delivered in real-time, word by word. This creates a better user experience by providing immediate feedback and reducing perceived latency. It's particularly useful for long-form content generation.";
        }

        if (lowerPrompt.Contains("minimal api") || lowerPrompt.Contains("asp.net"))
        {
            return "ASP.NET Core Minimal APIs provide a simplified approach to building HTTP APIs with minimal dependencies and ceremony. They're perfect for microservices, small APIs, and when you want to quickly prototype endpoints without the overhead of controllers.";
        }

        if (lowerPrompt.Contains("ai sdk") || lowerPrompt.Contains("aisdk"))
        {
            return "The AI SDK for .NET provides a unified interface for working with multiple AI providers including OpenAI, Anthropic, Google, and Azure. It supports text generation, streaming, function calling, embeddings, and structured outputs with a consistent API across all providers.";
        }

        if (lowerPrompt.Contains("code") || lowerPrompt.Contains("program"))
        {
            return "As a mock model, I can help demonstrate how the AI SDK integrates with your application. In production, you'd connect to real AI providers that can generate code, answer questions, analyze data, and perform various language tasks with high accuracy.";
        }

        // Default response for any other input
        return $"You asked: '{prompt}'. I'm a mock AI model for demonstration purposes. In production, replace me with a real provider like OpenAI, Anthropic, or Google to get actual AI-powered responses. The AI SDK makes this switch seamless!";
    }

    private static int CountTokens(string text)
    {
        // Rough approximation: ~1 token per 4 characters
        return Math.Max(1, text.Length / 4);
    }
}
