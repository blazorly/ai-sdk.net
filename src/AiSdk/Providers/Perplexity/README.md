# Perplexity AI Provider

The Perplexity AI provider for AI SDK .NET enables integration with Perplexity's powerful language models, including their unique Sonar models with real-time web search capabilities.

## Features

- Full ILanguageModel implementation
- Streaming and non-streaming text generation
- Online search integration (Sonar models)
- Tool/function calling support
- Timeout enforcement
- Comprehensive error handling
- OpenAI-compatible API format

## Available Models

### Sonar Models (with Online Search)

Sonar models have real-time access to the internet and can provide grounded, up-to-date information with citations.

- **llama-3.1-sonar-small-128k-online** - Fast and efficient with 128K context
- **llama-3.1-sonar-large-128k-online** - Balanced performance with 128K context
- **llama-3.1-sonar-huge-128k-online** - Maximum performance with 128K context

### Instruction Models

Standard instruction-following models without web search.

- **llama-3.1-8b-instruct** - Fast 8B parameter model
- **llama-3.1-70b-instruct** - High-performance 70B parameter model

## Installation

The Perplexity provider is included in the AiSdk package.

```bash
dotnet add package AiSdk
```

## Configuration

### Basic Configuration

```csharp
using AiSdk.Providers.Perplexity;

var config = new PerplexityConfiguration
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.perplexity.ai", // Optional, this is the default
    TimeoutSeconds = 30 // Optional timeout
};
```

### Using Environment Variables

```csharp
var config = new PerplexityConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("PERPLEXITY_API_KEY")!
};
```

## Usage Examples

### Using Factory Methods

The easiest way to create Perplexity models:

```csharp
using AiSdk.Providers.Perplexity;
using AiSdk.Abstractions;

// Create a Sonar model with online search
var model = PerplexityProvider.SonarLargeOnline("your-api-key");

// Create an instruction model
var instructModel = PerplexityProvider.Llama31_70B("your-api-key");

// Create any model by ID
var customModel = PerplexityProvider.ChatModel("llama-3.1-sonar-huge-128k-online", "your-api-key");
```

### Non-Streaming Generation

```csharp
using AiSdk.Providers.Perplexity;
using AiSdk.Abstractions;

var model = PerplexityProvider.SonarLargeOnline("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What are the latest developments in AI safety?"
        }
    },
    MaxTokens = 1000,
    Temperature = 0.7
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
```

### Streaming Generation

```csharp
var model = PerplexityProvider.SonarSmallOnline("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Explain quantum computing in simple terms"
        }
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
        Console.WriteLine($"\n\nFinish reason: {chunk.FinishReason}");
        if (chunk.Usage != null)
        {
            Console.WriteLine($"Total tokens: {chunk.Usage.TotalTokens}");
        }
    }
}
```

### Multi-turn Conversation

```csharp
var model = PerplexityProvider.Llama31_70B("your-api-key");

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

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);

// Add assistant response to conversation
messages.Add(new Message
{
    Role = MessageRole.Assistant,
    Content = result.Text
});

// Continue conversation
messages.Add(new Message
{
    Role = MessageRole.User,
    Content = "Can you show me a LINQ version?"
});

var followUp = await model.GenerateAsync(new LanguageModelCallOptions { Messages = messages });
Console.WriteLine(followUp.Text);
```

### Using Custom Configuration

```csharp
var config = new PerplexityConfiguration
{
    ApiKey = "your-api-key",
    BaseUrl = "https://api.perplexity.ai",
    TimeoutSeconds = 60
};

var model = PerplexityProvider.CreateChatModel(
    "llama-3.1-sonar-large-128k-online",
    config
);

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message { Role = MessageRole.User, Content = "Hello!" }
    }
};

var result = await model.GenerateAsync(options);
```

### Tool/Function Calling

```csharp
var model = PerplexityProvider.Llama31_70B("your-api-key");

var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = JsonDocument.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""location"": {
                    ""type"": ""string"",
                    ""description"": ""The city and state, e.g. San Francisco, CA""
                }
            },
            ""required"": [""location""]
        }")
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What's the weather in Seattle?"
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

### Error Handling

```csharp
using AiSdk.Providers.Perplexity.Exceptions;

var model = PerplexityProvider.SonarLargeOnline("your-api-key");

try
{
    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message { Role = MessageRole.User, Content = "Hello!" }
        }
    };

    var result = await model.GenerateAsync(options);
}
catch (PerplexityException ex)
{
    Console.WriteLine($"Perplexity API error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (ApiCallError ex)
{
    Console.WriteLine($"API call failed: {ex.Message}");
}
```

## Sonar Models: Online Search Integration

The Sonar models are Perplexity's flagship feature, providing real-time internet access for grounded, up-to-date responses:

```csharp
var model = PerplexityProvider.SonarLargeOnline("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What are the latest news about SpaceX launches?"
        }
    }
};

var result = await model.GenerateAsync(options);
// The model will search the web and provide current information with citations
Console.WriteLine(result.Text);
```

### Choosing the Right Sonar Model

- **Small**: Best for speed and cost-efficiency
- **Large**: Balanced performance for most use cases
- **Huge**: Maximum quality and reasoning capability

## Advanced Configuration

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(120)
};

var model = PerplexityProvider.CreateChatModel(
    "llama-3.1-sonar-large-128k-online",
    "your-api-key",
    httpClient: httpClient
);
```

### Custom Base URL

```csharp
var model = PerplexityProvider.CreateChatModel(
    "llama-3.1-sonar-large-128k-online",
    "your-api-key",
    baseUrl: "https://custom-proxy.example.com"
);
```

## API Reference

### PerplexityProvider

Static factory class for creating Perplexity models.

**Methods:**
- `CreateChatModel(modelId, apiKey, baseUrl?, httpClient?)` - Create model with basic parameters
- `CreateChatModel(modelId, config, httpClient?)` - Create model with configuration object
- `SonarSmallOnline(apiKey)` - Create Sonar Small model
- `SonarLargeOnline(apiKey)` - Create Sonar Large model
- `SonarHugeOnline(apiKey)` - Create Sonar Huge model
- `Llama31_8B(apiKey)` - Create Llama 3.1 8B model
- `Llama31_70B(apiKey)` - Create Llama 3.1 70B model
- `ChatModel(modelId, apiKey)` - Create any model by ID

### PerplexityConfiguration

Configuration record for Perplexity API client.

**Properties:**
- `ApiKey` (required) - Your Perplexity API key
- `BaseUrl` (optional) - API base URL, defaults to "https://api.perplexity.ai"
- `TimeoutSeconds` (optional) - Request timeout in seconds

### PerplexityChatLanguageModel

Main implementation of `ILanguageModel` for Perplexity.

**Properties:**
- `SpecificationVersion` - Returns "v1"
- `Provider` - Returns "perplexity"
- `ModelId` - The model identifier

**Methods:**
- `GenerateAsync(options, cancellationToken)` - Non-streaming generation
- `StreamAsync(options, cancellationToken)` - Streaming generation
- `GetSupportedUrlsAsync(cancellationToken)` - Get supported URL patterns

### PerplexityException

Exception thrown when Perplexity API errors occur. Inherits from `ApiCallError`.

**Properties:**
- `ErrorCode` - Perplexity-specific error code
- `StatusCode` - HTTP status code
- `Message` - Error message

**Methods:**
- `IsInstance(error)` - Check if an exception is a PerplexityException

## Best Practices

1. **Choose the Right Model**: Use Sonar models for questions requiring current information, instruction models for general tasks.

2. **Handle Errors Gracefully**: Always catch `PerplexityException` for API-specific errors.

3. **Manage Timeouts**: Set appropriate timeouts for your use case, especially for Sonar models which may take longer due to web searches.

4. **Optimize Token Usage**: Monitor `Usage` in responses to optimize costs.

5. **Stream for Long Responses**: Use `StreamAsync` for better user experience with lengthy outputs.

6. **Reuse HttpClient**: Share HttpClient instances across multiple requests for better performance.

## Limitations

- Image URLs are not currently supported by Perplexity API
- Tool calling support may vary by model
- Rate limits apply based on your Perplexity API plan

## Getting an API Key

Visit [Perplexity AI](https://www.perplexity.ai/) to sign up and obtain an API key.

## Resources

- [Perplexity API Documentation](https://docs.perplexity.ai/)
- [AI SDK .NET Documentation](../../README.md)
- [Model Pricing](https://www.perplexity.ai/pricing)

## License

This provider is part of the AI SDK .NET project. See the main project LICENSE for details.
