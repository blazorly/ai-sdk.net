# AiSdk.Providers.Cloudflare

Cloudflare Workers AI provider for AI SDK .NET - integrates Cloudflare's serverless AI models running on their global edge network with the unified AI SDK interface.

## Features

- **Edge AI Inference** - Run AI models on Cloudflare's global network
- **Multiple Models** - Llama 3, Mistral 7B, Intel Neural Chat 7B
- **Serverless** - No infrastructure management required
- **Low Latency** - Models run close to your users
- **Streaming Support** - Real-time token-by-token streaming
- **Cost-Effective** - Pay per request with generous free tier
- **Flexible Configuration** - API key, account ID, base URL, timeouts

## Installation

```bash
dotnet add package AiSdk
```

The Cloudflare provider is included in the consolidated AiSdk package.

## Quick Start

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.Cloudflare;

// Create a provider instance
var config = new CloudflareConfiguration
{
    ApiKey = "your-api-key",
    AccountId = "your-account-id"
};
var provider = new CloudflareProvider(config);

// Use the Llama 3 8B model
var model = provider.Llama3_8B();

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
var config = new CloudflareConfiguration
{
    ApiKey = "your-api-key",                                      // Required
    AccountId = "your-account-id",                                // Required
    BaseUrl = "https://api.cloudflare.com/client/v4",            // Optional (default shown)
    TimeoutSeconds = 60                                           // Optional (default: 100)
};

var provider = new CloudflareProvider(config);
```

### Environment Variables

```csharp
var config = new CloudflareConfiguration
{
    ApiKey = Environment.GetEnvironmentVariable("CLOUDFLARE_API_KEY")!,
    AccountId = Environment.GetEnvironmentVariable("CLOUDFLARE_ACCOUNT_ID")!
};
```

### Available Models

```csharp
// Using provider instance methods
var provider = new CloudflareProvider(config);

var llama8b = provider.Llama3_8B();          // @cf/meta/llama-3-8b-instruct
var llama70b = provider.Llama3_70B();        // @cf/meta/llama-3-70b-instruct
var mistral = provider.Mistral7B();          // @cf/mistral/mistral-7b-instruct-v0.1
var neuralChat = provider.NeuralChat7B();    // @cf/intel/neural-chat-7b

// Or use specific model ID
var model = provider.ChatModel("@cf/meta/llama-3-8b-instruct");

// Static convenience methods for quick initialization
var llama8bModel = CloudflareProvider.Llama3_8B("your-api-key", "your-account-id");
var llama70bModel = CloudflareProvider.Llama3_70B("your-api-key", "your-account-id");
var mistralModel = CloudflareProvider.Mistral7B("your-api-key", "your-account-id");
var neuralChatModel = CloudflareProvider.NeuralChat7B("your-api-key", "your-account-id");
```

## Usage Examples

### General Chat with Llama 3

```csharp
var provider = new CloudflareProvider(config);
var model = provider.Llama3_8B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a creative story about AI")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Using Llama 3 70B for Complex Tasks

```csharp
var provider = new CloudflareProvider(config);
var model = provider.Llama3_70B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Explain the theory of relativity in detail")
    },
    Temperature = 0.7,
    MaxTokens = 1000
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Instruction-Following with Mistral 7B

```csharp
var provider = new CloudflareProvider(config);
var model = provider.Mistral7B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "List the steps to bake a cake")
    },
    Temperature = 0.5
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Natural Conversations with Neural Chat

```csharp
var provider = new CloudflareProvider(config);
var model = provider.NeuralChat7B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.System, "You are a friendly assistant"),
        new Message(MessageRole.User, "Hello! How are you?")
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
        new Message(MessageRole.User, "Write a detailed explanation of machine learning")
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

### Advanced Options

```csharp
var options = new LanguageModelCallOptions
{
    Messages = messages,
    MaxTokens = 2000,
    Temperature = 0.7,
    TopP = 0.9
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
    new Message(MessageRole.User, "What is its population?")
};

var options = new LanguageModelCallOptions { Messages = messages };
var result = await model.GenerateAsync(options);
```

### Error Handling

```csharp
using AiSdk.Providers.Cloudflare.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
}
catch (CloudflareException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Invalid API key or account ID");
}
catch (CloudflareException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limited. Error code: " + ex.ErrorCode);
}
catch (CloudflareException ex)
{
    Console.WriteLine($"Cloudflare error: {ex.Message} (Status: {ex.StatusCode})");
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
      "cloudflare": {
        "ApiKey": "your-api-key",
        "AccountId": "your-account-id",
        "DefaultModel": "@cf/meta/llama-3-8b-instruct",
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
    options.DefaultProvider = "cloudflare";
    options.Providers.Add("cloudflare", new ProviderConfiguration
    {
        ApiKey = builder.Configuration["AiSdk:Providers:cloudflare:ApiKey"]!,
        AccountId = builder.Configuration["AiSdk:Providers:cloudflare:AccountId"]!,
        DefaultModel = "@cf/meta/llama-3-8b-instruct"
    });
});
```

## Model Comparison

| Model | Model ID | Parameters | Strengths | Use Case |
|-------|----------|------------|-----------|----------|
| Llama 3 8B | `@cf/meta/llama-3-8b-instruct` | 8B | Fast, efficient, general-purpose | Quick responses, general chat |
| Llama 3 70B | `@cf/meta/llama-3-70b-instruct` | 70B | High quality, complex reasoning | Detailed analysis, complex tasks |
| Mistral 7B | `@cf/mistral/mistral-7b-instruct-v0.1` | 7B | Instruction-following | Task completion, structured output |
| Neural Chat 7B | `@cf/intel/neural-chat-7b` | 7B | Natural conversations | Chat applications, dialogue |

## Supported Features

| Feature | Status |
|---------|--------|
| Chat Completions | ✅ Full support |
| Streaming | ✅ Full support |
| System Messages | ✅ Full support |
| Multi-turn Conversations | ✅ Full support |
| Temperature Control | ✅ Full support |
| Top-P Sampling | ✅ Full support |
| Max Tokens | ✅ Full support |
| Function/Tool Calling | ❌ Not supported by Workers AI |
| Vision | ❌ Not supported by Workers AI |
| JSON Mode | ⏸ Planned (available in Workers AI) |
| Embeddings | ⏸ Available via separate endpoint |

## Performance Tips

1. **Choose the Right Model**:
   - Use **Llama 3 8B** for fast, general-purpose tasks
   - Use **Llama 3 70B** for complex reasoning and high-quality output
   - Use **Mistral 7B** for instruction-following tasks
   - Use **Neural Chat 7B** for natural conversations

2. **Optimize for Speed**:
   ```csharp
   var config = new CloudflareConfiguration
   {
       ApiKey = "your-api-key",
       AccountId = "your-account-id",
       TimeoutSeconds = 30  // Shorter timeout for quick responses
   };

   var options = new LanguageModelCallOptions
   {
       Temperature = 0.5,  // Lower temperature for faster, more deterministic output
       MaxTokens = 500     // Limit tokens for quicker responses
   };
   ```

3. **Streaming for Long Responses**: Use streaming for detailed explanations to improve perceived performance
   ```csharp
   await foreach (var chunk in model.StreamAsync(options))
   {
       // Process chunks as they arrive
   }
   ```

4. **Edge Performance**: Cloudflare Workers AI runs on their global network, so responses are served from the nearest data center

5. **HttpClient Reuse**: The provider reuses HttpClient instances automatically for optimal performance

6. **Cancellation**: Always pass `CancellationToken` for long-running operations
   ```csharp
   using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
   var result = await model.GenerateAsync(options, cts.Token);
   ```

## Common Error Codes

| Status Code | Error Code | Description |
|-------------|------------|-------------|
| 401 | `7003` | Invalid authentication (API key or account ID) |
| 403 | `7004` | Access forbidden |
| 429 | `10000` | Rate limit exceeded |
| 400 | Various | Bad request format |
| 500 | Various | Cloudflare server error |

## Why Cloudflare Workers AI?

Cloudflare Workers AI offers unique advantages:

- **Global Edge Network**: Models run on Cloudflare's global network for low latency
- **Serverless**: No infrastructure to manage or scale
- **Cost-Effective**: Pay per request with generous free tier
- **Fast Deployment**: Deploy globally in seconds
- **Built-in Scaling**: Automatically scales to handle any load
- **Multiple Models**: Choose from various open-source models
- **Integrated Ecosystem**: Works seamlessly with Workers, Pages, and other Cloudflare products

Perfect for:
- Applications requiring low latency globally
- Serverless architectures
- Edge computing use cases
- Cost-conscious deployments
- Rapid prototyping and development
- Applications with variable or unpredictable load
- Developers already using Cloudflare's ecosystem

## Getting Started with Cloudflare Workers AI

1. Visit [Cloudflare Dashboard](https://dash.cloudflare.com)
2. Sign up for a free account
3. Navigate to Workers & Pages → AI
4. Note your Account ID from the dashboard
5. Create an API token with "Workers AI" permissions:
   - Go to My Profile → API Tokens
   - Create Token → Edit Cloudflare Workers AI template
   - Grant "Workers AI" permissions
   - Copy the token

## Account ID Location

Your Account ID can be found in multiple places:
- Cloudflare Dashboard → Workers & Pages (in the sidebar)
- Any Workers AI page URL: `https://dash.cloudflare.com/{ACCOUNT_ID}/ai`
- Account Settings page

## Use Case Examples

### Chatbot with Llama 3

```csharp
var provider = new CloudflareProvider(config);
var model = provider.Llama3_8B();

// Build conversation history
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a helpful chatbot.")
};

while (true)
{
    var userInput = Console.ReadLine();
    messages.Add(new Message(MessageRole.User, userInput));

    var options = new LanguageModelCallOptions { Messages = messages };
    var result = await model.GenerateAsync(options);

    Console.WriteLine(result.Text);
    messages.Add(new Message(MessageRole.Assistant, result.Text));
}
```

### Content Generation

```csharp
var provider = new CloudflareProvider(config);
var model = provider.Llama3_70B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Write a blog post about the future of AI")
    },
    Temperature = 0.8,  // Higher creativity
    MaxTokens = 2000
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Task Instructions with Mistral

```csharp
var provider = new CloudflareProvider(config);
var model = provider.Mistral7B();

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Create a numbered list of steps to deploy a web application")
    },
    Temperature = 0.3  // More deterministic for instructions
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Links

- [Cloudflare Workers AI Documentation](https://developers.cloudflare.com/workers-ai/)
- [Workers AI REST API](https://developers.cloudflare.com/workers-ai/get-started/rest-api/)
- [Workers AI Models](https://developers.cloudflare.com/workers-ai/models/)
- [Cloudflare Dashboard](https://dash.cloudflare.com)
- [AI SDK Documentation](../../README.md)
- [Examples](../../examples/)

## Additional Resources

- [OpenAI Compatible Endpoints](https://developers.cloudflare.com/workers-ai/configuration/open-ai-compatibility/)
- [Workers AI Pricing](https://developers.cloudflare.com/workers-ai/platform/pricing/)
- [Workers AI Limits](https://developers.cloudflare.com/workers-ai/platform/limits/)
- [Vercel AI SDK Integration](https://developers.cloudflare.com/workers-ai/configuration/ai-sdk/)

## License

Apache-2.0
