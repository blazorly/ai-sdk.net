# Together AI Provider

The Together AI provider for AI SDK .NET allows you to interact with Together AI's extensive catalog of 200+ open-source AI models through an OpenAI-compatible API.

## Features

- OpenAI-compatible API interface
- Access to 200+ open-source models
- Support for streaming and non-streaming responses
- Tool calling support
- Multiple model families: Llama, Qwen, Mixtral, DeepSeek, and more
- Vision model support
- High-performance inference

## Installation

```bash
dotnet add package AiSdk
```

## Configuration

### Using Configuration Object

```csharp
using AiSdk.Providers.TogetherAI;

var config = new TogetherAIConfiguration
{
    ApiKey = "your-together-ai-api-key",
    BaseUrl = "https://api.together.xyz/v1", // Optional, this is the default
    TimeoutSeconds = 120 // Optional
};

var provider = new TogetherAIProvider(config);
```

### Using Environment Variables

```csharp
var config = new TogetherAIConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("TOGETHER_API_KEY")!
};

var provider = new TogetherAIProvider(config);
```

## Quick Start

### Basic Chat Completion

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.TogetherAI;

// Create provider with configuration
var config = new TogetherAIConfiguration
{
    ApiKey = "your-together-ai-api-key"
};
var provider = new TogetherAIProvider(config);

// Get a model
var model = provider.Llama3_1_8B();

// Create a chat message
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Explain what Together AI is in one sentence."
        }
    }
};

// Generate response
var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Using Static Factory Methods

```csharp
using AiSdk.Providers.TogetherAI;

// Quick initialization without creating a provider instance
var model = TogetherAIProvider.Llama3_1_70B("your-together-ai-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Write a haiku about artificial intelligence."
        }
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Streaming Responses

```csharp
var model = provider.Llama3_1_8B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Write a short story about a robot learning to paint."
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

### Using Custom Model IDs

```csharp
// Use any Together AI model by ID
var model = provider.ChatModel("meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo");

// Or using static method
var customModel = TogetherAIProvider.ChatModel(
    "deepseek-ai/DeepSeek-V3",
    "your-together-ai-api-key"
);
```

## Available Models

### Llama Models

```csharp
// Llama 3.1 8B - Fast and efficient
var llama8b = provider.Llama3_1_8B();

// Llama 3.1 70B - Powerful for complex tasks
var llama70b = provider.Llama3_1_70B();

// Llama 3.1 405B - Largest, exceptional performance
var llama405b = provider.Llama3_1_405B();

// Llama 3.3 70B - Latest generation
var llama33 = provider.Llama3_3_70B();
```

### Qwen Models

```csharp
// Qwen 2.5 7B - Fast bilingual (Chinese/English)
var qwen7b = provider.Qwen2_5_7B();

// Qwen 2.5 72B - High-performance bilingual
var qwen72b = provider.Qwen2_5_72B();

// QwQ 32B - Advanced reasoning with chain-of-thought
var qwq = provider.QwQ32B();
```

### Mixtral Models

```csharp
// Mixtral 8x7B - Efficient mixture-of-experts
var mixtral8x7 = provider.Mixtral8x7B();

// Mixtral 8x22B - Large mixture-of-experts
var mixtral8x22 = provider.Mixtral8x22B();
```

### DeepSeek Models

```csharp
// DeepSeek V3 - Strong coding and reasoning
var deepseekV3 = provider.DeepSeekV3();

// DeepSeek R1 - Reasoning-focused
var deepseekR1 = provider.DeepSeekR1();
```

### Other Models

```csharp
// Nous Hermes 2 - Optimized for instruction following
var nousHermes = provider.NousHermes2Mixtral();
```

## Advanced Usage

### Tool Calling

```csharp
var model = provider.Llama3_1_70B();

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
                    "description": "The city and state, e.g. San Francisco, CA"
                }
            },
            "required": ["location"]
        }
        """)
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What's the weather like in New York?"
        }
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
    }
}
```

### Temperature and Sampling Control

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Generate a creative story."
        }
    },
    Temperature = 0.8,  // Higher for more creativity (0.0 - 2.0)
    TopP = 0.9,         // Nucleus sampling
    MaxTokens = 1000    // Limit response length
};

var result = await model.GenerateAsync(options);
```

### Multi-turn Conversations

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
        Content = "How do I sort a list in Python?"
    },
    new Message
    {
        Role = MessageRole.Assistant,
        Content = "You can use the sorted() function or the .sort() method."
    },
    new Message
    {
        Role = MessageRole.User,
        Content = "What's the difference between them?"
    }
};

var options = new LanguageModelCallOptions
{
    Messages = messages
};

var result = await model.GenerateAsync(options);
```

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};

var config = new TogetherAIConfiguration
{
    ApiKey = "your-together-ai-api-key"
};

var provider = new TogetherAIProvider(config, httpClient);
```

## Error Handling

```csharp
using AiSdk.Providers.TogetherAI.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
    Console.WriteLine(result.Text);
}
catch (TogetherAIException ex)
{
    Console.WriteLine($"Together AI Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
```

### Checking for Specific Errors

```csharp
try
{
    var result = await model.GenerateAsync(options);
}
catch (Exception ex)
{
    if (TogetherAIException.IsInstance(ex))
    {
        var togetherError = (TogetherAIException)ex;
        Console.WriteLine($"Together AI Error: {togetherError.Message}");
    }
    else
    {
        throw;
    }
}
```

## Model Selection Guide

### For Speed and Cost-Efficiency
- **Llama 3.1 8B** - Best for simple tasks, fast responses
- **Qwen 2.5 7B** - Good for bilingual applications
- **Mixtral 8x7B** - Efficient mixture-of-experts

### For General Purpose
- **Llama 3.1 70B** - Great balance of performance and speed
- **Llama 3.3 70B** - Latest generation improvements
- **Qwen 2.5 72B** - Excellent for Chinese/English tasks

### For Complex Reasoning
- **Llama 3.1 405B** - Top performance on difficult tasks
- **DeepSeek V3** - Strong coding and technical reasoning
- **DeepSeek R1** - Chain-of-thought reasoning
- **QwQ 32B** - Advanced reasoning capabilities

### For Code Generation
- **DeepSeek V3** - Specialized for coding tasks
- **Llama 3.1 70B** - Excellent general coding support

## API Reference

### TogetherAIConfiguration

```csharp
public record TogetherAIConfiguration
{
    public required string ApiKey { get; init; }
    public string BaseUrl { get; init; } = "https://api.together.xyz/v1";
    public int? TimeoutSeconds { get; init; }
}
```

### TogetherAIProvider Methods

| Method | Description | Model ID |
|--------|-------------|----------|
| `Llama3_1_8B()` | Llama 3.1 8B Instruct Turbo | meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo |
| `Llama3_1_70B()` | Llama 3.1 70B Instruct Turbo | meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo |
| `Llama3_1_405B()` | Llama 3.1 405B Instruct Turbo | meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo |
| `Llama3_3_70B()` | Llama 3.3 70B Instruct Turbo | meta-llama/Llama-3.3-70B-Instruct-Turbo |
| `Qwen2_5_7B()` | Qwen 2.5 7B Instruct Turbo | Qwen/Qwen2.5-7B-Instruct-Turbo |
| `Qwen2_5_72B()` | Qwen 2.5 72B Instruct Turbo | Qwen/Qwen2.5-72B-Instruct-Turbo |
| `QwQ32B()` | QwQ 32B Preview | Qwen/QwQ-32B-Preview |
| `Mixtral8x7B()` | Mixtral 8x7B Instruct | mistralai/Mixtral-8x7B-Instruct-v0.1 |
| `Mixtral8x22B()` | Mixtral 8x22B Instruct | mistralai/Mixtral-8x22B-Instruct-v0.1 |
| `DeepSeekV3()` | DeepSeek V3 | deepseek-ai/DeepSeek-V3 |
| `DeepSeekR1()` | DeepSeek R1 | deepseek-ai/DeepSeek-R1 |
| `NousHermes2Mixtral()` | Nous Hermes 2 Mixtral | NousResearch/Nous-Hermes-2-Mixtral-8x7B-DPO |
| `ChatModel(string)` | Custom model by ID | User-specified |

## Resources

- [Together AI Documentation](https://docs.together.ai/)
- [OpenAI Compatibility Guide](https://docs.together.ai/docs/openai-api-compatibility)
- [Together AI Models](https://www.together.ai/inference)
- [API Reference](https://api.together.xyz/)

## Support

For issues and questions:
- Together AI Support: [Together AI Documentation](https://docs.together.ai/)
- AI SDK .NET Issues: [GitHub Issues](https://github.com/your-repo/issues)

## License

This provider is part of the AI SDK .NET project and follows the same license terms.
