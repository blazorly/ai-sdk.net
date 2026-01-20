# StreamingExample - Real-Time Token Streaming

This example demonstrates how to use the AI SDK's streaming capabilities to receive and display AI-generated text in real-time as it's being produced.

## What is Streaming?

Streaming allows you to receive AI responses token-by-token as they're generated, rather than waiting for the complete response. This provides:

- **Immediate feedback**: Users see results instantly
- **Better UX**: Perceived performance improvement
- **Progress indication**: Natural loading experience
- **Interruptibility**: Can cancel long-running requests

## Features Demonstrated

### 1. Basic Streaming
```csharp
await foreach (var chunk in AiClient.StreamTextAsync(model, options))
{
    Console.Write(chunk.Delta);
}
```

### 2. Progress Tracking
Monitor token generation speed and count in real-time.

### 3. Response Collection
Stream to the UI while simultaneously collecting the complete response for processing.

## Running the Example

```bash
cd examples/StreamingExample
dotnet run
```

## Understanding the Code

### MockLanguageModel
A simulated model that demonstrates streaming without requiring an API key. It breaks responses into chunks with artificial delays to simulate network latency.

### Stream Processing
The `StreamTextAsync` method returns an `IAsyncEnumerable<LanguageModelStreamChunk>`, allowing you to process each chunk as it arrives:

```csharp
await foreach (var chunk in AiClient.StreamTextAsync(model, options))
{
    // chunk.Delta contains the new text
    // chunk.Type indicates the chunk type (TextDelta, Finish, etc.)
    // chunk.Usage contains token statistics (in final chunk)
}
```

## Key Concepts

**Chunk Types:**
- `TextDelta`: Contains partial text content
- `Finish`: Marks completion with usage statistics
- `ToolCallDelta`: Contains tool/function call information
- `Error`: Indicates an error occurred

**Best Practices:**
1. Always handle cancellation tokens for user interruption
2. Buffer output appropriately for your UI framework
3. Handle errors gracefully (network issues, rate limits)
4. Consider mobile data usage when streaming

## Adapting for Production

Replace the mock model with a real provider:

```csharp
// Using OpenAI (when provider is available)
var openai = new OpenAIProvider(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var model = openai.ChatModel("gpt-4");
```

## Advanced Use Cases

- **Chat interfaces**: Stream responses in conversational UIs
- **Code generation**: Show code as it's written
- **Long-form content**: Articles, documentation, reports
- **Real-time translation**: Stream translations as they're generated
- **Interactive storytelling**: Generate narratives interactively

## Performance Considerations

- Streaming uses slightly more bandwidth than non-streaming
- First token latency is typically much lower
- Ideal for responses where users need immediate feedback
- Consider non-streaming for batch processing or background tasks
