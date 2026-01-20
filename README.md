# AI SDK for .NET

> **Status:** Phases 1-8 Complete ✅ | Production Ready | .NET 10.0+

A comprehensive .NET SDK for integrating with AI providers, offering a unified interface for language models, embeddings, structured output, and streaming. **Single package, 32 providers included.**

## Current Status

| Phase | Status | Deliverables |
|-------|--------|-------------|
| **Phase 1** | ✅ Complete | Core abstractions, utilities, streaming |
| **Phase 2** | ✅ Complete | Core SDK + OpenAI provider |
| **Phase 3** | ✅ Complete | Anthropic, Azure, Google providers + examples |
| **Phase 4** | ✅ Complete | ASP.NET Core integration + web examples |
| **Phase 5** | ✅ Complete | 8 additional providers (Mistral, Groq, Cohere, GoogleVertex, Bedrock, Replicate, Perplexity, DeepSeek, OpenAI-Compatible) |
| **Phase 6** | ✅ Complete | 6 additional providers (Vercel, xAI, HuggingFace, Cerebras, Fireworks, TogetherAI) |
| **Phase 7** | ✅ Complete | 6 additional providers (AI21, Cloudflare, Baseten, Lepton, Novita, Writer) |
| **Phase 8** | ✅ Complete | 7 additional providers (LlamaFile, Friendli, Portkey, Fal, Luma, Stability, OpenRouter) |

**Test Coverage:** 182 tests passing (122 Core + 60 ASP.NET Core)

## Features

- **📦 Single Package** - Everything in one NuGet package, no dependency hell
- **🚀 Idiomatic .NET** - Async/await, IAsyncEnumerable, records, dependency injection
- **🔌 32 Providers Included** - OpenAI, Anthropic, Azure, Google, Groq, Mistral, Cohere, GoogleVertex, AmazonBedrock, Replicate, Perplexity, DeepSeek, OpenAI-Compatible, Vercel, xAI, HuggingFace, Cerebras, Fireworks, TogetherAI, AI21, Cloudflare, Baseten, Lepton, Novita, Writer, LlamaFile, Friendli, Portkey, Fal, Luma, Stability, OpenRouter
- **🌐 ASP.NET Core Ready** - DI integration, health checks, SSE streaming middleware
- **🎯 Structured Output** - Generate typed objects with JSON schema validation
- **⚡ Real-time Streaming** - Server-sent events for chat and completions
- **🧪 Fully Tested** - Comprehensive test suite with 100% pass rate
- **📖 Production Ready** - Timeout enforcement, unified error handling, real health checks

## Installation

### Simplified Installation - One Package for Everything

```bash
# Single package includes all providers
dotnet add package AiSdk

# Optional: ASP.NET Core integration (if building web apps)
dotnet add package AiSdk.AspNetCore
```

That's it! All 32 providers are included in the single `AiSdk` package.

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

## Included Providers

All providers are included in the single `AiSdk` package - no need to install separate provider packages!

### Language Model Providers (32 Complete / 19+ Planned)

#### Priority 1 - Major Cloud Providers (All Complete ✅)

| Provider | Models | Status |
|----------|--------|--------|
| **OpenAI** | GPT-4o, GPT-4 Turbo, GPT-3.5, o1 | ✅ Complete |
| **Anthropic** | Claude 3.5 Sonnet, Claude 3 Opus/Sonnet/Haiku | ✅ Complete |
| **Azure OpenAI** | Azure-hosted GPT models | ✅ Complete |
| **Google Gemini** | Gemini 1.5 Pro/Flash, Gemini 2.0 Flash | ✅ Complete |
| **Google Vertex AI** | Gemini + Claude on GCP | ✅ Complete |
| **Amazon Bedrock** | Multi-provider (Anthropic, Meta, Amazon Titan, etc.) | ✅ Complete |

#### Priority 2 - Fast Inference & Aggregators (All Complete ✅)

| Provider | Models | Status |
|----------|--------|--------|
| **Groq** | Llama 3.1 (70B/8B), Mixtral 8x7B, Gemma 7B | ✅ Complete |
| **Cerebras** | Llama 3.3 70B, Llama 3.1 (70B/8B) - Ultra-fast | ✅ Complete |
| **Fireworks** | Llama 3.1/3.3, FireFunction V2, Qwen 2.5, Mixtral, DeepSeek V3 | ✅ Complete |
| **TogetherAI** | 200+ models (Llama, Qwen, Mixtral, DeepSeek, etc.) | ✅ Complete |
| **Vercel AI Gateway** | Multi-provider gateway (OpenAI, Anthropic, Google, etc.) | ✅ Complete |

#### Priority 3 - Specialized & Ecosystem (All Complete ✅)

| Provider | Models | Status |
|----------|--------|--------|
| **Mistral** | Mistral Large 2, Mistral Medium, Mixtral 8x7B/8x22B | ✅ Complete |
| **Cohere** | Command R+, Command R, Command | ✅ Complete |
| **Perplexity** | Sonar models (online search), Llama 3.1 (8B/70B) | ✅ Complete |
| **DeepSeek** | DeepSeek Chat, DeepSeek Coder, DeepSeek Reasoner (R1) | ✅ Complete |
| **xAI (Grok)** | Grok-4, Grok-3, Grok-2-vision, Grok-2-image | ✅ Complete |
| **HuggingFace** | 10,000+ models (Llama2, Mistral7B, Mixtral8x7B, etc.) | ✅ Complete |
| **Replicate** | Llama 2 (70B/13B), Mixtral 8x7B, Mistral 7B | ✅ Complete |
| **OpenAI-Compatible** | Universal (Ollama, LocalAI, vLLM, LM Studio, Groq, etc.) | ✅ Complete |

#### Priority 4 - Specialized Infrastructure (All Complete ✅)

| Provider | Models | Status |
|----------|--------|--------|
| **AI21 Labs** | Jamba 1.5 Large/Mini, Jurassic-2 Ultra/Mid | ✅ Complete |
| **Cloudflare Workers AI** | Llama 3 (8B/70B), Mistral 7B, Neural Chat 7B | ✅ Complete |
| **Baseten** | Llama 3 (8B/70B), Mistral 7B, WizardLM-2 8x22B, Mixtral 8x7B | ✅ Complete |
| **Lepton AI** | Llama 3 (8B/70B), Mixtral 8x7B, WizardLM-2 7B, DBRX | ✅ Complete |
| **Novita AI** | Llama 3 (8B/70B), Mistral 7B, Qwen 2 72B | ✅ Complete |
| **Writer** | Palmyra X-004, Palmyra X-003, Palmyra-2 | ✅ Complete |

#### Priority 5 - Gateways & Local Execution (All Complete ✅)

| Provider | Models | Status |
|----------|--------|--------|
| **LlamaFile** | Local LLM execution (any llamafile model) | ✅ Complete |
| **Friendli AI** | Mixtral 8x7B, Llama 3.1 (70B/8B) | ✅ Complete |
| **Portkey AI Gateway** | Multi-provider routing with caching & observability | ✅ Complete |
| **Fal AI** | Claude 3.5 Sonnet, GPT-4o, Gemini Flash, Llama 3.2 | ✅ Complete |
| **Luma AI** | Dream Machine (future video generation) | ✅ Complete |
| **Stability AI** | StableLM 2 12B, StableLM Zephyr 1.6B, StableLM 3B | ✅ Complete |
| **OpenRouter** | 100+ models from all major providers | ✅ Complete |

#### Planned - Additional Providers

| Provider | Models | Status |
|----------|--------|--------|

### Other Provider Types (Future)

- 📋 **Audio Providers**: Deepgram, ElevenLabs, AssemblyAI, Azure Speech
- 📋 **Embedding Providers**: OpenAI Embeddings, Cohere Embed, Voyage AI
- 📋 **Image Generation**: Stability AI, DALL-E, Midjourney API
- 📋 **Video Generation**: Runway, Luma AI

## Project Structure

**Monorepo Design** - Single package, organized codebase:

```
ai-sdk.net/
├── src/
│   ├── AiSdk/                       # 📦 Main package (includes everything)
│   │   ├── Abstractions/            # Core interfaces, models, errors
│   │   ├── Core/                    # Utilities (streaming, JSON, HTTP)
│   │   ├── Providers/               # All 32 providers in one place
│   │   │   ├── OpenAI/              # OpenAI (GPT-4, GPT-3.5)
│   │   │   ├── Anthropic/           # Anthropic (Claude)
│   │   │   ├── Azure/               # Azure OpenAI
│   │   │   ├── Google/              # Google Gemini
│   │   │   ├── Groq/                # Groq (Llama 3.1, Mixtral)
│   │   │   ├── Mistral/             # Mistral AI
│   │   │   ├── Cohere/              # Cohere
│   │   │   ├── GoogleVertex/        # Google Vertex AI
│   │   │   ├── AmazonBedrock/       # Amazon Bedrock
│   │   │   ├── Replicate/           # Replicate (Llama 2, Mixtral)
│   │   │   ├── Perplexity/          # Perplexity (Sonar online search)
│   │   │   ├── DeepSeek/            # DeepSeek (Chat, Coder, Reasoner)
│   │   │   ├── OpenAICompatible/    # Universal OpenAI-compatible
│   │   │   ├── Vercel/              # Vercel AI Gateway
│   │   │   ├── XAI/                 # xAI (Grok)
│   │   │   ├── HuggingFace/         # HuggingFace Inference API
│   │   │   ├── Cerebras/            # Cerebras (ultra-fast)
│   │   │   ├── Fireworks/           # Fireworks AI
│   │   │   ├── TogetherAI/          # Together AI (200+ models)
│   │   │   ├── AI21/                # AI21 Labs (Jamba, Jurassic-2)
│   │   │   ├── Cloudflare/          # Cloudflare Workers AI
│   │   │   ├── Baseten/             # Baseten (Llama, Mistral, Mixtral)
│   │   │   ├── Lepton/              # Lepton AI (fast inference)
│   │   │   ├── Novita/              # Novita AI (Llama, Mistral, Qwen)
│   │   │   ├── Writer/              # Writer (Palmyra models)
│   │   │   ├── LlamaFile/           # LlamaFile (local execution)
│   │   │   ├── Friendli/            # Friendli AI (ultra-fast)
│   │   │   ├── Portkey/             # Portkey AI Gateway
│   │   │   ├── Fal/                 # Fal AI (multi-model)
│   │   │   ├── Luma/                # Luma AI (Dream Machine)
│   │   │   ├── Stability/           # Stability AI (StableLM)
│   │   │   └── OpenRouter/          # OpenRouter (100+ models)
│   │   ├── AiClient.cs              # High-level API
│   │   └── Models/                  # Shared models
│   └── AiSdk.AspNetCore/            # 📦 Optional web integration package
│       ├── Configuration/
│       ├── Extensions/
│       ├── HealthChecks/
│       └── Middleware/
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

**Benefits of Monorepo Structure:**
- 📦 Single package to install
- 🔄 Easier version management
- 🚀 All providers always compatible
- 📝 Simpler maintenance

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

## Using Different Providers

All providers are included - just switch the namespace:

```csharp
// OpenAI
using AiSdk.Providers.OpenAI;
var provider = new OpenAIProvider(new OpenAIConfiguration { ApiKey = "..." });
var model = provider.GPT4();

// Anthropic (Claude)
using AiSdk.Providers.Anthropic;
var provider = new AnthropicProvider(new AnthropicConfiguration { ApiKey = "..." });
var model = provider.Claude35Sonnet();

// Google Gemini
using AiSdk.Providers.Google;
var provider = new GoogleProvider(new GoogleConfiguration { ApiKey = "..." });
var model = provider.Gemini15Pro();

// Groq (ultra-fast inference)
using AiSdk.Providers.Groq;
var provider = new GroqProvider(new GroqConfiguration { ApiKey = "..." });
var model = provider.Llama3_1_70B();

// Mistral
using AiSdk.Providers.Mistral;
var provider = new MistralProvider(new MistralConfiguration { ApiKey = "..." });
var model = provider.MistralLarge();

// Cohere
using AiSdk.Providers.Cohere;
var provider = new CohereProvider(new CohereConfiguration { ApiKey = "..." });
var model = provider.CommandRPlus();

// Azure OpenAI
using AiSdk.Providers.Azure;
var provider = new AzureOpenAIProvider(new AzureOpenAIConfiguration { ... });
var model = provider.ChatModel("deployment-name");

// Google Vertex AI (on GCP)
using AiSdk.Providers.GoogleVertex;
var provider = new GoogleVertexProvider(new GoogleVertexConfiguration { ... });
var model = provider.Gemini15Pro();

// Amazon Bedrock (multi-provider)
using AiSdk.Providers.AmazonBedrock;
var provider = new AmazonBedrockProvider(new AmazonBedrockConfiguration { ... });
var model = provider.Claude35Sonnet();  // or Llama3, TitanText, etc.

// Replicate
using AiSdk.Providers.Replicate;
var provider = new ReplicateProvider(new ReplicateConfiguration { ApiKey = "..." });
var model = provider.Llama2_70B();

// Perplexity (online search)
using AiSdk.Providers.Perplexity;
var provider = new PerplexityProvider(new PerplexityConfiguration { ApiKey = "..." });
var model = provider.SonarLargeOnline();

// DeepSeek (code & reasoning)
using AiSdk.Providers.DeepSeek;
var provider = new DeepSeekProvider(new DeepSeekConfiguration { ApiKey = "..." });
var model = provider.Coder();  // or Chat(), Reasoner()

// OpenAI-Compatible (Ollama, LocalAI, vLLM, LM Studio)
using AiSdk.Providers.OpenAICompatible;
var model = OpenAICompatibleProvider.ForOllama("llama2");
// or ForLocalAI(), ForVLLM(), ForLMStudio(), or custom endpoint
```

All providers implement the same `ILanguageModel` interface, so you can easily switch between them.

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

**Phase 5 (Complete):**
- ✅ Groq provider (Llama 3.1, Mixtral, Gemma)
- ✅ Mistral provider (Mistral Large, Medium, Mixtral)
- ✅ Cohere provider (Command R+, Command R)
- ✅ Google Vertex AI provider
- ✅ Amazon Bedrock provider (multi-model aggregator)
- ✅ Replicate provider (Llama 2, Mixtral, Mistral)
- ✅ Perplexity provider (Sonar online search models)
- ✅ DeepSeek provider (Chat, Coder, Reasoner R1)
- ✅ OpenAI-Compatible provider (universal connector)

**Next Up (Phase 6):**
- Audio providers (Deepgram, ElevenLabs, AssemblyAI)
- Embedding support
- Image generation support
- Enhanced telemetry with OpenTelemetry

## Package Information

| Package | Description | Size |
|---------|-------------|------|
| **AiSdk** | Core SDK + all 13 providers | Single DLL |
| **AiSdk.AspNetCore** | Optional ASP.NET Core integration | Lightweight |

**Why Single Package?**
- ✅ Simpler dependency management
- ✅ All providers always version-compatible
- ✅ Easier to maintain and update
- ✅ No "which package do I need?" confusion
- ✅ Better for monorepo/enterprise scenarios

## License

Apache-2.0


