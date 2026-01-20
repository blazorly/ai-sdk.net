# AiSdk.Providers.Anthropic

Anthropic provider for AI SDK .NET - integrates Claude 3.5 Sonnet, Claude 3 Opus, and other Claude models with the unified AI SDK interface.

## Features

- **Claude Models** - Claude 3.5 Sonnet, Claude 3 Opus/Sonnet/Haiku
- **Streaming Support** - Real-time token-by-token streaming with SSE
- **Tool Use** - Anthropic's tool/function calling system
- **Vision Support** - Image understanding with Claude 3 models
- **System Messages** - Dedicated system message handling
- **Flexible Configuration** - API key, base URL, API version, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Anthropic
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Anthropic;

// Create provider
var provider = new AnthropicProvider(new AnthropicConfiguration
{
    ApiKey = "your-api-key"
});

// Get a model
var model = provider.Claude35Sonnet();

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain quantum entanglement simply")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration

```csharp
var config = new AnthropicConfiguration
{
    ApiKey = "sk-ant-...",                                // Required
    BaseUrl = "https://api.anthropic.com/v1",            // Optional (default shown)
    ApiVersion = "2023-06-01",                           // Optional (default shown)
    TimeoutSeconds = 90                                  // Optional (default: 100)
};
```

### Environment Variables

```csharp
var config = new AnthropicConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var claude35Sonnet = provider.Claude35Sonnet();        // claude-3-5-sonnet-20241022
var claude3Opus = provider.Claude3Opus();              // claude-3-opus-20240229
var claude3Sonnet = provider.Claude3Sonnet();          // claude-3-sonnet-20240229
var claude3Haiku = provider.Claude3Haiku();            // claude-3-haiku-20240307

// Or use specific model ID
var model = provider.ChatModel("claude-3-5-sonnet-20241022");
```

## Usage Examples

### System Messages

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.System, "You are a helpful coding assistant."),
        new Message(MessageRole.User, "Write a Python function to sort a list")
    }
};

var result = await model.GenerateAsync(options);
```

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a sonnet about artificial intelligence")
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
        Console.WriteLine($"\nTokens used: {chunk.Usage?.TotalTokens}");
    }
}
```

### Tool Use

```csharp
var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "calculate",
        Description = "Perform mathematical calculations",
        Parameters = JsonSchema.FromType<CalculateRequest>()
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What is 15% of 240?")
    },
    Tools = tools,
    MaxTokens = 1024  // Required for Anthropic
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        var args = toolCall.Arguments.RootElement;

        // Execute tool...
        var toolResult = ExecuteTool(toolCall.ToolName, args);

        // Send result back to Claude
        var followUpOptions = new LanguageModelCallOptions
        {
            Messages = new List<Message>
            {
                new Message(MessageRole.User, "What is 15% of 240?"),
                new Message(MessageRole.Assistant, result.Text, ToolCalls: result.ToolCalls),
                new Message(
                    Role: MessageRole.Tool,
                    Content: toolResult,
                    Name: toolCall.ToolCallId
                )
            },
            Tools = tools,
            MaxTokens = 1024
        };

        var finalResult = await model.GenerateAsync(followUpOptions);
        Console.WriteLine(finalResult.Text);
    }
}
```

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 4096,              // Required for Anthropic (default: 4096)
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\nHuman:", "\n\nAssistant:" }
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
            Content: "Describe this image in detail",
            Attachments: new List<Attachment>
            {
                new Attachment
                {
                    ContentType = "image/jpeg",
                    Url = "https://example.com/photo.jpg"
                }
            }
        )
    },
    MaxTokens = 1024
};

var result = await model.GenerateAsync(options);
```

### Multi-turn Conversation

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a history expert."),
    new Message(MessageRole.User, "Who was the first person in space?"),
    new Message(MessageRole.Assistant, "Yuri Gagarin was the first person in space."),
    new Message(MessageRole.User, "What year did that happen?")
};

var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 1024
};

var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Anthropic.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (AnthropicException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (AnthropicException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine($"Rate limited. Error type: {ex.ErrorType}");
}
catch (AnthropicException ex)
{
    Console.WriteLine($"Anthropic error: {ex.Message} (Status: {ex.StatusCode}, Type: {ex.ErrorType})");
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
      "anthropic": {
        "ApiKey": "sk-ant-...",
        "DefaultModel": "claude-3-5-sonnet-20241022",
        "TimeoutSeconds": 90,
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
    options.DefaultProvider = "anthropic";
    options.Providers.Add("anthropic", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:anthropic:ApiKey"]!,
        DefaultModel = "claude-3-5-sonnet-20241022"
    });
});
```

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Tool Use | ✅ Full support |
| Vision (Images) | ✅ Supported via attachments |
| System Messages | ✅ Native support |
| Multi-modal | ✅ Images supported |
| Prompt Caching | ⏸ Planned |
| PDF Support | ⏸ Planned |

## Important Notes

1. **MaxTokens Required**: Anthropic requires the `MaxTokens` parameter. If not specified, the default is 4096.

2. **System Messages**: System messages are handled separately by Anthropic. The SDK automatically extracts them.

3. **Tool Results**: Tool results use the `Name` field for the `tool_use_id`:
   ```csharp
   new Message(
       Role: MessageRole.Tool,
       Content: result,
       Name: toolCall.ToolCallId  // Use ToolCallId as Name
   )
   ```

4. **Streaming Events**: Anthropic uses detailed SSE events. The SDK handles:
   - `content_block_start` - Tool use begins
   - `content_block_delta` - Text or JSON deltas
   - `content_block_stop` - Tool use complete
   - `message_delta` - Finish reason and usage
   - `message_stop` - Final message

## Performance Tips

1. **Model Selection**: Use Claude 3.5 Sonnet for best balance of speed and capability
2. **Streaming**: Recommended for long responses
3. **MaxTokens**: Set appropriately to avoid truncation
4. **Cancellation**: Always support cancellation for long operations

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
var result = await model.GenerateAsync(options, cts.Token);
```

## Common Error Types

| Error Type | Description |
|------------|-------------|
| `invalid_request_error` | Invalid request parameters |
| `authentication_error` | Invalid API key |
| `permission_error` | Insufficient permissions |
| `not_found_error` | Resource not found |
| `rate_limit_error` | Too many requests |
| `api_error` | Internal Anthropic error |
| `overloaded_error` | Service overloaded |

## Links

- [Anthropic API Documentation](https://docs.anthropic.com/claude/reference/getting-started-with-the-api)
- [Claude Models](https://docs.anthropic.com/claude/docs/models-overview)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
