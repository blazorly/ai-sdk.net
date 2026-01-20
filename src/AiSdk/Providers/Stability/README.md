# AiSdk.Providers.Stability

Stability AI provider for AI SDK .NET - integrates StableLM language models with the unified AI SDK interface.

## Overview

**Important Note**: Stability AI is primarily known for their image generation models (Stable Diffusion, SDXL). This provider focuses on their language models (StableLM series) for text generation and chat capabilities.

StableLM models are open-source language models that can be:
- **Self-hosted** using frameworks like vLLM, LM Studio, or similar with OpenAI-compatible endpoints
- **Accessed through third-party platforms** that host StableLM models
- **Future-ready** for potential Stability AI hosted text generation API endpoints

## Features

- **StableLM Models** - Support for StableLM 2 12B, StableLM 2 Zephyr 1.6B, and other variants
- **OpenAI-Compatible API** - Works with any OpenAI-compatible endpoint hosting StableLM
- **Streaming Support** - Real-time token-by-token streaming with SSE
- **Tool Use** - Function calling capabilities (if supported by deployment)
- **Flexible Configuration** - API key, base URL, timeouts
- **Extensible Design** - Structure ready for future image generation features

## Installation

```bash
dotnet add package AiSdk.Providers.Stability
```

## Quick Start

### Self-Hosted StableLM with vLLM

First, deploy StableLM using vLLM:

```bash
# Install vLLM
pip install vllm

# Run StableLM 2 12B with OpenAI-compatible API
python -m vllm.entrypoints.openai.api_server \
    --model stabilityai/stablelm-2-12b \
    --host 0.0.0.0 \
    --port 8000
```

Then use it with the SDK:

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Stability;

// Create a model instance
var model = StabilityProvider.CreateChatModel(
    modelId: "stablelm-2-12b",
    apiKey: "dummy-key-for-local",  // vLLM doesn't require auth by default
    baseUrl: "http://localhost:8000/v1"
);

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain quantum computing in simple terms")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Configuration

### Basic Configuration

```csharp
var config = new StabilityConfiguration
{
    ApiKey = "your-api-key",                        // Required (or dummy for local)
    BaseUrl = "http://localhost:8000/v1",          // Your StableLM deployment URL
    TimeoutSeconds = 90                            // Optional (default: 100)
};

var model = StabilityProvider.CreateChatModel("stablelm-2-12b", config);
```

### Environment Variables

```csharp
var config = new StabilityConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("STABILITY_API_KEY") ?? "local",
    BaseUrl = Environment.GetEnvironmentVariable("STABLELM_BASE_URL") ?? "http://localhost:8000/v1"
};
```

### Available Model Shortcuts

```csharp
// StableLM 2 12B (recommended for general use)
var model1 = StabilityProvider.StableLM2_12B(
    apiKey: "your-key",
    baseUrl: "http://localhost:8000/v1"
);

// StableLM 2 Zephyr 1.6B (lightweight, fast)
var model2 = StabilityProvider.StableLM2Zephyr_1_6B(
    apiKey: "your-key",
    baseUrl: "http://localhost:8000/v1"
);

// StableLM 3B
var model3 = StabilityProvider.StableLM_3B(
    apiKey: "your-key",
    baseUrl: "http://localhost:8000/v1"
);

// Generic chat model (for any variant)
var model4 = StabilityProvider.ChatModel(
    modelId: "stabilityai/stablelm-tuned-alpha-7b",
    apiKey: "your-key",
    baseUrl: "http://localhost:8000/v1"
);
```

## Usage Examples

### Basic Chat Completion

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.System, "You are a helpful coding assistant."),
        new Message(MessageRole.User, "Write a Python function to calculate factorial")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Streaming

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a short story about a robot learning to paint")
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

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 2048,
    Temperature = 0.7,
    TopP = 0.9,
    StopSequences = new List<string> { "\n\n", "END" }
};

var result = await model.GenerateAsync(options);
```

### Multi-turn Conversation

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a history expert."),
    new Message(MessageRole.User, "Who was the first person in space?"),
    new Message(MessageRole.Assistant, "Yuri Gagarin was the first person in space on April 12, 1961."),
    new Message(MessageRole.User, "What spacecraft did he use?")
};

var options = new LanguageModelCallOptions
{
    Messages = messages
};

var result = await model.GenerateAsync(options);
```

### Tool Use (if supported)

```csharp
var tools = new List<ToolDefinition>
{
    new ToolDefinition
    {
        Name = "get_weather",
        Description = "Get current weather for a location",
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
        // Execute tool and send result back...
    }
}
```

### Error Handling

```csharp
using AiSdk.Providers.Stability.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (StabilityException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key or authentication failed");
}
catch (StabilityException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited");
}
catch (StabilityException ex)
{
    Console.WriteLine($"Stability error: {ex.Message} (Status: {ex.StatusCode}, Type: {ex.ErrorType})");
}
catch (AiSdkException ex)
{
    Console.WriteLine($"SDK error: {ex.Message}");
}
```

## Deployment Options

### 1. vLLM (Recommended)

```bash
# Install vLLM
pip install vllm

# Run with OpenAI-compatible API
python -m vllm.entrypoints.openai.api_server \
    --model stabilityai/stablelm-2-12b \
    --host 0.0.0.0 \
    --port 8000 \
    --api-key your-secret-key
```

### 2. LM Studio

1. Download and install [LM Studio](https://lmstudio.ai/)
2. Load a StableLM model from Hugging Face
3. Start the local server (default: http://localhost:1234/v1)
4. Use that URL as the baseUrl in your configuration

### 3. Third-Party Platforms

Use platforms like Replicate, Together AI, or Hugging Face Inference API that host StableLM models.

### 4. Future: Stability AI Hosted API

If Stability AI launches a hosted text generation API, update the baseUrl to their endpoint.

## Configuration with ASP.NET Core

### appsettings.json

```json
{
  "AiSdk": {
    "Providers": {
      "stability": {
        "ApiKey": "your-api-key",
        "BaseUrl": "http://localhost:8000/v1",
        "DefaultModel": "stablelm-2-12b",
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
    options.DefaultProvider = "stability";
    options.Providers.Add("stability", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:stability:ApiKey"]!,
        BaseUrl = builder.Configuration["AiSdk:Providers:stability:BaseUrl"]!,
        DefaultModel = "stablelm-2-12b"
    });
});
```

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| Tool Use | ✅ Supported (deployment-dependent) |
| System Messages | ✅ Full support |
| Multi-turn Chat | ✅ Full support |
| Image Generation | 🔮 Future feature (Stable Diffusion integration) |
| Vision (Images) | ⏸ Not yet available in StableLM |
| Audio | ⏸ Not yet available |

## StableLM Model Comparison

| Model | Parameters | Use Case | Recommended For |
|-------|-----------|----------|-----------------|
| StableLM 2 12B | 12B | General chat, complex tasks | Production use |
| StableLM 2 Zephyr 1.6B | 1.6B | Fast responses, simple tasks | Low-latency applications |
| StableLM Base Alpha 3B | 3B | Balanced performance | Development/testing |
| StableLM Tuned Alpha 7B | 7B | Instruction-following | Fine-tuned applications |

## Important Notes

1. **Deployment Required**: Unlike cloud-based providers, StableLM models require you to deploy them yourself or use a third-party hosting service.

2. **OpenAI Compatibility**: This provider uses the OpenAI-compatible chat completions format, which is supported by most modern LLM serving frameworks.

3. **Model Selection**: Choose the right model size for your use case. Larger models (12B) are more capable but slower and require more resources.

4. **Authentication**: For local deployments, you can use a dummy API key. For production deployments, always use proper authentication.

5. **Base URL Format**: Ensure your base URL ends with `/v1` for OpenAI compatibility (e.g., `http://localhost:8000/v1`).

## Future Roadmap

This provider is designed with extensibility in mind for future Stability AI features:

- **Image Generation**: Integration with Stable Diffusion API for text-to-image, image-to-image
- **Image Editing**: Inpainting, outpainting, image manipulation
- **Multimodal Models**: Future StableLM variants with vision capabilities
- **Hosted API**: Support for Stability AI's hosted text generation service (if launched)
- **Advanced Features**: Prompt weighting, negative prompts for image generation

## Performance Tips

1. **Model Selection**: Use StableLM 2 Zephyr 1.6B for low-latency applications
2. **Streaming**: Always use streaming for long responses to improve user experience
3. **Batch Processing**: Process multiple requests in parallel when possible
4. **GPU Acceleration**: Deploy vLLM with GPU support for better performance
5. **Quantization**: Use quantized models (4-bit, 8-bit) to reduce memory usage

## Common Error Types

| Error Code | Description | Solution |
|------------|-------------|----------|
| 401 | Authentication failed | Check API key (or disable auth for local) |
| 404 | Model not found | Verify model is loaded in your deployment |
| 429 | Rate limited | Add retry logic or reduce request rate |
| 500 | Server error | Check deployment logs |
| 503 | Service unavailable | Ensure deployment is running |

## Links

- [Stability AI](https://stability.ai/)
- [StableLM on GitHub](https://github.com/Stability-AI/StableLM)
- [StableLM on Hugging Face](https://huggingface.co/stabilityai)
- [vLLM Documentation](https://docs.vllm.ai/)
- [LM Studio](https://lmstudio.ai/)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## License

Apache-2.0
