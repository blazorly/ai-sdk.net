# Blazor Server AI Chat Example

This example demonstrates how to build a **real-time AI chat application** using **Blazor Server** with the **AiSdk** library. It showcases streaming responses, modern UI design, and seamless real-time communication via SignalR.

## Features

- **Real-Time Streaming**: AI responses stream word-by-word in real-time
- **Blazor Server Architecture**: Server-side rendering with SignalR for bidirectional communication
- **Modern Chat UI**: Beautiful, responsive interface with smooth animations
- **Message History**: Maintains conversation context for multi-turn dialogues
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Loading States**: Visual indicators for streaming and processing states
- **Keyboard Shortcuts**: Press Enter to send, Shift+Enter for new line
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices
- **Accessibility**: Supports reduced motion preferences and keyboard navigation
- **Mock Model**: Includes a mock language model for testing without API keys

## Architecture

### Components

1. **Pages/Chat.razor**: Main chat interface with streaming logic
2. **Components/MessageBubble.razor**: Reusable message component with role-based styling
3. **Models/ChatMessage.cs**: Message data model
4. **MockLanguageModel.cs**: Mock AI implementation for testing
5. **Program.cs**: Application configuration and DI setup

### How Real-Time Streaming Works

Blazor Server uses **SignalR** for real-time communication between the server and client:

1. **User Input**: User types a message and clicks send
2. **Server Processing**: Message is sent to the server via SignalR
3. **AI Streaming**: The AI model streams tokens back asynchronously
4. **Real-Time Updates**: Each token triggers `StateHasChanged()` to update the UI
5. **SignalR Transport**: Updates are sent to the client over the persistent WebSocket connection
6. **UI Rendering**: Blazor re-renders only the changed portions of the DOM

```
User Input → SignalR → Server → AI Model → Stream Tokens
                ↑                              ↓
                └──────── StateHasChanged() ───┘
                         SignalR Transport
```

### State Management

- **messages**: List of all chat messages
- **streamingContent**: Current streaming response being built
- **isStreaming**: Boolean flag to disable input during streaming
- **errorMessage**: Stores any error messages for display

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- (Optional) OpenAI API key for real AI responses

## Getting Started

### 1. Navigate to the Example Directory

```bash
cd examples/BlazorServerExample
```

### 2. Run the Application

```bash
dotnet run
```

### 3. Open Your Browser

Navigate to `https://localhost:5001` (or the URL shown in your console).

### 4. Start Chatting!

Type a message and press Enter to see the AI streaming response in action.

## Using a Real AI Model

By default, this example uses a `MockLanguageModel` for demonstration. To use a real AI model:

### Option 1: OpenAI

1. **Update Program.cs**:

```csharp
// Comment out the mock model
// builder.Services.AddSingleton<ILanguageModel, MockLanguageModel>();

// Add OpenAI
builder.Services.AddOpenAI(options =>
{
    options.ApiKey = builder.Configuration["OpenAI:ApiKey"] ??
        throw new InvalidOperationException("OpenAI API key not configured");
    options.DefaultModel = "gpt-4";
});
```

2. **Set your API key** in `appsettings.json`:

```json
{
  "OpenAI": {
    "ApiKey": "your-api-key-here",
    "DefaultModel": "gpt-4"
  }
}
```

3. **Or use environment variables**:

```bash
export OPENAI__APIKEY="your-api-key-here"
dotnet run
```

### Option 2: Azure OpenAI

```csharp
builder.Services.AddAzureOpenAI(options =>
{
    options.Endpoint = builder.Configuration["AzureOpenAI:Endpoint"];
    options.ApiKey = builder.Configuration["AzureOpenAI:ApiKey"];
    options.DeploymentName = "gpt-4";
});
```

### Option 3: Anthropic Claude

```csharp
builder.Services.AddAnthropic(options =>
{
    options.ApiKey = builder.Configuration["Anthropic:ApiKey"];
    options.DefaultModel = "claude-3-sonnet-20240229";
});
```

## Project Structure

```
BlazorServerExample/
├── Components/
│   ├── App.razor                 # Root component
│   ├── Routes.razor              # Routing configuration
│   ├── _Imports.razor            # Global using statements
│   ├── Layout/
│   │   └── MainLayout.razor      # Main layout
│   └── MessageBubble.razor       # Message component
├── Models/
│   └── ChatMessage.cs            # Message data model
├── Pages/
│   └── Chat.razor                # Main chat page
├── wwwroot/
│   └── css/
│       └── app.css               # Styling
├── MockLanguageModel.cs          # Mock AI implementation
├── Program.cs                    # App configuration
├── appsettings.json              # Configuration
└── BlazorServerExample.csproj    # Project file
```

## Key Implementation Details

### Streaming in Blazor Server

The streaming implementation uses `IAsyncEnumerable<T>` with Blazor's component lifecycle:

```csharp
await foreach (var chunk in LanguageModel.StreamTextAsync(request))
{
    if (!string.IsNullOrEmpty(chunk.Text))
    {
        assistantContent.Append(chunk.Text);
        streamingContent = assistantContent.ToString();

        // Trigger UI update
        StateHasChanged();
        await ScrollToBottom();
    }

    // Prevent overwhelming the UI
    await Task.Delay(10);
}
```

### Component Lifecycle

1. **OnInitialized**: Component initialization
2. **StateHasChanged**: Manually triggered after each token
3. **Render**: Blazor efficiently updates only changed DOM elements
4. **SignalR**: Automatically transports render updates to the client

### Performance Considerations

- **Throttling**: Small delay (10ms) between token updates prevents UI overload
- **Efficient Rendering**: Only the streaming message bubble re-renders
- **SignalR Circuit**: Maintains a persistent connection for minimal latency
- **Memory Management**: Old messages remain in memory; consider pagination for long chats

## Customization

### Styling

Edit `wwwroot/css/app.css` to customize:
- Color scheme (CSS variables at the top)
- Message bubble appearance
- Animation speeds
- Responsive breakpoints

### Mock Responses

Edit `MockLanguageModel.cs` to:
- Change response patterns
- Adjust streaming speed
- Add more contextual responses

### UI Layout

Modify `Pages/Chat.razor` to:
- Add message timestamps
- Include user avatars
- Add file upload support
- Implement message editing

## Troubleshooting

### Issue: Application won't start

**Solution**: Ensure .NET 10.0 SDK is installed:
```bash
dotnet --version
```

### Issue: SignalR connection fails

**Solution**: Check browser console for errors. Ensure WebSockets are not blocked by firewall/proxy.

### Issue: Streaming is slow

**Solution**: Adjust the delay in `Chat.razor`:
```csharp
await Task.Delay(10); // Reduce for faster updates
```

### Issue: CSS not loading

**Solution**: Ensure static files middleware is configured in `Program.cs`:
```csharp
app.UseStaticFiles();
```

## Best Practices

1. **Error Boundaries**: Always wrap AI calls in try-catch blocks
2. **Cancellation Tokens**: Implement proper cancellation for streaming operations
3. **Input Validation**: Sanitize and validate user input
4. **Rate Limiting**: Consider rate limiting for production deployments
5. **Connection Resilience**: Handle SignalR reconnection scenarios
6. **Message Limits**: Implement message length limits and conversation pruning
7. **Accessibility**: Ensure keyboard navigation and screen reader support

## Production Considerations

Before deploying to production:

1. **Authentication**: Add user authentication and authorization
2. **Persistence**: Store chat history in a database
3. **Rate Limiting**: Implement per-user rate limits
4. **Monitoring**: Add logging and telemetry
5. **Error Handling**: Implement comprehensive error recovery
6. **Security**: Validate and sanitize all user inputs
7. **Scalability**: Consider Azure SignalR Service for horizontal scaling
8. **Costs**: Monitor AI API usage and implement usage limits

## Resources

- [Blazor Documentation](https://learn.microsoft.com/aspnet/core/blazor/)
- [SignalR Documentation](https://learn.microsoft.com/aspnet/core/signalr/)
- [AiSdk Documentation](../../README.md)
- [OpenAI API Reference](https://platform.openai.com/docs/api-reference)

## License

This example is part of the AiSdk project and follows the same license.

## Support

For issues or questions:
- Open an issue on GitHub
- Check existing examples in the repository
- Review the main AiSdk documentation
