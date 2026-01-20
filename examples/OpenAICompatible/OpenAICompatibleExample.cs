using AiSdk.Abstractions;
using AiSdk.Providers.OpenAICompatible;
using AiSdk.Providers.OpenAICompatible.Exceptions;

namespace Examples.OpenAICompatible;

/// <summary>
/// Demonstrates usage of the OpenAI-Compatible provider with various local and cloud endpoints.
/// </summary>
public class OpenAICompatibleExample
{
    /// <summary>
    /// Example using Ollama (local inference).
    /// </summary>
    public static async Task OllamaExample()
    {
        Console.WriteLine("=== Ollama Example ===");

        // Create model using Ollama
        var model = OpenAICompatibleProvider.ForOllama("llama2");

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.System,
                    Content = "You are a helpful assistant."
                },
                new Message
                {
                    Role = MessageRole.User,
                    Content = "What is the capital of France?"
                }
            },
            Temperature = 0.7,
            MaxTokens = 100
        });

        Console.WriteLine($"Response: {result.Text}");
        Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
    }

    /// <summary>
    /// Example using Ollama with streaming.
    /// </summary>
    public static async Task OllamaStreamingExample()
    {
        Console.WriteLine("\n=== Ollama Streaming Example ===");

        var model = OpenAICompatibleProvider.ForOllama("llama2");

        Console.Write("Response: ");
        await foreach (var chunk in model.StreamAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Write a haiku about programming"
                }
            }
        }))
        {
            if (chunk.Type == ChunkType.TextDelta)
            {
                Console.Write(chunk.Delta);
            }
            else if (chunk.Type == ChunkType.Finish)
            {
                Console.WriteLine($"\n\nFinish Reason: {chunk.FinishReason}");
                if (chunk.Usage != null)
                {
                    Console.WriteLine($"Tokens: {chunk.Usage.TotalTokens}");
                }
            }
        }
    }

    /// <summary>
    /// Example using LocalAI.
    /// </summary>
    public static async Task LocalAIExample()
    {
        Console.WriteLine("\n=== LocalAI Example ===");

        var model = OpenAICompatibleProvider.ForLocalAI(
            modelId: "gpt-3.5-turbo",
            baseUrl: "http://localhost:8080/v1"
        );

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Explain quantum computing in simple terms"
                }
            },
            MaxTokens = 200
        });

        Console.WriteLine($"Response: {result.Text}");
    }

    /// <summary>
    /// Example using vLLM.
    /// </summary>
    public static async Task VLLMExample()
    {
        Console.WriteLine("\n=== vLLM Example ===");

        var model = OpenAICompatibleProvider.ForVLLM(
            modelId: "meta-llama/Llama-2-7b-chat-hf",
            baseUrl: "http://localhost:8000/v1"
        );

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.System,
                    Content = "You are a coding assistant."
                },
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Write a Python function to calculate fibonacci numbers"
                }
            },
            Temperature = 0.3
        });

        Console.WriteLine($"Response: {result.Text}");
    }

    /// <summary>
    /// Example using LM Studio.
    /// </summary>
    public static async Task LMStudioExample()
    {
        Console.WriteLine("\n=== LM Studio Example ===");

        var model = OpenAICompatibleProvider.ForLMStudio("local-model");

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Tell me an interesting fact about space"
                }
            }
        });

        Console.WriteLine($"Response: {result.Text}");
    }

    /// <summary>
    /// Example using Groq cloud service.
    /// </summary>
    public static async Task GroqExample()
    {
        Console.WriteLine("\n=== Groq Example ===");

        var apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("GROQ_API_KEY environment variable not set");
            return;
        }

        var model = OpenAICompatibleProvider.ForGroq(
            modelId: "llama2-70b-4096",
            apiKey: apiKey
        );

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Explain neural networks"
                }
            },
            MaxTokens = 500
        });

        Console.WriteLine($"Response: {result.Text}");
        Console.WriteLine($"Tokens used: {result.Usage.TotalTokens}");
    }

    /// <summary>
    /// Example using custom endpoint with full configuration.
    /// </summary>
    public static async Task CustomEndpointExample()
    {
        Console.WriteLine("\n=== Custom Endpoint Example ===");

        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = "http://custom-server:8000/v1",
            ApiKey = null,  // No API key needed for local server
            TimeoutSeconds = 300  // 5 minutes timeout
        };

        var model = OpenAICompatibleProvider.CreateChatModel("custom-model", config);

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Hello, how are you?"
                }
            }
        });

        Console.WriteLine($"Response: {result.Text}");
    }

    /// <summary>
    /// Example with error handling.
    /// </summary>
    public static async Task ErrorHandlingExample()
    {
        Console.WriteLine("\n=== Error Handling Example ===");

        var model = OpenAICompatibleProvider.ForOllama("llama2");

        try
        {
            var result = await model.GenerateAsync(new LanguageModelCallOptions
            {
                Messages = new[]
                {
                    new Message
                    {
                        Role = MessageRole.User,
                        Content = "Test message"
                    }
                }
            });

            Console.WriteLine($"Response: {result.Text}");
        }
        catch (OpenAICompatibleException ex)
        {
            Console.WriteLine($"API Error: {ex.Message}");
            Console.WriteLine($"Status Code: {ex.StatusCode}");
            Console.WriteLine($"Error Code: {ex.ErrorCode}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Network Error: {ex.Message}");
            Console.WriteLine("Make sure the server is running and accessible.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Example with multiple parameters.
    /// </summary>
    public static async Task AdvancedParametersExample()
    {
        Console.WriteLine("\n=== Advanced Parameters Example ===");

        var model = OpenAICompatibleProvider.ForOllama("llama2");

        var result = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = new[]
            {
                new Message
                {
                    Role = MessageRole.System,
                    Content = "You are a creative writer."
                },
                new Message
                {
                    Role = MessageRole.User,
                    Content = "Write the beginning of a sci-fi story"
                }
            },
            Temperature = 0.9,      // High creativity
            TopP = 0.95,            // Nucleus sampling
            MaxTokens = 300,        // Maximum response length
            StopSequences = new[] { "\n\n", "THE END" }  // Stop on these sequences
        });

        Console.WriteLine($"Response: {result.Text}");
        Console.WriteLine($"Finish Reason: {result.FinishReason}");
    }

    /// <summary>
    /// Example with conversation history.
    /// </summary>
    public static async Task ConversationExample()
    {
        Console.WriteLine("\n=== Conversation Example ===");

        var model = OpenAICompatibleProvider.ForOllama("llama2");

        var messages = new List<Message>
        {
            new Message
            {
                Role = MessageRole.System,
                Content = "You are a helpful assistant."
            },
            new Message
            {
                Role = MessageRole.User,
                Content = "What is 2+2?"
            }
        };

        // First turn
        var result1 = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = messages
        });

        Console.WriteLine($"User: What is 2+2?");
        Console.WriteLine($"Assistant: {result1.Text}");

        // Add response to conversation
        messages.Add(new Message
        {
            Role = MessageRole.Assistant,
            Content = result1.Text
        });

        // Second turn
        messages.Add(new Message
        {
            Role = MessageRole.User,
            Content = "Can you explain why?"
        });

        var result2 = await model.GenerateAsync(new LanguageModelCallOptions
        {
            Messages = messages
        });

        Console.WriteLine($"User: Can you explain why?");
        Console.WriteLine($"Assistant: {result2.Text}");
    }

    /// <summary>
    /// Main entry point to run all examples.
    /// </summary>
    public static async Task Main(string[] args)
    {
        Console.WriteLine("OpenAI-Compatible Provider Examples");
        Console.WriteLine("====================================\n");

        try
        {
            // Note: Uncomment the examples you want to run
            // Make sure the corresponding server is running first!

            // Local inference examples
            // await OllamaExample();
            // await OllamaStreamingExample();
            // await LocalAIExample();
            // await VLLMExample();
            // await LMStudioExample();

            // Cloud service examples
            // await GroqExample();

            // Advanced examples
            // await CustomEndpointExample();
            await ErrorHandlingExample();
            // await AdvancedParametersExample();
            // await ConversationExample();

            Console.WriteLine("\n\nExamples completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n\nError running examples: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
