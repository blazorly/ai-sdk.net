using AiSdk.Abstractions;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace BlazorServerExample;

/// <summary>
/// Mock implementation of ILanguageModel for testing and demonstration purposes.
/// Simulates streaming responses with realistic delays.
/// </summary>
public class MockLanguageModel : ILanguageModel
{
    private readonly Random _random = new();

    public string SpecificationVersion => "v1";
    public string ModelId => "mock-model-v1";
    public string Provider => "mock";

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(
            new Dictionary<string, IReadOnlyList<string>>());
    }

    /// <summary>
    /// Generates a complete response (non-streaming).
    /// </summary>
    public async Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        var lastMessage = options.Messages.LastOrDefault();
        var userMessage = lastMessage?.Content ?? "Hello";
        var response = GenerateMockResponse(userMessage);

        return new LanguageModelGenerateResult
        {
            Text = response,
            FinishReason = FinishReason.Stop,
            Usage = new Usage(
                InputTokens: 50,
                OutputTokens: response.Split(' ').Length,
                TotalTokens: 50 + response.Split(' ').Length
            )
        };
    }

    /// <summary>
    /// Generates a streaming response with realistic token-by-token delivery.
    /// </summary>
    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Initial delay to simulate connection
        await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken);

        var lastMessage = options.Messages.LastOrDefault();
        var userMessage = lastMessage?.Content ?? "Hello";
        var response = GenerateMockResponse(userMessage);
        var words = response.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            var word = words[i];
            var chunk = i < words.Length - 1 ? word + " " : word;

            yield return new LanguageModelStreamChunk
            {
                Type = ChunkType.TextDelta,
                Delta = chunk,
                FinishReason = i == words.Length - 1 ? FinishReason.Stop : null
            };

            // Simulate realistic typing speed (30-80ms per word)
            var delay = _random.Next(30, 80);
            await Task.Delay(TimeSpan.FromMilliseconds(delay), cancellationToken);
        }
    }

    /// <summary>
    /// Generates a contextual mock response based on the user's message.
    /// </summary>
    private string GenerateMockResponse(string userMessage)
    {
        userMessage = userMessage.ToLowerInvariant();

        // Contextual responses based on keywords
        if (userMessage.Contains("hello") || userMessage.Contains("hi"))
        {
            return "Hello! I'm your AI assistant powered by AiSdk in Blazor Server. How can I help you today?";
        }
        else if (userMessage.Contains("blazor"))
        {
            return "Blazor is a fantastic framework! This example demonstrates real-time streaming AI chat using Blazor Server with SignalR for seamless client-server communication.";
        }
        else if (userMessage.Contains("stream"))
        {
            return "Streaming allows me to send responses word-by-word in real-time, providing a more interactive and engaging user experience. You're seeing it in action right now!";
        }
        else if (userMessage.Contains("how") && userMessage.Contains("work"))
        {
            return "I'm a mock language model that simulates AI responses. In a production environment, you'd replace me with a real AI model like OpenAI's GPT, Anthropic's Claude, or Azure OpenAI.";
        }
        else if (userMessage.Contains("feature") || userMessage.Contains("can you"))
        {
            return "I can demonstrate streaming responses, handle multiple messages, maintain conversation context, and show real-time updates. This is a mock implementation, but the same architecture works with real AI models!";
        }
        else if (userMessage.Contains("thank"))
        {
            return "You're welcome! Feel free to ask me anything else about Blazor, AI SDK, or streaming chat implementations.";
        }
        else
        {
            // Generic responses
            var responses = new[]
            {
                "That's an interesting question! This mock model is simulating a streaming response to demonstrate how real-time AI chat works in Blazor Server.",
                "I'm processing your message and generating a response in real-time. Notice how the text appears word by word? That's the streaming feature in action!",
                "Great question! In a production scenario, you'd connect this to OpenAI, Anthropic Claude, or Azure OpenAI. The streaming architecture remains the same.",
                "I understand you're asking about that topic. This example showcases how AiSdk integrates seamlessly with Blazor Server for real-time AI interactions.",
                "Thanks for your message! This demonstrates the power of combining Blazor's real-time capabilities with AI SDK's streaming features for a responsive chat experience."
            };

            return responses[_random.Next(responses.Length)];
        }
    }
}
