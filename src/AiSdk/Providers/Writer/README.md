# AiSdk.Providers.Writer

Writer provider for AI SDK .NET - integrates Writer's Palmyra models with the unified AI SDK interface.

## Features

- **Latest Palmyra Models** - Access to Palmyra X 004, X 003, and Palmyra 2
- **Advanced Reasoning** - Powerful language understanding and generation
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **OpenAI-Compatible API** - Familiar API format for easy migration
- **Enterprise-Ready** - Built for production workloads
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk
```

The Writer provider is included in the consolidated AiSdk package.

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Writer;

// Create a provider instance
var config = new WriterConfiguration
{
    ApiKey = "your-api-key"
};
var provider = new WriterProvider(config);

// Use the latest Palmyra model
var model = provider.PalmyraX_004();

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
var config = new WriterConfiguration
{
    ApiKey = "sk-...",                                   // Required
    BaseUrl = "https://api.writer.com/v1",               // Optional (default shown)
    TimeoutSeconds = 60                                   // Optional (default: 100)
};

var provider = new WriterProvider(config);
```

### Environment Variables

```csharp
var config = new WriterConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("WRITER_API_KEY")!
};
```

### Available Models

```csharp
// Using provider instance methods
var provider = new WriterProvider(config);

var palmyraX004 = provider.PalmyraX_004();   // palmyra-x-004 - Latest flagship
var palmyraX003 = provider.PalmyraX_003();   // palmyra-x-003 - Previous generation
var palmyra2 = provider.Palmyra2();          // palmyra-2 - Efficient model

// Or use specific model ID
var model = provider.ChatModel("palmyra-x-004");

// Static convenience methods for quick initialization
var model1 = WriterProvider.PalmyraX_004("your-api-key");
var model2 = WriterProvider.PalmyraX_003("your-api-key");
var model3 = WriterProvider.Palmyra2("your-api-key");
```

## Usage Examples

### General Chat

```csharp
var provider = new WriterProvider(config);
var model = provider.PalmyraX_004();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a creative story about AI")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Content Generation

```csharp
var provider = new WriterProvider(config);
var model = provider.PalmyraX_004();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a blog post about sustainable technology")
    },
    Temperature = 0.7,  // Balanced creativity
    MaxTokens = 2000
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Business Writing

```csharp
var provider = new WriterProvider(config);
var model = provider.PalmyraX_004();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.System, "You are a professional business writer."),
        new Message(MessageRole.User, "Draft a quarterly business report summary")
    },
    Temperature = 0.5  // More focused output
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
        new Message(MessageRole.User, "Write a detailed explanation of machine learning")
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
    MaxTokens = 2000,
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
    new Message(MessageRole.User, "What is machine learning?"),
    new Message(MessageRole.Assistant, "Machine learning is a subset of artificial intelligence..."),
    new Message(MessageRole.User, "Can you give me a practical example?")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Writer.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (WriterException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (WriterException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (WriterException ex)
{
    Console.WriteLine($"Writer error: {ex.Message} (Status: {ex.StatusCode})");
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
      "writer": {
        "ApiKey": "sk-...",
        "DefaultModel": "palmyra-x-004",
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
    options.DefaultProvider = "writer";
    options.Providers.Add("writer", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:writer:ApiKey"]!,
        DefaultModel = "palmyra-x-004"
    });
});
```

## Model Comparison

| Model | Model ID | Context | Strengths | Use Case |
|-------|----------|---------|-----------|----------|
| Palmyra X 004 | `palmyra-x-004` | 128K | Latest flagship, advanced reasoning, long context | Complex tasks, enterprise applications |
| Palmyra X 003 | `palmyra-x-003` | 128K | Strong performance, proven reliability | Production workloads, general purpose |
| Palmyra 2 | `palmyra-2` | 32K | Efficient, fast responses | Cost-effective applications |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Long Context | ✅ Up to 128K tokens |
| Content Generation | ✅ Full support |
| Multi-turn Conversations | ✅ Full support |
| Vision | ❌ Not supported |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use **Palmyra X 004** for complex tasks requiring advanced reasoning
   - Use **Palmyra X 003** for proven production reliability
   - Use **Palmyra 2** for cost-effective general-purpose tasks

2. **Optimize for Content Generation**:
   ```csharp
   var config = new WriterConfiguration
   {
       ApiKey = "sk-...",
       TimeoutSeconds = 120  // Longer timeout for detailed content
   };

   var options = new LanguageModelCallOptions
   {
       Temperature = 0.7,  // Balanced creativity
       MaxTokens = 4000    // Allow comprehensive responses
   };
   ```

3. **Streaming for Long Responses**: Use streaming for detailed content to improve perceived performance
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

4. **Long Context Windows**: Palmyra X models support up to 128K tokens
   ```csharp
   var options = new LanguageModelCallOptions
   {
       Messages = longConversationHistory,  // Can include extensive context
       MaxTokens = 2000
   };
   ```

5. **HttpClient Reuse**: The provider reuses HttpClient instances automatically for optimal performance

6. **Cancellation**: Always pass `CancellationToken` for long-running operations
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
   var result = await model.GenerateAsync(options, cts.Token);
   ```

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `invalid_api_key` | Invalid authentication |
| 429 | `rate_limit_exceeded` | Too many requests |
| 400 | `invalid_request_error` | Bad request format |
| 500 | `server_error` | Writer server error |

## Why Writer?

Writer's Palmyra models offer exceptional capabilities:

- **Enterprise-Focused**: Built for business and enterprise applications
- **Long Context**: Support for 128K tokens in flagship models
- **Advanced Reasoning**: Powerful language understanding and generation
- **Content Quality**: High-quality outputs for professional writing
- **OpenAI Compatible**: Easy migration from OpenAI
- **Production Ready**: Proven reliability and performance

Perfect for:
- Enterprise content generation
- Business communications
- Technical writing and documentation
- Marketing and creative content
- Customer support automation
- Knowledge base systems
- Long-form content creation
- Applications requiring extensive context

## Use Case Recommendations

### For Content Creation
```csharp
var provider = new WriterProvider(config);
var model = provider.PalmyraX_004();

// Optimized for creative content
var options = new LanguageModelCallOptions
{
    Temperature = 0.8,  // More creative
    MaxTokens = 4000
};
```

### For Business Writing
```csharp
var provider = new WriterProvider(config);
var model = provider.PalmyraX_004();

// Professional and focused
var options = new LanguageModelCallOptions
{
    Temperature = 0.5,  // More deterministic
    MaxTokens = 2000
};
```

### For Long Documents
```csharp
var provider = new WriterProvider(config);
var model = provider.PalmyraX_004();

// Utilize long context window
var options = new LanguageModelCallOptions
{
    Messages = extensiveContext,  // Up to 128K tokens
    Temperature = 0.6,
    MaxTokens = 4000
};
```

### For Cost-Effective Applications
```csharp
var provider = new WriterProvider(config);
var model = provider.Palmyra2();

// Efficient and economical
var options = new LanguageModelCallOptions
{
    Temperature = 0.7,
    MaxTokens = 1500
};
```

## Getting an API Key

1. Visit [Writer Platform](https://writer.com)
2. Sign up for an account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the key for use in your application

## Links

- [Writer Platform](https://writer.com)
- [Writer Documentation](https://dev.writer.com/docs)
- [Writer API Reference](https://dev.writer.com/api-reference)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
