# Portkey AI Gateway Provider

The Portkey AI Gateway provider gives you access to 200+ AI models from multiple providers (OpenAI, Anthropic, Cohere, Azure OpenAI, and more) through a single, unified API. Portkey provides enterprise features like routing, caching, fallbacks, load balancing, and comprehensive observability.

## Features

- **Multi-Provider Access**: Access models from OpenAI, Anthropic, Cohere, Azure OpenAI, Google, and 20+ other providers
- **Virtual Keys**: Securely store provider API keys in Portkey's vault and reference them via virtual keys
- **Smart Routing**: Route requests across multiple models with automatic failover and load balancing
- **Caching**: Built-in semantic caching to reduce costs and latency
- **Observability**: Comprehensive logging, analytics, and monitoring for all requests
- **OpenAI-Compatible**: Built on the OpenAI API format for maximum compatibility
- **Streaming Support**: Full support for streaming responses via Server-Sent Events (SSE)
- **Tool/Function Calling**: Native support for tool and function calling
- **Cost Tracking**: Automatic cost tracking across all providers

## Installation

The Portkey provider is included in the main AiSdk package:

```bash
dotnet add package AiSdk
```

## Authentication

You'll need a Portkey API key. You can obtain one by:

1. Signing up for a free account at [portkey.ai](https://portkey.ai)
2. Navigating to your dashboard
3. Creating an API key in the API keys section

## Basic Usage

### Using Direct Provider Authentication

```csharp
using AiSdk.Providers.Portkey;

// Create a model with provider name (requires provider API key to be set as virtual key)
var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: "your-portkey-api-key",
    provider: "openai",
    virtualKey: "your-openai-virtual-key"
);
```

### Using Virtual Keys (Recommended)

Virtual keys allow you to securely store your provider API keys in Portkey's vault:

```csharp
using AiSdk.Providers.Portkey;

// Create a virtual key in the Portkey dashboard for your OpenAI API key
// Then reference it when creating the model
var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: "your-portkey-api-key",
    virtualKey: "openai-virtual-key-id"
);
```

### Using Configuration Object

```csharp
using AiSdk.Providers.Portkey;

var config = new PortkeyConfiguration
{
    ApiKey = "your-portkey-api-key",
    Provider = "anthropic",
    VirtualKey = "anthropic-virtual-key-id",
    BaseUrl = "https://api.portkey.ai/v1", // Optional custom endpoint
    TimeoutSeconds = 60 // Optional custom timeout
};

var model = PortkeyProvider.CreateChatModel("claude-3-5-sonnet-20241022", config);
```

## Generate Text

### Non-Streaming

```csharp
using AiSdk.Providers.Portkey;
using AiSdk.Abstractions;

var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: "your-portkey-api-key",
    provider: "openai",
    virtualKey: "your-virtual-key"
);

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
using AiSdk.Providers.Portkey;
using AiSdk.Abstractions;

var model = PortkeyProvider.ChatModel(
    modelId: "claude-3-5-sonnet-20241022",
    apiKey: "your-portkey-api-key",
    virtualKey: "anthropic-virtual-key"
);

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
using AiSdk.Providers.Portkey;
using AiSdk.Abstractions;

var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: "your-portkey-api-key",
    virtualKey: "openai-virtual-key"
);

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
using AiSdk.Providers.Portkey;
using AiSdk.Abstractions;
using System.Text.Json;

var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: "your-portkey-api-key",
    virtualKey: "openai-virtual-key"
);

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

## Advanced Features

### Virtual Keys Setup

Virtual keys allow you to securely store provider API keys in Portkey's vault:

1. Log in to your Portkey dashboard at [app.portkey.ai](https://app.portkey.ai)
2. Navigate to "Virtual Keys" in the left sidebar
3. Click "Add Virtual Key"
4. Select your provider (OpenAI, Anthropic, etc.)
5. Enter your provider's API key
6. Give it a memorable name
7. Copy the generated virtual key ID

Use the virtual key in your code:

```csharp
var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: "your-portkey-api-key",
    virtualKey: "virtual-key-id-from-dashboard"
);
```

### Multi-Provider Routing with Fallbacks

Portkey supports advanced routing configurations through their Config system. You can set up fallbacks, load balancing, and more through the Portkey dashboard or API.

Example with configuration in dashboard:
1. Create a Config in Portkey dashboard with multiple providers
2. Reference the Config ID in your requests using custom headers

```csharp
// For advanced routing, you can use Portkey's Config feature
// This requires creating a Config in the Portkey dashboard
// and passing the Config ID via custom headers
```

### Semantic Caching

Portkey provides built-in semantic caching to reduce costs and latency. Enable it in your Portkey dashboard:

1. Navigate to "Caches" in the Portkey dashboard
2. Enable semantic caching for your workspace
3. Configure cache TTL and similarity threshold
4. All subsequent requests will automatically use caching

No code changes required - caching works automatically once enabled!

### Cost Tracking and Analytics

Portkey automatically tracks costs and usage across all providers:

1. View real-time cost analytics in the Portkey dashboard
2. Set budget alerts and rate limits
3. Track usage by model, provider, or custom metadata
4. Export detailed usage reports

### Observability Features

Every request through Portkey is logged and can be analyzed:

- **Request Logs**: View all requests with full details
- **Tracing**: Trace requests across your entire stack
- **Analytics**: Analyze performance, costs, and usage patterns
- **Alerts**: Set up alerts for errors, latency, or budget thresholds

Access observability features in the Portkey dashboard.

## Supported Providers

Portkey supports 200+ models from these providers:

- **OpenAI**: GPT-4, GPT-3.5, and more
- **Anthropic**: Claude 3.5, Claude 3, and more
- **Cohere**: Command, Command Light
- **Azure OpenAI**: All Azure OpenAI models
- **Google**: PaLM, Gemini
- **Mistral**: Mistral Large, Medium, Small
- **Anyscale**: Llama 2, Mistral
- **Together AI**: Various open-source models
- **Perplexity**: Sonar models
- **And many more**: See [full list](https://portkey.ai/docs/integrations/llms)

## Model ID Format

Model IDs match the provider's native format:

Examples:
- OpenAI: `gpt-4`, `gpt-3.5-turbo`, `gpt-4-turbo-preview`
- Anthropic: `claude-3-5-sonnet-20241022`, `claude-3-opus-20240229`
- Cohere: `command`, `command-light`
- Azure OpenAI: Use your deployment name
- Mistral: `mistral-large-latest`, `mistral-medium`

## Configuration Options

### PortkeyConfiguration

```csharp
public record PortkeyConfiguration
{
    // Required: Your Portkey API key
    public required string ApiKey { get; init; }

    // Optional: Target provider name (openai, anthropic, cohere, etc.)
    public string? Provider { get; init; }

    // Optional: Virtual key ID from Portkey vault
    public string? VirtualKey { get; init; }

    // Optional: Custom base URL (default: https://api.portkey.ai/v1)
    public string BaseUrl { get; init; } = "https://api.portkey.ai/v1";

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
using AiSdk.Providers.Portkey;
using AiSdk.Providers.Portkey.Exceptions;

try
{
    var model = PortkeyProvider.ChatModel(
        modelId: "gpt-4",
        apiKey: "your-portkey-api-key",
        virtualKey: "your-virtual-key"
    );
    var result = await model.GenerateAsync(options);
}
catch (PortkeyException ex)
{
    Console.WriteLine($"Portkey API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Best Practices

1. **Use Virtual Keys**: Store provider API keys securely in Portkey's vault instead of hardcoding them.

```csharp
// Good - uses virtual key
var model = PortkeyProvider.ChatModel(
    modelId: "gpt-4",
    apiKey: portkeyApiKey,
    virtualKey: "openai-virtual-key"
);

// Avoid - requires managing provider keys in code
```

2. **Reuse HttpClient**: Create a single HttpClient instance and reuse it across multiple model instances for better performance.

```csharp
var httpClient = new HttpClient();
var model1 = PortkeyProvider.ChatModel("gpt-4", apiKey, virtualKey: vk1, httpClient: httpClient);
var model2 = PortkeyProvider.ChatModel("claude-3-5-sonnet-20241022", apiKey, virtualKey: vk2, httpClient: httpClient);
```

3. **Use Environment Variables**: Store your Portkey API key securely in environment variables.

```csharp
var apiKey = Environment.GetEnvironmentVariable("PORTKEY_API_KEY");
var model = PortkeyProvider.ChatModel("gpt-4", apiKey!, virtualKey: "openai-vk");
```

4. **Enable Caching**: Enable semantic caching in Portkey dashboard to reduce costs and latency for similar requests.

5. **Monitor Usage**: Use Portkey's dashboard to monitor costs, latency, and errors in real-time.

6. **Set Budget Alerts**: Configure budget alerts in Portkey to prevent unexpected costs.

7. **Use Streaming for Long Responses**: Stream responses for better user experience with long-form content.

8. **Implement Retry Logic**: While Portkey provides automatic failover, implement additional retry logic for network issues.

## Resources

- [Portkey Documentation](https://portkey.ai/docs)
- [Virtual Keys Guide](https://portkey.ai/docs/product/ai-gateway/virtual-keys)
- [Caching Documentation](https://portkey.ai/docs/product/ai-gateway/cache-simple-and-semantic)
- [Observability Guide](https://portkey.ai/docs/product/observability)
- [Supported Providers](https://portkey.ai/docs/integrations/llms)
- [API Reference](https://portkey.ai/docs/api-reference)
- [Routing and Fallbacks](https://portkey.ai/docs/product/ai-gateway/configs)

## Support

For issues and questions:
- Portkey AI Gateway: https://portkey.ai/docs
- Portkey Discord: https://discord.gg/portkey
- AI SDK .NET: https://github.com/ai-sdk/ai-sdk.net

## License

This provider is part of the AI SDK .NET project and follows the same license.
