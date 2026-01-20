# OpenAI-Compatible Provider

A universal provider for AI SDK .NET that enables connection to ANY OpenAI-compatible API endpoint. This includes local inference servers like Ollama, LocalAI, vLLM, and LM Studio, as well as cloud services that provide OpenAI-compatible APIs.

## Supported Platforms

- **Ollama** - Local inference server
- **LocalAI** - Self-hosted OpenAI alternative
- **vLLM** - High-throughput inference engine
- **LM Studio** - Desktop app for running local models
- **Text Generation WebUI** - Gradio-based web interface
- **Groq** - Cloud inference service
- **Together AI** - Cloud inference service
- **Any custom OpenAI-compatible endpoint**

## Installation

The OpenAI-Compatible provider is included in the main AiSdk package:

```bash
dotnet add package AiSdk
```

## Quick Start

### Using Ollama

```csharp
using AiSdk.Providers.OpenAICompatible;

// Simple usage with default Ollama endpoint (http://localhost:11434/v1)
var model = OpenAICompatibleProvider.ForOllama("llama2");

// Or with custom configuration
var model = OpenAICompatibleProvider.ForOllama(
    modelId: "llama2",
    baseUrl: "http://localhost:11434/v1"
);

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

### Using LocalAI

```csharp
using AiSdk.Providers.OpenAICompatible;

var model = OpenAICompatibleProvider.ForLocalAI(
    modelId: "gpt-3.5-turbo",
    baseUrl: "http://localhost:8080/v1"
);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Explain quantum computing" }
    },
    MaxTokens = 500
});
```

### Using vLLM

```csharp
using AiSdk.Providers.OpenAICompatible;

var model = OpenAICompatibleProvider.ForVLLM(
    modelId: "meta-llama/Llama-2-7b-chat-hf",
    baseUrl: "http://localhost:8000/v1"
);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.System, Content = "You are a helpful assistant." },
        new Message { Role = MessageRole.User, Content = "Write a Python function to sort a list" }
    },
    Temperature = 0.7
});
```

### Using LM Studio

```csharp
using AiSdk.Providers.OpenAICompatible;

// Default LM Studio endpoint (http://localhost:1234/v1)
var model = OpenAICompatibleProvider.ForLMStudio("local-model");

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Tell me a joke" }
    }
});
```

### Using Text Generation WebUI

```csharp
using AiSdk.Providers.OpenAICompatible;

var model = OpenAICompatibleProvider.ForTextGenerationWebUI(
    modelId: "TheBloke_Mistral-7B-Instruct-v0.2-GPTQ",
    baseUrl: "http://localhost:5000/v1"
);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "What is machine learning?" }
    }
});
```

### Using Groq (Cloud Service)

```csharp
using AiSdk.Providers.OpenAICompatible;

var model = OpenAICompatibleProvider.ForGroq(
    modelId: "llama2-70b-4096",
    apiKey: "your-groq-api-key"
);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Explain neural networks" }
    }
});
```

### Using Together AI (Cloud Service)

```csharp
using AiSdk.Providers.OpenAICompatible;

var model = OpenAICompatibleProvider.ForTogetherAI(
    modelId: "togethercomputer/llama-2-7b-chat",
    apiKey: "your-together-api-key"
);

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Write a haiku about coding" }
    }
});
```

## Custom Endpoints

For any other OpenAI-compatible API:

```csharp
using AiSdk.Providers.OpenAICompatible;

// Method 1: Using CreateChatModel with parameters
var model = OpenAICompatibleProvider.CreateChatModel(
    modelId: "my-model",
    baseUrl: "http://my-server:8000/v1",
    apiKey: "optional-api-key",  // null if not needed
    timeoutSeconds: 120
);

// Method 2: Using CreateChatModel with configuration object
var config = new OpenAICompatibleConfiguration
{
    BaseUrl = "http://my-server:8000/v1",
    ApiKey = "optional-api-key",  // null if not needed
    TimeoutSeconds = 120
};

var model = OpenAICompatibleProvider.CreateChatModel("my-model", config);
```

## Streaming

All OpenAI-compatible endpoints support streaming:

```csharp
using AiSdk.Providers.OpenAICompatible;

var model = OpenAICompatibleProvider.ForOllama("llama2");

await foreach (var chunk in model.StreamAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Write a short story" }
    }
}))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
}
```

## Advanced Configuration

### Custom HTTP Client

```csharp
using AiSdk.Providers.OpenAICompatible;

var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};

var model = OpenAICompatibleProvider.ForOllama(
    modelId: "llama2",
    httpClient: httpClient
);
```

### With Custom Headers

```csharp
using AiSdk.Providers.OpenAICompatible;

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");

var config = new OpenAICompatibleConfiguration
{
    BaseUrl = "http://localhost:11434/v1",
    TimeoutSeconds = 60
};

var model = new OpenAICompatibleChatLanguageModel("llama2", config, httpClient);
```

### Temperature and Other Parameters

```csharp
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message { Role = MessageRole.User, Content = "Generate creative text" }
    },
    Temperature = 0.9,      // Higher = more creative
    TopP = 0.95,           // Nucleus sampling
    MaxTokens = 1000,      // Maximum response length
    StopSequences = new[] { "\n\n", "END" }  // Stop generation on these
});
```

## Tool/Function Calling

If your OpenAI-compatible endpoint supports function calling:

```csharp
using System.Text.Json;

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
        new Message { Role = MessageRole.User, Content = "What's the weather in Paris?" }
    },
    Tools = tools
});

if (result.ToolCalls?.Count > 0)
{
    var toolCall = result.ToolCalls[0];
    Console.WriteLine($"Tool: {toolCall.ToolName}");
    Console.WriteLine($"Arguments: {toolCall.Arguments}");
}
```

## Common Model IDs

### Ollama
- `llama2` - Llama 2 7B
- `mistral` - Mistral 7B
- `codellama` - Code Llama
- `mixtral` - Mixtral 8x7B
- `phi` - Microsoft Phi-2
- `neural-chat` - Intel Neural Chat

### Groq
- `llama2-70b-4096` - Llama 2 70B
- `mixtral-8x7b-32768` - Mixtral 8x7B
- `gemma-7b-it` - Google Gemma 7B

### Together AI
- `togethercomputer/llama-2-7b-chat`
- `mistralai/Mistral-7B-Instruct-v0.2`
- `NousResearch/Nous-Hermes-2-Mixtral-8x7B-DPO`

## Error Handling

```csharp
using AiSdk.Providers.OpenAICompatible.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (OpenAICompatibleException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
```

## Setting Up Local Servers

### Ollama

1. Install Ollama: https://ollama.ai
2. Pull a model: `ollama pull llama2`
3. Server runs automatically at http://localhost:11434

```csharp
var model = OpenAICompatibleProvider.ForOllama("llama2");
```

### LocalAI

1. Run with Docker:
```bash
docker run -p 8080:8080 -v $PWD/models:/models -ti --rm quay.io/go-skynet/local-ai:latest
```

2. Use in code:
```csharp
var model = OpenAICompatibleProvider.ForLocalAI("gpt-3.5-turbo", "http://localhost:8080/v1");
```

### vLLM

1. Install: `pip install vllm`
2. Run server:
```bash
python -m vllm.entrypoints.openai.api_server --model meta-llama/Llama-2-7b-chat-hf
```

3. Use in code:
```csharp
var model = OpenAICompatibleProvider.ForVLLM("meta-llama/Llama-2-7b-chat-hf", "http://localhost:8000/v1");
```

### LM Studio

1. Download LM Studio: https://lmstudio.ai
2. Load a model and start the server (default port: 1234)
3. Use in code:
```csharp
var model = OpenAICompatibleProvider.ForLMStudio("local-model");
```

## Best Practices

1. **Timeout Configuration**: Set appropriate timeouts for local vs cloud endpoints
   ```csharp
   var config = new OpenAICompatibleConfiguration
   {
       BaseUrl = "http://localhost:11434/v1",
       TimeoutSeconds = 300  // 5 minutes for local inference
   };
   ```

2. **API Key Management**: Use environment variables for API keys
   ```csharp
   var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
   var model = OpenAICompatibleProvider.ForGroq("llama2-70b-4096", apiKey);
   ```

3. **Model Selection**: Choose models appropriate for your hardware/use case
   - Local: 7B models for consumer GPUs, 13B+ for workstations
   - Cloud: Larger models (70B+) for best quality

4. **Error Handling**: Always wrap API calls in try-catch blocks
   ```csharp
   try
   {
       var result = await model.GenerateAsync(options);
   }
   catch (OpenAICompatibleException ex)
   {
       // Handle API errors
   }
   catch (HttpRequestException ex)
   {
       // Handle network errors
   }
   ```

## Migration from OpenAI Provider

The OpenAI-Compatible provider uses the same API surface as the OpenAI provider, making migration simple:

```csharp
// Before (OpenAI)
using AiSdk.Providers.OpenAI;
var model = OpenAIProvider.CreateChatModel("gpt-4", apiKey);

// After (Ollama)
using AiSdk.Providers.OpenAICompatible;
var model = OpenAICompatibleProvider.ForOllama("llama2");

// The rest of your code stays the same
var result = await model.GenerateAsync(options);
```

## Troubleshooting

### Connection Refused
Ensure your local server is running and the port matches:
```bash
# Check if Ollama is running
curl http://localhost:11434/v1/models

# Check if vLLM is running
curl http://localhost:8000/v1/models
```

### Model Not Found
Verify the model ID matches what's available on your server:
```bash
# Ollama
ollama list

# LocalAI
curl http://localhost:8080/v1/models
```

### Timeout Errors
Increase timeout for slower local inference:
```csharp
var config = new OpenAICompatibleConfiguration
{
    BaseUrl = "http://localhost:11434/v1",
    TimeoutSeconds = 600  // 10 minutes
};
```

## License

This provider is part of the AI SDK .NET project and follows the same license.

## Contributing

Contributions are welcome! Please submit issues and pull requests to the main AI SDK .NET repository.
