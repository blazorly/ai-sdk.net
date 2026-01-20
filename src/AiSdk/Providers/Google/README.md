# AiSdk.Providers.Google

Google provider for AI SDK .NET - integrates Gemini 1.5 Pro, Gemini 1.5 Flash, and other Gemini models with the unified AI SDK interface.

## Features

- **Gemini Models** - Gemini 1.5 Pro, Gemini 1.5 Flash, Gemini 1.0 Pro
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Google's function declaration system
- **Multi-modal** - Text, images, audio, and video support
- **Long Context** - Up to 1M tokens context window (Gemini 1.5 Pro)
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Google
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Google;

// Create provider
var provider = new GoogleProvider(new GoogleConfiguration
{
    ApiKey = "your-api-key"
});

// Get a model
var model = provider.Gemini15Pro();

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain machine learning in simple terms")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration

```csharp
var config = new GoogleConfiguration
{
    ApiKey = "AIza...",                                           // Required
    BaseUrl = "https://generativelanguage.googleapis.com/v1beta", // Optional (default)
    TimeoutSeconds = 120                                          // Optional (default: 100)
};
```

### Environment Variables

```csharp
var config = new GoogleConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var gemini15Pro = provider.Gemini15Pro();          // gemini-1.5-pro
var gemini15Flash = provider.Gemini15Flash();      // gemini-1.5-flash
var gemini10Pro = provider.Gemini10Pro();          // gemini-1.0-pro

// Or use specific model ID
var model = provider.ChatModel("gemini-1.5-pro-latest");
```

## Usage Examples

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a creative story about robots")
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
        Console.WriteLine($"\nFinish reason: {chunk.FinishReason}");
        Console.WriteLine($"Tokens: {chunk.Usage?.TotalTokens}");
    }
}
```

### Function Calling

```csharp
var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "get_current_time",
        Description = "Get the current time in a specific timezone",
        Parameters = JsonSchema.FromType<TimeRequest>()
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What time is it in Tokyo?")
    },
    Tools = tools
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Function: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments}");

        // Execute the function...
        var functionResult = ExecuteFunction(toolCall.ToolName, toolCall.Arguments);

        // Send result back to Gemini
        var followUpOptions = new LanguageModelCallOptions
        {
            Messages = new List<Message>
            {
                new Message(MessageRole.User, "What time is it in Tokyo?"),
                new Message(MessageRole.Assistant, result.Text, ToolCalls: result.ToolCalls),
                new Message(
                    Role: MessageRole.Tool,
                    Content: functionResult,
                    Name: toolCall.ToolName
                )
            },
            Tools = tools
        };

        var finalResult = await model.GenerateAsync(followUpOptions);
        Console.WriteLine(finalResult.Text);
    }
}
```

### Multi-modal: Images

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(
            Role: MessageRole.User,
            Content: "Describe what you see in this image",
            Attachments: new List<Attachment>
            {
                new Attachment
                {
                    ContentType = "image/jpeg",
                    Url = "https://example.com/photo.jpg"
                }
            }
        )
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Multi-modal: Video

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(
            Role: MessageRole.User,
            Content = "Summarize the main events in this video",
            Attachments: new List<Attachment>
            {
                new Attachment
                {
                    ContentType = "video/mp4",
                    Url = "https://example.com/video.mp4"
                }
            }
        )
    }
};

var result = await model.GenerateAsync(options);
```

### Multi-modal: Audio

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(
            Role: MessageRole.User,
            Content: "Transcribe and translate this audio to English",
            Attachments: new List<Attachment>
            {
                new Attachment
                {
                    ContentType = "audio/mp3",
                    Url = "https://example.com/audio.mp3"
                }
            }
        )
    }
};

var result = await model.GenerateAsync(options);
```

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 8192,
    Temperature = 0.7,
    TopP = 0.95,
    StopSequences = new List<string> { "END", "\n---\n" }
};

var result = await model.GenerateAsync(options);
```

### Long Context (Gemini 1.5 Pro)

```csharp
// Gemini 1.5 Pro supports up to 1M tokens
var longDocument = File.ReadAllText("very_long_document.txt");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, $"Summarize this document:\n\n{longDocument}")
    },
    MaxTokens = 4096
};

var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Google.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (GoogleException ex) when (ex.StatusCode == 400)
{
    Console.WriteLine($"Invalid request: {ex.Message}");
}
catch (GoogleException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine($"Quota exceeded. Status: {ex.ErrorStatus}");
}
catch (GoogleException ex)
{
    Console.WriteLine($"Google API error: {ex.Message} (Status: {ex.StatusCode}, Error: {ex.ErrorStatus})");
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
      "google": {
        "ApiKey": "AIza...",
        "DefaultModel": "gemini-1.5-pro",
        "TimeoutSeconds": 120,
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
    options.DefaultProvider = "google";
    options.Providers.Add("google", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:google:ApiKey"]!,
        DefaultModel = "gemini-1.5-pro"
    });
});
```

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function Calling | ✅ Full support |
| Multi-modal (Images) | ✅ Full support |
| Multi-modal (Audio) | ✅ Full support |
| Multi-modal (Video) | ✅ Full support |
| Long Context (1M tokens) | ✅ Gemini 1.5 Pro |
| System Instructions | ⏸ Planned |
| Embeddings | ⏸ Planned |
| Code Execution | ⏸ Planned |

## Model Comparison

| Model | Context Window | Speed | Capabilities |
|-------|----------------|-------|-------------|
| **Gemini 1.5 Pro** | 1M tokens | Medium | Best quality, multi-modal, long context |
| **Gemini 1.5 Flash** | 1M tokens | Fast | Fast, efficient, multi-modal |
| **Gemini 1.0 Pro** | 32K tokens | Fast | Text-only, good for simple tasks |

## Performance Tips

1. **Model Selection**:
   - Use **Gemini 1.5 Flash** for fast responses and simple tasks
   - Use **Gemini 1.5 Pro** for complex reasoning and long context

2. **Multi-modal**:
   - Supported formats: JPEG, PNG, WebP, GIF (images), MP4, MPEG, AVI (video), MP3, WAV (audio)
   - Keep media files under recommended size limits

3. **Context Length**:
   - Gemini 1.5 Pro supports up to 1M tokens
   - Use for large documents, codebases, or multi-turn conversations

4. **Cancellation**:
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
   var result = await model.GenerateAsync(options, cts.Token);
   ```

## Common Error Status Codes

| Status | Error Status | Description |
|--------|-------------|-------------|
| 400 | `INVALID_ARGUMENT` | Invalid request parameters |
| 401 | `UNAUTHENTICATED` | Invalid or missing API key |
| 403 | `PERMISSION_DENIED` | Insufficient permissions |
| 404 | `NOT_FOUND` | Model or resource not found |
| 429 | `RESOURCE_EXHAUSTED` | Quota exceeded |
| 500 | `INTERNAL` | Internal Google error |
| 503 | `UNAVAILABLE` | Service temporarily unavailable |

## Quota and Limits

- **Free tier**: 60 requests per minute
- **Pay-as-you-go**: Higher rate limits
- **Context length**: Up to 1M tokens (Gemini 1.5 Pro/Flash)
- **Max output tokens**: 8,192

## Important Notes

1. **System Messages**: Currently mapped to user messages. Native system instruction support is planned.

2. **Tool Results**: Use the function name in the `Name` field:
   ```csharp
   new Message(
       Role: MessageRole.Tool,
       Content: result,
       Name: toolCall.ToolName  // Use ToolName as Name
   )
   ```

3. **Finish Reasons**:
   - `STOP` - Natural completion
   - `MAX_TOKENS` - Max tokens reached
   - `SAFETY` - Content filtered for safety
   - `RECITATION` - Content filtered for recitation

## Links

- [Google AI for Developers](https://ai.google.dev/)
- [Gemini API Documentation](https://ai.google.dev/docs)
- [Model Pricing](https://ai.google.dev/pricing)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
