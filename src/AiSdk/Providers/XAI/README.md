# AiSdk.Providers.XAI

xAI (Grok) provider for AI SDK .NET - integrates Grok models with the unified AI SDK interface.

## Features

- **Chat Completions** - Grok 4, Grok 3, Grok 2, and specialized variants
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **Vision Models** - Image understanding with Grok 2 Vision
- **Reasoning Models** - Advanced reasoning with Grok 4.1 Fast Reasoning
- **Code Generation** - Specialized Grok Code Fast models
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.XAI
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.XAI;

// Create a model using factory method
var model = XAIProvider.Grok4("your-api-key");

// Or create with configuration
var config = new XAIConfiguration
{
    ApiKey = "your-api-key"
};
var model = XAIProvider.CreateChatModel("grok-4", config);

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
var config = new XAIConfiguration
{
    ApiKey = "xai-...",                          // Required
    BaseUrl = "https://api.x.ai/v1",             // Optional (default shown)
    TimeoutSeconds = 60                          // Optional (default: 100)
};
```

### Environment Variables

```csharp
var config = new XAIConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("XAI_API_KEY")!
};
```

### Available Models

```csharp
// Grok 4 Models
var grok4 = XAIProvider.Grok4(apiKey);                               // grok-4
var grok41FastReasoning = XAIProvider.Grok4_1FastReasoning(apiKey);  // grok-4-1-fast-reasoning
var grok41FastNonReasoning = XAIProvider.Grok4_1FastNonReasoning(apiKey); // grok-4-1-fast-non-reasoning
var grok4FastReasoning = XAIProvider.Grok4FastReasoning(apiKey);     // grok-4-fast-reasoning
var grok4FastNonReasoning = XAIProvider.Grok4FastNonReasoning(apiKey); // grok-4-fast-non-reasoning

// Code Generation
var grokCode = XAIProvider.GrokCodeFast1(apiKey);                    // grok-code-fast-1

// Grok 3 Models
var grok3 = XAIProvider.Grok3(apiKey);                               // grok-3
var grok3Mini = XAIProvider.Grok3Mini(apiKey);                       // grok-3-mini

// Grok 2 Models
var grok2Vision = XAIProvider.Grok2Vision(apiKey);                   // grok-2-vision-1212
var grok2Image = XAIProvider.Grok2Image(apiKey);                     // grok-2-image-1212
var grok2 = XAIProvider.Grok2(apiKey);                               // grok-2-1212

// Or use specific model ID
var model = XAIProvider.CreateChatModel("grok-4", apiKey);
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
    MaxTokens = 1000,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\n", "END" },
    ToolChoice = "get_weather"  // Force specific tool
};

var result = await model.GenerateAsync(options);
```

### Vision (Images)

```csharp
var model = XAIProvider.Grok2Vision(apiKey);

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

var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.XAI.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (XAIException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (XAIException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (XAIException ex)
{
    Console.WriteLine($"xAI error: {ex.Message} (Status: {ex.StatusCode})");
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
      "xai": {
        "ApiKey": "xai-...",
        "DefaultModel": "grok-4",
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
    options.DefaultProvider = "xai";
    options.Providers.Add("xai", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:xai:ApiKey"]!,
        DefaultModel = "grok-4"
    });
});
```

## Model Comparison

| Model | Use Case | Speed | Capabilities |
|-------|----------|-------|--------------|
| grok-4 | General purpose | Standard | Most capable, latest features |
| grok-4-1-fast-reasoning | Complex reasoning | Ultra-fast | Advanced reasoning, optimized |
| grok-4-1-fast-non-reasoning | Simple queries | Ultra-fast | Quick responses, no reasoning |
| grok-4-fast-reasoning | Balanced reasoning | Fast | Good reasoning with speed |
| grok-4-fast-non-reasoning | Quick tasks | Fast | Simple queries, optimized |
| grok-code-fast-1 | Code generation | Fast | Code-specific tasks |
| grok-3 | General purpose | Standard | Previous generation |
| grok-3-mini | Efficiency | Fast | Compact, cost-effective |
| grok-2-vision-1212 | Image understanding | Standard | Vision capabilities |
| grok-2-image-1212 | Image generation | Standard | Image manipulation |
| grok-2-1212 | General purpose | Standard | Grok 2 baseline |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision (Grok 2 Vision) | ✅ Supported via attachments |
| Reasoning Models | ✅ Grok 4.1 Fast Reasoning |
| Code Generation | ✅ Grok Code Fast models |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use `grok-4-1-fast-non-reasoning` for simple queries
   - Use `grok-4-1-fast-reasoning` for complex reasoning tasks
   - Use `grok-code-fast-1` for code-related tasks

2. **Timeouts**: Set appropriate `TimeoutSeconds` for your use case

3. **Streaming**: Use streaming for long responses to improve perceived performance

4. **HttpClient Reuse**: The provider reuses HttpClient instances automatically

5. **Cancellation**: Always pass `CancellationToken` for long-running operations

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
| 500 | `server_error` | xAI server error |

## xAI API Features

### Server-Side Tools (Advanced)

xAI's Responses API provides server-side tools that models can autonomously execute:

- **web_search** - Real-time web search and page browsing
- **x_search** - Search X/Twitter posts, users, and threads
- **code_execution** - Execute Python code for calculations and data analysis

*Note: Server-side tools require the Responses API endpoint, which may differ from the standard Chat API.*

### Files API

xAI supports file uploads for use in chat conversations via the Files API.

## Links

- [xAI API Documentation](https://docs.x.ai)
- [xAI Models and Pricing](https://docs.x.ai/docs/models)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
