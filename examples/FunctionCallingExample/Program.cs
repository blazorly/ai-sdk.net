using System.Text.Json;
using AiSdk;
using AiSdk.Abstractions;
using FunctionCallingExample;
using FunctionCallingExample.Tools;

Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   AI SDK for .NET - Function/Tool Calling Example            ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Create tools with strongly-typed inputs and outputs
var weatherTool = WeatherTool.Create();
var calculatorTool = CalculatorTool.Create();

// Create a mock language model for demonstration
var model = new MockLanguageModel();

Console.WriteLine("Demonstration 1: Weather Tool");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Call the model with tools available
var weatherResult = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "What's the weather like in Paris?",
        Tools = new[] { weatherTool.Definition, calculatorTool.Definition },
        ToolChoice = "auto" // Let the model decide which tool to use
    });

Console.WriteLine($"User: What's the weather like in Paris?");
Console.WriteLine();

// Check if the model wants to call a tool
if (weatherResult.FinishReason == FinishReason.ToolCalls && weatherResult.ToolCalls != null)
{
    foreach (var toolCall in weatherResult.ToolCalls)
    {
        Console.WriteLine($"Model decided to call tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments.RootElement.GetRawText()}");
        Console.WriteLine();

        // Execute the tool
        if (toolCall.ToolName == "get_weather")
        {
            var result = await weatherTool.ExecuteAsync(toolCall.Arguments);
            Console.WriteLine("Tool Result:");
            Console.WriteLine($"  City: {result.City}");
            Console.WriteLine($"  Temperature: {result.Temperature}°{result.Unit}");
            Console.WriteLine($"  Condition: {result.Condition}");
            Console.WriteLine($"  Humidity: {result.Humidity}%");
        }
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 2: Calculator Tool");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

var calcResult = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "Calculate 42 times 17",
        Tools = new[] { weatherTool.Definition, calculatorTool.Definition },
        ToolChoice = "auto"
    });

Console.WriteLine($"User: Calculate 42 times 17");
Console.WriteLine();

if (calcResult.FinishReason == FinishReason.ToolCalls && calcResult.ToolCalls != null)
{
    foreach (var toolCall in calcResult.ToolCalls)
    {
        Console.WriteLine($"Model decided to call tool: {toolCall.ToolName}");
        Console.WriteLine($"Arguments: {toolCall.Arguments.RootElement.GetRawText()}");
        Console.WriteLine();

        if (toolCall.ToolName == "calculate")
        {
            var result = await calculatorTool.ExecuteAsync(toolCall.Arguments);
            Console.WriteLine("Tool Result:");
            Console.WriteLine($"  {result.Expression}");
        }
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 3: Multiple Tool Calls");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Test with a weather query
var tokyoWeatherResult = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "What's the weather in Tokyo?",
        Tools = new[] { weatherTool.Definition, calculatorTool.Definition }
    });

Console.WriteLine("User: What's the weather in Tokyo?");
Console.WriteLine();

if (tokyoWeatherResult.ToolCalls != null)
{
    var toolCall = tokyoWeatherResult.ToolCalls[0];
    var toolResult = await weatherTool.ExecuteAsync(toolCall.Arguments);

    Console.WriteLine($"Assistant: [Calling {toolCall.ToolName}]");
    Console.WriteLine($"The weather in {toolResult.City} is {toolResult.Condition} with a temperature of {toolResult.Temperature}°{toolResult.Unit}.");
    Console.WriteLine();
}

Console.WriteLine();
Console.WriteLine("Demonstration 4: Error Handling");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

try
{
    var errorInput = new CalculatorInput
    {
        Operation = "divide",
        A = 10,
        B = 0
    };

    var errorResult = calculatorTool.Execute(errorInput, CancellationToken.None).Result;
}
catch (Exception ex)
{
    Console.WriteLine($"Error caught: {ex.InnerException?.Message ?? ex.Message}");
    Console.WriteLine("Proper error handling is essential when executing tools!");
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 5: Tool Execution Flow");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Show the complete flow of tool calling
Console.WriteLine("Complete Tool Calling Flow:");
Console.WriteLine("1. User makes a request");
Console.WriteLine("2. Model analyzes request and selects appropriate tool");
Console.WriteLine("3. Model returns tool call with structured arguments");
Console.WriteLine("4. Application executes tool with those arguments");
Console.WriteLine("5. Tool returns structured result");
Console.WriteLine("6. Application can use result or send back to model");
Console.WriteLine();

var demoResult = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "Calculate the sum of 123 and 456",
        Tools = new[] { weatherTool.Definition, calculatorTool.Definition }
    });

if (demoResult.ToolCalls != null)
{
    var toolCall = demoResult.ToolCalls[0];
    Console.WriteLine($"Step 1: User asked: 'Calculate the sum of 123 and 456'");
    Console.WriteLine($"Step 2: Model selected tool: '{toolCall.ToolName}'");
    Console.WriteLine($"Step 3: Model provided arguments: {toolCall.Arguments.RootElement.GetRawText()}");

    var execResult = await calculatorTool.ExecuteAsync(toolCall.Arguments);
    Console.WriteLine($"Step 4: Tool executed successfully");
    Console.WriteLine($"Step 5: Tool result: {execResult.Expression}");
    Console.WriteLine($"Step 6: Application can now display or process this result");
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   Key Concepts Demonstrated:                                 ║");
Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
Console.WriteLine("║   • Defining tools with strongly-typed input/output          ║");
Console.WriteLine("║   • Model-driven tool selection                              ║");
Console.WriteLine("║   • Executing tools and processing results                   ║");
Console.WriteLine("║   • Error handling in tool execution                         ║");
Console.WriteLine("║   • Complete tool calling workflow                           ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Next Steps:");
Console.WriteLine("  1. Create custom tools for your domain");
Console.WriteLine("  2. Implement real API integrations (weather, database, etc.)");
Console.WriteLine("  3. Add authentication and rate limiting");
Console.WriteLine("  4. Handle asynchronous tool execution");
Console.WriteLine("  5. Implement tool result caching");
