# Fireworks AI Provider

A provider for AI SDK .NET that enables fast inference with open-source LLMs through Fireworks AI's OpenAI-compatible API. Fireworks AI offers blazing-fast inference for models like Llama, Qwen, Mixtral, and custom fine-tuned models.

## Features

- OpenAI-compatible API for easy integration
- Ultra-fast inference optimized for production workloads
- Support for popular open-source models (Llama, Qwen, Mixtral, etc.)
- Function calling with FireFunction V2
- Custom model deployment support
- Streaming responses with usage statistics
- Comprehensive error handling

## Installation

The Fireworks provider is included in the main AiSdk package:

```bash
dotnet add package AiSdk
```

## Quick Start

### Get Your API Key

1. Sign up at [Fireworks AI](https://fireworks.ai/)
2. Navigate to your account settings to get your API key
3. Set it as an environment variable:
   ```bash
   export FIREWORKS_API_KEY="your-api-key-here"
   ```

### Basic Usage

```csharp
using AiSdk.Providers.Fireworks;

// Get your API key from environment variable
var apiKey = Environment.GetEnvironmentVariable("FIREWORKS_API_KEY");

// Use a popular model
var model = FireworksProvider.Llama3_1_70B(apiKey);

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

### Llama Models

```csharp
// Llama 3.1 70B Instruct - Flagship model with excellent reasoning
var model = FireworksProvider.Llama3_1_70B(apiKey);

// Llama 3.1 8B Instruct - Efficient and fast
var model = FireworksProvider.Llama3_1_8B(apiKey);

// Llama 3.3 70B Instruct - Latest version with improved capabilities
var model = FireworksProvider.Llama3_3_70B(apiKey);
```

### Function Calling

```csharp
// FireFunction V2 - State-of-the-art function calling model
var model = FireworksProvider.FireFunction_V2(apiKey);
```

### Qwen Models

```csharp
// Qwen 2.5 72B Instruct - Multilingual with strong coding abilities
var model = FireworksProvider.Qwen2_5_72B(apiKey);

// Qwen 2.5 Coder 32B - Specialized for code generation
var model = FireworksProvider.Qwen2_5_Coder_32B(apiKey);
```

### Mixtral Models

```csharp
// Mixtral 8x7B Instruct - Mixture of Experts with 32K context
var model = FireworksProvider.Mixtral_8x7B(apiKey);

// Mixtral 8x22B Instruct - Larger version with 64K context
var model = FireworksProvider.Mixtral_8x22B(apiKey);
```

### DeepSeek Models

```csharp
// DeepSeek V3 - Powerful reasoning model
var model = FireworksProvider.DeepSeek_V3(apiKey);
```

### Custom Models

```csharp
// Use any Fireworks model by ID
var model = FireworksProvider.ChatModel("accounts/fireworks/models/your-model-id", apiKey);

// Or use account-based models (fine-tuned models)
var model = FireworksProvider.ChatModel("accounts/YOUR_ACCOUNT/models/YOUR_MODEL", apiKey);
```

## Streaming

All models support streaming responses:

```csharp
using AiSdk.Providers.Fireworks;

var model = FireworksProvider.Llama3_1_70B(apiKey);

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
using AiSdk.Providers.Fireworks;

var config = new FireworksConfiguration
{
    ApiKey = apiKey,
    BaseUrl = "https://api.fireworks.ai/inference/v1",  // Default
    TimeoutSeconds = 120
};

var model = FireworksProvider.CreateChatModel("accounts/fireworks/models/llama-v3p1-70b-instruct", config);
```

### Custom HTTP Client

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};

var model = FireworksProvider.CreateChatModel(
    "accounts/fireworks/models/llama-v3p1-70b-instruct",
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

Fireworks AI supports function calling with the FireFunction V2 model:

```csharp
using System.Text.Json;
using AiSdk.Providers.Fireworks;

var model = FireworksProvider.FireFunction_V2(apiKey);

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
using AiSdk.Providers.Fireworks.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (FireworksException ex)
{
    Console.WriteLine($"Fireworks AI Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network Error: {ex.Message}");
}
```

## Model Selection Guide

### For General Tasks
- **Llama 3.3 70B**: Best overall quality and reasoning
- **Llama 3.1 70B**: Strong performance, well-tested
- **Llama 3.1 8B**: Fast inference for simpler tasks

### For Code Generation
- **Qwen 2.5 Coder 32B**: Specialized for programming tasks
- **Llama 3.1 70B**: Excellent code understanding and generation

### For Function Calling
- **FireFunction V2**: Purpose-built for tool use and orchestration

### For Multilingual Tasks
- **Qwen 2.5 72B**: Strong multilingual capabilities

### For Complex Reasoning
- **DeepSeek V3**: Advanced reasoning abilities
- **Mixtral 8x22B**: Large context window with strong performance

## Best Practices

1. **API Key Management**: Always use environment variables
   ```csharp
   var apiKey = Environment.GetEnvironmentVariable("FIREWORKS_API_KEY")
       ?? throw new InvalidOperationException("FIREWORKS_API_KEY not set");
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
           catch (FireworksException ex) when (ex.StatusCode == 429)
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

4. **Context Window**: Be mindful of token limits
   - Llama 3.1: 128K tokens
   - Mixtral 8x7B: 32K tokens
   - Mixtral 8x22B: 64K tokens

5. **Streaming for Long Responses**: Always use streaming for responses over 500 tokens
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

## Pricing and Performance

Fireworks AI offers:
- Serverless inference up to 6,000 RPM
- 2.5 billion tokens/day capacity
- Pay-per-use pricing (check [Fireworks AI pricing](https://fireworks.ai/pricing))
- Ultra-low latency (0.25s for Mixtral 8x22b)

## Use Cases

### Code Generation
```csharp
var model = FireworksProvider.Qwen2_5_Coder_32B(apiKey);
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
var model = FireworksProvider.Llama3_3_70B(apiKey);
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

### Data Analysis
```csharp
var model = FireworksProvider.Qwen2_5_72B(apiKey);
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Analyze this dataset and provide insights: [data]" }
    },
    Temperature = 0.3
});
```

## Troubleshooting

### Authentication Errors
Ensure your API key is valid and properly set:
```bash
echo $FIREWORKS_API_KEY
```

### Rate Limits
If you hit rate limits, implement retry logic with exponential backoff.

### Timeout Errors
Increase timeout for longer generations:
```csharp
var config = new FireworksConfiguration
{
    ApiKey = apiKey,
    TimeoutSeconds = 300  // 5 minutes
};
```

### Model Not Found
Verify the model ID is correct. For account-based models, use:
```
accounts/YOUR_ACCOUNT_ID/models/YOUR_MODEL_NAME
```

## Resources

- [Fireworks AI Documentation](https://docs.fireworks.ai/)
- [OpenAI Compatibility Guide](https://docs.fireworks.ai/tools-sdks/openai-compatibility)
- [Available Models](https://fireworks.ai/models)
- [API Reference](https://docs.fireworks.ai/api-reference/introduction)

## License

This provider is part of the AI SDK .NET project and follows the same license.

## Contributing

Contributions are welcome! Please submit issues and pull requests to the main AI SDK .NET repository.
