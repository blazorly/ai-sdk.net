# StructuredOutputExample - JSON Parsing and Typed Objects

This example demonstrates how to generate strongly-typed C# objects from AI model responses using JSON schemas. Instead of working with raw text, you get validated, type-safe objects that integrate seamlessly with your application.

## What is Structured Output?

Structured output allows you to:
- **Define data schemas**: Use C# classes to specify the exact structure you want
- **Get typed objects**: Receive strongly-typed objects instead of strings
- **Automatic validation**: The SDK validates responses match your schema
- **Type safety**: Use IntelliSense, compile-time checking, and refactoring tools

## How It Works

1. **Define Model Classes**: Create C# classes representing your desired structure
2. **Generate Objects**: Call `GenerateObjectAsync<T>()` with your model type
3. **Receive Typed Data**: Get back a fully-typed object matching your class
4. **Use in Code**: Work with the object like any other C# object

## Features Demonstrated

### 1. Complex Nested Structures
Generate recipes with ingredients, instructions, and nutrition information - all strongly-typed.

### 2. Business Objects
Create person profiles, product analyses, and other domain models automatically.

### 3. Streaming Structured Output
Receive partial objects as they're being generated for real-time feedback.

### 4. Native C# Integration
Use generated objects with LINQ, Entity Framework, JSON serialization, etc.

## Running the Example

```bash
cd examples/StructuredOutputExample
dotnet run
```

## Project Structure

```
StructuredOutputExample/
├── Models/
│   ├── RecipeModels.cs      # Recipe, Ingredient, NutritionInfo
│   ├── PersonModels.cs      # PersonProfile, Address, Professional
│   └── ProductModels.cs     # ProductAnalysis, PricingStrategy
├── MockLanguageModel.cs     # Simulated model generating JSON
├── Program.cs               # Main examples
└── README.md               # This file
```

## Creating Model Classes

### Basic Structure

```csharp
public class MyModel
{
    // Required properties
    public required string Name { get; set; }
    public required int Age { get; set; }

    // Optional properties
    public string? Email { get; set; }

    // Nested objects
    public required Address Address { get; set; }

    // Collections
    public List<string>? Tags { get; set; }
}
```

### Using Attributes

```csharp
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class ValidatedModel
{
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Range(0, 150)]
    public required int Age { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [JsonPropertyName("phone_number")]
    public string? Phone { get; set; }
}
```

## Usage Examples

### Basic Object Generation

```csharp
var result = await AiClient.GenerateObjectAsync<Recipe>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Generate a recipe for chocolate cake"
    });

Recipe recipe = result.Object;
Console.WriteLine($"Recipe: {recipe.Name}");
```

### With System Prompt

```csharp
var result = await AiClient.GenerateObjectAsync<ProductAnalysis>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Analyze our new smartphone",
        System = "You are an expert product analyst. Be thorough and data-driven."
    });
```

### Streaming Objects

```csharp
await foreach (var chunk in AiClient.StreamObjectAsync<Recipe>(
    model,
    new StreamObjectOptions
    {
        Prompt = "Generate a recipe for pasta",
        OnChunk = delta => Console.Write(delta)
    }))
{
    if (chunk.IsComplete && chunk.Object != null)
    {
        // Final complete object
        var recipe = chunk.Object;
        ProcessRecipe(recipe);
    }
    else if (chunk.Object != null)
    {
        // Partial object (may be incomplete)
        ShowProgress(chunk.Object);
    }
}
```

### Advanced Options

```csharp
var result = await AiClient.GenerateObjectAsync<MyModel>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Generate data",
        Mode = "tool",              // Use tool calling instead of JSON mode
        Name = "MyModelGenerator",  // Custom tool name
        Description = "Generates MyModel objects",
        MaxTokens = 1000,
        Temperature = 0.7
    });
```

## Generation Modes

### JSON Mode (Default)
```csharp
Mode = "json"  // Model outputs JSON directly
```
- Schema is injected into system prompt
- Model generates JSON response
- Best for simple structures

### Tool Mode
```csharp
Mode = "tool"  // Uses function calling
```
- Schema becomes a tool definition
- Model calls the tool with structured data
- More reliable for complex structures
- Better validation

## Working with Generated Objects

### LINQ Queries
```csharp
var vegetarianRecipes = recipes
    .Where(r => r.Tags?.Contains("Vegetarian") ?? false)
    .OrderBy(r => r.PrepTimeMinutes)
    .ToList();
```

### Entity Framework
```csharp
dbContext.Recipes.Add(generatedRecipe);
await dbContext.SaveChangesAsync();
```

### JSON Serialization
```csharp
var json = JsonSerializer.Serialize(recipe, new JsonSerializerOptions
{
    WriteIndented = true
});
```

### Validation
```csharp
var validationContext = new ValidationContext(recipe);
var validationResults = new List<ValidationResult>();
bool isValid = Validator.TryValidateObject(
    recipe,
    validationContext,
    validationResults,
    validateAllProperties: true
);
```

## Best Practices

### 1. Clear Property Names
Use descriptive names that the AI can understand:
```csharp
// Good
public required string EmailAddress { get; set; }
public required int AgeInYears { get; set; }

// Less clear
public required string Email { get; set; }
public required int Age { get; set; }
```

### 2. Provide Descriptions
Use XML comments - some providers use these for better generation:
```csharp
/// <summary>
/// The customer's primary email address for communication.
/// </summary>
public required string Email { get; set; }
```

### 3. Use Appropriate Types
Choose types that match the data:
```csharp
public DateTime CreatedAt { get; set; }      // Dates
public decimal Price { get; set; }           // Money
public Uri Website { get; set; }             // URLs
public Guid Id { get; set; }                 // Identifiers
```

### 4. Handle Validation Errors
```csharp
try
{
    var result = await AiClient.GenerateObjectAsync<MyModel>(model, options);
}
catch (JsonException ex)
{
    // Model output didn't match schema
    Console.WriteLine($"Invalid structure: {ex.Message}");
}
```

### 5. Set Appropriate Temperature
Lower temperature (0.0-0.3) for more consistent structured output:
```csharp
new GenerateObjectOptions
{
    Prompt = "Generate data",
    Temperature = 0.2  // More deterministic
}
```

## Real-World Use Cases

### 1. Data Extraction
Extract structured data from unstructured text:
```csharp
var result = await AiClient.GenerateObjectAsync<Invoice>(
    model,
    new GenerateObjectOptions
    {
        Prompt = $"Extract invoice data from: {emailText}"
    });
```

### 2. Form Generation
Generate form data from natural language:
```csharp
var form = await AiClient.GenerateObjectAsync<ContactForm>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "User said: 'My name is John, email john@example.com, I need help with billing'"
    });
```

### 3. Data Augmentation
Generate synthetic data for testing:
```csharp
for (int i = 0; i < 100; i++)
{
    var testUser = await AiClient.GenerateObjectAsync<UserProfile>(
        model,
        new GenerateObjectOptions
        {
            Prompt = "Generate a realistic test user profile"
        });
    testUsers.Add(testUser);
}
```

### 4. Content Generation
Create structured content:
```csharp
var article = await AiClient.GenerateObjectAsync<BlogPost>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Write a blog post about AI in healthcare"
    });
```

### 5. Analysis and Insights
Generate structured analysis:
```csharp
var sentiment = await AiClient.GenerateObjectAsync<SentimentAnalysis>(
    model,
    new GenerateObjectOptions
    {
        Prompt = $"Analyze sentiment: {customerReview}"
    });
```

## Troubleshooting

**Model returns invalid JSON:**
- Try `Mode = "tool"` for better structure enforcement
- Lower the temperature (0.0-0.3)
- Simplify your model class
- Add more descriptive property names

**Properties are null:**
- Make sure properties aren't marked as `required`
- Check that property names match what the model generates
- Use `JsonPropertyName` attribute for alternative names

**Performance issues:**
- Use streaming for large objects
- Cache generated objects when possible
- Batch multiple requests
- Use appropriate `MaxTokens` setting

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

## Advanced Patterns

### Polymorphic Objects
```csharp
public abstract class BaseEvent
{
    public required string Type { get; set; }
}

public class ClickEvent : BaseEvent
{
    public required string ElementId { get; set; }
}

public class FormEvent : BaseEvent
{
    public required Dictionary<string, string> FormData { get; set; }
}
```

### Union Types
```csharp
public class Response
{
    public string? TextResponse { get; set; }
    public ErrorInfo? Error { get; set; }
}
```

### Recursive Structures
```csharp
public class Category
{
    public required string Name { get; set; }
    public List<Category>? Subcategories { get; set; }
}
```
