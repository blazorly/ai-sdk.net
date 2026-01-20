# AiSdk.Providers.Baseten

Baseten provider for AI SDK .NET - integrates powerful open-source models like Llama 3, Mistral, and Mixtral with the unified AI SDK interface.

## Features

- **Production-Ready Inference** - Scalable model serving with Baseten's infrastructure
- **Chat Completions** - Llama 3, Mistral 7B, WizardLM-2, Mixtral models
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **OpenAI-Compatible API** - Familiar API format for easy integration
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Baseten
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Baseten;

// Create a model using convenience method
var model = BasetenProvider.Llama3_70B("your-api-key");

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain machine learning in one sentence")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration

```csharp
var config = new BasetenConfiguration
{
    ApiKey = "baseten_...",                                  // Required
    BaseUrl = "https://api.baseten.co/v1",                  // Optional (default shown)
    TimeoutSeconds = 60                                      // Optional (default: 100)
};

var model = BasetenProvider.CreateChatModel("meta-llama-3-70b-instruct", config);
```

### Environment Variables

```csharp
var config = new BasetenConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("BASETEN_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var llama8b = BasetenProvider.Llama3_8B("your-api-key");           // meta-llama-3-8b-instruct
var llama70b = BasetenProvider.Llama3_70B("your-api-key");         // meta-llama-3-70b-instruct
var mistral = BasetenProvider.Mistral7B("your-api-key");           // mistral-7b-instruct
var wizard = BasetenProvider.WizardLM2_8x22B("your-api-key");      // wizardlm-2-8x22b
var mixtral = BasetenProvider.Mixtral8x7B("your-api-key");         // mixtral-8x7b-instruct

// Or use specific model ID
var model = BasetenProvider.ChatModel("meta-llama-3-70b-instruct", "your-api-key");
```

## Usage Examples

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a poem about artificial intelligence")
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
        new Message(MessageRole.User, "What's the weather in New York?")
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

### Multi-Turn Conversations

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a helpful assistant."),
    new Message(MessageRole.User, "What is machine learning?"),
    new Message(MessageRole.Assistant, "Machine learning is a subset of AI that enables systems to learn from data."),
    new Message(MessageRole.User, "Give me an example")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Baseten.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (BasetenException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (BasetenException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (BasetenException ex)
{
    Console.WriteLine($"Baseten error: {ex.Message} (Status: {ex.StatusCode})");
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
      "baseten": {
        "ApiKey": "baseten_...",
        "DefaultModel": "meta-llama-3-70b-instruct",
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
    options.DefaultProvider = "baseten";
    options.Providers.Add("baseten", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:baseten:ApiKey"]!,
        DefaultModel = "meta-llama-3-70b-instruct"
    });
});
```

## Model Comparison

| Model | Model ID | Parameters | Use Case |
|-------|----------|------------|----------|
| Llama 3 8B | `meta-llama-3-8b-instruct` | 8B | Fast, efficient for most tasks |
| Llama 3 70B | `meta-llama-3-70b-instruct` | 70B | High quality, complex reasoning |
| Mistral 7B | `mistral-7b-instruct` | 7B | Instruction following, efficiency |
| WizardLM-2 8x22B | `wizardlm-2-8x22b` | 8x22B | Advanced reasoning, complex tasks |
| Mixtral 8x7B | `mixtral-8x7b-instruct` | 8x7B | Balanced performance, MoE architecture |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision | ❌ Not supported |
| JSON Mode | ⏸ Depends on model |
| Embeddings | ⏸ Check model capabilities |

## Performance Tips

1. **Choose the Right Model**:
   - Use Llama 3 8B or Mistral 7B for fast, efficient inference
   - Use Llama 3 70B for high-quality outputs requiring complex reasoning
   - Use WizardLM-2 8x22B for advanced reasoning tasks
   - Use Mixtral 8x7B for balanced performance with mixture-of-experts

2. **Timeouts**: Set appropriate `TimeoutSeconds` for your use case
   ```csharp
   var config = new BasetenConfiguration
   {
       ApiKey = "baseten_...",
       TimeoutSeconds = 60  // Adjust based on expected response time
   };
   ```

3. **Streaming**: Use streaming for long responses to improve perceived performance
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

4. **HttpClient Reuse**: The provider reuses HttpClient instances automatically

5. **Cancellation**: Always pass `CancellationToken` for long-running operations
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
   var result = await model.GenerateAsync(options, cts.Token);
   ```

6. **Temperature Settings**:
   - Lower temperature (0.1-0.5): More focused, deterministic outputs
   - Medium temperature (0.6-0.8): Balanced creativity and coherence
   - Higher temperature (0.9-1.0): More creative, diverse outputs

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `invalid_api_key` | Invalid authentication |
| 429 | `rate_limit_exceeded` | Too many requests |
| 400 | `invalid_request_error` | Bad request |
| 500 | `server_error` | Baseten server error |

## Why Baseten?

Baseten provides production-ready infrastructure for deploying and serving ML models:

- **Scalability**: Automatic scaling based on demand
- **Reliability**: High availability and fault tolerance
- **Performance**: Optimized inference with GPU acceleration
- **Open Models**: Access to popular open-source models like Llama 3, Mistral, Mixtral
- **OpenAI Compatibility**: Easy migration from OpenAI with compatible API
- **Cost-Effective**: Competitive pricing for model serving

Perfect for:
- Production applications requiring reliable model serving
- Applications needing open-source model alternatives
- Teams wanting infrastructure management handled
- Projects requiring scalable ML inference
- Cost-conscious workloads with high throughput needs

## Getting an API Key

1. Visit [Baseten](https://www.baseten.co)
2. Sign up for an account
3. Navigate to your account settings
4. Create a new API key
5. Copy the key (starts with `baseten_`)

## Model Deployment

Baseten also allows you to deploy your own custom models. Check the Baseten documentation for deploying custom models and using them with this provider by specifying the model ID.

## Links

- [Baseten Documentation](https://docs.baseten.co)
- [Baseten API Reference](https://docs.baseten.co/api-reference)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
