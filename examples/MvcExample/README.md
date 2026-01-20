# MVC Example - AI SDK for .NET

A comprehensive ASP.NET Core MVC application demonstrating how to integrate the AI SDK for .NET into a web application with a modern, responsive chat interface.

## Features

This example showcases:

- **MVC Architecture**: Full ASP.NET Core MVC implementation with proper separation of concerns
- **Real-time Streaming**: Server-Sent Events (SSE) for streaming AI responses
- **Modern UI**: Responsive chat interface built with Bootstrap 5
- **Dual Response Modes**: Toggle between streaming and non-streaming responses
- **Configuration Options**: Adjustable temperature, max tokens, and system prompts
- **Token Tracking**: Real-time statistics for message count and token usage
- **Error Handling**: Comprehensive error handling and user feedback
- **No API Key Required**: Uses MockLanguageModel for demonstration

## Project Structure

```
MvcExample/
├── Controllers/
│   └── ChatController.cs          # Handles chat requests and streaming
├── Models/
│   ├── ChatRequest.cs             # Request model for chat messages
│   └── ChatResponse.cs            # Response model with usage info
├── Views/
│   ├── Chat/
│   │   └── Index.cshtml           # Main chat interface
│   ├── Shared/
│   │   └── _Layout.cshtml         # Layout with Bootstrap
│   ├── _ViewImports.cshtml        # View imports
│   └── _ViewStart.cshtml          # Default layout configuration
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Custom styles and animations
│   └── js/
│       └── site.js                # Chat functionality and SSE handling
├── MockLanguageModel.cs           # Mock AI model (no API key needed)
├── Program.cs                     # Application setup and DI configuration
├── appsettings.json               # Configuration settings
└── MvcExample.csproj              # Project file
```

## Running the Example

### Prerequisites

- .NET 10.0 SDK or later
- A web browser

### Steps

1. **Navigate to the example directory**:
   ```bash
   cd examples/MvcExample
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Open your browser**:
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - The chat interface will load automatically

5. **Try the features**:
   - Type a message and click "Send" or press Enter
   - Toggle between streaming and non-streaming modes
   - Adjust temperature and max tokens in the sidebar
   - Customize the system prompt to guide AI behavior
   - View real-time token usage statistics

## How It Works

### Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                         Browser (Client)                        │
│  ┌────────────────┐  ┌──────────────────┐  ┌─────────────────┐ │
│  │  Index.cshtml  │  │    site.css      │  │    site.js      │ │
│  │  (Razor View)  │  │   (Styling)      │  │  (AJAX + SSE)   │ │
│  └────────────────┘  └──────────────────┘  └─────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
                              │ HTTP/SSE
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    ASP.NET Core MVC (Server)                    │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │              ChatController                                │ │
│  │  • Index() → Returns chat view                             │ │
│  │  • SendMessage() → Non-streaming JSON response             │ │
│  │  • StreamMessage() → SSE streaming response                │ │
│  └────────────────────────────────────────────────────────────┘ │
│                              │                                   │
│                              ▼                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │              Dependency Injection                          │ │
│  │              (ILanguageModel)                              │ │
│  └────────────────────────────────────────────────────────────┘ │
│                              │                                   │
│                              ▼                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │              AI SDK Core (AiClient)                        │ │
│  │  • GenerateTextAsync() → Complete response                 │ │
│  │  • StreamTextAsync() → Streaming response                  │ │
│  └────────────────────────────────────────────────────────────┘ │
│                              │                                   │
│                              ▼                                   │
│  ┌────────────────────────────────────────────────────────────┐ │
│  │         MockLanguageModel (ILanguageModel)                 │ │
│  │  • GenerateAsync() → Returns complete text                 │ │
│  │  • StreamAsync() → Yields text chunks                      │ │
│  └────────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────────┘
```

### Request Flow

#### Non-Streaming Mode

1. User types message and clicks "Send"
2. JavaScript sends POST request to `/Chat/SendMessage`
3. `ChatController.SendMessage()` receives request
4. Controller calls `AiClient.GenerateTextAsync()` with user's message
5. AI SDK invokes `MockLanguageModel.GenerateAsync()`
6. Complete response is returned as JSON
7. JavaScript adds message to chat UI

#### Streaming Mode

1. User types message with "Stream Response" enabled
2. JavaScript sends POST request to `/Chat/StreamMessage`
3. `ChatController.StreamMessage()` receives request and sets SSE headers
4. Controller calls `AiClient.StreamTextAsync()` with user's message
5. AI SDK invokes `MockLanguageModel.StreamAsync()`
6. Each chunk is sent as Server-Sent Event
7. JavaScript receives chunks and updates UI in real-time

### Key Components

#### ChatController.cs

The controller handles both streaming and non-streaming requests:

```csharp
public class ChatController : Controller
{
    private readonly ILanguageModel _model;

    // Non-streaming: Returns complete JSON response
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        var result = await AiClient.GenerateTextAsync(_model, options);
        return Json(new ChatResponse { Text = result.Text, ... });
    }

    // Streaming: Uses SSE to stream chunks
    [HttpPost]
    public async Task StreamMessage([FromBody] ChatRequest request)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        await foreach (var chunk in AiClient.StreamTextAsync(_model, options))
        {
            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();
        }
    }
}
```

#### site.js

Handles both AJAX and SSE communication:

```javascript
// Non-streaming: Standard fetch with JSON response
async function handleRegularResponse(request) {
    const response = await fetch('/Chat/SendMessage', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(request)
    });
    const data = await response.json();
    addMessage(data.text, 'assistant', data.usage);
}

// Streaming: Uses ReadableStream to process SSE
async function handleStreamingResponse(request) {
    const response = await fetch('/Chat/StreamMessage', { ... });
    const reader = response.body.getReader();
    const decoder = new TextDecoder();

    while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        // Process SSE messages and update UI
        const chunk = decoder.decode(value);
        // Parse and display each chunk
    }
}
```

## Configuring for Production

To use a real AI provider instead of the mock model:

### 1. Update Program.cs

Replace the mock model registration with a real provider:

```csharp
// Option 1: OpenAI
using AiSdk.Providers.OpenAI;

builder.Services.AddSingleton<ILanguageModel>(sp =>
{
    var apiKey = builder.Configuration["AiSdk:OpenAI:ApiKey"]
        ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    var openai = new OpenAIProvider(apiKey: apiKey);
    return openai.ChatModel("gpt-4");
});

// Option 2: Anthropic Claude
using AiSdk.Providers.Anthropic;

builder.Services.AddSingleton<ILanguageModel>(sp =>
{
    var apiKey = builder.Configuration["AiSdk:Anthropic:ApiKey"]
        ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
    var anthropic = new AnthropicProvider(apiKey: apiKey);
    return anthropic.ChatModel("claude-3-opus-20240229");
});
```

### 2. Update appsettings.json

Add your API key configuration:

```json
{
  "AiSdk": {
    "OpenAI": {
      "ApiKey": "sk-your-api-key-here",
      "DefaultModel": "gpt-4"
    }
  }
}
```

### 3. Use Environment Variables (Recommended)

For better security, use environment variables:

```bash
# Linux/macOS
export OPENAI_API_KEY="sk-your-api-key-here"

# Windows (PowerShell)
$env:OPENAI_API_KEY="sk-your-api-key-here"

# Windows (Command Prompt)
set OPENAI_API_KEY=sk-your-api-key-here
```

### 4. Install Provider Package

Add the provider package to your project:

```bash
dotnet add package AiSdk.Providers.OpenAI
# or
dotnet add package AiSdk.Providers.Anthropic
```

## Customization Ideas

### Add Conversation History

Modify `ChatController` to maintain conversation context:

```csharp
private static List<Message> conversationHistory = new();

public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
{
    conversationHistory.Add(new Message { Role = "user", Content = request.Message });

    var options = new GenerateTextOptions
    {
        Messages = conversationHistory,
        System = request.SystemPrompt
    };

    var result = await AiClient.GenerateTextAsync(_model, options);
    conversationHistory.Add(new Message { Role = "assistant", Content = result.Text });

    return Json(new ChatResponse { Text = result.Text, ... });
}
```

### Add User Authentication

Integrate ASP.NET Core Identity to save chat history per user:

```csharp
[Authorize]
public class ChatController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    // Load user's chat history from database
    // Save messages to database
}
```

### Add Function Calling

Enable AI to call your custom tools:

```csharp
var options = new GenerateTextOptions
{
    Prompt = request.Message,
    Tools = new[]
    {
        new WeatherTool(),
        new CalculatorTool()
    }
};
```

### Add Image Support

Allow users to upload images with their messages:

```csharp
public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
{
    var options = new GenerateTextOptions
    {
        Prompt = request.Message,
        Images = request.ImageUrls
    };

    var result = await AiClient.GenerateTextAsync(_model, options);
    return Json(new ChatResponse { Text = result.Text, ... });
}
```

## API Endpoints

### GET /Chat/Index

Returns the main chat interface (Razor view).

### POST /Chat/SendMessage

Non-streaming chat endpoint.

**Request Body**:
```json
{
  "message": "Hello, AI!",
  "systemPrompt": "You are a helpful assistant",
  "temperature": 0.7,
  "maxTokens": 500,
  "stream": false
}
```

**Response**:
```json
{
  "text": "Hello! How can I help you today?",
  "finishReason": "Stop",
  "usage": {
    "inputTokens": 10,
    "outputTokens": 20,
    "totalTokens": 30
  },
  "isError": false,
  "errorMessage": null
}
```

### POST /Chat/StreamMessage

Streaming chat endpoint using Server-Sent Events (SSE).

**Request Body**: Same as SendMessage

**Response** (SSE stream):
```
data: {"type":"delta","text":"Hello"}

data: {"type":"delta","text":" there!"}

data: {"type":"usage","usage":{"inputTokens":10,"outputTokens":5,"totalTokens":15}}

data: {"type":"done"}
```

## Technologies Used

- **Backend**: ASP.NET Core 10.0 MVC
- **AI Integration**: AI SDK for .NET
- **Frontend**: HTML5, CSS3, JavaScript (ES6+)
- **UI Framework**: Bootstrap 5.3
- **Icons**: Bootstrap Icons
- **Communication**: AJAX, Server-Sent Events (SSE)
- **Styling**: CSS3 with animations and transitions

## Best Practices Demonstrated

1. **Dependency Injection**: Proper use of DI for `ILanguageModel`
2. **Async/Await**: All async operations use proper async patterns
3. **Error Handling**: Comprehensive try-catch blocks and user feedback
4. **Separation of Concerns**: MVC pattern with clear responsibilities
5. **Responsive Design**: Mobile-first, responsive layout
6. **Accessibility**: Semantic HTML and ARIA attributes
7. **Security**: No sensitive data in client-side code
8. **Performance**: Streaming for better perceived performance
9. **User Experience**: Loading states, animations, and feedback

## Troubleshooting

### Port Already in Use

If port 5000/5001 is already in use, specify a different port:

```bash
dotnet run --urls "http://localhost:5002;https://localhost:5003"
```

### Streaming Not Working

1. Check browser console for JavaScript errors
2. Verify SSE headers are set correctly in `StreamMessage`
3. Ensure response buffering is disabled
4. Test with non-streaming mode first

### Styles Not Loading

1. Verify static files middleware is enabled: `app.UseStaticFiles()`
2. Check file paths in `_Layout.cshtml`
3. Clear browser cache (Ctrl+Shift+Delete)
4. Check browser console for 404 errors

## Learn More

- [AI SDK Documentation](../../README.md)
- [ASP.NET Core MVC](https://docs.microsoft.com/aspnet/core/mvc)
- [Server-Sent Events](https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events)
- [Bootstrap Documentation](https://getbootstrap.com/docs/5.3/)

## Next Steps

1. Explore other examples in the `examples/` directory
2. Try integrating a real AI provider (OpenAI, Anthropic)
3. Add conversation history and context
4. Implement user authentication
5. Add function calling capabilities
6. Deploy to Azure App Service or similar platform

## License

This example is part of the AI SDK for .NET project and follows the same license.
