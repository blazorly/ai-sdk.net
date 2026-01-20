using AiSdk;
using AiSdk.Abstractions;

namespace GettingStarted;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   AI SDK for .NET - Getting Started                          ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine("Welcome to the AI SDK for .NET!");
        Console.WriteLine();
        Console.WriteLine("This SDK provides a unified, type-safe interface for working");
        Console.WriteLine("with multiple AI providers including OpenAI, Anthropic, and Google.");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Quick Start Guide");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("1. Install Provider Package:");
        Console.WriteLine("   dotnet add package AiSdk.Providers.OpenAI");
        Console.WriteLine();

        Console.WriteLine("2. Initialize a Model:");
        Console.WriteLine("   var openai = new OpenAIProvider(apiKey: \"your-api-key\");");
        Console.WriteLine("   var model = openai.ChatModel(\"gpt-4\");");
        Console.WriteLine();

        Console.WriteLine("3. Generate Text:");
        Console.WriteLine("   var result = await AiClient.GenerateTextAsync(");
        Console.WriteLine("       model,");
        Console.WriteLine("       new GenerateTextOptions");
        Console.WriteLine("       {");
        Console.WriteLine("           Prompt = \"Tell me a joke about programming\"");
        Console.WriteLine("       });");
        Console.WriteLine();
        Console.WriteLine("   Console.WriteLine(result.Text);");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Core Features");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("✓ Text Generation     - Generate text with any AI model");
        Console.WriteLine("✓ Streaming           - Real-time token streaming");
        Console.WriteLine("✓ Function Calling    - Let AI call your functions");
        Console.WriteLine("✓ Structured Output   - Generate typed C# objects");
        Console.WriteLine("✓ Multi-turn Chat     - Maintain conversation context");
        Console.WriteLine("✓ Provider Agnostic   - Switch providers without code changes");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Example Projects");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("Explore these working examples to learn the SDK:");
        Console.WriteLine();
        Console.WriteLine("1. StreamingExample");
        Console.WriteLine("   Real-time token streaming with progress tracking");
        Console.WriteLine("   → cd ../StreamingExample && dotnet run");
        Console.WriteLine();
        Console.WriteLine("2. FunctionCallingExample");
        Console.WriteLine("   AI-powered tools with WeatherTool and CalculatorTool");
        Console.WriteLine("   → cd ../FunctionCallingExample && dotnet run");
        Console.WriteLine();
        Console.WriteLine("3. StructuredOutputExample");
        Console.WriteLine("   Generate typed objects (recipes, profiles, analyses)");
        Console.WriteLine("   → cd ../StructuredOutputExample && dotnet run");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Supported Providers");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("Provider packages (coming soon):");
        Console.WriteLine("  • AiSdk.Providers.OpenAI    - GPT-4, GPT-3.5, etc.");
        Console.WriteLine("  • AiSdk.Providers.Anthropic - Claude 3 Opus, Sonnet, Haiku");
        Console.WriteLine("  • AiSdk.Providers.Google    - Gemini Pro, Gemini Ultra");
        Console.WriteLine("  • AiSdk.Providers.Azure     - Azure OpenAI Service");
        Console.WriteLine("  • AiSdk.Providers.Local     - Ollama, LM Studio, etc.");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Common Use Cases");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("• Chatbots & Assistants    - Build conversational AI");
        Console.WriteLine("• Content Generation       - Create articles, emails, code");
        Console.WriteLine("• Data Extraction          - Parse unstructured data");
        Console.WriteLine("• Analysis & Insights      - Analyze text, sentiment, topics");
        Console.WriteLine("• Code Assistance          - Generate, review, explain code");
        Console.WriteLine("• Automation               - AI-powered workflows");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Getting Your API Key");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("OpenAI:");
        Console.WriteLine("  1. Visit https://platform.openai.com/");
        Console.WriteLine("  2. Sign up or log in");
        Console.WriteLine("  3. Go to API Keys section");
        Console.WriteLine("  4. Create a new API key");
        Console.WriteLine();

        Console.WriteLine("Anthropic:");
        Console.WriteLine("  1. Visit https://console.anthropic.com/");
        Console.WriteLine("  2. Sign up or log in");
        Console.WriteLine("  3. Go to API Keys");
        Console.WriteLine("  4. Create a new key");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Best Practices");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("1. Store API keys securely (environment variables, key vault)");
        Console.WriteLine("2. Implement error handling and retries");
        Console.WriteLine("3. Use streaming for better user experience");
        Console.WriteLine("4. Set appropriate temperature (0.0-0.3 for factual, 0.7-1.0 for creative)");
        Console.WriteLine("5. Implement rate limiting for production");
        Console.WriteLine("6. Monitor token usage and costs");
        Console.WriteLine("7. Use cancellation tokens for long operations");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Example Code Snippet");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("// Complete working example:");
        Console.WriteLine();
        Console.WriteLine("using AiSdk;");
        Console.WriteLine("using AiSdk.Providers.OpenAI;");
        Console.WriteLine();
        Console.WriteLine("var apiKey = Environment.GetEnvironmentVariable(\"OPENAI_API_KEY\");");
        Console.WriteLine("var openai = new OpenAIProvider(apiKey: apiKey);");
        Console.WriteLine("var model = openai.ChatModel(\"gpt-4\");");
        Console.WriteLine();
        Console.WriteLine("// Generate text");
        Console.WriteLine("var result = await AiClient.GenerateTextAsync(");
        Console.WriteLine("    model,");
        Console.WriteLine("    new GenerateTextOptions");
        Console.WriteLine("    {");
        Console.WriteLine("        System = \"You are a helpful assistant\",");
        Console.WriteLine("        Prompt = \"Explain async/await in C#\",");
        Console.WriteLine("        MaxTokens = 500,");
        Console.WriteLine("        Temperature = 0.7");
        Console.WriteLine("    });");
        Console.WriteLine();
        Console.WriteLine("Console.WriteLine(result.Text);");
        Console.WriteLine("Console.WriteLine($\"Tokens used: {result.Usage.TotalTokens}\");");
        Console.WriteLine();

        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine("  Resources & Support");
        Console.WriteLine("─────────────────────────────────────────────────────────────────");
        Console.WriteLine();

        Console.WriteLine("📖 Documentation: See README.md in this directory");
        Console.WriteLine("💡 Examples: Explore the example projects");
        Console.WriteLine("🐛 Issues: Report bugs on GitHub");
        Console.WriteLine("💬 Community: Join discussions and ask questions");
        Console.WriteLine();

        Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   Ready to Get Started?                                      ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
        Console.WriteLine("║   1. Check out the example projects                          ║");
        Console.WriteLine("║   2. Read the README.md for detailed guides                  ║");
        Console.WriteLine("║   3. Get an API key from your preferred provider             ║");
        Console.WriteLine("║   4. Start building amazing AI-powered applications!         ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        Console.WriteLine("Note: Provider packages are currently in development.");
        Console.WriteLine("Run the other example projects to see the SDK in action with mock models!");
    }
}
