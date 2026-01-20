# AI21 Labs Provider

A provider for AI SDK .NET that enables access to AI21 Labs' powerful language models through their OpenAI-compatible API. AI21 Labs offers Jamba models with industry-leading 256K context windows and the proven Jurassic-2 family.

## Features

- OpenAI-compatible API for easy integration
- Jamba 1.5 models with 256K context window
- Jurassic-2 foundation models with excellent instruction following
- Function calling and tool use support
- Streaming responses with usage statistics
- Comprehensive error handling
- Multi-turn conversation support

## Installation

The AI21 Labs provider is included in the main AiSdk package:

```bash
dotnet add package AiSdk
```

## Quick Start

### Get Your API Key

1. Sign up at [AI21 Labs](https://www.ai21.com/)
2. Navigate to your account settings to get your API key
3. Set it as an environment variable:
   ```bash
   export AI21_API_KEY="your-api-key-here"
   ```

### Basic Usage

```csharp
using AiSdk.Providers.AI21;

// Get your API key from environment variable
var apiKey = Environment.GetEnvironmentVariable("AI21_API_KEY");

// Use the latest Jamba model
var model = AI21Provider.Jamba15Large(apiKey);

// Generate text
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "What is the capital of France?" }
    }
});

Console.WriteLine(result.Text);
```

## Available Models

### Jamba 1.5 Models

AI21's latest generation with 256K context window:

```csharp
// Jamba 1.5 Large - Flagship model with 256K context window
// Best for: Complex reasoning, long-context tasks, multilingual applications
var model = AI21Provider.Jamba15Large(apiKey);

// Jamba 1.5 Mini - Compact and efficient with 256K context window
// Best for: Cost-effective applications, fast inference, long-context understanding
var model = AI21Provider.Jamba15Mini(apiKey);
```

### Jurassic-2 Models

Proven foundation models with excellent instruction following:

```csharp
// Jurassic-2 Ultra - Most powerful foundation model
// Best for: Complex tasks, nuanced understanding, high-quality generation
var model = AI21Provider.Jurassic2Ultra(apiKey);

// Jurassic-2 Mid - Balanced performance and efficiency
// Best for: General-purpose applications, cost-effective solutions
var model = AI21Provider.Jurassic2Mid(apiKey);
```

### Custom Models

```csharp
// Use any AI21 Labs model by ID
var model = AI21Provider.ChatModel("your-model-id", apiKey);
```

## Streaming

All models support streaming responses:

```csharp
using AiSdk.Providers.AI21;

var model = AI21Provider.Jamba15Large(apiKey);

await foreach (var chunk in model.StreamAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Write a short story about a robot" }
    }
}))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
    else if (chunk.Type == ChunkType.Finish)
    {
        Console.WriteLine($"\n\nTokens used: {chunk.Usage?.TotalTokens}");
    }
}
```

## Advanced Configuration

### Using Configuration Object

```csharp
using AiSdk.Providers.AI21;

var config = new AI21Configuration
{
    ApiKey = apiKey,
    BaseUrl = "https://api.ai21.com/studio/v1",  // Default
    TimeoutSeconds = 120
};

var model = AI21Provider.CreateChatModel("jamba-1.5-large", config);
```

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};

var model = AI21Provider.CreateChatModel(
    "jamba-1.5-large",
    apiKey,
    httpClient: httpClient
);
```

### Temperature and Other Parameters

```csharp
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Generate creative text" }
    },
    Temperature = 0.9,      // Higher = more creative (0.0 - 2.0)
    TopP = 0.95,           // Nucleus sampling (0.0 - 1.0)
    MaxTokens = 2000,      // Maximum response length
    StopSequences = new[] { "\n\n", "END" }  // Stop generation on these
});
```

## Function Calling

AI21 Labs models support function calling and tool use:

```csharp
using System.Text.Json;
using AiSdk.Providers.AI21;

var model = AI21Provider.Jamba15Large(apiKey);

var tools = new List<Tool>
{
    new Tool
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
                    ""enum"": [""celsius"", ""fahrenheit""],
                    ""description"": ""Temperature unit""
                }
            },
            ""required"": [""location""]
        }")
    }
};

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "What's the weather in San Francisco?" }
    },
    Tools = tools
});

if (result.ToolCalls?.Count > 0)
{
    var toolCall = result.ToolCalls[0];
    Console.WriteLine($"Tool: {toolCall.ToolName}");
    Console.WriteLine($"Arguments: {toolCall.Arguments}");

    // Execute the function and send back the result
    var functionResult = GetWeather(toolCall.Arguments);

    var finalResult = await model.GenerateAsync(new LanguageModelCallOptions
    {
        Messages = new[]
        {
            new Message { Role = MessageRole.User, Content = "What's the weather in San Francisco?" },
            new Message
            {
                Role = MessageRole.Assistant,
                Content = null,
                ToolCalls = result.ToolCalls
            },
            new Message
            {
                Role = MessageRole.Tool,
                Content = functionResult,
                Name = toolCall.ToolCallId
            }
        },
        Tools = tools
    });

    Console.WriteLine(finalResult.Text);
}
```

## Multi-Turn Conversations

```csharp
var messages = new List<Message>
{
    new Message { Role = MessageRole.System, Content = "You are a helpful coding assistant." },
    new Message { Role = MessageRole.User, Content = "Write a function to calculate fibonacci numbers" }
};

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = messages
});

// Add assistant's response to conversation
messages.Add(new Message
{
    Role = MessageRole.Assistant,
    Content = result.Text
});

// Continue conversation
messages.Add(new Message
{
    Role = MessageRole.User,
    Content = "Can you optimize it with memoization?"
});

var result2 = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = messages
});
```

## Error Handling

```csharp
using AiSdk.Providers.AI21.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (AI21Exception ex)
{
    Console.WriteLine($"AI21 Labs Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network Error: {ex.Message}");
}
```

## Model Selection Guide

### For Long-Context Tasks
- **Jamba 1.5 Large**: 256K context window, best for document analysis, large codebases
- **Jamba 1.5 Mini**: 256K context window, cost-effective for long-context tasks

### For General Tasks
- **Jurassic-2 Ultra**: Excellent instruction following and generation quality
- **Jurassic-2 Mid**: Balanced performance for general-purpose applications

### For Complex Reasoning
- **Jamba 1.5 Large**: Strong reasoning with long-context understanding
- **Jurassic-2 Ultra**: Nuanced understanding for complex tasks

### For Cost-Effective Solutions
- **Jamba 1.5 Mini**: Fast inference with 256K context
- **Jurassic-2 Mid**: Balanced performance and cost

## Best Practices

1. **API Key Management**: Always use environment variables
   ```csharp
   var apiKey = Environment.GetEnvironmentVariable("AI21_API_KEY")
       ?? throw new InvalidOperationException("AI21_API_KEY not set");
   ```

2. **Rate Limiting**: Implement exponential backoff for retries
   ```csharp
   async Task<LanguageModelGenerateResult> GenerateWithRetry(int maxRetries = 3)
   {
       for (int i = 0; i < maxRetries; i++)
       {
           try
           {
               return await model.GenerateAsync(options);
           }
           catch (AI21Exception ex) when (ex.StatusCode == 429)
           {
               await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
           }
       }
       throw new Exception("Max retries exceeded");
   }
   ```

3. **Temperature Settings**:
   - Use 0.0-0.3 for factual, deterministic outputs
   - Use 0.7-0.9 for creative, diverse outputs
   - Use 1.0+ for highly creative or random outputs

4. **Context Window**: Leverage the 256K context window in Jamba models
   - Jamba 1.5 Large/Mini: 256K tokens
   - Jurassic-2 models: Standard context windows

5. **Streaming for Long Responses**: Always use streaming for responses over 500 tokens
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

## Use Cases

### Document Analysis with Long Context

```csharp
var model = AI21Provider.Jamba15Large(apiKey);

// Analyze a large document (up to 256K tokens)
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.User,
            Content = $"Analyze this document and provide key insights:\n\n{largeDocument}"
        }
    },
    Temperature = 0.3
});
```

### Code Generation

```csharp
var model = AI21Provider.Jamba15Large(apiKey);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Write a REST API in C# using minimal APIs" }
    },
    Temperature = 0.2
});
```

### Creative Writing

```csharp
var model = AI21Provider.Jurassic2Ultra(apiKey);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Write a sci-fi short story" }
    },
    Temperature = 0.9,
    MaxTokens = 2000
});
```

### Multi-Document Question Answering

```csharp
var model = AI21Provider.Jamba15Large(apiKey);

// Leverage 256K context to analyze multiple documents
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.User,
            Content = $"Based on these documents:\n\nDocument 1: {doc1}\n\nDocument 2: {doc2}\n\nDocument 3: {doc3}\n\nAnswer: {question}"
        }
    },
    Temperature = 0.2
});
```

## Troubleshooting

### Authentication Errors
Ensure your API key is valid and properly set:
```bash
echo $AI21_API_KEY
```

### Rate Limits
If you hit rate limits, implement retry logic with exponential backoff.

### Timeout Errors
Increase timeout for longer generations:
```csharp
var config = new AI21Configuration
{
    ApiKey = apiKey,
    TimeoutSeconds = 300  // 5 minutes
};
```

### Context Length Errors
For Jamba models, you can use up to 256K tokens. Monitor your token usage:
```csharp
var result = await model.GenerateAsync(options);
Console.WriteLine($"Input tokens: {result.Usage.InputTokens}");
Console.WriteLine($"Output tokens: {result.Usage.OutputTokens}");
```

## Pricing and Performance

AI21 Labs offers:
- Competitive pricing for Jamba models
- Industry-leading 256K context window
- Fast inference with efficient architecture
- Enterprise support available
- Check [AI21 Labs pricing](https://www.ai21.com/pricing) for details

## Key Features of Jamba Models

### 256K Context Window
Jamba models support the longest context window in the industry:
- Analyze entire codebases
- Process multiple documents simultaneously
- Maintain context in very long conversations
- Handle complex multi-step reasoning tasks

### Efficient Architecture
Jamba uses a hybrid architecture combining:
- Transformer layers for attention
- Mamba layers for efficiency
- Mixture of Experts (MoE) for scaling

### Multilingual Capabilities
Strong support for multiple languages:
- English (primary)
- Spanish, French, Portuguese
- And many more

## Resources

- [AI21 Labs Documentation](https://docs.ai21.com/)
- [API Reference](https://docs.ai21.com/reference/api-reference)
- [Model Cards](https://www.ai21.com/jamba)
- [AI21 Studio](https://studio.ai21.com/)

## License

This provider is part of the AI SDK .NET project and follows the same license.

## Contributing

Contributions are welcome! Please submit issues and pull requests to the main AI SDK .NET repository.
