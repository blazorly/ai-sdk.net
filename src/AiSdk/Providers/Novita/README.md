# AiSdk.Providers.Novita

Novita AI provider for AI SDK .NET - integrates Novita's powerful open-source models including Llama 3, Mistral, and Qwen with the unified AI SDK interface.

## Features

- **Multiple Open-Source Models** - Llama 3 (8B/70B), Mistral 7B, Qwen 2 72B
- **OpenAI-Compatible API** - Familiar API format for easy integration
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **Flexible Configuration** - API key, base URL, timeouts
- **Cost-Effective** - Access to powerful open-source models

## Installation

```bash
dotnet add package AiSdk
```

The Novita provider is included in the consolidated AiSdk package.

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Novita;

// Create a provider instance
var config = new NovitaConfiguration
{
    ApiKey = "your-api-key"
};
var provider = new NovitaProvider(config);

// Use the Llama 3 8B model
var model = provider.Llama3_8B();

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
var config = new NovitaConfiguration
{
    ApiKey = "your-api-key",                               // Required
    BaseUrl = "https://api.novita.ai/v3/openai",           // Optional (default shown)
    TimeoutSeconds = 60                                     // Optional (default: 100)
};

var provider = new NovitaProvider(config);
```

### Environment Variables

```csharp
var config = new NovitaConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("NOVITA_API_KEY")!
};
```

### Available Models

```csharp
// Using provider instance methods
var provider = new NovitaProvider(config);

var llama8B = provider.Llama3_8B();      // Meta Llama 3 8B - Fast and efficient
var llama70B = provider.Llama3_70B();    // Meta Llama 3 70B - Powerful and accurate
var mistral = provider.Mistral7B();      // Mistral 7B Instruct - Efficient chat
var qwen = provider.Qwen2_72B();         // Qwen 2 72B - Advanced bilingual

// Or use specific model ID
var model = provider.ChatModel("meta-llama/llama-3-8b-instruct");

// Static convenience methods for quick initialization
var llama8BModel = NovitaProvider.Llama3_8B("your-api-key");
var llama70BModel = NovitaProvider.Llama3_70B("your-api-key");
var mistralModel = NovitaProvider.Mistral7B("your-api-key");
var qwenModel = NovitaProvider.Qwen2_72B("your-api-key");
```

## Usage Examples

### General Chat with Llama 3

```csharp
var provider = new NovitaProvider(config);
var model = provider.Llama3_8B();

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

### Advanced Reasoning with Llama 3 70B

```csharp
var provider = new NovitaProvider(config);
var model = provider.Llama3_70B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Solve this complex logic puzzle: If all A are B, and some B are C, what can we conclude about A and C?")
    },
    Temperature = 0.7
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Efficient Chat with Mistral 7B

```csharp
var provider = new NovitaProvider(config);
var model = provider.Mistral7B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What are the benefits of renewable energy?")
    },
    Temperature = 0.8
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Bilingual Tasks with Qwen 2 72B

```csharp
var provider = new NovitaProvider(config);
var model = provider.Qwen2_72B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Translate 'artificial intelligence' to Chinese and explain its significance")
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
    new Message(MessageRole.System, "You are a helpful assistant."),
    new Message(MessageRole.User, "What is machine learning?"),
    new Message(MessageRole.Assistant, "Machine learning is a subset of AI..."),
    new Message(MessageRole.User, "Can you give me an example?")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Novita.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (NovitaException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (NovitaException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (NovitaException ex)
{
    Console.WriteLine($"Novita error: {ex.Message} (Status: {ex.StatusCode})");
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
      "novita": {
        "ApiKey": "your-api-key",
        "DefaultModel": "meta-llama/llama-3-8b-instruct",
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
    options.DefaultProvider = "novita";
    options.Providers.Add("novita", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:novita:ApiKey"]!,
        DefaultModel = "meta-llama/llama-3-8b-instruct"
    });
});
```

## Model Comparison

| Model | Model ID | Parameters | Strengths | Use Case |
|-------|----------|------------|-----------|----------|
| Llama 3 8B | `meta-llama/llama-3-8b-instruct` | 8B | Fast, efficient, good balance | General chat, quick tasks |
| Llama 3 70B | `meta-llama/llama-3-70b-instruct` | 70B | Powerful, accurate, advanced reasoning | Complex tasks, detailed analysis |
| Mistral 7B | `mistralai/mistral-7b-instruct-v0.3` | 7B | Efficient, instruction-following | Chat, task completion |
| Qwen 2 72B | `qwen/qwen-2-72b-instruct` | 72B | Bilingual (EN/CN), strong reasoning | Multilingual, advanced tasks |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Multiple Models | ✅ Full support |
| OpenAI Compatibility | ✅ Full support |
| Vision | ❌ Not supported |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use **Llama 3 8B** for fast, general-purpose tasks with good quality
   - Use **Llama 3 70B** for complex reasoning and highest quality outputs
   - Use **Mistral 7B** for efficient instruction-following and chat
   - Use **Qwen 2 72B** for bilingual tasks and advanced reasoning

2. **Optimize for Speed**:
   ```csharp
   var config = new NovitaConfiguration
   {
       ApiKey = "your-api-key",
       TimeoutSeconds = 120  // Longer timeout for complex tasks
   };

   var options = new LanguageModelCallOptions
   {
       MaxTokens = 1000,      // Limit tokens for faster response
       Temperature = 0.5      // Lower temperature for consistency
   };
   ```

3. **Streaming for Long Responses**: Use streaming for detailed explanations to improve perceived performance
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

4. **Model Selection**:
   - Start with 8B models for development and testing
   - Scale up to 70B models for production when quality is critical
   - Use smaller models (7B/8B) for cost optimization

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
| 500 | `server_error` | Novita server error |

## Why Novita AI?

Novita AI offers access to powerful open-source models with competitive advantages:

- **Open-Source Models**: Access to Llama 3, Mistral, Qwen, and more
- **OpenAI-Compatible**: Easy migration from OpenAI with familiar API
- **Cost-Effective**: Competitive pricing for high-quality models
- **Multiple Model Sizes**: Choose from 7B to 72B parameter models
- **Fast Inference**: Optimized infrastructure for quick responses
- **Flexible Deployment**: On-demand access without model hosting

Perfect for:
- Applications requiring cost-effective AI integration
- Projects leveraging open-source models
- Multilingual applications (with Qwen models)
- Development and prototyping with various model sizes
- Migration from proprietary to open-source models
- Applications requiring OpenAI compatibility

## Use Case Recommendations

### For Fast General Tasks (Llama 3 8B)
```csharp
var provider = new NovitaProvider(config);
var model = provider.Llama3_8B();

// Optimized for speed and efficiency
var options = new LanguageModelCallOptions
{
    Temperature = 0.7,
    MaxTokens = 1000
};
```

### For Complex Reasoning (Llama 3 70B)
```csharp
var provider = new NovitaProvider(config);
var model = provider.Llama3_70B();

// Allow more thinking space
var options = new LanguageModelCallOptions
{
    Temperature = 0.6,
    MaxTokens = 2000
};
```

### For Efficient Chat (Mistral 7B)
```csharp
var provider = new NovitaProvider(config);
var model = provider.Mistral7B();

// Balanced settings
var options = new LanguageModelCallOptions
{
    Temperature = 0.8,
    MaxTokens = 1500
};
```

### For Bilingual Tasks (Qwen 2 72B)
```csharp
var provider = new NovitaProvider(config);
var model = provider.Qwen2_72B();

// Optimized for multilingual
var options = new LanguageModelCallOptions
{
    Temperature = 0.7,
    MaxTokens = 2000
};
```

## Getting an API Key

1. Visit [Novita AI Platform](https://novita.ai)
2. Sign up for an account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the key for use in your application

## Links

- [Novita AI Platform](https://novita.ai)
- [Novita AI Documentation](https://novita.ai/docs)
- [Novita AI API Reference](https://novita.ai/api-reference)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
