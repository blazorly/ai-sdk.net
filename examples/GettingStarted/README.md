# Getting Started with AI SDK for .NET

Welcome to the AI SDK for .NET! This guide will help you get started with building AI-powered applications in C#.

## What is AI SDK for .NET?

The AI SDK for .NET is a comprehensive framework for integrating AI capabilities into your .NET applications. Inspired by Vercel's AI SDK, it provides a unified, provider-agnostic API for working with various AI models.

## Key Features

- **Provider Agnostic**: Use OpenAI, Anthropic, Google, and other providers with the same API
- **Streaming Support**: Real-time token streaming for responsive UIs
- **Function Calling**: Let AI models call your functions and tools
- **Structured Output**: Generate strongly-typed C# objects from AI responses
- **Type Safe**: Full IntelliSense support and compile-time checking
- **Async/Await**: Built on modern async patterns for optimal performance

## Installation

Once published to NuGet, install the core SDK:

```bash
dotnet add package AiSdk
```

Install provider packages as needed:

```bash
dotnet add package AiSdk.Providers.OpenAI
dotnet add package AiSdk.Providers.Anthropic
dotnet add package AiSdk.Providers.Google
```

## Quick Start

### 1. Basic Text Generation

```csharp
using AiSdk;
using AiSdk.Providers.OpenAI;

// Initialize the provider
var openai = new OpenAIProvider(apiKey: "your-api-key");
var model = openai.ChatModel("gpt-4");

// Generate text
var result = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "Explain quantum computing in simple terms"
    });

Console.WriteLine(result.Text);
```

### 2. Streaming Responses

```csharp
// Stream tokens as they're generated
await foreach (var chunk in AiClient.StreamTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "Write a short story about a robot"
    }))
{
    if (chunk.Delta != null)
    {
        Console.Write(chunk.Delta);
    }
}
```

### 3. Structured Output

```csharp
// Define a model class
public class Recipe
{
    public required string Name { get; set; }
    public required List<string> Ingredients { get; set; }
    public required List<string> Instructions { get; set; }
}

// Generate structured data
var result = await AiClient.GenerateObjectAsync<Recipe>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Create a recipe for chocolate chip cookies"
    });

Recipe recipe = result.Object;
Console.WriteLine($"Recipe: {recipe.Name}");
```

### 4. Function Calling

```csharp
using AiSdk.Tools;

// Define a tool
var weatherTool = Tool.Create<WeatherInput, WeatherOutput>(
    name: "get_weather",
    description: "Get current weather for a city",
    execute: (input) => GetWeather(input.City)
);

// Use with AI
var result = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "What's the weather in London?",
        Tools = new[] { weatherTool.Definition }
    });

// Execute tool if called
if (result.ToolCalls != null)
{
    foreach (var toolCall in result.ToolCalls)
    {
        var output = await weatherTool.ExecuteAsync(toolCall.Arguments);
        Console.WriteLine($"Weather: {output.Temperature}°C");
    }
}
```

## Core Concepts

### Models

Models are the AI systems you interact with. Different providers offer different models:

```csharp
// OpenAI
var gpt4 = openai.ChatModel("gpt-4");
var gpt35 = openai.ChatModel("gpt-3.5-turbo");

// Anthropic
var claude = anthropic.ChatModel("claude-3-opus-20240229");

// Google
var gemini = google.ChatModel("gemini-pro");
```

### Options

Control model behavior with options:

```csharp
new GenerateTextOptions
{
    System = "You are a helpful assistant",
    Prompt = "Your question here",
    MaxTokens = 1000,
    Temperature = 0.7,  // 0.0 = deterministic, 2.0 = creative
    TopP = 0.9,
    StopSequences = new[] { "END" }
}
```

### Messages

For multi-turn conversations:

```csharp
var messages = new List<Message>
{
    new Message(MessageRole.System, "You are a helpful assistant"),
    new Message(MessageRole.User, "Hello!"),
    new Message(MessageRole.Assistant, "Hi! How can I help?"),
    new Message(MessageRole.User, "What's the weather?")
};

var result = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions { Messages = messages }
);
```

### Usage and Costs

Track token usage:

```csharp
var result = await AiClient.GenerateTextAsync(model, options);

Console.WriteLine($"Prompt tokens: {result.Usage.PromptTokens}");
Console.WriteLine($"Completion tokens: {result.Usage.CompletionTokens}");
Console.WriteLine($"Total tokens: {result.Usage.TotalTokens}");
```

## Examples

The SDK includes comprehensive examples:

### [StreamingExample](../StreamingExample/)
Learn how to implement real-time streaming for responsive UIs.

### [FunctionCallingExample](../FunctionCallingExample/)
Discover how to extend AI capabilities with custom tools and functions.

### [StructuredOutputExample](../StructuredOutputExample/)
Generate strongly-typed C# objects from AI responses.

## Configuration

### Environment Variables

Store API keys securely:

```bash
export OPENAI_API_KEY="sk-..."
export ANTHROPIC_API_KEY="sk-ant-..."
```

```csharp
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
var openai = new OpenAIProvider(apiKey: apiKey);
```

### Dependency Injection

Integrate with .NET DI:

```csharp
services.AddSingleton<ILanguageModel>(sp =>
{
    var apiKey = configuration["OpenAI:ApiKey"];
    var openai = new OpenAIProvider(apiKey: apiKey);
    return openai.ChatModel("gpt-4");
});
```

## Best Practices

### 1. Error Handling

Always handle potential errors:

```csharp
try
{
    var result = await AiClient.GenerateTextAsync(model, options);
}
catch (ApiCallError ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (InvalidPromptError ex)
{
    Console.WriteLine($"Invalid prompt: {ex.Message}");
}
```

### 2. Cancellation

Support cancellation for long-running operations:

```csharp
var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(30));

try
{
    var result = await AiClient.GenerateTextAsync(
        model,
        options,
        cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation timed out");
}
```

### 3. Rate Limiting

Implement rate limiting for production:

```csharp
using Polly;

var retryPolicy = Policy
    .Handle<ApiCallError>()
    .WaitAndRetryAsync(
        retryCount: 3,
        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
    );

await retryPolicy.ExecuteAsync(async () =>
{
    return await AiClient.GenerateTextAsync(model, options);
});
```

### 4. Prompt Engineering

Write effective prompts:

```csharp
new GenerateTextOptions
{
    System = "You are an expert software architect. " +
             "Provide detailed, technical explanations. " +
             "Use code examples when helpful.",
    Prompt = "Explain the repository pattern in C# with examples"
}
```

## Common Patterns

### Chat Bot

```csharp
var messages = new List<Message>();

while (true)
{
    Console.Write("You: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput)) break;

    messages.Add(new Message(MessageRole.User, userInput));

    var result = await AiClient.GenerateTextAsync(
        model,
        new GenerateTextOptions { Messages = messages }
    );

    messages.Add(new Message(MessageRole.Assistant, result.Text));
    Console.WriteLine($"AI: {result.Text}");
}
```

### Data Processing Pipeline

```csharp
var items = new[] { "item1", "item2", "item3" };

var results = await Task.WhenAll(items.Select(async item =>
{
    var result = await AiClient.GenerateObjectAsync<Analysis>(
        model,
        new GenerateObjectOptions
        {
            Prompt = $"Analyze: {item}"
        }
    );
    return result.Object;
}));
```

### Progressive Enhancement

```csharp
// Start with a simple response
var draft = await AiClient.GenerateTextAsync(model, new GenerateTextOptions
{
    Prompt = "Write a draft email",
    Temperature = 0.8
});

// Refine it
var refined = await AiClient.GenerateTextAsync(model, new GenerateTextOptions
{
    Messages = new[]
    {
        new Message(MessageRole.User, "Write a draft email"),
        new Message(MessageRole.Assistant, draft.Text),
        new Message(MessageRole.User, "Make it more professional and concise")
    },
    Temperature = 0.3
});
```

## Running This Example

```bash
cd examples/GettingStarted
dotnet run
```

Note: This example requires provider packages to be implemented. See other examples for working demonstrations with mock models.

## Next Steps

1. **Explore Examples**: Check out the other example projects
2. **Read Documentation**: Visit the full API documentation
3. **Join Community**: Participate in discussions and issues on GitHub
4. **Build Something**: Start integrating AI into your application!

## Resources

- [API Reference](../../docs/api-reference.md)
- [Provider Documentation](../../docs/providers.md)
- [Best Practices Guide](../../docs/best-practices.md)
- [GitHub Repository](https://github.com/your-repo/ai-sdk.net)

## Support

- 📫 Issues: [GitHub Issues](https://github.com/your-repo/ai-sdk.net/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/your-repo/ai-sdk.net/discussions)
- 📖 Docs: [Documentation Site](https://ai-sdk-net.dev)

## License

This SDK is open source and available under the MIT License.
