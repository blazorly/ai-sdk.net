# AI SDK Minimal API Example

A comprehensive example demonstrating how to integrate the AI SDK for .NET with ASP.NET Core Minimal APIs. This example showcases production-ready patterns including streaming responses, error handling, logging, and CORS configuration.

## Features

- **Minimal API Pattern**: Clean, modern ASP.NET Core endpoint definitions
- **Chat Completion Endpoint**: Non-streaming AI responses
- **Streaming Endpoint**: Real-time token streaming with Server-Sent Events (SSE)
- **Health Check**: Monitor API status
- **CORS Configuration**: Cross-origin request support
- **Swagger/OpenAPI**: Interactive API documentation
- **Mock Model**: Test without API keys
- **Production-Ready**: Comprehensive error handling and logging
- **Dependency Injection**: Proper service registration

## Project Structure

```
MinimalApiExample/
├── Program.cs                       # Main application with endpoint definitions
├── MockLanguageModel.cs             # Mock AI model for testing
├── MinimalApiExample.csproj         # Project file
├── appsettings.json                 # Configuration
├── appsettings.Development.json     # Development configuration
└── README.md                        # This file
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK or later
- Any code editor (Visual Studio, VS Code, Rider, etc.)

### Running the Example

1. Navigate to the example directory:
   ```bash
   cd examples/MinimalApiExample
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. The API will start on:
   - HTTP: http://localhost:5000
   - HTTPS: https://localhost:5001
   - Swagger UI: https://localhost:5001/swagger

## API Endpoints

### 1. Health Check
**GET** `/health`

Check if the API is running.

```bash
curl http://localhost:5000/health
```

**Response:**
```json
{
  "status": "Healthy",
  "timestamp": "2026-01-20T10:30:00Z",
  "service": "AI SDK Minimal API",
  "version": "1.0.0"
}
```

### 2. API Information
**GET** `/api/info`

Get information about the API and available endpoints.

```bash
curl http://localhost:5000/api/info
```

**Response:**
```json
{
  "service": "AI SDK Minimal API Example",
  "description": "Demonstrates AI SDK integration with ASP.NET Core Minimal APIs",
  "model": {
    "provider": "mock",
    "modelId": "mock-api-model",
    "specVersion": "v3"
  },
  "endpoints": [
    {
      "method": "GET",
      "path": "/health",
      "description": "Health check endpoint"
    },
    {
      "method": "POST",
      "path": "/api/chat",
      "description": "Non-streaming chat completion"
    },
    {
      "method": "POST",
      "path": "/api/chat/stream",
      "description": "Streaming chat with SSE"
    }
  ]
}
```

### 3. Chat Completion (Non-Streaming)
**POST** `/api/chat`

Send a message and receive a complete AI response.

**Request Body:**
```json
{
  "message": "What is ASP.NET Core?",
  "systemMessage": "You are a helpful programming tutor.",
  "maxTokens": 500,
  "temperature": 0.7
}
```

**cURL Example:**
```bash
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Explain minimal APIs in .NET"
  }'
```

**Response:**
```json
{
  "message": "ASP.NET Core Minimal APIs provide a simplified approach to building HTTP APIs...",
  "finishReason": "Stop",
  "usage": {
    "inputTokens": 15,
    "outputTokens": 45,
    "totalTokens": 60
  },
  "model": "mock-api-model",
  "provider": "mock"
}
```

### 4. Streaming Chat (Server-Sent Events)
**POST** `/api/chat/stream`

Receive AI responses in real-time as they're generated.

**Request Body:**
```json
{
  "message": "Tell me about streaming in AI applications",
  "systemMessage": "You are a helpful assistant.",
  "maxTokens": 1000,
  "temperature": 0.7
}
```

**cURL Example:**
```bash
curl -X POST http://localhost:5000/api/chat/stream \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Explain streaming responses"
  }'
```

**Response (Server-Sent Events):**
```
event: delta
data: {"text":"Streaming "}

event: delta
data: {"text":"is "}

event: delta
data: {"text":"a "}

event: complete
data: {"finishReason":"Stop","usage":{"inputTokens":10,"outputTokens":50,"totalTokens":60},"model":"mock-api-model","provider":"mock"}
```

### JavaScript Client Example

```javascript
// Non-streaming request
async function chat(message) {
  const response = await fetch('http://localhost:5000/api/chat', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ message })
  });

  const data = await response.json();
  console.log(data.message);
}

// Streaming request
async function streamChat(message) {
  const response = await fetch('http://localhost:5000/api/chat/stream', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ message })
  });

  const reader = response.body.getReader();
  const decoder = new TextDecoder();

  while (true) {
    const { done, value } = await reader.read();
    if (done) break;

    const chunk = decoder.decode(value);
    const lines = chunk.split('\n');

    for (const line of lines) {
      if (line.startsWith('data: ')) {
        const data = JSON.parse(line.slice(6));
        if (data.text) {
          process.stdout.write(data.text);
        }
      }
    }
  }
}

// Usage
await chat('What is .NET?');
await streamChat('Explain async/await');
```

## Configuration

### Using the Mock Model (Default)

The example uses `MockLanguageModel` by default, which requires no API keys and is perfect for testing.

### Switching to a Real AI Provider

#### Option 1: OpenAI

1. Update `Program.cs`:
   ```csharp
   using AiSdk.Providers.OpenAI;

   builder.Services.AddSingleton<ILanguageModel>(sp =>
   {
       var apiKey = builder.Configuration["OpenAI:ApiKey"]
           ?? throw new InvalidOperationException("OpenAI API key not configured");
       return new OpenAIProvider(apiKey: apiKey).ChatModel("gpt-4");
   });
   ```

2. Add your API key to `appsettings.json`:
   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-your-api-key-here"
     }
   }
   ```

#### Option 2: Anthropic (Claude)

1. Update `Program.cs`:
   ```csharp
   using AiSdk.Providers.Anthropic;

   builder.Services.AddSingleton<ILanguageModel>(sp =>
   {
       var apiKey = builder.Configuration["Anthropic:ApiKey"]
           ?? throw new InvalidOperationException("Anthropic API key not configured");
       return new AnthropicProvider(apiKey: apiKey).ChatModel("claude-3-5-sonnet-20241022");
   });
   ```

2. Add your API key to `appsettings.json`:
   ```json
   {
     "Anthropic": {
       "ApiKey": "sk-ant-your-api-key-here"
     }
   }
   ```

#### Option 3: Google Gemini

1. Update `Program.cs`:
   ```csharp
   using AiSdk.Providers.Google;

   builder.Services.AddSingleton<ILanguageModel>(sp =>
   {
       var apiKey = builder.Configuration["Google:ApiKey"]
           ?? throw new InvalidOperationException("Google API key not configured");
       return new GoogleProvider(apiKey: apiKey).ChatModel("gemini-pro");
   });
   ```

2. Add your API key to `appsettings.json`:
   ```json
   {
     "Google": {
       "ApiKey": "your-google-api-key-here"
     }
   }
   ```

## Testing with cURL

### Basic Chat Test
```bash
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{"message": "Hello, how are you?"}'
```

### Chat with Custom System Message
```bash
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Write a haiku about coding",
    "systemMessage": "You are a creative poet",
    "temperature": 0.9
  }'
```

### Streaming Test
```bash
curl -X POST http://localhost:5000/api/chat/stream \
  -H "Content-Type: application/json" \
  -d '{"message": "Count from 1 to 10"}' \
  --no-buffer
```

### Health Check Test
```bash
curl http://localhost:5000/health
```

## Production Deployment

### 1. Environment Variables

For production, use environment variables instead of `appsettings.json`:

```bash
export OpenAI__ApiKey="sk-your-api-key"
export ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

### 2. Docker Deployment

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MinimalApiExample.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinimalApiExample.dll"]
```

Build and run:
```bash
docker build -t minimal-api-example .
docker run -p 5000:5000 -e OpenAI__ApiKey="your-key" minimal-api-example
```

### 3. Security Best Practices

- Never commit API keys to source control
- Use Azure Key Vault or AWS Secrets Manager for production
- Enable HTTPS in production
- Implement rate limiting
- Add authentication/authorization
- Use API key rotation
- Monitor and log all requests

### 4. Performance Optimization

- Enable response compression
- Use HTTP/2
- Implement caching where appropriate
- Configure connection pooling
- Use async/await properly
- Monitor memory usage

## Error Handling

The API includes comprehensive error handling:

```bash
# Invalid request (missing message)
curl -X POST http://localhost:5000/api/chat \
  -H "Content-Type: application/json" \
  -d '{}'

# Response:
{
  "error": "Message is required"
}

# Server error handling
# All exceptions are logged and return proper HTTP status codes
```

## Logging

Logs are written to:
- Console (development)
- Application Insights (production, if configured)
- Custom logging providers (as configured)

Example log output:
```
info: MinimalApiExample.Program[0]
      Chat completion requested: Hello, how are you?
info: MinimalApiExample.Program[0]
      Chat completion successful. Tokens used: 60
```

## Learn More

- [AI SDK Documentation](https://github.com/ai-sdk-dotnet/ai-sdk.net)
- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Server-Sent Events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)
- [OpenAPI/Swagger](https://swagger.io/specification/)

## Related Examples

- `GettingStarted` - Basic AI SDK usage
- `StreamingExample` - Console-based streaming
- `FunctionCallingExample` - Tool/function calling
- `StructuredOutputExample` - JSON schema generation
- `MvcExample` - Traditional MVC integration

## License

Apache-2.0

## Support

For issues and questions:
- GitHub Issues: [https://github.com/ai-sdk-dotnet/ai-sdk.net/issues](https://github.com/ai-sdk-dotnet/ai-sdk.net/issues)
- Documentation: [https://github.com/ai-sdk-dotnet/ai-sdk.net](https://github.com/ai-sdk-dotnet/ai-sdk.net)
