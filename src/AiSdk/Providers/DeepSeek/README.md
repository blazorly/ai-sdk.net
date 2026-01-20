# AiSdk.Providers.DeepSeek

DeepSeek provider for AI SDK .NET - integrates DeepSeek's powerful chat, code, and reasoning models with the unified AI SDK interface.

## Features

- **Multiple Specialized Models** - Chat, Coder, and Reasoner (R1) models
- **Advanced Reasoning** - DeepSeek-R1 with chain-of-thought capabilities
- **Code Generation** - Specialized DeepSeek-Coder for programming tasks
- **Chat Completions** - General-purpose DeepSeek-Chat model
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **OpenAI-Compatible API** - Familiar API format for easy migration
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk
```

The DeepSeek provider is included in the consolidated AiSdk package.

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.DeepSeek;

// Create a provider instance
var config = new DeepSeekConfiguration
{
    ApiKey = "your-api-key"
};
var provider = new DeepSeekProvider(config);

// Use the Chat model
var model = provider.Chat();

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
var config = new DeepSeekConfiguration
{
    ApiKey = "sk-...",                                   // Required
    BaseUrl = "https://api.deepseek.com",                // Optional (default shown)
    TimeoutSeconds = 60                                   // Optional (default: 100)
};

var provider = new DeepSeekProvider(config);
```

### Environment Variables

```csharp
var config = new DeepSeekConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY")!
};
```

### Available Models

```csharp
// Using provider instance methods
var provider = new DeepSeekProvider(config);

var chat = provider.Chat();          // deepseek-chat - General purpose
var coder = provider.Coder();        // deepseek-coder - Code-focused
var reasoner = provider.Reasoner();  // deepseek-reasoner - Advanced reasoning (R1)

// Or use specific model ID
var model = provider.ChatModel("deepseek-chat");

// Static convenience methods for quick initialization
var chatModel = DeepSeekProvider.Chat("your-api-key");
var coderModel = DeepSeekProvider.Coder("your-api-key");
var reasonerModel = DeepSeekProvider.Reasoner("your-api-key");
```

## Usage Examples

### General Chat

```csharp
var provider = new DeepSeekProvider(config);
var model = provider.Chat();

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

### Code Generation

```csharp
var provider = new DeepSeekProvider(config);
var model = provider.Coder();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a Python function to calculate Fibonacci numbers using dynamic programming")
    },
    Temperature = 0.3  // Lower temperature for more deterministic code
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Advanced Reasoning (DeepSeek-R1)

```csharp
var provider = new DeepSeekProvider(config);
var model = provider.Reasoner();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Solve this complex logic puzzle: If all A are B, and some B are C, what can we conclude about A and C?")
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
        new Message(MessageRole.User, "Write a detailed explanation of neural networks")
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
    new Message(MessageRole.System, "You are a helpful coding assistant."),
    new Message(MessageRole.User, "How do I implement a binary search in C#?"),
    new Message(MessageRole.Assistant, "Here's a binary search implementation in C#..."),
    new Message(MessageRole.User, "Can you add error handling to it?")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.DeepSeek.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (DeepSeekException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (DeepSeekException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (DeepSeekException ex)
{
    Console.WriteLine($"DeepSeek error: {ex.Message} (Status: {ex.StatusCode})");
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
      "deepseek": {
        "ApiKey": "sk-...",
        "DefaultModel": "deepseek-chat",
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
    options.DefaultProvider = "deepseek";
    options.Providers.Add("deepseek", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:deepseek:ApiKey"]!,
        DefaultModel = "deepseek-chat"
    });
});
```

## Model Comparison

| Model | Model ID | Context | Strengths | Use Case |
|-------|----------|---------|-----------|----------|
| DeepSeek Chat | `deepseek-chat` | 32K+ | General conversations, writing, analysis | Versatile general-purpose tasks |
| DeepSeek Coder | `deepseek-coder` | 32K+ | Code generation, debugging, technical docs | Programming and software development |
| DeepSeek Reasoner | `deepseek-reasoner` | 32K+ | Chain-of-thought reasoning, complex logic | Complex problem-solving, math, puzzles |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Code Generation | ✅ Full support (via Coder model) |
| Advanced Reasoning | ✅ Full support (via Reasoner model) |
| Vision | ❌ Not supported by DeepSeek |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use **DeepSeek Chat** for general conversations and content generation
   - Use **DeepSeek Coder** for programming tasks, code review, and debugging
   - Use **DeepSeek Reasoner (R1)** for complex reasoning, mathematics, and logic problems

2. **Optimize for Code Generation**:
   ```csharp
   var config = new DeepSeekConfiguration
   {
       ApiKey = "sk-...",
       TimeoutSeconds = 120  // Longer timeout for complex code generation
   };

   var options = new LanguageModelCallOptions
   {
       Temperature = 0.3,  // Lower temperature for more deterministic output
       MaxTokens = 4000    // Allow more tokens for detailed code
   };
   ```

3. **Streaming for Long Responses**: Use streaming for detailed explanations to improve perceived performance
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

4. **Reasoning Model Best Practices**:
   - Allow more tokens for chain-of-thought reasoning
   - Use clear, specific prompts for complex problems
   - Consider streaming to see reasoning process unfold

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
| 500 | `server_error` | DeepSeek server error |

## Why DeepSeek?

DeepSeek models offer exceptional capabilities:

- **Cost-Effective**: Competitive pricing for high-performance models
- **Code Expertise**: DeepSeek-Coder rivals specialized coding models
- **Advanced Reasoning**: DeepSeek-R1 provides chain-of-thought reasoning
- **Long Context**: Support for 32K+ tokens across all models
- **Open Research**: Built on cutting-edge AI research
- **Versatile**: Three specialized models for different use cases

Perfect for:
- Software development and code generation
- Complex problem-solving and reasoning tasks
- Educational applications requiring detailed explanations
- Content creation and analysis
- Applications requiring long-context understanding
- Cost-conscious deployments needing high-quality output

## Use Case Recommendations

### For Code Generation
```csharp
var provider = new DeepSeekProvider(config);
var model = provider.Coder();

// Optimized for code
var options = new LanguageModelCallOptions
{
    Temperature = 0.2,  // Deterministic
    MaxTokens = 4000
};
```

### For Complex Reasoning
```csharp
var provider = new DeepSeekProvider(config);
var model = provider.Reasoner();

// Allow thinking space
var options = new LanguageModelCallOptions
{
    Temperature = 0.5,
    MaxTokens = 8000  // More tokens for reasoning chain
};
```

### For General Chat
```csharp
var provider = new DeepSeekProvider(config);
var model = provider.Chat();

// Balanced settings
var options = new LanguageModelCallOptions
{
    Temperature = 0.7,
    MaxTokens = 2000
};
```

## Getting an API Key

1. Visit [DeepSeek Platform](https://platform.deepseek.com)
2. Sign up for an account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the key (starts with `sk-`)

## Links

- [DeepSeek Platform](https://platform.deepseek.com)
- [DeepSeek Documentation](https://platform.deepseek.com/docs)
- [DeepSeek API Reference](https://platform.deepseek.com/api-docs)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
