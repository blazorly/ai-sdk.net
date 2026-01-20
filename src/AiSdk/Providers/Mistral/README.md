# AiSdk.Providers.Mistral

Mistral AI provider for AI SDK .NET - integrates Mistral Large, Mistral Medium, Mistral Small, and other Mistral AI models with the unified AI SDK interface.

## Features

- **Chat Completions** - Mistral Large, Medium, Small, and open models
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **Vision Models** - Image understanding with Pixtral models
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Mistral
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Mistral;

// Create a model using convenience method
var model = MistralProvider.MistralLarge("your-api-key");

// Or create with configuration
var config = new MistralConfiguration
{
    ApiKey = "your-api-key"
};
var model = MistralProvider.CreateChatModel("mistral-large-latest", config);

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
var config = new MistralConfiguration
{
    ApiKey = "your-mistral-api-key",                     // Required
    BaseUrl = "https://api.mistral.ai/v1",               // Optional (default shown)
    TimeoutSeconds = 60                                   // Optional (default: 100)
};
```

### Environment Variables

```csharp
var config = new MistralConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("MISTRAL_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var mistralLarge = MistralProvider.MistralLarge("api-key");      // mistral-large-latest
var mistralMedium = MistralProvider.MistralMedium("api-key");    // mistral-medium-latest
var mistralSmall = MistralProvider.MistralSmall("api-key");      // mistral-small-latest

// Generic method
var customModel = MistralProvider.ChatModel("open-mistral-7b", "api-key");

// Or use CreateChatModel with specific model ID
var model = MistralProvider.CreateChatModel("mistral-large-latest", "api-key");
```

## Usage Examples

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a haiku about coding")
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
        new Message(MessageRole.User, "What's the weather in Paris?")
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
    MaxTokens = 1000,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\n", "END" },
    ToolChoice = "get_weather"  // Force specific tool
};

var result = await model.GenerateAsync(options);
```

### Vision (Images) with Pixtral

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(
            Role: MessageRole.User,
            Content: "What's in this image?",
            Attachments: new List<Attachment>
            {
                new Attachment
                {
                    ContentType = "image/jpeg",
                    Url = "https://example.com/image.jpg"
                }
            }
        )
    }
};

var model = MistralProvider.ChatModel("pixtral-12b-2409", apiKey);
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Mistral.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (MistralException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (MistralException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (MistralException ex)
{
    Console.WriteLine($"Mistral AI error: {ex.Message} (Status: {ex.StatusCode})");
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
      "mistral": {
        "ApiKey": "your-mistral-api-key",
        "DefaultModel": "mistral-large-latest",
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
    options.DefaultProvider = "mistral";
    options.Providers.Add("mistral", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:mistral:ApiKey"]!,
        DefaultModel = "mistral-large-latest"
    });
});
```

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision (Pixtral) | ✅ Supported via attachments |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |
| Fine-tuning API | ⏸ Planned |

## Available Models

### Production Models

- **mistral-large-latest** - Flagship model, most capable for complex tasks
- **mistral-medium-latest** - Balanced performance and speed
- **mistral-small-latest** - Fast and cost-effective for simple tasks

### Specialized Models

- **pixtral-12b-2409** - Vision model for image understanding
- **open-mistral-7b** - Open-source lightweight model
- **open-mixtral-8x7b** - Open-source mixture of experts model
- **open-mixtral-8x22b** - Larger mixture of experts model

### Legacy Models

- **mistral-tiny** - Deprecated, use mistral-small-latest
- **mistral-medium** - Deprecated, use mistral-medium-latest

## Performance Tips

1. **Timeouts**: Set appropriate `TimeoutSeconds` for your use case
2. **Streaming**: Use streaming for long responses to improve perceived performance
3. **HttpClient Reuse**: The provider reuses HttpClient instances automatically
4. **Cancellation**: Always pass `CancellationToken` for long-running operations

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var result = await model.GenerateAsync(options, cts.Token);
```

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `invalid_api_key` | Invalid authentication |
| 429 | `rate_limit_exceeded` | Too many requests |
| 400 | `invalid_request_error` | Bad request |
| 500 | `server_error` | Mistral AI server error |

## Comparison with Other Providers

### Mistral vs OpenAI

- **Similarities**: Both use OpenAI-compatible API format
- **Differences**: Mistral offers European data hosting and competitive pricing
- **Models**: Mistral Large competes with GPT-4, Mistral Small with GPT-3.5

### When to Use Mistral

- Need European data residency
- Cost-effective alternative to OpenAI
- Open-source model requirements (open-mistral-7b, mixtral)
- Excellent multilingual support (especially French)

## Links

- [Mistral AI Documentation](https://docs.mistral.ai/)
- [Mistral AI Platform](https://console.mistral.ai/)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
