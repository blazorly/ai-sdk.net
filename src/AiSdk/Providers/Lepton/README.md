# AiSdk.Providers.Lepton

Lepton AI provider for AI SDK .NET - integrates powerful open-source models like Llama, Mixtral, WizardLM, and DBRX with the unified AI SDK interface.

## Features

- **High-Performance Models** - Access to Llama 3, Mixtral, WizardLM-2, and DBRX models
- **Chat Completions** - Full conversational AI capabilities
- **Streaming Support** - Real-time token-by-token streaming
- **Function Calling** - Tool/function calling capabilities
- **OpenAI-Compatible API** - Familiar API format for easy integration
- **Flexible Configuration** - API key, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk.Providers.Lepton
```

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Lepton;

// Create a model using convenience method
var model = LeptonProvider.Llama3_70B("your-api-key");

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
var config = new LeptonConfiguration
{
    ApiKey = "your-api-key",                               // Required
    BaseUrl = "https://api.lepton.ai/api/v1",             // Optional (default shown)
    TimeoutSeconds = 60                                    // Optional (default: 100)
};

var model = LeptonProvider.CreateChatModel("llama3-70b", config);
```

### Environment Variables

```csharp
var config = new LeptonConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("LEPTON_API_KEY")!
};
```

### Available Models

```csharp
// Convenience methods
var llama8b = LeptonProvider.Llama3_8B("your-api-key");          // llama3-8b
var llama70b = LeptonProvider.Llama3_70B("your-api-key");        // llama3-70b
var mixtral = LeptonProvider.Mixtral8x7B("your-api-key");        // mixtral-8x7b
var wizard = LeptonProvider.WizardLM2_7B("your-api-key");        // wizardlm-2-7b
var dbrx = LeptonProvider.DBRX("your-api-key");                  // dbrx

// Or use specific model ID
var model = LeptonProvider.ChatModel("llama3-70b", "your-api-key");
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

### Multi-Turn Conversations

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a helpful assistant."),
    new Message(MessageRole.User, "What is the capital of France?"),
    new Message(MessageRole.Assistant, "The capital of France is Paris."),
    new Message(MessageRole.User, "What's the population?")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Lepton.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (LeptonException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key");
}
catch (LeptonException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (LeptonException ex)
{
    Console.WriteLine($"Lepton AI error: {ex.Message} (Status: {ex.StatusCode})");
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
      "lepton": {
        "ApiKey": "your-api-key",
        "DefaultModel": "llama3-70b",
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
    options.DefaultProvider = "lepton";
    options.Providers.Add("lepton", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:lepton:ApiKey"]!,
        DefaultModel = "llama3-70b"
    });
});
```

## Model Comparison

| Model | Model ID | Context | Best For |
|-------|----------|---------|----------|
| Llama 3 8B | `llama3-8b` | 8K | Fast inference, efficient tasks |
| Llama 3 70B | `llama3-70b` | 8K | Complex reasoning, high quality |
| Mixtral 8x7B | `mixtral-8x7b` | 32K | Long context, expert reasoning |
| WizardLM-2 7B | `wizardlm-2-7b` | 8K | Instruction following |
| DBRX | `dbrx` | 32K | High performance, Databricks optimized |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Function/Tool Calling | ✅ Full support |
| Vision | ❌ Not supported by Lepton AI |
| JSON Mode | ⏸ Planned |
| Embeddings | ⏸ Planned |

## Performance Tips

1. **Choose the Right Model**:
   - Use Llama 3 8B for fast, efficient tasks
   - Use Llama 3 70B for complex reasoning and high-quality outputs
   - Use Mixtral 8x7B for long-context tasks (32K tokens)
   - Use WizardLM-2 7B for instruction-following tasks
   - Use DBRX for high-performance enterprise workloads

2. **Timeouts**: Set appropriate `TimeoutSeconds` for your use case
   ```csharp
   var config = new LeptonConfiguration
   {
       ApiKey = "your-api-key",
       TimeoutSeconds = 30
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

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `invalid_api_key` | Invalid authentication |
| 429 | `rate_limit_exceeded` | Too many requests |
| 400 | `invalid_request_error` | Bad request |
| 500 | `server_error` | Lepton AI server error |

## Why Lepton AI?

Lepton AI provides:

- **Powerful Models**: Access to state-of-the-art open-source models
- **Cost-Effective**: Competitive pricing for high-quality inference
- **OpenAI Compatibility**: Easy migration from OpenAI-compatible APIs
- **Flexible Deployment**: Cloud-based inference with multiple model options
- **Enterprise Ready**: Production-grade infrastructure and reliability

Perfect for:
- Production applications requiring reliable inference
- Projects leveraging open-source LLMs
- Cost-sensitive workloads
- Applications requiring diverse model options
- Teams migrating from OpenAI-compatible APIs

## Getting an API Key

1. Visit [Lepton AI](https://lepton.ai)
2. Sign up for an account
3. Navigate to API Keys section
4. Create a new API key
5. Copy and securely store your key

## Links

- [Lepton AI Documentation](https://lepton.ai/docs)
- [Lepton AI API Reference](https://lepton.ai/docs/api-reference)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
