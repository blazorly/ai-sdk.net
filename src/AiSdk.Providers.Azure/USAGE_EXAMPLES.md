# Azure OpenAI Provider - Usage Examples

## Table of Contents
1. [Basic Text Generation](#basic-text-generation)
2. [Streaming Responses](#streaming-responses)
3. [Multi-turn Conversations](#multi-turn-conversations)
4. [Function/Tool Calling](#functiontool-calling)
5. [Error Handling](#error-handling)
6. [Configuration Options](#configuration-options)

## Basic Text Generation

### Simple Question-Answer

```csharp
using AiSdk.Providers.Azure;
using AiSdk.Abstractions;

var model = AzureOpenAIProvider.CreateChatModel(
    deploymentName: "gpt-4",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")
);

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What is the capital of France?"
        }
    }
};

var result = await model.GenerateAsync(options);
Console.WriteLine($"Answer: {result.Text}");
Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
```

### With Temperature Control

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Write a creative story opening"
        }
    },
    Temperature = 0.9,  // Higher temperature for more creativity
    MaxTokens = 200
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

## Streaming Responses

### Stream Text Generation

```csharp
var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "Explain quantum computing in simple terms"
        }
    }
};

Console.Write("Response: ");
await foreach (var chunk in model.StreamAsync(options))
{
    if (chunk.Type == ChunkType.TextDelta)
    {
        Console.Write(chunk.Delta);
    }
    else if (chunk.Type == ChunkType.Finish)
    {
        Console.WriteLine();
        Console.WriteLine($"\nFinish reason: {chunk.FinishReason}");
        if (chunk.Usage != null)
        {
            Console.WriteLine($"Total tokens: {chunk.Usage.TotalTokens}");
        }
    }
}
```

## Multi-turn Conversations

### System Message + Conversation History

```csharp
var messages = new List<Message>
{
    new Message
    {
        Role = MessageRole.System,
        Content = "You are a helpful assistant that speaks in a friendly tone."
    },
    new Message
    {
        Role = MessageRole.User,
        Content = "Hi! What's your name?"
    },
    new Message
    {
        Role = MessageRole.Assistant,
        Content = "Hello! I'm an AI assistant here to help you."
    },
    new Message
    {
        Role = MessageRole.User,
        Content = "Can you help me write code?"
    }
};

var options = new LanguageModelCallOptions
{
    Messages = messages
};

var result = await model.GenerateAsync(options);
Console.WriteLine(result.Text);
```

### Interactive Chat Loop

```csharp
var conversationHistory = new List<Message>
{
    new Message
    {
        Role = MessageRole.System,
        Content = "You are a helpful coding assistant."
    }
};

while (true)
{
    Console.Write("You: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput) || userInput.ToLower() == "exit")
        break;

    conversationHistory.Add(new Message
    {
        Role = MessageRole.User,
        Content = userInput
    });

    var options = new LanguageModelCallOptions
    {
        Messages = conversationHistory
    };

    var result = await model.GenerateAsync(options);

    conversationHistory.Add(new Message
    {
        Role = MessageRole.Assistant,
        Content = result.Text
    });

    Console.WriteLine($"Assistant: {result.Text}\n");
}
```

## Function/Tool Calling

### Weather Tool Example

```csharp
using System.Text.Json;

var weatherToolSchema = JsonDocument.Parse(@"{
    ""type"": ""object"",
    ""properties"": {
        ""location"": {
            ""type"": ""string"",
            ""description"": ""The city and state, e.g., San Francisco, CA""
        },
        ""unit"": {
            ""type"": ""string"",
            ""enum"": [""celsius"", ""fahrenheit""],
            ""description"": ""The temperature unit""
        }
    },
    ""required"": [""location""]
}");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What's the weather like in Seattle?"
        }
    },
    Tools = new List<Tool>
    {
        new Tool
        {
            Name = "get_weather",
            Description = "Get the current weather for a location",
            Parameters = weatherToolSchema
        }
    }
};

var result = await model.GenerateAsync(options);

if (result.ToolCalls?.Count > 0)
{
    foreach (var toolCall in result.ToolCalls)
    {
        Console.WriteLine($"Tool called: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments.RootElement.GetRawText()}");

        // Execute the tool and send back the result
        var toolResult = ExecuteWeatherTool(toolCall.Arguments);

        // Add tool response to conversation
        options.Messages.Add(new Message
        {
            Role = MessageRole.Assistant,
            Content = result.Text
        });

        options.Messages.Add(new Message
        {
            Role = MessageRole.Tool,
            Name = toolCall.ToolCallId,
            Content = toolResult
        });

        // Get final response
        var finalResult = await model.GenerateAsync(options);
        Console.WriteLine($"Final response: {finalResult.Text}");
    }
}

static string ExecuteWeatherTool(JsonDocument args)
{
    // Simulate weather API call
    var location = args.RootElement.GetProperty("location").GetString();
    return $"{{\"temperature\": 72, \"condition\": \"sunny\", \"location\": \"{location}\"}}";
}
```

### Calculator Tool Example

```csharp
var calculatorSchema = JsonDocument.Parse(@"{
    ""type"": ""object"",
    ""properties"": {
        ""operation"": {
            ""type"": ""string"",
            ""enum"": [""add"", ""subtract"", ""multiply"", ""divide""]
        },
        ""a"": { ""type"": ""number"" },
        ""b"": { ""type"": ""number"" }
    },
    ""required"": [""operation"", ""a"", ""b""]
}");

var options = new LanguageModelCallOptions
{
    Messages = new List<Message>
    {
        new Message
        {
            Role = MessageRole.User,
            Content = "What is 123 multiplied by 456?"
        }
    },
    Tools = new List<Tool>
    {
        new Tool
        {
            Name = "calculator",
            Description = "Perform mathematical calculations",
            Parameters = calculatorSchema
        }
    }
};

var result = await model.GenerateAsync(options);
// Handle tool call similar to weather example
```

## Error Handling

### Comprehensive Error Handling

```csharp
using AiSdk.Providers.Azure.Exceptions;

try
{
    var result = await model.GenerateAsync(options);
    Console.WriteLine(result.Text);
}
catch (AzureOpenAIException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Authentication failed. Check your API key.");
}
catch (AzureOpenAIException ex) when (ex.StatusCode == 429)
{
    Console.WriteLine("Rate limit exceeded. Please wait and try again.");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (AzureOpenAIException ex) when (ex.StatusCode == 400)
{
    Console.WriteLine($"Bad request: {ex.Message}");
}
catch (AzureOpenAIException ex)
{
    Console.WriteLine($"Azure OpenAI error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### Retry Logic

```csharp
async Task<LanguageModelGenerateResult> GenerateWithRetry(
    AzureOpenAIChatLanguageModel model,
    LanguageModelCallOptions options,
    int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await model.GenerateAsync(options);
        }
        catch (AzureOpenAIException ex) when (ex.StatusCode == 429 && i < maxRetries - 1)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, i)); // Exponential backoff
            Console.WriteLine($"Rate limited. Retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }
    throw new Exception("Max retries exceeded");
}
```

## Configuration Options

### Using Azure AD Authentication

```csharp
// Get Azure AD token (example using DefaultAzureCredential)
using Azure.Identity;

var credential = new DefaultAzureCredential();
var tokenRequestContext = new Azure.Core.TokenRequestContext(
    new[] { "https://cognitiveservices.azure.com/.default" }
);
var token = await credential.GetTokenAsync(tokenRequestContext);

var model = AzureOpenAIProvider.CreateChatModelWithAzureAd(
    deploymentName: "gpt-4",
    endpoint: "https://your-resource.openai.azure.com",
    azureAdToken: token.Token
);
```

### Custom HTTP Client with Timeout

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromMinutes(2)
};

httpClient.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");

var model = AzureOpenAIProvider.CreateChatModel(
    deploymentName: "gpt-4",
    endpoint: "https://your-resource.openai.azure.com",
    apiKey: "your-api-key",
    httpClient: httpClient
);
```

### Using Different API Versions

```csharp
var config = new AzureOpenAIConfiguration
{
    Endpoint = "https://your-resource.openai.azure.com",
    DeploymentName = "gpt-4",
    ApiKey = "your-api-key",
    ApiVersion = "2023-12-01-preview"  // Use specific API version
};

var model = AzureOpenAIProvider.CreateChatModel(config);
```

### Multiple Models with Same Configuration

```csharp
var endpoint = "https://your-resource.openai.azure.com";
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

var gpt4 = AzureOpenAIProvider.GPT4(
    deploymentName: "gpt-4-deployment",
    endpoint: endpoint,
    apiKey: apiKey
);

var gpt35 = AzureOpenAIProvider.GPT35Turbo(
    deploymentName: "gpt-35-deployment",
    endpoint: endpoint,
    apiKey: apiKey
);

// Use different models for different tasks
var complexResult = await gpt4.GenerateAsync(complexOptions);
var simpleResult = await gpt35.GenerateAsync(simpleOptions);
```

## Best Practices

1. **Store credentials securely**: Use environment variables or Azure Key Vault
2. **Handle rate limits**: Implement retry logic with exponential backoff
3. **Manage conversation history**: Trim old messages to stay within token limits
4. **Monitor token usage**: Track usage through the `Usage` property in results
5. **Use appropriate models**: Choose GPT-3.5 for simple tasks, GPT-4 for complex reasoning
6. **Set reasonable timeouts**: Configure HTTP client timeouts based on your needs
7. **Validate tool schemas**: Ensure JSON schemas are valid before sending
8. **Test error handling**: Verify your application handles all error cases gracefully
