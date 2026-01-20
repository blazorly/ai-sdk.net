# AiSdk.Providers.Groq

Groq provider for AI SDK .NET - integrates ultra-fast Llama, Mixtral, and Gemma models with the unified AI SDK interface.

## Features

- **Lightning-Fast Inference** - Groq's LPU technology delivers industry-leading speed
- **Chat Completions** - Llama 3.1 70B/8B, Mixtral 8x7B, Gemma 7B
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **OpenAI-Compatible API** - Familiar API format for easy migration
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Groq
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Groq;

// Create a model using convenience method
var model = GroqProvider.Llama3_1_70B("your-api-key");

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain quantum computing in one sentence")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration

```csharp
var config = new GroqConfiguration
{
    ApiKey = "gsk_...",                                      // Required
    BaseUrl = "https://api.groq.com/openai/v1",             // Optional (default shown)
    TimeoutSeconds = 60                                      // Optional (default: 100)
};

var model = GroqProvider.CreateChatModel("llama-3.1-70b-versatile", config);
```

### Environment Variables

```csharp
var config = new GroqConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var llama70b = GroqProvider.Llama3_1_70B("your-api-key");      // llama-3.1-70b-versatile
var llama8b = GroqProvider.Llama3_1_8B("your-api-key");        // llama-3.1-8b-instant
var mixtral = GroqProvider.Mixtral8x7B("your-api-key");        // mixtral-8x7b-32768
var gemma = GroqProvider.Gemma7B("your-api-key");              // gemma-7b-it

// Or use specific model ID
var model = GroqProvider.ChatModel("llama-3.1-70b-versatile", "your-api-key");
```

## Usage Examples

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a haiku about coding")
    }
};

await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
}
```

### Function Calling

```csharp
var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "get_weather",
        Description = "Get the current weather in a location",
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
        Console.WriteLine($"Arguments: {toolCall.Arguments}");

        // Execute the tool and send result back...
    }
}
```

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 1000,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\n", "END" },
    ToolChoice = "get_weather"  // Force specific tool
};

var result = await model.GenerateAsync(options);
```

### Multi-Turn Conversations

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a helpful assistant."),
    new Message(MessageRole.User, "What is the capital of France?"),
    new Message(MessageRole.Assistant, "The capital of France is Paris."),
    new Message(MessageRole.User, "What's the population?")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Groq.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (GroqException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (GroqException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (GroqException ex)
{
    Console.WriteLine($"Groq error: {ex.Message} (Status: {ex.StatusCode})");
}
catch (AiSdkException ex)
{
    Console.WriteLine($"SDK error: {ex.Message}");
}
```

## Configuration with ASP.NET Core

### appsettings.json

```json
{
  "AiSdk": {
    "Providers": {
      "groq": {
        "ApiKey": "gsk_...",
        "DefaultModel": "llama-3.1-70b-versatile",
        "TimeoutSeconds": 60,
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
    options.DefaultProvider = "groq";
    options.Providers.Add("groq", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:groq:ApiKey"]!,
        DefaultModel = "llama-3.1-70b-versatile"
    });
});
```

## Model Comparison

| Model | Model ID | Context | Speed | Use Case |
|-------|----------|---------|-------|----------|
| Llama 3.1 70B | `llama-3.1-70b-versatile` | 8K | Fast | Balanced performance, high quality |
| Llama 3.1 8B | `llama-3.1-8b-instant` | 8K | Ultra-Fast | Real-time apps, low latency |
| Mixtral 8x7B | `mixtral-8x7b-32768` | 32K | Fast | Long context, complex reasoning |
| Gemma 7B IT | `gemma-7b-it` | 8K | Fast | Instruction following |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision | ❌ Not supported by Groq |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use Llama 3.1 8B for ultra-low latency applications
   - Use Llama 3.1 70B for balanced performance and quality
   - Use Mixtral 8x7B for long-context tasks (32K tokens)

2. **Timeouts**: Set appropriate `TimeoutSeconds` for your use case
   ```csharp
   var config = new GroqConfiguration
   {
       ApiKey = "gsk_...",
       TimeoutSeconds = 30  // Groq is fast, shorter timeouts work well
   };
   ```

3. **Streaming**: Use streaming for long responses to improve perceived performance
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

4. **HttpClient Reuse**: The provider reuses HttpClient instances automatically

5. **Cancellation**: Always pass `CancellationToken` for long-running operations
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
   var result = await model.GenerateAsync(options, cts.Token);
   ```

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `invalid_api_key` | Invalid authentication |
| 429 | `rate_limit_exceeded` | Too many requests |
| 400 | `invalid_request_error` | Bad request |
| 500 | `server_error` | Groq server error |

## Why Groq?

Groq's Language Processing Unit (LPU) technology delivers:

- **Speed**: 10-100x faster inference than traditional GPUs
- **Low Latency**: Sub-second response times for real-time applications
- **Predictable Performance**: Consistent throughput and latency
- **Energy Efficient**: Lower power consumption than GPU alternatives
- **Open Models**: Access to Llama, Mixtral, and Gemma models

Perfect for:
- Real-time chatbots and assistants
- Interactive applications requiring instant responses
- High-throughput batch processing
- Cost-sensitive workloads
- Applications requiring predictable latency

## Getting an API Key

1. Visit [Groq Console](https://console.groq.com)
2. Sign up for a free account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the key (starts with `gsk_`)

## Links

- [Groq Documentation](https://console.groq.com/docs)
- [Groq API Reference](https://console.groq.com/docs/api-reference)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
