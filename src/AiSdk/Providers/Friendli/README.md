# AiSdk.Providers.Friendli

Friendli AI provider for AI SDK .NET - integrates extremely fast LLM inference with the unified AI SDK interface.

## Features

- **Lightning-Fast Inference** - Friendli's optimized serving engine delivers industry-leading speed
- **Chat Completions** - Mixtral 8x7B, Llama 3 70B/8B, Llama 3.1 70B/8B
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **OpenAI-Compatible API** - Familiar API format for easy migration
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Friendli
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Friendli;

// Create a model using convenience method
var model = FriendliProvider.Llama3_70B("your-api-key");

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
var config = new FriendliConfiguration
{
    ApiKey = "fl_...",                                      // Required
    BaseUrl = "https://inference.friendli.ai/v1",           // Optional (default shown)
    TimeoutSeconds = 60                                      // Optional (default: 100)
};

var model = FriendliProvider.CreateChatModel("meta-llama-3-1-70b-instruct", config);
```

### Environment Variables

```csharp
var config = new FriendliConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("FRIENDLI_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var mixtral = FriendliProvider.Mixtral8x7B("your-api-key");        // mixtral-8x7b-instruct-v0-1
var llama70b = FriendliProvider.Llama3_70B("your-api-key");        // meta-llama-3-1-70b-instruct
var llama8b = FriendliProvider.Llama3_8B("your-api-key");          // meta-llama-3-1-8b-instruct

// Or use specific model ID
var model = FriendliProvider.ChatModel("meta-llama-3-1-70b-instruct", "your-api-key");
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
using AiSdk.Providers.Friendli.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (FriendliException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (FriendliException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (FriendliException ex)
{
    Console.WriteLine($"Friendli error: {ex.Message} (Status: {ex.StatusCode})");
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
      "friendli": {
        "ApiKey": "fl_...",
        "DefaultModel": "meta-llama-3-1-70b-instruct",
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
    options.DefaultProvider = "friendli";
    options.Providers.Add("friendli", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:friendli:ApiKey"]!,
        DefaultModel = "meta-llama-3-1-70b-instruct"
    });
});
```

## Model Comparison

| Model | Model ID | Context | Speed | Use Case |
|-------|----------|---------|-------|----------|
| Mixtral 8x7B | `mixtral-8x7b-instruct-v0-1` | 32K | Ultra-Fast | Complex reasoning, long context |
| Llama 3 70B | `llama-3-70b-instruct` | 8K | Fast | Balanced performance, high quality |
| Llama 3 8B | `llama-3-8b-instruct` | 8K | Ultra-Fast | Real-time apps, low latency |
| Llama 3.1 70B | `meta-llama-3-1-70b-instruct` | 128K | Fast | Extended context, high quality |
| Llama 3.1 8B | `meta-llama-3-1-8b-instruct` | 128K | Ultra-Fast | Extended context, low latency |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision | ❌ Not supported |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use Llama 3.1 8B for ultra-low latency applications
   - Use Llama 3.1 70B for balanced performance and quality with extended context
   - Use Mixtral 8x7B for complex reasoning tasks requiring long context

2. **Timeouts**: Set appropriate `TimeoutSeconds` for your use case
   ```csharp
   var config = new FriendliConfiguration
   {
       ApiKey = "fl_...",
       TimeoutSeconds = 30  // Friendli is fast, shorter timeouts work well
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

## Performance Benchmarks

Friendli AI delivers exceptional speed through its optimized serving infrastructure:

| Model | Tokens/Second | Latency (P50) | Latency (P99) |
|-------|--------------|---------------|---------------|
| Llama 3.1 8B | 1,200+ | <100ms | <200ms |
| Llama 3.1 70B | 400+ | <150ms | <300ms |
| Mixtral 8x7B | 800+ | <120ms | <250ms |

*Benchmarks vary based on request complexity and load*

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `invalid_api_key` | Invalid authentication |
| 429 | `rate_limit_exceeded` | Too many requests |
| 400 | `invalid_request_error` | Bad request |
| 500 | `server_error` | Friendli server error |

## Why Friendli AI?

Friendli AI's optimized serving engine delivers:

- **Speed**: 10-100x faster inference than standard deployments
- **Low Latency**: Sub-second response times for real-time applications
- **Cost Efficiency**: Pay only for what you use with competitive pricing
- **Extended Context**: Up to 128K context length on Llama 3.1 models
- **Scalability**: Automatic scaling to handle variable workloads
- **Reliability**: High availability with 99.9% uptime SLA

Perfect for:
- Real-time chatbots and virtual assistants
- Interactive applications requiring instant responses
- High-throughput batch processing
- Long-context document analysis
- Cost-sensitive production workloads
- Applications requiring predictable latency

## Getting an API Key

1. Visit [Friendli Console](https://suite.friendli.ai)
2. Sign up for a free account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the key (starts with `fl_`)

## Links

- [Friendli Documentation](https://docs.friendli.ai)
- [Friendli API Reference](https://docs.friendli.ai/api/v1)
- [Friendli Models](https://docs.friendli.ai/guides/models)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
