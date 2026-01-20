# AiSdk.Providers.OpenAI

OpenAI provider for AI SDK .NET - integrates GPT-4, GPT-3.5 Turbo, and other OpenAI models with the unified AI SDK interface.

## Features

- **Chat Completions** - GPT-4, GPT-4 Turbo, GPT-3.5 Turbo
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **Vision Models** - Image understanding with GPT-4 Vision
- **Flexible Configuration** - API key, organization, project, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.OpenAI
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAI;

// Create provider
var provider = new OpenAIProvider(new OpenAIConfiguration
{
    ApiKey = "your-api-key"
});

// Get a model
var model = provider.ChatModel("gpt-4");

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
var config = new OpenAIConfiguration
{
    ApiKey = "sk-...",                                    // Required
    BaseUrl = "https://api.openai.com/v1",               // Optional (default shown)
    Organization = "org-...",                             // Optional
    Project = "proj_...",                                 // Optional
    TimeoutSeconds = 60                                   // Optional (default: 100)
};
```

### Environment Variables

```csharp
var config = new OpenAIConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var gpt4 = provider.GPT4();                    // gpt-4
var gpt4Turbo = provider.GPT4Turbo();          // gpt-4-turbo-preview
var gpt35 = provider.GPT35Turbo();             // gpt-3.5-turbo

// Or use specific model ID
var model = provider.ChatModel("gpt-4-1106-preview");
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
using AiSdk.Providers.OpenAI.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (OpenAIException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (OpenAIException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (OpenAIException ex)
{
    Console.WriteLine($"OpenAI error: {ex.Message} (Status: {ex.StatusCode})");
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
      "openai": {
        "ApiKey": "sk-...",
        "DefaultModel": "gpt-4",
        "TimeoutSeconds": 60,
        "OrganizationId": "org-...",
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
    options.DefaultProvider = "openai";
    options.Providers.Add("openai", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:openai:ApiKey"]!,
        DefaultModel = "gpt-4"
    });
});
```

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision (GPT-4V) | ✅ Supported via attachments |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |
| Image Generation (DALL-E) | ⏸ Planned |
| Speech (TTS) | ⏸ Planned |

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
| 500 | `server_error` | OpenAI server error |

## Links

- [OpenAI API Documentation](https://platform.openai.com/docs)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
