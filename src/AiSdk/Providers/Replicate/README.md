# Replicate Provider for AI SDK .NET

The Replicate provider enables you to use Replicate's AI models through the AI SDK .NET framework. Replicate provides access to various open-source language models including Llama 2, Mistral, and CodeLlama.

## Features

- Full ILanguageModel implementation
- Support for popular open-source models (Llama 2, Mistral, Mixtral)
- Streaming and non-streaming text generation
- Configurable timeout enforcement
- Comprehensive error handling with ReplicateException
- XML documentation for all public APIs

## Installation

The Replicate provider is included in the consolidated AiSdk package:

```bash
dotnet add package AiSdk
```

## Quick Start

### Basic Usage

```csharp
using AiSdk.Providers.Replicate;
using AiSdk.Abstractions;

// Create a Llama 2 70B model
var model = ReplicateProvider.Llama2_70B("your-replicate-api-key");

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What is the capital of France?"
        }
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Using Configuration

```csharp
using AiSdk.Providers.Replicate;

var config = new ReplicateConfiguration
{
    ApiKey = "your-replicate-api-key",
    BaseUrl = "https://api.replicate.com/v1", // Optional, this is the default
    TimeoutSeconds = 120 // Optional timeout in seconds
};

var model = new ReplicateChatLanguageModel("meta/llama-2-70b-chat", config);
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
            Content = "Write a short story about a robot."
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

### Factory Methods

The `ReplicateProvider` class provides convenient factory methods for popular models:

```csharp
// Llama 2 70B Chat - Meta's largest Llama 2 chat model
var llama70b = ReplicateProvider.Llama2_70B(apiKey);

// Llama 2 13B Chat - Balanced performance and speed
var llama13b = ReplicateProvider.Llama2_13B(apiKey);

// Mixtral 8x7B Instruct - Mistral's mixture of experts model
var mixtral = ReplicateProvider.Mixtral8x7B(apiKey);

// Mistral 7B Instruct - Efficient 7B parameter model
var mistral = ReplicateProvider.Mistral7B(apiKey);

// Generic model creation
var customModel = ReplicateProvider.ChatModel("owner/model-name", apiKey);
```

### Popular Model Versions

- **meta/llama-2-70b-chat** - Llama 2 70B optimized for chat
- **meta/llama-2-13b-chat** - Llama 2 13B optimized for chat
- **mistralai/mixtral-8x7b-instruct-v0.1** - Mixtral 8x7B Instruct
- **mistralai/mistral-7b-instruct-v0.2** - Mistral 7B Instruct v0.2

## Configuration Options

### ReplicateConfiguration

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| ApiKey | string | Yes | - | Your Replicate API token |
| BaseUrl | string | No | https://api.replicate.com/v1 | The Replicate API base URL |
| TimeoutSeconds | int? | No | null | Request timeout in seconds (uses HttpClient default if not set) |

## Call Options

The `LanguageModelCallOptions` supports the following parameters:

| Parameter | Type | Description |
|-----------|------|-------------|
| Messages | List&lt;Message&gt; | The conversation messages |
| MaxTokens | int? | Maximum number of tokens to generate |
| Temperature | double? | Sampling temperature (0.0 to 2.0) |
| TopP | double? | Nucleus sampling parameter |
| StopSequences | List&lt;string&gt;? | Sequences that stop generation |

### Example with Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.System,
            Content = "You are a helpful assistant."
        },
        new Message
        {
            Role = MessageRole.User,
            Content = "Explain quantum computing."
        }
    },
    MaxTokens = 500,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\n" }
};

var result = await model.GenerateAsync(options);
```

## Error Handling

The provider uses `ReplicateException` for all API errors:

```csharp
using AiSdk.Providers.Replicate.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (ReplicateException ex)
{
    Console.WriteLine($"Replicate error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
```

### Error Types

- **Authentication errors** - Invalid API key (401)
- **Rate limit errors** - Too many requests (429)
- **Model errors** - Invalid model version or prediction failure
- **Timeout errors** - Prediction took too long to complete

## How It Works

### Non-Streaming Mode

1. Creates a prediction request with your input
2. Polls the prediction endpoint until completion
3. Returns the complete result with finish reason and usage

### Streaming Mode

1. Creates a prediction with streaming enabled
2. Opens a Server-Sent Events (SSE) connection
3. Streams output chunks as they're generated
4. Closes when the model signals completion

## Authentication

Get your Replicate API token from [replicate.com/account](https://replicate.com/account).

You can provide the API key in three ways:

1. **Direct configuration**:
```csharp
var config = new ReplicateConfiguration { ApiKey = "r8_..." };
```

2. **Environment variable**:
```csharp
var apiKey = Environment.GetEnvironmentVariable("REPLICATE_API_TOKEN");
var model = ReplicateProvider.Llama2_70B(apiKey);
```

3. **Configuration file** (recommended for production):
```csharp
var configuration = builder.Configuration;
var apiKey = configuration["Replicate:ApiKey"];
```

## Advanced Usage

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};

var model = ReplicateProvider.CreateChatModel(
    "meta/llama-2-70b-chat",
    apiKey,
    httpClient: httpClient
);
```

### Custom Base URL

For using Replicate proxies or custom endpoints:

```csharp
var config = new ReplicateConfiguration
{
    ApiKey = apiKey,
    BaseUrl = "https://your-proxy.com/v1"
};

var model = new ReplicateChatLanguageModel("meta/llama-2-70b-chat", config);
```

## Limitations

- **Token Usage**: Replicate doesn't provide detailed token usage metrics in the standard API response
- **Tool Calling**: Not currently supported by most Replicate models
- **URL Content**: Image URLs are not supported in the same way as providers like OpenAI
- **Polling**: Non-streaming requests use polling, which may add latency

## Best Practices

1. **Use streaming for long responses** to provide better user experience
2. **Set appropriate timeouts** based on your model and expected response length
3. **Handle rate limits** with exponential backoff and retry logic
4. **Cache API keys** securely, never hardcode them
5. **Choose the right model** - larger models are more capable but slower and more expensive

## Examples

### Multi-turn Conversation

```csharp
var messages = new List<Message>
{
    new Message { Role = MessageRole.System, Content = "You are a helpful coding assistant." },
    new Message { Role = MessageRole.User, Content = "How do I sort a list in Python?" },
    new Message { Role = MessageRole.Assistant, Content = "You can use the sorted() function..." },
    new Message { Role = MessageRole.User, Content = "What about in reverse order?" }
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Temperature Control

```csharp
// More creative (higher temperature)
var creativeOptions = new LanguageModelCallOptions
{
    Messages = messages,
    Temperature = 1.5
};

// More focused (lower temperature)
var focusedOptions = new LanguageModelCallOptions
{
    Messages = messages,
    Temperature = 0.3
};
```

## Resources

- [Replicate Documentation](https://replicate.com/docs)
- [Replicate Models](https://replicate.com/explore)
- [AI SDK .NET Documentation](https://github.com/yourusername/ai-sdk.net)
- [Get API Token](https://replicate.com/account)

## Support

For issues specific to the Replicate provider:
1. Check the [Replicate API status](https://status.replicate.com/)
2. Verify your API token is valid
3. Review the error message and status code
4. Check the model version exists on Replicate

## License

This provider is part of the AI SDK .NET project and follows the same license.
