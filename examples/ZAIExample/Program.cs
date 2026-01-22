using AiSdk.Abstractions;
using AiSdk.Providers.ZAI;

// Get API key from environment variable
var apiKey = Environment.GetEnvironmentVariable("ZAI_API_KEY") 
    ?? throw new InvalidOperationException("ZAI_API_KEY environment variable not set");

// Create Z.AI configuration
var config = new ZAIConfiguration
{
    ApiKey = apiKey,
    TimeoutSeconds = 120
};

// Create provider
var provider = new ZAIProvider(config);

Console.WriteLine("=== Z.AI Provider Examples ===\n");

// Example 1: Basic Chat with GLM-4.7
Console.WriteLine("1. Basic Chat with GLM-4.7:");
await BasicChatExample(provider);
Console.WriteLine();

// Example 2: Code Generation with CodeGeeX-4
Console.WriteLine("2. Code Generation with CodeGeeX-4:");
await CodeGenerationExample(provider);
Console.WriteLine();

// Example 3: Streaming Response
Console.WriteLine("3. Streaming Response:");
await StreamingExample(provider);
Console.WriteLine();

// Example 4: Extended Context with GLM-4-32B-128K
Console.WriteLine("4. Extended Context (GLM-4-32B-128K):");
await ExtendedContextExample(provider);
Console.WriteLine();

// Example 5: Tool Calling
Console.WriteLine("5. Tool Calling:");
await ToolCallingExample(provider);
Console.WriteLine();

Console.WriteLine("\nAll examples completed!");

static async Task BasicChatExample(ZAIProvider provider)
{
    var model = provider.GLM47();

    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.System, "You are a helpful AI assistant."),
            new Message(MessageRole.User, "Explain quantum computing in simple terms.")
        },
        Temperature = 0.7
    };

    var result = await model.GenerateAsync(options);
    Console.WriteLine($"Response: {result.Text}");
    Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
}

static async Task CodeGenerationExample(ZAIProvider provider)
{
    // Use CodeGeeX-4 for code tasks
    var codeModel = provider.CodeGeeX4();

    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.User, 
                "Write a C# function to calculate the nth Fibonacci number using dynamic programming. Include XML documentation.")
        },
        Temperature = 0.3
    };

    var result = await codeModel.GenerateAsync(options);
    Console.WriteLine($"Generated Code:\n{result.Text}");
    Console.WriteLine($"\nTokens used: {result.Usage.TotalTokens}");
}

static async Task StreamingExample(ZAIProvider provider)
{
    var model = provider.GLM47();

    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.User, 
                "Write a short story about an AI learning to appreciate art.")
        },
        Temperature = 0.8
    };

    Console.Write("Streaming response: ");
    
    await foreach (var chunk in model.StreamAsync(options))
    {
        if (chunk.Type == ChunkType.TextDelta)
        {
            Console.Write(chunk.Delta);
        }
        else if (chunk.Type == ChunkType.Finish)
        {
            Console.WriteLine($"\n\nFinish reason: {chunk.FinishReason}");
            if (chunk.Usage != null)
            {
                Console.WriteLine($"Tokens used: {chunk.Usage.TotalTokens}");
            }
        }
    }
}

static async Task ExtendedContextExample(ZAIProvider provider)
{
    // Use the 128K context model for long documents
    var longContextModel = provider.GLM432B128K();

    // Simulate a long document
    var longDocument = string.Join("\n", Enumerable.Range(1, 50)
        .Select(i => $"Section {i}: This is a detailed paragraph about topic {i}. " +
                    $"It contains important information that should be analyzed. " +
                    $"The content is relevant to understanding the overall theme."));

    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.User, 
                $"Analyze this document and provide a comprehensive summary:\n\n{longDocument}")
        },
        Temperature = 0.5,
        MaxTokens = 500
    };

    var result = await longContextModel.GenerateAsync(options);
    Console.WriteLine($"Summary: {result.Text}");
    Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
}

static async Task ToolCallingExample(ZAIProvider provider)
{
    var model = provider.GLM47();

    var weatherTool = new ToolDefinition
    {
        Name = "get_weather",
        Description = "Get the current weather for a location",
        Parameters = System.Text.Json.JsonDocument.Parse(@"{
            ""type"": ""object"",
            ""properties"": {
                ""location"": {
                    ""type"": ""string"",
                    ""description"": ""The city and country, e.g., Paris, France""
                },
                ""unit"": {
                    ""type"": ""string"",
                    ""enum"": [""celsius"", ""fahrenheit""],
                    ""description"": ""The temperature unit""
                }
            },
            ""required"": [""location""]
        }")
    };

    var options = new LanguageModelCallOptions
    {
        Messages = new List<Message>
        {
            new Message(MessageRole.User, 
                "What's the weather like in Tokyo? I prefer Celsius.")
        },
        Tools = new List<ToolDefinition> { weatherTool }
    };

    var result = await model.GenerateAsync(options);

    if (result.ToolCalls?.Count > 0)
    {
        foreach (var toolCall in result.ToolCalls)
        {
            Console.WriteLine($"Tool Called: {toolCall.ToolName}");
            Console.WriteLine($"Tool Call ID: {toolCall.ToolCallId}");
            Console.WriteLine($"Arguments: {toolCall.Arguments.RootElement.GetRawText()}");
            
            // In a real application, you would execute the tool and send back the result
            // For this example, we'll just simulate the response
            Console.WriteLine("\n(Tool would be executed here in a real application)");
        }
    }
    else
    {
        Console.WriteLine($"Response: {result.Text}");
    }
}
