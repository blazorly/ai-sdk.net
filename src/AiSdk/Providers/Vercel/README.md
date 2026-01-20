# Vercel AI Gateway Provider

The Vercel AI Gateway provider gives you access to hundreds of AI models from multiple providers (OpenAI, Anthropic, Google, Meta, Mistral, DeepSeek, and more) through a single, unified API. The gateway provides features like automatic failover, rate limiting, and simplified authentication.

## Features

- **Multi-Provider Access**: Access models from OpenAI, Anthropic, Google, Meta, Mistral, DeepSeek, and other providers
- **Unified API**: Use a single consistent API regardless of the underlying model provider
- **OpenAI-Compatible**: Built on the OpenAI API format for maximum compatibility
- **Streaming Support**: Full support for streaming responses via Server-Sent Events (SSE)
- **Tool/Function Calling**: Native support for tool and function calling
- **Automatic Failover**: Configure fallback models when primary models are unavailable
- **Rate Limiting**: Built-in rate limiting and request management
- **BYOK Support**: Bring your own API keys from upstream providers

## Installation

The Vercel provider is included in the main AiSdk package:

```bash
dotnet add package AiSdk
```

## Authentication

You'll need a Vercel AI Gateway API key. You can obtain one by:

1. Signing up for a Vercel account at [vercel.com](https://vercel.com)
2. Navigating to your team's AI Gateway settings
3. Creating an API key in the API keys section

Alternatively, you can use OIDC authentication when deployed to Vercel.

## Basic Usage

### Using Factory Methods for Popular Models

```csharp
using AiSdk.Providers.Vercel;

// OpenAI models via Vercel AI Gateway
var gpt4o = VercelProvider.OpenAI_GPT4o("your-api-key");
var gpt4oMini = VercelProvider.OpenAI_GPT4oMini("your-api-key");

// Anthropic models via Vercel AI Gateway
var claudeOpus = VercelProvider.Anthropic_ClaudeOpus4_5("your-api-key");
var claudeSonnet = VercelProvider.Anthropic_ClaudeSonnet4_5("your-api-key");

// Google models via Vercel AI Gateway
var gemini2Flash = VercelProvider.Google_Gemini2Flash("your-api-key");
var gemini15Pro = VercelProvider.Google_Gemini1_5Pro("your-api-key");

// Meta Llama models via Vercel AI Gateway
var llama33 = VercelProvider.Meta_Llama3_3_70B("your-api-key");
var llama31 = VercelProvider.Meta_Llama3_1_405B("your-api-key");

// Mistral models via Vercel AI Gateway
var mistralLarge = VercelProvider.Mistral_Large("your-api-key");

// DeepSeek models via Vercel AI Gateway
var deepseekV3 = VercelProvider.DeepSeek_V3("your-api-key");
```

### Using Custom Models

```csharp
using AiSdk.Providers.Vercel;

// Create a model using any provider/model-name format
var model = VercelProvider.ChatModel("provider/model-name", "your-api-key");

// Examples:
var xaiGrok = VercelProvider.ChatModel("xai/grok-2", "your-api-key");
var cohereCommand = VercelProvider.ChatModel("cohere/command-r-plus", "your-api-key");
var perplexitySonar = VercelProvider.ChatModel("perplexity/sonar-pro", "your-api-key");
```

### Advanced Configuration

```csharp
using AiSdk.Providers.Vercel;

var config = new VercelConfiguration
{
    ApiKey = "your-api-key",
    BaseUrl = "https://ai-gateway.vercel.sh/v3/ai", // Optional custom endpoint
    TimeoutSeconds = 60 // Optional custom timeout
};

var model = VercelProvider.CreateChatModel("openai/gpt-4o", config);
```

## Generate Text

### Non-Streaming

```csharp
using AiSdk.Providers.Vercel;
using AiSdk.Abstractions;

var model = VercelProvider.OpenAI_GPT4o("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new() { Role = MessageRole.User, Content = "Explain quantum computing in simple terms." }
    },
    Temperature = 0.7,
    MaxTokens = 500
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
```

### Streaming

```csharp
using AiSdk.Providers.Vercel;
using AiSdk.Abstractions;

var model = VercelProvider.Anthropic_ClaudeSonnet4_5("your-api-key");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new() { Role = MessageRole.User, Content = "Write a short story about a robot." }
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
            Console.WriteLine($"Tokens used: {chunk.Usage.TotalTokens}");
        }
    }
}
```

## Multi-Turn Conversations

```csharp
using AiSdk.Providers.Vercel;
using AiSdk.Abstractions;

var model = VercelProvider.Google_Gemini2Flash("your-api-key");

var messages = new List<Message>
{
    new() { Role = MessageRole.System, Content = "You are a helpful programming assistant." },
    new() { Role = MessageRole.User, Content = "What is dependency injection?" }
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
    Content = "Can you show me an example in C#?"
});

options = new LanguageModelCallOptions { Messages = messages };
result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Tool/Function Calling

```csharp
using AiSdk.Providers.Vercel;
using AiSdk.Abstractions;
using System.Text.Json;

var model = VercelProvider.OpenAI_GPT4o("your-api-key");

var tools = new List<Tool>
{
    new()
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = JsonDocument.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""location"": {
                    ""type"": ""string"",
                    ""description"": ""The city and state, e.g. San Francisco, CA""
                },
                ""unit"": {
                    ""type"": ""string"",
                    ""enum"": [""celsius"", ""fahrenheit""]
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
        new() { Role = MessageRole.User, Content = "What's the weather in New York?" }
    },
    Tools = tools
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments.RootElement.GetRawText()}");

        // Execute the tool and add result to conversation
        var toolResult = ExecuteWeatherTool(toolCall.Arguments);

        options.Messages.Add(new Message
        {
            Role = MessageRole.Tool,
            Name = toolCall.ToolCallId,
            Content = toolResult
        });
    }

    // Get final response with tool results
    result = await model.GenerateAsync(options);
    Console.WriteLine(result.Text);
}

string ExecuteWeatherTool(JsonDocument args)
{
    var location = args.RootElement.GetProperty("location").GetString();
    return $"The weather in {location} is sunny and 72°F";
}
```

## Available Models

The Vercel AI Gateway provides access to models from multiple providers. Here are some popular options:

### OpenAI
- `openai/gpt-4o` - Flagship multimodal model
- `openai/gpt-4o-mini` - Fast, affordable small model
- `openai/gpt-4-turbo` - Enhanced GPT-4 with longer context
- `openai/gpt-3.5-turbo` - Fast, inexpensive model

### Anthropic
- `anthropic/claude-opus-4.5` - Most powerful Claude model
- `anthropic/claude-sonnet-4.5` - Balanced performance
- `anthropic/claude-3.5-sonnet` - Previous generation
- `anthropic/claude-3.5-haiku` - Fastest, most compact

### Google
- `google/gemini-2.0-flash` - Latest fast multimodal model
- `google/gemini-1.5-pro` - Most capable Gemini model
- `google/gemini-1.5-flash` - Fast and efficient

### Meta (Llama)
- `meta/llama-3.3-70b-instruct` - Latest 70B model
- `meta/llama-3.1-405b-instruct` - Largest Llama model
- `meta/llama-3.1-70b-instruct` - Balanced option
- `meta/llama-3.1-8b-instruct` - Compact and efficient

### Mistral
- `mistral/mistral-large` - Flagship model
- `mistral/mistral-small` - Cost-effective option

### DeepSeek
- `deepseek/deepseek-v3` - Latest version
- `deepseek/deepseek-chat` - General-purpose model

For the complete list of available models, visit: https://vercel.com/ai-gateway/models

## Model ID Format

Model IDs follow the format: `provider/model-name`

Examples:
- `openai/gpt-4o`
- `anthropic/claude-sonnet-4.5`
- `google/gemini-2.0-flash`
- `meta/llama-3.3-70b-instruct`
- `xai/grok-2`
- `cohere/command-r-plus`
- `perplexity/sonar-pro`

## Configuration Options

### VercelConfiguration

```csharp
public record VercelConfiguration
{
    // Required: Your Vercel AI Gateway API key
    public required string ApiKey { get; init; }

    // Optional: Custom base URL (default: https://ai-gateway.vercel.sh/v3/ai)
    public string BaseUrl { get; init; } = "https://ai-gateway.vercel.sh/v3/ai";

    // Optional: Request timeout in seconds (default: 100)
    public int? TimeoutSeconds { get; init; }
}
```

### LanguageModelCallOptions

```csharp
public class LanguageModelCallOptions
{
    // Required: Conversation messages
    public required List<Message> Messages { get; set; }

    // Optional: Maximum tokens to generate
    public int? MaxTokens { get; set; }

    // Optional: Temperature (0.0 to 2.0)
    public double? Temperature { get; set; }

    // Optional: Top-p sampling
    public double? TopP { get; set; }

    // Optional: Stop sequences
    public IReadOnlyList<string>? StopSequences { get; set; }

    // Optional: Available tools/functions
    public List<Tool>? Tools { get; set; }

    // Optional: Force specific tool
    public string? ToolChoice { get; set; }
}
```

## Error Handling

```csharp
using AiSdk.Providers.Vercel;
using AiSdk.Providers.Vercel.Exceptions;

try
{
    var model = VercelProvider.OpenAI_GPT4o("your-api-key");
    var result = await model.GenerateAsync(options);
}
catch (VercelException ex)
{
    Console.WriteLine($"Vercel API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Best Practices

1. **Reuse HttpClient**: Create a single HttpClient instance and reuse it across multiple model instances for better performance.

```csharp
var httpClient = new HttpClient();
var model1 = VercelProvider.CreateChatModel("openai/gpt-4o", apiKey, httpClient: httpClient);
var model2 = VercelProvider.CreateChatModel("anthropic/claude-sonnet-4.5", apiKey, httpClient: httpClient);
```

2. **Use Environment Variables**: Store your API key securely in environment variables.

```csharp
var apiKey = Environment.GetEnvironmentVariable("VERCEL_AI_GATEWAY_API_KEY");
var model = VercelProvider.OpenAI_GPT4o(apiKey!);
```

3. **Handle Timeouts**: Set appropriate timeouts for long-running requests.

```csharp
var config = new VercelConfiguration
{
    ApiKey = apiKey,
    TimeoutSeconds = 120 // 2 minutes for complex requests
};
```

4. **Use Streaming for Long Responses**: Stream responses for better user experience with long-form content.

5. **Implement Retry Logic**: The gateway includes automatic failover, but you may want additional retry logic for network issues.

## Resources

- [Vercel AI Gateway Documentation](https://vercel.com/docs/ai-gateway)
- [Available Models](https://vercel.com/ai-gateway/models)
- [Vercel AI SDK](https://ai-sdk.dev/)
- [Authentication Guide](https://vercel.com/docs/ai-gateway/authentication)
- [Getting Started](https://vercel.com/docs/ai-gateway/getting-started)

## Support

For issues and questions:
- Vercel AI Gateway: https://vercel.com/docs/ai-gateway
- AI SDK .NET: https://github.com/ai-sdk/ai-sdk.net

## License

This provider is part of the AI SDK .NET project and follows the same license.
