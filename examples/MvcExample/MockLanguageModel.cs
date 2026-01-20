using System.Runtime.CompilerServices;
using AiSdk.Abstractions;

namespace MvcExample;

/// <summary>
/// A mock language model that simulates AI responses for demo purposes.
/// This allows the example to run without requiring an API key.
/// In production, replace this with a real provider like OpenAI or Anthropic.
/// </summary>
public class MockLanguageModel : ILanguageModel
{
    public string SpecificationVersion => "v3";
    public string Provider => "mock";
    public string ModelId => "mock-chat-model";

    private static readonly string[] SampleResponses = new[]
    {
        "Hello! I'm a mock AI assistant. In a real application, I would be powered by models like GPT-4 or Claude. How can I help you today?",
        "That's an interesting question! As a mock model, I can demonstrate the streaming and non-streaming capabilities of the AI SDK.",
        "The AI SDK for .NET provides a unified interface for working with multiple AI providers. It's designed to be flexible and easy to use.",
        "I can help with various tasks including answering questions, generating content, and providing assistance. What would you like to know?",
        "This MVC example demonstrates how to integrate AI capabilities into an ASP.NET Core application with a modern, responsive UI.",
        "In production, you would configure a real AI provider in Program.cs and provide your API key through environment variables or configuration.",
        "The SDK supports both streaming and non-streaming responses, allowing you to choose the best experience for your users.",
        "You can switch between different AI providers without changing your application code, thanks to the unified ILanguageModel interface."
    };

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
        // Select a response based on the input length to add variety
        var messageCount = options.Messages?.Count ?? 0;
        var responseIndex = messageCount % SampleResponses.Length;
        var responseText = SampleResponses[responseIndex];

        var result = new LanguageModelGenerateResult
        {
            Text = responseText,
            FinishReason = FinishReason.Stop,
            Usage = new Usage(
                InputTokens: 10,
                OutputTokens: responseText.Split(' ').Length,
                TotalTokens: 10 + responseText.Split(' ').Length
            )
        };

        return Task.FromResult(result);
    }

    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Select a response based on the input to add variety
        var messageCount = options.Messages?.Count ?? 0;
        var responseIndex = messageCount % SampleResponses.Length;
        var responseText = SampleResponses[responseIndex];

        var words = responseText.Split(' ');

        // Stream each word with a small delay to simulate real-time generation
        foreach (var word in words)
        {
            // Simulate network latency
            await Task.Delay(30, cancellationToken);

            yield return new LanguageModelStreamChunk
            {
                Type = ChunkType.TextDelta,
                Delta = word + " "
            };
        }

        // Send final chunk with finish reason and usage stats
        yield return new LanguageModelStreamChunk
        {
            Type = ChunkType.Finish,
            FinishReason = FinishReason.Stop,
            Usage = new Usage(
                InputTokens: 10,
                OutputTokens: words.Length,
                TotalTokens: 10 + words.Length
            )
        };
    }
}
