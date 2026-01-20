# AiSdk.AspNetCore

ASP.NET Core integration package for the AI SDK for .NET, providing dependency injection, health checks, and middleware support for AI-powered applications.

## Features

- **Dependency Injection**: Seamless integration with ASP.NET Core's built-in DI container
- **Configuration Binding**: Support for configuration from appsettings.json or environment variables
- **Health Checks**: Built-in health check implementation for monitoring AI provider availability
- **Streaming Middleware**: Middleware for handling Server-Sent Events (SSE) streaming responses
- **Multiple Providers**: Configure and manage multiple AI providers in a single application

## Installation

```bash
dotnet add package AiSdk.AspNetCore
```

## Quick Start

### 1. Basic Setup

```csharp
using AiSdk.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add AI SDK services
builder.Services.AddAiSdk(options =>
{
    options.DefaultProvider = "openai";
    options.Providers["openai"] = new ProviderConfiguration
    {
        ApiKey = "your-api-key",
        DefaultModel = "gpt-4"
    };
});

var app = builder.Build();
app.Run();
```

### 2. Configuration from appsettings.json

**appsettings.json:**
```json
{
  "AiSdk": {
    "DefaultProvider": "openai",
    "EnableHealthChecks": true,
    "EnableTelemetry": true,
    "Providers": {
      "openai": {
        "ApiKey": "your-openai-api-key",
        "DefaultModel": "gpt-4",
        "Enabled": true
      },
      "anthropic": {
        "ApiKey": "your-anthropic-api-key",
        "DefaultModel": "claude-3-opus-20240229",
        "Enabled": true
      }
    }
  }
}
```

**Program.cs:**
```csharp
builder.Services.AddAiSdk(builder.Configuration.GetSection("AiSdk"));
```

### 3. Add Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddAiSdkHealthCheck(
        name: "ai-sdk",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "ai", "ready" });

app.MapHealthChecks("/health");
```

### 4. Enable Streaming Middleware

```csharp
using AiSdk.AspNetCore.Middleware;

var app = builder.Build();

// Add streaming middleware for SSE support
app.UseAiSdkStreaming();

app.Run();
```

## Usage Examples

### Using ILanguageModel in Controllers

```csharp
using AiSdk.Abstractions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILanguageModel _model;

    public ChatController(ILanguageModel model)
    {
        _model = model;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateResponse([FromBody] ChatRequest request)
    {
        var result = await AiClient.GenerateTextAsync(_model, new GenerateTextOptions
        {
            Prompt = request.Message,
            MaxTokens = 1000
        });

        return Ok(new { response = result.Text });
    }
}
```

### Streaming Responses

```csharp
[HttpPost("stream")]
public async Task StreamResponse([FromBody] ChatRequest request)
{
    Response.ContentType = "text/event-stream";

    await foreach (var chunk in AiClient.StreamTextAsync(_model, new GenerateTextOptions
    {
        Prompt = request.Message,
        MaxTokens = 1000
    }))
    {
        await Response.WriteAsync($"data: {chunk.Delta}\n\n");
        await Response.Body.FlushAsync();
    }
}
```

## Configuration Options

### AiSdkOptions

| Property | Type | Description |
|----------|------|-------------|
| `DefaultProvider` | string | Default AI provider to use when not explicitly specified |
| `Providers` | Dictionary | Provider-specific configuration settings |
| `TimeoutSeconds` | int? | Global timeout for all AI provider calls |
| `EnableHealthChecks` | bool | Enable health checks for AI providers (default: true) |
| `EnableTelemetry` | bool | Enable telemetry and observability (default: true) |

### ProviderConfiguration

| Property | Type | Description |
|----------|------|-------------|
| `ApiKey` | string | API key for the provider |
| `BaseUrl` | string | Base URL for the provider's API |
| `DefaultModel` | string | Default model to use for this provider |
| `OrganizationId` | string | Organization ID (if applicable) |
| `TimeoutSeconds` | int? | Provider-specific timeout |
| `AdditionalSettings` | Dictionary | Additional provider-specific settings |
| `Enabled` | bool | Whether this provider is enabled (default: true) |

## Health Check Details

The `AiSdkHealthCheck` monitors the availability of AI SDK services. To add it to your health checks:

```csharp
builder.Services.AddHealthChecks()
    .AddAiSdkHealthCheck();
```

The health check returns:
- **Healthy**: All AI SDK services are available
- **Degraded**: AI SDK services may have issues but are operational
- **Unhealthy**: AI SDK services are unavailable

## Middleware

### StreamingMiddleware

The streaming middleware automatically configures responses for Server-Sent Events when:
- The request path starts with `/api/stream`
- The `Accept` header contains `text/event-stream`

It sets the following headers:
- `Content-Type: text/event-stream`
- `Cache-Control: no-cache`
- `Connection: keep-alive`

## Best Practices

1. **Store API keys securely**: Use Azure Key Vault, AWS Secrets Manager, or User Secrets for development
2. **Configure timeouts**: Set appropriate timeouts to prevent long-running requests
3. **Enable health checks**: Monitor AI provider availability in production
4. **Use streaming for long responses**: Improve user experience with streaming for lengthy AI-generated content
5. **Handle errors gracefully**: Implement proper error handling and fallback mechanisms

## Environment Variables

You can override configuration using environment variables:

```bash
export AiSdk__DefaultProvider="openai"
export AiSdk__Providers__openai__ApiKey="your-api-key"
export AiSdk__Providers__openai__DefaultModel="gpt-4"
```

## License

Apache-2.0

## Related Packages

- **AiSdk**: Core SDK package
- **AiSdk.Abstractions**: Shared abstractions and interfaces
- **AiSdk.Core**: Core utilities and shared functionality
- **AiSdk.Providers.OpenAI**: OpenAI provider implementation
- **AiSdk.Providers.Anthropic**: Anthropic provider implementation
- **AiSdk.Providers.Google**: Google AI provider implementation
