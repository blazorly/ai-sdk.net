# FunctionCallingExample - Tool/Function Calling

This example demonstrates how to use function calling (also known as tool calling) with the AI SDK. Function calling allows AI models to invoke external functions and tools to extend their capabilities beyond text generation.

## What is Function Calling?

Function calling enables AI models to:
- **Access external data**: Retrieve real-time information (weather, stocks, databases)
- **Perform actions**: Send emails, create calendar events, make API calls
- **Complex computations**: Execute calculations, data transformations
- **Interact with systems**: Control IoT devices, query databases, call web services

## How It Works

1. **Define Tools**: Create tool definitions with input/output types
2. **Provide to Model**: Pass available tools to the AI model
3. **Model Decides**: The model determines which tool(s) to call based on the prompt
4. **Execute Tools**: Your code executes the selected tools
5. **Return Results**: Tool results are sent back to the model for final response

## Features Demonstrated

### 1. Tool Definition
Using strongly-typed input and output:

```csharp
var weatherTool = Tool.Create<WeatherInput, WeatherOutput>(
    name: "get_weather",
    description: "Get current weather for a city",
    execute: GetWeather
);
```

### 2. Tool Execution
The SDK automatically:
- Validates input types
- Serializes/deserializes JSON
- Handles errors gracefully

### 3. Multi-Turn Conversations
Tools can be part of ongoing conversations where the model uses previous tool results.

## Running the Example

```bash
cd examples/FunctionCallingExample
dotnet run
```

## Project Structure

```
FunctionCallingExample/
├── Tools/
│   ├── WeatherTool.cs      # Weather information tool
│   └── CalculatorTool.cs   # Mathematical calculations
├── MockLanguageModel.cs    # Simulated model with tool calling
├── Program.cs              # Main examples
└── README.md              # This file
```

## Creating Custom Tools

### Step 1: Define Input/Output Types

```csharp
public record MyToolInput
{
    public required string Parameter1 { get; init; }
    public int Parameter2 { get; init; }
}

public record MyToolOutput
{
    public required string Result { get; init; }
}
```

### Step 2: Create Tool Definition

```csharp
public static class MyTool
{
    public static ToolWithExecution<MyToolInput, MyToolOutput> Create()
    {
        return Tool.Create<MyToolInput, MyToolOutput>(
            name: "my_tool",
            description: "Clear description of what this tool does",
            execute: ExecuteMyTool
        );
    }

    private static MyToolOutput ExecuteMyTool(MyToolInput input)
    {
        // Your implementation here
        return new MyToolOutput { Result = "Success" };
    }
}
```

### Step 3: Use in Requests

```csharp
var myTool = MyTool.Create();

var result = await AiClient.GenerateTextAsync(
    model,
    new GenerateTextOptions
    {
        Prompt = "Your prompt here",
        Tools = new[] { myTool.Definition }
    });

// Check if model wants to use the tool
if (result.ToolCalls != null)
{
    foreach (var toolCall in result.ToolCalls)
    {
        var output = await myTool.ExecuteAsync(toolCall.Arguments);
        // Process output...
    }
}
```

## Tool Choice Strategies

Control when and how tools are used:

```csharp
// Let model decide
ToolChoice = "auto"

// Force tool use
ToolChoice = "required"

// Prevent tool use
ToolChoice = "none"

// Force specific tool
ToolChoice = "get_weather"
```

## Best Practices

### 1. Clear Descriptions
Tool descriptions should be:
- Concise but complete
- Specify what the tool does
- Mention key parameters
- Indicate return value type

### 2. Input Validation
Always validate tool inputs:

```csharp
private static Output ExecuteTool(Input input)
{
    ArgumentNullException.ThrowIfNull(input);

    if (string.IsNullOrWhiteSpace(input.RequiredField))
        throw new ArgumentException("Required field missing");

    // Execute tool logic...
}
```

### 3. Error Handling
Handle errors gracefully:

```csharp
try
{
    var result = await tool.ExecuteAsync(arguments);
}
catch (JsonException ex)
{
    // Invalid arguments
}
catch (InvalidOperationException ex)
{
    // Tool-specific error
}
```

### 4. Async Operations
For tools that make API calls:

```csharp
return Tool.Create<Input, Output>(
    name: "async_tool",
    description: "Tool with async operations",
    execute: async (input, ct) =>
    {
        var data = await FetchDataAsync(input, ct);
        return ProcessData(data);
    }
);
```

## Real-World Use Cases

### Database Queries
```csharp
Tool.Create<QueryInput, QueryOutput>(
    "query_database",
    "Query the customer database",
    async (input, ct) => await ExecuteQuery(input.Sql, ct)
);
```

### API Integrations
```csharp
Tool.Create<EmailInput, EmailOutput>(
    "send_email",
    "Send an email via SendGrid",
    async (input, ct) => await SendGridClient.SendAsync(input, ct)
);
```

### File Operations
```csharp
Tool.Create<FileInput, FileOutput>(
    "read_file",
    "Read content from a file",
    input => File.ReadAllText(input.Path)
);
```

### Calendar Integration
```csharp
Tool.Create<EventInput, EventOutput>(
    "create_event",
    "Create a calendar event",
    async (input, ct) => await CalendarApi.CreateEventAsync(input, ct)
);
```

## Security Considerations

1. **Validate Inputs**: Never trust tool inputs without validation
2. **Rate Limiting**: Implement rate limits for expensive operations
3. **Authorization**: Check user permissions before executing tools
4. **Sandbox Execution**: Run untrusted tools in isolated environments
5. **Audit Logging**: Log all tool executions for security review

## Performance Tips

1. **Caching**: Cache tool results when appropriate
2. **Parallel Execution**: Execute independent tools concurrently
3. **Timeouts**: Set reasonable timeouts for external calls
4. **Batch Operations**: Combine multiple operations when possible

## Adapting for Production

Replace MockLanguageModel with real providers:

```csharp
// OpenAI
var openai = new OpenAIProvider(apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var model = openai.ChatModel("gpt-4");

// Anthropic
var anthropic = new AnthropicProvider(apiKey: Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY"));
var model = anthropic.ChatModel("claude-3-opus-20240229");
```

## Troubleshooting

**Model doesn't call tools:**
- Check tool descriptions are clear
- Verify prompt indicates tool use is needed
- Try `ToolChoice = "required"`

**Arguments parsing fails:**
- Verify Input type properties match schema
- Check for required properties
- Ensure proper JSON serialization attributes

**Tool execution errors:**
- Add comprehensive error handling
- Log errors for debugging
- Return user-friendly error messages
