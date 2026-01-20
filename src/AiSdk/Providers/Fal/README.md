# Fal AI Provider for AI SDK .NET

Fast inference for image generation, video generation, and LLMs.

## Overview

Fal AI is a generative media platform that provides lightning-fast inference capabilities for AI models. While primarily known for its exceptional image and video generation capabilities (Stable Diffusion, FLUX, etc.), Fal AI also offers access to popular language models through a unified API.

## Key Features

- **Fast Inference**: Up to 4x faster than traditional alternatives
- **Unified API**: Access multiple LLM providers through a single interface
- **Multiple Model Support**: Claude, GPT-4, Gemini, Llama, and more
- **Streaming Support**: Real-time streaming responses for interactive applications
- **Tool Calling**: Full support for function calling and tool integration
- **Future Expansion**: Foundation for image and video generation features

## Installation

```bash
dotnet add package AiSdk
```

## Quick Start

### Basic Usage

```csharp
using AiSdk.Providers.Fal;
using AiSdk.Abstractions;

// Option 1: Using static factory method
var model = FalProvider.Claude35Sonnet("your-api-key");

// Option 2: Using configuration
var config = new FalConfiguration
{
    ApiKey = "your-api-key"
};

var provider = new FalProvider(config);
var model = provider.Claude35Sonnet();
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

Fal AI provides access to various language models from different providers:

### Claude 3.5 Sonnet (Anthropic)
```csharp
var model = provider.Claude35Sonnet();
```
- High-performance model with excellent reasoning capabilities
- Great for complex analysis and content generation
- Strong coding abilities

### GPT-4o (OpenAI)
```csharp
var model = provider.Gpt4o();
```
- OpenAI's flagship multimodal model
- Excellent general-purpose capabilities
- Strong performance across diverse tasks

### Gemini Flash 1.5 (Google)
```csharp
var model = provider.GeminiFlash15();
```
- Fast and efficient model from Google
- Good balance of speed and capability
- Cost-effective for high-volume applications

### Llama 3.2 3B Instruct (Meta)
```csharp
var model = provider.Llama323BInstruct();
```
- Efficient open-source model
- Good for general tasks with lower latency
- Cost-effective option

### Custom Model
```csharp
var model = provider.ChatModel("provider/model-name");
```
- Use any Fal AI supported model by ID
- Format: "provider/model-name"
- Examples: "anthropic/claude-3.5-sonnet", "openai/gpt-4o"

## Advanced Configuration

### Custom Base URL and Timeout

```csharp
var config = new FalConfiguration
{
    ApiKey = "your-api-key",
    BaseUrl = "https://fal.run",
    TimeoutSeconds = 60
};

var provider = new FalProvider(config);
```

### Using Custom HttpClient

```csharp
var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");

var provider = new FalProvider(config, httpClient);
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

Fal AI supports function calling for building AI agents:

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
using AiSdk.Providers.Fal.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (FalException ex)
{
    Console.WriteLine($"Fal AI API error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
```

## Use Cases

### Current: Language Model Inference
- Text generation and completion
- Chat applications
- Question answering
- Code generation
- Content creation

### Future: Image & Video Generation
Fal AI's primary strength lies in generative media:
- **Image Generation**: Stable Diffusion, FLUX, ControlNet
- **Video Generation**: Video synthesis and manipulation
- **Real-time Generation**: Fast inference for interactive applications
- **Custom Models**: Deploy and run your own models

This provider is designed to be extensible for these capabilities in future releases.

## Performance Tips

1. **Choose the Right Model**: Use lighter models (Gemini Flash, Llama 3.2) for high-throughput scenarios
2. **Use Streaming**: For interactive applications, streaming provides better user experience
3. **Leverage Fast Inference**: Fal AI's infrastructure is optimized for speed
4. **Batch Requests**: When processing multiple independent requests, leverage parallelism

## Why Fal AI?

- **Speed**: Up to 4x faster inference than alternatives
- **Unified Access**: Multiple model providers through a single API
- **Flexible Pricing**: Pay only for what you use
- **Production Ready**: Reliable infrastructure used by thousands of developers
- **Future Ready**: Foundation for image/video generation capabilities
- **Developer Friendly**: Simple API with comprehensive documentation

## API Reference

For detailed API documentation, visit:
- [Fal AI Documentation](https://docs.fal.ai/)
- [LLM Tutorial](https://docs.fal.ai/examples/model-apis/use-llms)
- [API Reference](https://fal.ai/models)

## Getting an API Key

1. Visit [Fal AI](https://fal.ai/)
2. Sign up for an account
3. Navigate to your dashboard
4. Generate a new API key
5. Set the `FAL_KEY` environment variable or use it directly in configuration

## Authentication

Fal AI uses API key authentication with the format `Key YOUR_API_KEY`. The provider automatically handles this:

```csharp
// Automatically adds "Key" prefix to your API key
var config = new FalConfiguration
{
    ApiKey = "your-api-key-here"  // Will be sent as "Key your-api-key-here"
};
```

## Roadmap

### Current Features
- Language model inference (text generation)
- Streaming support
- Tool calling
- Multiple model support

### Planned Features
- Image generation support (Stable Diffusion, FLUX)
- Video generation support
- Real-time generation endpoints
- Custom model deployment
- Advanced multimodal capabilities

## Examples

### Multi-turn Conversation

```csharp
var messages = new List<Message>
{
    new Message
    {
        Role = MessageRole.System,
        Content = "You are a helpful coding assistant."
    },
    new Message
    {
        Role = MessageRole.User,
        Content = "How do I reverse a string in C#?"
    }
};

var options = new LanguageModelCallOptions
{
    Messages = messages,
    Temperature = 0.7
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);

// Continue conversation
messages.Add(new Message
{
    Role = MessageRole.Assistant,
    Content = result.Text
});

messages.Add(new Message
{
    Role = MessageRole.User,
    Content = "Can you show me an example with LINQ?"
});

var result2 = await model.GenerateAsync(new LanguageModelCallOptions { Messages = messages });
Console.WriteLine(result2.Text);
```

### Comparing Multiple Models

```csharp
var config = new FalConfiguration { ApiKey = "your-api-key" };
var provider = new FalProvider(config);

var models = new[]
{
    ("Claude 3.5", provider.Claude35Sonnet()),
    ("GPT-4o", provider.Gpt4o()),
    ("Gemini Flash", provider.GeminiFlash15())
};

var prompt = "Explain recursion in 2 sentences.";
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message { Role = MessageRole.User, Content = prompt }
    }
};

foreach (var (name, model) in models)
{
    var result = await model.GenerateAsync(options);
    Console.WriteLine($"\n{name}:");
    Console.WriteLine(result.Text);
}
```

## Support

For issues and questions:
- [Fal AI Documentation](https://docs.fal.ai/)
- [Fal AI Discord](https://discord.gg/fal-ai)
- [AI SDK .NET GitHub Issues](https://github.com/yourusername/ai-sdk.net/issues)

## License

This provider is part of the AI SDK .NET project.
