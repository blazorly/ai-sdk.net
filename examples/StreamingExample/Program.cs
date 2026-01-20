using AiSdk;
using AiSdk.Abstractions;
using StreamingExample;

Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   AI SDK for .NET - Real-Time Streaming Example              ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Create a mock language model for demonstration
// In production, you would use a real provider like OpenAI:
// var model = new OpenAIProvider(apiKey: "your-api-key").ChatModel("gpt-4");
var model = new MockLanguageModel();

Console.WriteLine("Demonstration 1: Basic Streaming");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Basic streaming example - tokens appear in real-time
Console.Write("AI: ");
await foreach (var chunk in AiClient.StreamTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "Tell me about the benefits of streaming responses"
    }))
{
    // Each chunk contains a partial response (delta)
    // Write it immediately to show real-time generation
    if (chunk.Delta != null)
    {
        Console.Write(chunk.Delta);
    }

    // The final chunk contains usage statistics
    if (chunk.Type == ChunkType.Finish && chunk.Usage != null)
    {
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"Token Usage: {chunk.Usage.TotalTokens} total " +
                         $"({chunk.Usage.InputTokens} input + {chunk.Usage.OutputTokens} output)");
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 2: Streaming with Progress Indication");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Advanced example with progress tracking
var tokenCount = 0;
var startTime = DateTime.UtcNow;

Console.Write("AI: ");
await foreach (var chunk in AiClient.StreamTextAsync(
    model,
    new GenerateTextOptions
    {
        System = "You are a helpful assistant that explains technical concepts clearly.",
        Prompt = "Explain why streaming is important for user experience"
    }))
{
    if (chunk.Delta != null)
    {
        Console.Write(chunk.Delta);
        tokenCount++;

        // Show a progress indicator every 10 tokens
        if (tokenCount % 10 == 0)
        {
            var elapsed = DateTime.UtcNow - startTime;
            Console.Write($"[{tokenCount} tokens in {elapsed.TotalSeconds:F1}s]");
        }
    }

    if (chunk.Type == ChunkType.Finish)
    {
        var totalElapsed = DateTime.UtcNow - startTime;
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"Streaming completed in {totalElapsed.TotalSeconds:F2} seconds");
        Console.WriteLine($"Average speed: {tokenCount / totalElapsed.TotalSeconds:F1} tokens/second");
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 3: Collecting Full Response");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Sometimes you need both streaming AND the complete response
var fullResponse = new System.Text.StringBuilder();

Console.WriteLine("Streaming in real-time:");
Console.Write("AI: ");

await foreach (var chunk in AiClient.StreamTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "List three benefits of async programming",
        MaxTokens = 200
    }))
{
    if (chunk.Delta != null)
    {
        Console.Write(chunk.Delta);
        fullResponse.Append(chunk.Delta);
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Complete response for processing:");
Console.WriteLine($"Length: {fullResponse.Length} characters");
Console.WriteLine($"Preview: {fullResponse.ToString().Substring(0, Math.Min(50, fullResponse.Length))}...");

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   Key Concepts Demonstrated:                                 ║");
Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
Console.WriteLine("║   • Real-time token streaming for immediate feedback         ║");
Console.WriteLine("║   • Progress tracking during generation                      ║");
Console.WriteLine("║   • Collecting full response while streaming                 ║");
Console.WriteLine("║   • Usage statistics and performance metrics                 ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Next Steps:");
Console.WriteLine("  1. Replace MockLanguageModel with a real provider");
Console.WriteLine("  2. Experiment with different prompts and settings");
Console.WriteLine("  3. Add error handling for production use");
Console.WriteLine("  4. Implement custom UI rendering (e.g., markdown, syntax highlighting)");
