# OpenRouter Provider for AI SDK .NET

OpenRouter is a unified API that provides access to 100+ AI models from multiple providers including OpenAI, Anthropic, Google, Meta, Mistral, and many more. This provider enables seamless integration with OpenRouter's multi-provider AI routing platform.

## Features

- **100+ Models**: Access models from OpenAI, Anthropic, Google, Meta, Mistral, DeepSeek, and more
- **Unified API**: Single API for all providers with consistent interface
- **Cost Optimization**: Compare pricing across providers and choose the best option
- **OpenAI-Compatible**: Uses the familiar OpenAI chat completions format
- **Custom Headers**: Support for HTTP-Referer and X-Title headers
- **Streaming Support**: Full support for streaming responses
- **Tool Calling**: Complete function calling support
- **Model Discovery**: Easy access to newly released models

## Installation

Add the AI SDK .NET package to your project:

```bash
dotnet add package AiSdk
```

## Getting Started

### 1. Get Your API Key

Sign up for an OpenRouter account and get your API key:
- Visit [https://openrouter.ai/keys](https://openrouter.ai/keys)
- Create a new API key
- Add credits to your account at [https://openrouter.ai/credits](https://openrouter.ai/credits)

### 2. Basic Usage

```csharp
using AiSdk;
using AiSdk.Providers.OpenRouter;

// Create a model using a convenience method
var model = OpenRouterProvider.Claude3_5_Sonnet("your-api-key");

// Generate text
var result = await model.GenerateTextAsync("What is the capital of France?");
Console.WriteLine(result);
```

### 3. Using Different Models

```csharp
// OpenAI GPT-4 Turbo
var gpt4 = OpenRouterProvider.GPT4_Turbo("your-api-key");

// Anthropic Claude 3.5 Sonnet
var claude = OpenRouterProvider.Claude3_5_Sonnet("your-api-key");

// Google Gemini Pro
var gemini = OpenRouterProvider.GeminiPro("your-api-key");

// Meta Llama 3 70B
var llama = OpenRouterProvider.Llama3_70B("your-api-key");

// Mistral Mixtral 8x7B
var mixtral = OpenRouterProvider.Mixtral8x7B("your-api-key");
```

## Configuration

### Basic Configuration

```csharp
var config = new OpenRouterConfiguration
{
    ApiKey = "your-api-key",
    BaseUrl = "https://openrouter.ai/api/v1", // Optional, this is the default
    TimeoutSeconds = 120 // Optional timeout
};

var model = OpenRouterProvider.CreateChatModel("openai/gpt-4-turbo", config);
```

### Advanced Configuration with Custom Headers

OpenRouter supports optional headers for attribution and analytics:

```csharp
var config = new OpenRouterConfiguration
{
    ApiKey = "your-api-key",
    SiteUrl = "https://your-site.com", // Sent as HTTP-Referer header
    AppName = "My AI Application", // Sent as X-Title header
    TimeoutSeconds = 120
};

var model = OpenRouterProvider.CreateChatModel("anthropic/claude-3.5-sonnet", config);
```

These headers help OpenRouter:
- Attribute API usage to your site
- Track application analytics
- Some models may require the HTTP-Referer header

## Available Models

### Popular Models by Provider

#### OpenAI Models
- `openai/gpt-4-turbo` - Most capable GPT-4 model
- `openai/gpt-4o` - Flagship multimodal model
- `openai/gpt-4o-mini` - Affordable small model
- `openai/gpt-3.5-turbo` - Fast, inexpensive model

#### Anthropic Models
- `anthropic/claude-opus-4.5` - Most powerful Claude model
- `anthropic/claude-3.5-sonnet` - Balanced performance
- `anthropic/claude-3.5-haiku` - Fastest Claude model

#### Google Models
- `google/gemini-pro` - Powerful reasoning model
- `google/gemini-1.5-pro` - Most capable Gemini
- `google/gemini-2.0-flash` - Latest fast model

#### Meta Models
- `meta-llama/llama-3.3-70b-instruct` - Latest 70B model
- `meta-llama/llama-3.1-405b-instruct` - Largest Llama
- `meta-llama/llama-3-70b-instruct` - Llama 3 70B

#### Mistral Models
- `mistralai/mistral-large` - Flagship Mistral model
- `mistralai/mixtral-8x7b-instruct` - Mixture-of-experts

#### DeepSeek Models
- `deepseek/deepseek-v3` - Latest DeepSeek model
- `deepseek/deepseek-chat` - General-purpose chat

### Using Any Model

You can use any model available on OpenRouter:

```csharp
// Generic method for any model
var model = OpenRouterProvider.ChatModel("provider/model-id", "your-api-key");

// Examples
var qwen = OpenRouterProvider.ChatModel("qwen/qwen-2.5-72b-instruct", "your-api-key");
var cohere = OpenRouterProvider.ChatModel("cohere/command-r-plus", "your-api-key");
var perplexity = OpenRouterProvider.ChatModel("perplexity/llama-3.1-sonar-huge", "your-api-key");
```

Visit [https://openrouter.ai/models](https://openrouter.ai/models) for the complete list of available models.

## Model Discovery

OpenRouter continuously adds new models. To discover available models:

1. Visit the [OpenRouter Models page](https://openrouter.ai/models)
2. Browse by provider, capabilities, or pricing
3. Use the model ID with `ChatModel()` method

```csharp
// Example: Using a newly released model
var newModel = OpenRouterProvider.ChatModel("new-provider/new-model", "your-api-key");
```

## Pricing Comparison

OpenRouter provides transparent pricing across providers. Example pricing (as of 2026):

| Model | Input (per 1M tokens) | Output (per 1M tokens) |
|-------|----------------------|------------------------|
| GPT-4 Turbo | $10.00 | $30.00 |
| Claude 3.5 Sonnet | $3.00 | $15.00 |
| Gemini Pro | $0.50 | $1.50 |
| Llama 3 70B | $0.60 | $0.80 |
| Mixtral 8x7B | $0.24 | $0.24 |

Check current pricing at [https://openrouter.ai/models](https://openrouter.ai/models)

## Usage Examples

### Text Generation

```csharp
using AiSdk;
using AiSdk.Providers.OpenRouter;

var model = OpenRouterProvider.Claude3_5_Sonnet("your-api-key");

var result = await model.GenerateTextAsync(
    "Explain quantum computing in simple terms",
    maxTokens: 500,
    temperature: 0.7
);

Console.WriteLine(result);
```

### Streaming Responses

```csharp
var model = OpenRouterProvider.GPT4_Turbo("your-api-key");

await foreach (var chunk in model.StreamTextAsync("Write a short story about AI"))
{
    Console.Write(chunk);
}
```

### Conversation with Messages

```csharp
var model = OpenRouterProvider.GeminiPro("your-api-key");

var messages = new List<Message>
{
    new Message { Role = MessageRole.System, Content = "You are a helpful coding assistant." },
    new Message { Role = MessageRole.User, Content = "How do I reverse a string in C#?" }
};

var result = await model.GenerateTextAsync(messages);
Console.WriteLine(result);
```

### Tool Calling (Function Calling)

```csharp
var model = OpenRouterProvider.Claude3_5_Sonnet("your-api-key");

var tools = new List<Tool>
{
    new Tool
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = JsonSerializer.Deserialize<JsonDocument>("""
        {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "The city and state, e.g. San Francisco, CA"
                }
            },
            "required": ["location"]
        }
        """)
    }
};

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message { Role = MessageRole.User, Content = "What's the weather in New York?" }
    },
    Tools = tools
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Any() == true)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Function: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments}");
    }
}
```

### Multi-Provider Routing Strategy

Compare responses from different models:

```csharp
var config = new OpenRouterConfiguration
{
    ApiKey = "your-api-key",
    SiteUrl = "https://your-site.com",
    AppName = "Multi-Model Comparison App"
};

var models = new[]
{
    OpenRouterProvider.CreateChatModel("openai/gpt-4-turbo", config),
    OpenRouterProvider.CreateChatModel("anthropic/claude-3.5-sonnet", config),
    OpenRouterProvider.CreateChatModel("google/gemini-pro", config)
};

var prompt = "Explain the benefits of renewable energy";

foreach (var model in models)
{
    Console.WriteLine($"\n=== {model.ModelId} ===");
    var result = await model.GenerateTextAsync(prompt);
    Console.WriteLine(result);
}
```

### Cost-Optimized Routing

Use cheaper models for simple tasks, premium models for complex ones:

```csharp
async Task<string> GenerateWithCostOptimization(string prompt)
{
    // Determine complexity (simple heuristic)
    bool isComplex = prompt.Length > 500 || prompt.Contains("analyze") || prompt.Contains("complex");

    var model = isComplex
        ? OpenRouterProvider.Claude3_5_Sonnet("your-api-key") // Premium model
        : OpenRouterProvider.Llama3_70B("your-api-key"); // Cost-effective model

    return await model.GenerateTextAsync(prompt);
}
```

### Custom HttpClient

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(5)
};

var config = new OpenRouterConfiguration
{
    ApiKey = "your-api-key"
};

var model = OpenRouterProvider.CreateChatModel("openai/gpt-4-turbo", config, httpClient);
```

## Error Handling

```csharp
using AiSdk.Providers.OpenRouter.Exceptions;

try
{
    var model = OpenRouterProvider.GPT4_Turbo("your-api-key");
    var result = await model.GenerateTextAsync("Hello!");
}
catch (OpenRouterException ex)
{
    Console.WriteLine($"OpenRouter Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Error Code: {ex.ErrorCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"General Error: {ex.Message}");
}
```

## Best Practices

### 1. API Key Security
```csharp
// Use environment variables or secure configuration
var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");
var model = OpenRouterProvider.Claude3_5_Sonnet(apiKey!);
```

### 2. Timeout Configuration
```csharp
var config = new OpenRouterConfiguration
{
    ApiKey = "your-api-key",
    TimeoutSeconds = 120 // Set appropriate timeout for your use case
};
```

### 3. Model Selection
- **Simple tasks**: Use cost-effective models (Llama, Mixtral)
- **Complex reasoning**: Use premium models (Claude, GPT-4)
- **Multimodal**: Use models that support images (GPT-4o, Claude)
- **Long context**: Use models with large context windows (Claude, Gemini)

### 4. Rate Limiting
OpenRouter has rate limits based on your plan. Implement retry logic:

```csharp
async Task<string> GenerateWithRetry(ILanguageModel model, string prompt, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await model.GenerateTextAsync(prompt);
        }
        catch (OpenRouterException ex) when (ex.StatusCode == 429)
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
        }
    }
    throw new Exception("Max retries exceeded");
}
```

## Resources

- **OpenRouter Website**: [https://openrouter.ai](https://openrouter.ai)
- **API Documentation**: [https://openrouter.ai/docs](https://openrouter.ai/docs)
- **Model List**: [https://openrouter.ai/models](https://openrouter.ai/models)
- **Pricing**: [https://openrouter.ai/models](https://openrouter.ai/models) (includes per-model pricing)
- **API Keys**: [https://openrouter.ai/keys](https://openrouter.ai/keys)
- **Credits**: [https://openrouter.ai/credits](https://openrouter.ai/credits)
- **Discord Community**: [https://discord.gg/openrouter](https://discord.gg/openrouter)

## Supported Features

- ✅ Text Generation
- ✅ Streaming
- ✅ Tool/Function Calling
- ✅ Multi-turn Conversations
- ✅ System Messages
- ✅ Temperature Control
- ✅ Max Tokens
- ✅ Top-P Sampling
- ✅ Stop Sequences
- ✅ Custom Headers (HTTP-Referer, X-Title)
- ✅ Error Handling
- ✅ 100+ Models from Multiple Providers

## License

This provider is part of the AI SDK .NET project and follows the same license terms.

## Contributing

Contributions are welcome! Please refer to the main AI SDK .NET repository for contribution guidelines.

## Support

For issues specific to this provider, please open an issue in the AI SDK .NET repository.
For OpenRouter-specific questions, refer to [OpenRouter's documentation](https://openrouter.ai/docs) or their Discord community.
