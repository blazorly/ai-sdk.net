# Z.AI Provider

The Z.AI provider enables integration with [Z.AI's open platform](https://z.ai), offering access to powerful large language models including GLM-4 series and CodeGeeX for code generation.

## Features

- **Chat Completions**: General-purpose conversation and text generation
- **Code Generation**: Specialized CodeGeeX models for programming tasks
- **Deep Thinking**: Advanced reasoning mode for complex problem-solving
- **Streaming Support**: Real-time response streaming
- **Tool Calling**: Function calling capabilities
- **Extended Context**: Models with up to 128K token context windows

## Supported Models

### General Purpose Models
- **glm-4.7**: Latest general-purpose model for conversations, writing, and analysis
- **glm-4.6**: Previous generation model for general tasks
- **glm-4-32b-0414-128k**: Extended context model (128K tokens) for long document processing

### Code-Focused Models
- **codegeex-4**: Specialized model optimized for code generation, debugging, and technical tasks

## Configuration

```csharp
using AiSdk.Providers.ZAI;

// Create configuration
var config = new ZAIConfiguration
{
    ApiKey = "your-z.ai-api-key",
    BaseUrl = "https://api.z.ai/api/paas/v4/", // Optional, this is the default
    TimeoutSeconds = 100 // Optional
};

// Create provider
var provider = new ZAIProvider(config);
```

## Usage Examples

### Basic Chat

```csharp
using AiSdk.Providers.ZAI;
using AiSdk.Abstractions;

var config = new ZAIConfiguration { ApiKey = "your-api-key" };
var provider = new ZAIProvider(config);

// Use the latest GLM-4.7 model
var model = provider.GLM47();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.System, "You are a helpful assistant."),
        new Message(MessageRole.User, "What is quantum computing?")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Code Generation

```csharp
var provider = new ZAIProvider(config);

// Use CodeGeeX-4 for code tasks
var codeModel = provider.CodeGeeX4();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a C# function to calculate Fibonacci numbers")
    }
};

var result = await codeModel.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Streaming Response

```csharp
var model = provider.GLM47();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Tell me a story about AI")
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

### Extended Context Model

```csharp
var provider = new ZAIProvider(config);

// Use the 128K context model for long documents
var longContextModel = provider.GLM432B128K();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Analyze this long document: " + longDocument)
    }
};

var result = await longContextModel.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Custom Model

```csharp
var provider = new ZAIProvider(config);

// Use any Z.AI model by ID
var customModel = provider.ChatModel("glm-4.6");

var result = await customModel.GenerateAsync(options);
```

### Static Factory Methods

```csharp
// Create model directly without provider instance
var model = ZAIProvider.CreateChatModel(
    modelId: "glm-4.7",
    apiKey: "your-api-key"
);

var result = await model.GenerateAsync(options);
```

## Advanced Features

### Deep Thinking Mode

Z.AI supports a "thinking" mode for complex reasoning tasks. While not directly exposed in the API, the reasoning content is automatically included in responses when applicable.

### Tool Calling

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "What's the weather in Paris?")
    },
    Tools = new List<ToolDefinition>
    {
        new ToolDefinition
        {
            Name = "get_weather",
            Description = "Get the current weather for a location",
            Parameters = JsonDocument.Parse(@"{
                ""type"": ""object"",
                ""properties"": {
                    ""location"": { ""type"": ""string"" }
                }
            }")
        }
    }
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments}");
    }
}
```

## API Reference

### ZAIConfiguration

- `ApiKey` (required): Your Z.AI API key
- `BaseUrl` (optional): API base URL (default: "https://api.z.ai/api/paas/v4/")
- `TimeoutSeconds` (optional): Request timeout in seconds

### ZAIProvider Methods

- `GLM47()`: Creates a GLM-4.7 model instance
- `GLM46()`: Creates a GLM-4.6 model instance
- `GLM432B128K()`: Creates a GLM-4-32B with 128K context model instance
- `CodeGeeX4()`: Creates a CodeGeeX-4 code model instance
- `ChatModel(string modelId)`: Creates a model with custom model ID
- `CreateChatModel(...)`: Static factory method for creating models

## Getting an API Key

1. Visit [Z.AI Open Platform](https://z.ai)
2. Sign up for an account
3. Navigate to API keys section
4. Generate a new API key

## Additional Resources

- [Z.AI Documentation](https://docs.z.ai)
- [Z.AI Models](https://docs.z.ai/guides/llm)
- [API Reference](https://docs.z.ai/api-reference)

## OpenAI Compatibility

Z.AI's API is OpenAI-compatible, which means you can also use the OpenAI provider with Z.AI's base URL if preferred. However, using the dedicated Z.AI provider provides better support for Z.AI-specific features like deep thinking mode and proper model naming.
