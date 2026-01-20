# AiSdk.Providers.Cohere

Cohere provider for AI SDK .NET - integrates Command R+, Command R, and other Cohere models with the unified AI SDK interface.

## Features

- **Cohere Models** - Command R+, Command R, Command, Command Light
- **Streaming Support** - Real-time token-by-token streaming with SSE
- **Tool Use** - Cohere's tool/function calling system
- **Citations** - Automatic citation tracking for grounded responses
- **Multi-turn Conversations** - Full chat history support
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Cohere
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Cohere;

// Create a model
var model = CohereProvider.CommandRPlus("your-api-key");

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain quantum computing simply")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration

```csharp
var config = new CohereConfiguration
{
    ApiKey = "your-api-key",                          // Required
    BaseUrl = "https://api.cohere.ai/v1",            // Optional (default shown)
    TimeoutSeconds = 90                              // Optional (default: 100)
};

var model = CohereProvider.CreateChatModel("command-r-plus", config);
```

### Environment Variables

```csharp
var config = new CohereConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("COHERE_API_KEY")!
};

var model = CohereProvider.CommandRPlus(config.ApiKey);
```

### Available Models

```csharp
// Convenience methods
var commandRPlus = CohereProvider.CommandRPlus(apiKey);    // command-r-plus (best for complex tasks)
var commandR = CohereProvider.CommandR(apiKey);            // command-r (balanced performance)
var command = CohereProvider.Command(apiKey);              // command (standard model)

// Or use specific model ID
var model = CohereProvider.CreateChatModel("command-r-plus", apiKey);
```

## Usage Examples

### Simple Chat

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What is the capital of France?")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a short story about AI")
    }
};

await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
    else if (chunk.Type == ChunkType.Finish)
    {
        Console.WriteLine($"\nTokens used: {chunk.Usage?.TotalTokens}");
    }
}
```

### Multi-turn Conversation

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.User, "What is machine learning?"),
    new Message(MessageRole.Assistant, "Machine learning is a subset of AI..."),
    new Message(MessageRole.User, "Can you give me an example?")
};

var options = new LanguageModelCallOptions
{
    Messages = messages
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Tool Use

```csharp
var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = JsonSchema.FromType<WeatherRequest>()
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What's the weather in San Francisco?")
    },
    Tools = tools
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        var args = toolCall.Arguments.RootElement;

        // Execute tool...
        var toolResult = await GetWeather(args.GetProperty("location").GetString()!);

        // Send result back to Cohere
        var followUpOptions = new LanguageModelCallOptions
        {
            Messages = new List<Message>
            {
                new Message(MessageRole.User, "What's the weather in San Francisco?"),
                new Message(MessageRole.Assistant, result.Text, ToolCalls: result.ToolCalls),
                new Message(
                    Role: MessageRole.Tool,
                    Content: toolResult,
                    Name: toolCall.ToolName  // Use tool name for Cohere
                )
            },
            Tools = tools
        };

        var finalResult = await model.GenerateAsync(followUpOptions);
        Console.WriteLine(finalResult.Text);
    }
}
```

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 2048,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\n", "END" }
};

var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Cohere.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (CohereException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (CohereException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited");
}
catch (CohereException ex)
{
    Console.WriteLine($"Cohere error: {ex.Message} (Status: {ex.StatusCode})");
}
catch (AiSdkException ex)
{
    Console.WriteLine($"SDK error: {ex.Message}");
}
```

### Streaming with Tool Calls

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Calculate 25% of 800")
    },
    Tools = tools
};

await foreach (var chunk in model.StreamAsync(options))
{
    switch (chunk.Type)
    {
        case ChunkType.TextDelta:
            Console.Write(chunk.Delta);
            break;

        case ChunkType.ToolCallDelta:
            Console.WriteLine($"\nTool called: {chunk.ToolCall?.ToolName}");
            break;

        case ChunkType.Finish:
            Console.WriteLine($"\nFinished: {chunk.FinishReason}");
            Console.WriteLine($"Tokens: {chunk.Usage?.TotalTokens}");
            break;
    }
}
```

## Configuration with ASP.NET Core

### appsettings.json

```json
{
  "AiSdk": {
    "Providers": {
      "cohere": {
        "ApiKey": "your-api-key",
        "DefaultModel": "command-r-plus",
        "TimeoutSeconds": 90,
        "Enabled": true
      }
    }
  }
}
```

### Startup Registration

```csharp
using AiSdk.AspNetCore;

builder.Services.AddAiSdk(options =>
{
    options.DefaultProvider = "cohere";
    options.Providers.Add("cohere", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:cohere:ApiKey"]!,
        DefaultModel = "command-r-plus"
    });
});
```

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Tool Use | ✅ Full support |
| Multi-turn Conversations | ✅ Full support |
| Citations | ✅ Returned in response |
| Temperature Control | ✅ Supported |
| Max Tokens | ✅ Supported |
| Top-P Sampling | ✅ Supported |
| Stop Sequences | ✅ Supported |
| Vision (Images) | ❌ Not supported in chat API |
| System Messages | ⚠️ Limited (can be prepended to user message) |

## Important Notes

1. **Message Format**: Cohere uses a different message format than OpenAI/Anthropic:
   - The latest message must be separated as the `message` field
   - Previous messages go in `chat_history` array
   - Roles are `USER` and `CHATBOT` (mapped automatically)

2. **Tool Results**: Tool results use the tool name in the `Name` field:
   ```csharp
   new Message(
       Role: MessageRole.Tool,
       Content: result,
       Name: toolCall.ToolName  // Use ToolName, not ToolCallId
   )
   ```

3. **System Messages**: Cohere's chat endpoint doesn't have dedicated system message support. The SDK currently skips system messages. For production use, consider prepending system content to the first user message.

4. **Streaming Events**: Cohere uses specific SSE event types:
   - `text-generation` - Text content chunks
   - `tool-calls-generation` - Tool calls
   - `stream-end` - Final message with metadata

5. **Citations**: Cohere provides automatic citations in responses. Access them via the raw response metadata (future SDK enhancement).

## Model Comparison

| Model | Best For | Context | Speed |
|-------|----------|---------|-------|
| **command-r-plus** | Complex reasoning, coding, analysis | 128k | Slower |
| **command-r** | General tasks, balanced performance | 128k | Medium |
| **command** | Simple tasks, fast responses | 4k | Fast |
| **command-light** | Basic completions | 4k | Very Fast |

## Performance Tips

1. **Model Selection**:
   - Use Command R+ for complex reasoning and multi-step tasks
   - Use Command R for general chat and balanced performance
   - Use Command for simple, fast responses

2. **Streaming**: Always use streaming for long responses to improve perceived latency

3. **Temperature**:
   - Lower (0.1-0.3) for factual, deterministic responses
   - Medium (0.5-0.7) for balanced creativity
   - Higher (0.8-1.0) for creative writing

4. **Cancellation**: Support cancellation for long operations
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
   var result = await model.GenerateAsync(options, cts.Token);
   ```

## Common Error Codes

| Status Code | Description |
|-------------|-------------|
| 400 | Bad Request - Invalid parameters |
| 401 | Unauthorized - Invalid API key |
| 429 | Rate Limit - Too many requests |
| 500 | Internal Server Error |

## API Rate Limits

Cohere rate limits vary by account tier:
- **Trial**: 5 requests/minute
- **Production**: 10,000 requests/minute (default)
- **Enterprise**: Custom limits

Implement exponential backoff for production applications:

```csharp
async Task<LanguageModelGenerateResult> GenerateWithRetry(
    LanguageModelCallOptions options,
    int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await model.GenerateAsync(options);
        }
        catch (CohereException ex) when (ex.StatusCode == 429)
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }
    throw new InvalidOperationException("Max retries exceeded");
}
```

## Links

- [Cohere API Documentation](https://docs.cohere.com/reference/chat)
- [Cohere Models](https://docs.cohere.com/docs/models)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
