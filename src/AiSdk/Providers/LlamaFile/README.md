# LlamaFile Provider for AI SDK .NET

The LlamaFile provider enables you to run large language models locally on your machine using [llamafile](https://github.com/Mozilla-Ocho/llamafile) - single-file executables that contain both the model weights and the inference engine.

## Benefits of Local Execution

- **Privacy**: Your data never leaves your machine
- **No API Costs**: Run models without any API fees
- **Offline Use**: Works without internet connection
- **Low Latency**: No network overhead for local processing
- **Full Control**: Complete control over model versions and configuration

## Installation

The LlamaFile provider is included in the AiSdk package:

```bash
dotnet add package AiSdk
```

## Getting Started with LlamaFile

### 1. Download a LlamaFile

Visit the [llamafile releases page](https://github.com/Mozilla-Ocho/llamafile/releases) or [HuggingFace](https://huggingface.co/models?other=llamafile) to download a pre-built llamafile. Popular options include:

- **Llama 3.2 3B** (small, fast): `llama-3.2-3b-instruct.llamafile`
- **Llama 3.1 8B** (balanced): `llama-3.1-8b-instruct.llamafile`
- **Mistral 7B** (high quality): `mistral-7b-instruct-v0.2.llamafile`
- **TinyLlama 1.1B** (extremely fast): `tinyllama-1.1b-chat.llamafile`

### 2. Make the File Executable

On Linux/macOS:
```bash
chmod +x llama-3.2-3b-instruct.llamafile
```

On Windows:
- Rename the file to add `.exe` extension: `llama-3.2-3b-instruct.llamafile.exe`

### 3. Start the LlamaFile Server

Run the llamafile to start a local OpenAI-compatible API server:

```bash
# Linux/macOS
./llama-3.2-3b-instruct.llamafile --server --port 8080

# Windows
llama-3.2-3b-instruct.llamafile.exe --server --port 8080
```

The server will start at `http://localhost:8080` by default.

### 4. Optional: Custom Port

To run on a different port:

```bash
./llama-3.2-3b-instruct.llamafile --server --port 8081
```

## Usage Examples

### Basic Usage

```csharp
using AiSdk.Providers.LlamaFile;

// Create a model instance (assumes server running on default port 8080)
var model = LlamaFileProvider.LocalModel();

// Generate text
var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What is the capital of France?"
        }
    }
});

Console.WriteLine(result.Text);
```

### Custom Port Configuration

```csharp
// Connect to llamafile server on custom port
var model = LlamaFileProvider.LocalModel(baseUrl: "http://localhost:8081/v1");

// Or use ChatModel method with specific model ID
var model = LlamaFileProvider.ChatModel(
    modelId: "llama-3-8b",
    baseUrl: "http://localhost:8081/v1"
);
```

### Using Configuration Object

```csharp
var config = new LlamaFileConfiguration
{
    BaseUrl = "http://localhost:8080/v1",
    TimeoutSeconds = 300 // Increase timeout for slower machines
};

var model = LlamaFileProvider.CreateChatModel("local-model", config);
```

### Streaming Responses

```csharp
var model = LlamaFileProvider.LocalModel();

await foreach (var chunk in model.StreamAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Write a short story about a robot."
        }
    }
}))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
}
```

### System Messages and Temperature Control

```csharp
var model = LlamaFileProvider.LocalModel();

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.System,
            Content = "You are a helpful coding assistant."
        },
        new Message
        {
            Role = MessageRole.User,
            Content = "Explain what a closure is in JavaScript."
        }
    },
    Temperature = 0.7,
    MaxTokens = 500
});

Console.WriteLine(result.Text);
```

### Multi-Turn Conversations

```csharp
var model = LlamaFileProvider.LocalModel();
var conversationHistory = new List<Message>();

// First turn
conversationHistory.Add(new Message
{
    Role = MessageRole.User,
    Content = "What is recursion?"
});

var response1 = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = conversationHistory
});

conversationHistory.Add(new Message
{
    Role = MessageRole.Assistant,
    Content = response1.Text
});

// Second turn
conversationHistory.Add(new Message
{
    Role = MessageRole.User,
    Content = "Can you give me a simple example?"
});

var response2 = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = conversationHistory
});

Console.WriteLine(response2.Text);
```

### Error Handling

```csharp
using AiSdk.Providers.LlamaFile.Exceptions;

try
{
    var model = LlamaFileProvider.LocalModel();
    var result = await model.GenerateAsync(options);
}
catch (LlamaFileException ex)
{
    Console.WriteLine($"LlamaFile error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine("Cannot connect to llamafile server. Is it running?");
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Advanced Configuration

### Running Multiple Models

You can run multiple llamafile instances on different ports:

```bash
# Terminal 1 - Llama 3 8B on port 8080
./llama-3-8b.llamafile --server --port 8080

# Terminal 2 - Mistral 7B on port 8081
./mistral-7b.llamafile --server --port 8081
```

Then connect to specific models:

```csharp
var llama3 = LlamaFileProvider.LocalModel(baseUrl: "http://localhost:8080/v1");
var mistral = LlamaFileProvider.LocalModel(baseUrl: "http://localhost:8081/v1");
```

### Performance Tuning

LlamaFile supports various command-line options for performance:

```bash
# Use specific number of CPU threads
./model.llamafile --server --port 8080 --threads 8

# Enable GPU acceleration (if available)
./model.llamafile --server --port 8080 --gpu auto

# Set context size
./model.llamafile --server --port 8080 --ctx-size 4096

# Set batch size for faster processing
./model.llamafile --server --port 8080 --batch-size 512
```

### Increased Timeout for Slower Machines

```csharp
var config = new LlamaFileConfiguration
{
    BaseUrl = "http://localhost:8080/v1",
    TimeoutSeconds = 600 // 10 minutes for very slow machines
};

var model = LlamaFileProvider.CreateChatModel("local-model", config);
```

## Common Use Cases

### 1. Privacy-Sensitive Applications

```csharp
// Process sensitive data locally without sending to external APIs
var model = LlamaFileProvider.LocalModel();

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.System,
            Content = "Analyze this medical record and summarize key findings."
        },
        new Message
        {
            Role = MessageRole.User,
            Content = sensitivePatientData
        }
    }
});
```

### 2. Offline Development

```csharp
// Work without internet connection
var model = LlamaFileProvider.LocalModel();

var result = await model.GenerateAsync(new LanguageModelCallOptions
{
    Messages = new[]
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Generate unit tests for this C# class: " + codeSnippet
        }
    }
});
```

### 3. Batch Processing

```csharp
var model = LlamaFileProvider.LocalModel();
var documents = LoadDocuments();

foreach (var doc in documents)
{
    var summary = await model.GenerateAsync(new LanguageModelCallOptions
    {
        Messages = new[]
        {
            new Message
            {
                Role = MessageRole.User,
                Content = $"Summarize this document: {doc.Content}"
            }
        }
    });

    await SaveSummary(doc.Id, summary.Text);
}
```

## Troubleshooting

### Server Not Starting

**Issue**: LlamaFile server won't start

**Solutions**:
- Ensure the file has execute permissions (`chmod +x` on Linux/macOS)
- Check if port 8080 is already in use
- Try a different port: `--port 8081`
- Check system requirements (RAM, disk space)

### Connection Refused

**Issue**: `HttpRequestException: Connection refused`

**Solutions**:
- Verify the llamafile server is running
- Check the correct port is being used
- Ensure firewall isn't blocking localhost connections
- Verify the base URL matches the server configuration

### Slow Performance

**Issue**: Model responses are very slow

**Solutions**:
- Use a smaller model (e.g., TinyLlama instead of Llama 3 70B)
- Reduce `MaxTokens` in your requests
- Enable GPU acceleration if available: `--gpu auto`
- Increase thread count: `--threads 8`
- Reduce context size: `--ctx-size 2048`

### Out of Memory

**Issue**: System runs out of memory

**Solutions**:
- Use a smaller model that fits your RAM
- Close other applications
- Reduce batch size: `--batch-size 256`
- Check model requirements before downloading

## Model Recommendations

| Model | Size | RAM Required | Speed | Quality | Best For |
|-------|------|--------------|-------|---------|----------|
| TinyLlama 1.1B | ~1GB | 2GB | Very Fast | Basic | Quick responses, testing |
| Llama 3.2 3B | ~3GB | 4GB | Fast | Good | General purpose, development |
| Llama 3.1 8B | ~8GB | 12GB | Medium | Very Good | Production, balanced performance |
| Mistral 7B | ~7GB | 10GB | Medium | Excellent | High-quality responses |
| Llama 3.1 70B | ~70GB | 80GB+ | Slow | Excellent | Maximum quality (requires powerful hardware) |

## Resources

- [LlamaFile GitHub Repository](https://github.com/Mozilla-Ocho/llamafile)
- [LlamaFile Documentation](https://github.com/Mozilla-Ocho/llamafile/blob/main/README.md)
- [Download Pre-built Models](https://github.com/Mozilla-Ocho/llamafile/releases)
- [HuggingFace LlamaFile Models](https://huggingface.co/models?other=llamafile)
- [AI SDK .NET Documentation](https://github.com/ai-sdk-dotnet/ai-sdk)

## API Compatibility

LlamaFile provides an OpenAI-compatible API, which means:
- Standard chat completions endpoint
- Streaming support via Server-Sent Events (SSE)
- Temperature, top_p, and other sampling parameters
- System, user, and assistant message roles
- Tool/function calling support (model-dependent)

## License

This provider is part of the AI SDK .NET project and follows the same license terms.
