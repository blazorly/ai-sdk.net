# AI SDK for .NET

> **Status:** Phases 1-4 Complete ✅ | Actively Developed | .NET 10.0+

A comprehensive .NET SDK for integrating with AI providers, offering a unified interface for language models, embeddings, structured output, and streaming. Ported from the [Vercel AI SDK](https://sdk.vercel.ai).

## Current Status

| Phase | Status | Deliverables |
|-------|--------|-------------|
| **Phase 1** | ✅ Complete | Core abstractions, utilities, streaming |
| **Phase 2** | ✅ Complete | Core SDK + OpenAI provider |
| **Phase 3** | ✅ Complete | Anthropic, Azure, Google providers + examples |
| **Phase 4** | ✅ Complete | ASP.NET Core integration + web examples |
| **Phase 5** | 📋 Planned | Additional providers (Mistral, Groq, Cohere, etc.) |

**Test Coverage:** 182 tests passing (122 Core + 60 ASP.NET Core)

## Features

- **🚀 Idiomatic .NET** - Async/await, IAsyncEnumerable, records, dependency injection
- **🔌 Multiple Providers** - OpenAI, Anthropic (Claude), Azure OpenAI, Google (Gemini)
- **📦 ASP.NET Core Ready** - DI integration, health checks, SSE streaming middleware
- **🎯 Structured Output** - Generate typed objects with JSON schema validation
- **⚡ Real-time Streaming** - Server-sent events for chat and completions
- **🧪 Fully Tested** - Comprehensive test suite with 100% pass rate
- **📖 Production Ready** - Error handling, telemetry, logging, cancellation tokens

## Installation

### Core SDK + Providers

```bash
# Core abstractions and utilities
dotnet add package AiSdk.Abstractions
dotnet add package AiSdk.Core

# Provider packages (choose one or more)
dotnet add package AiSdk.Providers.OpenAI
dotnet add package AiSdk.Providers.Anthropic
dotnet add package AiSdk.Providers.Azure
dotnet add package AiSdk.Providers.Google
```

### ASP.NET Core Integration

```bash
dotnet add package AiSdk.AspNetCore
```

## Quick Start

### Console Application

```csharp
using AiSdk.Abstractions;
using AiSdk.Providers.OpenAI;

// Create a language model
var provider = new OpenAIProvider(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY")!);
var model = provider.ChatModel("gpt-4");

// Generate text
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message(MessageRole.User, "Tell me a joke about programming")
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);

// Stream text in real-time
await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
}
```

### ASP.NET Core Application

```csharp
using AiSdk.AspNetCore;
using AiSdk.Providers.Anthropic;

var builder = WebApplication.CreateBuilder(args);

// Register AI SDK with dependency injection
builder.Services.AddAiSdk(options =>
{
    options.DefaultProvider = "anthropic";
    options.Providers.Add("anthropic", new ProviderConfiguration
    {
        ApiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")!,
        DefaultModel = "claude-3-5-sonnet-20241022"
    });
});

var app = builder.Build();

// Enable SSE streaming middleware
app.UseAiSdkStreaming();

// Create chat endpoint with streaming
app.MapPost("/api/chat", async (ChatRequest request, ILanguageModel model) =>
{
    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.User, request.Message)
        }
    };

    return Results.Stream(async stream =>
    {
        await foreach (var chunk in model.StreamAsync(options))
        {
            if (chunk.Type == ChunkType.TextDelta)
            {
                await stream.WriteAsync($"data: {chunk.Delta}\n\n");
                await stream.FlushAsync();
            }
        }
    }, "text/event-stream");
});

app.Run();

record ChatRequest(string Message);
```

## Supported Providers

### Currently Available (Phase 1-4)
- ✅ **OpenAI** - GPT-4, GPT-3.5, GPT-4 Turbo
- ✅ **Anthropic** - Claude 3.5 Sonnet, Claude 3 Opus/Sonnet/Haiku
- ✅ **Azure OpenAI** - Azure-hosted GPT models
- ✅ **Google** - Gemini 1.5 Pro/Flash, Gemini 2.0

### Coming Soon (Phase 5+)
- 📋 Mistral, Groq, Cohere, Perplexity
- 📋 Amazon Bedrock (multi-provider aggregator)
- 📋 Google Vertex AI (multi-provider aggregator)
- 📋 Audio providers (Deepgram, ElevenLabs, AssemblyAI)

## Project Structure

```
ai-sdk.net/
├── src/
│   ├── AiSdk.Abstractions/          # Core interfaces (ILanguageModel, etc.)
│   ├── AiSdk.Core/                  # Utilities (streaming, JSON, SSE parsing)
│   ├── AiSdk.AspNetCore/            # ASP.NET Core integration
│   ├── AiSdk.Providers.OpenAI/      # OpenAI provider
│   ├── AiSdk.Providers.Anthropic/   # Anthropic (Claude) provider
│   ├── AiSdk.Providers.Azure/       # Azure OpenAI provider
│   └── AiSdk.Providers.Google/      # Google (Gemini) provider
├── tests/
│   ├── AiSdk.Abstractions.Tests/    # 4 tests
│   ├── AiSdk.Core.Tests/            # 118 tests
│   └── AiSdk.AspNetCore.Tests/      # 60 tests
└── examples/
    ├── GettingStartedExample/       # Basic usage patterns
    ├── StreamingExample/            # Real-time streaming
    ├── ToolCallingExample/          # Function/tool calling
    ├── StructuredOutputExample/     # Generate typed objects
    ├── MinimalApiExample/           # ASP.NET Core Minimal API
    ├── MvcExample/                  # ASP.NET Core MVC
    └── BlazorServerExample/         # Blazor Server with SignalR
```

## Example Applications

All examples are fully functional and can be run directly:

### Console Examples

```bash
# Basic usage patterns
cd examples/GettingStartedExample && dotnet run

# Real-time streaming
cd examples/StreamingExample && dotnet run

# Function calling with tools
cd examples/ToolCallingExample && dotnet run

# Structured output (typed objects)
cd examples/StructuredOutputExample && dotnet run
```

### Web Examples

```bash
# Minimal API (modern REST API)
cd examples/MinimalApiExample && dotnet run
# Visit: http://localhost:5000/swagger

# MVC application (traditional web app)
cd examples/MvcExample && dotnet run
# Visit: http://localhost:5001

# Blazor Server (interactive SPA)
cd examples/BlazorServerExample && dotnet run
# Visit: http://localhost:5002
```

## Advanced Features

### Structured Output

Generate strongly-typed objects with automatic JSON schema validation:

```csharp
public record Person(string Name, int Age, string City);

var options = new StreamObjectOptions<Person>
{
    Schema = JsonSchema.FromType<Person>(),
    Prompt = "Generate a person: John, 30 years old, lives in Seattle"
};

var person = await StreamObjectAsync(model, options);
Console.WriteLine($"{person.Name} is {person.Age} years old");
```

### Tool/Function Calling

Define tools that the model can call:

```csharp
var tools = new List<Tool>
{
    new Tool
    {
        Name = "get_weather",
        Description = "Get the current weather in a location",
        Parameters = JsonSchema.FromType<WeatherRequest>()
    }
};

var options = new LanguageModelCallOptions
{
    Messages = messages,
    Tools = tools
};

var result = await model.GenerateAsync(options);
if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        // Handle tool execution
    }
}
```

### Health Checks (ASP.NET Core)

Monitor AI service availability:

```csharp
builder.Services.AddAiSdk(options =>
{
    options.EnableHealthChecks = true;
});

app.MapHealthChecks("/health");
```

### Configuration (ASP.NET Core)

Configure via appsettings.json:

```json
{
  "AiSdk": {
    "DefaultProvider": "anthropic",
    "TimeoutSeconds": 120,
    "EnableHealthChecks": true,
    "EnableTelemetry": true,
    "Providers": {
      "anthropic": {
        "ApiKey": "sk-ant-...",
        "DefaultModel": "claude-3-5-sonnet-20241022",
        "TimeoutSeconds": 90
      },
      "openai": {
        "ApiKey": "sk-...",
        "DefaultModel": "gpt-4",
        "BaseUrl": "https://api.openai.com/v1"
      }
    }
  }
}
```

## Building from Source

```bash
# Clone repository
git clone https://github.com/ai-sdk-dotnet/ai-sdk.net.git
cd ai-sdk.net

# Build all projects
dotnet build

# Run tests (182 tests)
dotnet test

# Run specific example
cd examples/MinimalApiExample
dotnet run
```

### Requirements

- .NET 10.0 SDK or later
- API keys for providers you want to use

## Roadmap

See [DOTNET_PORTING_PLAN.md](../DOTNET_PORTING_PLAN.md) for the complete implementation plan.

**Next Up (Phase 5):**
- Mistral, Groq, Cohere providers
- Amazon Bedrock (multi-provider aggregator)
- Google Vertex AI (multi-provider aggregator)
- Audio providers (Deepgram, ElevenLabs, AssemblyAI)

## License

Apache-2.0

## Acknowledgments

This project is a .NET port of the [Vercel AI SDK](https://sdk.vercel.ai), bringing the excellent TypeScript SDK to the .NET ecosystem.

