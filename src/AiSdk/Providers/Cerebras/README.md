# Cerebras Provider for AI SDK .NET

Ultra-fast AI inference powered by the world's largest AI processor.

## Overview

Cerebras provides blazing-fast inference for large language models, leveraging the Cerebras CS-3 system - the world's largest and fastest AI processor. This provider enables you to run Llama models with unprecedented speed and efficiency.

## Key Features

- **Ultra-Fast Inference**: Industry-leading inference speeds powered by Cerebras wafer-scale architecture
- **OpenAI-Compatible API**: Familiar API structure for easy integration
- **Optimized Llama Models**: Access to Llama 3.1 and Llama 3.3 models optimized for speed
- **Streaming Support**: Real-time streaming responses for interactive applications
- **Tool Calling**: Full support for function calling and tool integration

## Installation

```bash
dotnet add package AiSdk
```

## Quick Start

### Basic Usage

```csharp
using AiSdk.Providers.Cerebras;
using AiSdk.Abstractions;

// Option 1: Using static factory method
var model = CerebrasProvider.Llama33_70B("your-api-key");

// Option 2: Using configuration
var config = new CerebrasConfiguration
{
    ApiKey = "your-api-key"
};

var provider = new CerebrasProvider(config);
var model = provider.Llama33_70B();
```

### Generate Text

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Explain quantum computing in simple terms."
        }
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Streaming Responses

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Write a story about space exploration."
        }
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

## Available Models

The Cerebras provider offers several high-performance Llama models:

### Llama 3.3 70B
```csharp
var model = provider.Llama33_70B();
```
- Latest Llama model with excellent capabilities
- Optimized for ultra-fast inference
- Best for complex reasoning and generation tasks

### Llama 3.1 70B
```csharp
var model = provider.Llama31_70B();
```
- Powerful 70B parameter model
- Great for complex tasks requiring deep understanding
- Excellent balance of speed and capability

### Llama 3.1 8B
```csharp
var model = provider.Llama31_8B();
```
- Efficient 8B parameter model
- Fastest option for general-purpose tasks
- Ideal for high-throughput applications

### Custom Model
```csharp
var model = provider.ChatModel("model-name");
```
- Use any Cerebras model by ID
- Flexibility for new model releases

## Advanced Configuration

### Custom Base URL and Timeout

```csharp
var config = new CerebrasConfiguration
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.cerebras.ai/v1",
    TimeoutSeconds = 60
};

var provider = new CerebrasProvider(config);
```

### Using Custom HttpClient

```csharp
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");

var provider = new CerebrasProvider(config, httpClient);
```

### Model Parameters

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 2000,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new[] { "\n\n" }
};
```

## Tool Calling

Cerebras supports function calling for building AI agents:

```csharp
var tools = new List<Tool>
{
    new Tool
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = JsonDocument.Parse("""
        {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "City name"
                }
            },
            "required": ["location"]
        }
        """)
    }
};

var options = new LanguageModelCallOptions
{
    Messages = messages,
    Tools = tools
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    // Handle tool calls
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments}");
    }
}
```

## Error Handling

```csharp
using AiSdk.Providers.Cerebras.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (CerebrasException ex)
{
    Console.WriteLine($"Cerebras API error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
```

## Performance Tips

1. **Use Streaming**: For interactive applications, streaming provides the fastest time-to-first-token
2. **Choose the Right Model**: Use Llama 3.1 8B for high-throughput, Llama 3.3 70B for complex reasoning
3. **Optimize Prompts**: Cerebras's speed allows for more extensive prompt engineering
4. **Batch Requests**: When processing multiple independent requests, leverage the speed advantage

## Why Cerebras?

- **World's Largest AI Processor**: 850,000 cores on a single wafer-scale chip
- **Unmatched Speed**: Up to 100x faster inference than traditional GPUs
- **Linear Scaling**: Predictable performance as you scale
- **Cost Effective**: Pay for the speed you need with competitive pricing
- **Production Ready**: Battle-tested infrastructure used by Fortune 500 companies

## API Reference

For detailed API documentation, visit:
- [Cerebras Inference Docs](https://inference-docs.cerebras.ai/)
- [Cerebras API Reference](https://inference-docs.cerebras.ai/api-reference)

## Getting an API Key

1. Visit [Cerebras Cloud](https://cloud.cerebras.ai/)
2. Sign up for an account
3. Navigate to API settings
4. Generate a new API key

## Support

For issues and questions:
- [Cerebras Documentation](https://inference-docs.cerebras.ai/)
- [AI SDK .NET GitHub Issues](https://github.com/yourusername/ai-sdk.net/issues)

## License

This provider is part of the AI SDK .NET project.
