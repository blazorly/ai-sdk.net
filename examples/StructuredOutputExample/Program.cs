using System.Text.Json;
using AiSdk;
using AiSdk.Models;
using StructuredOutputExample;
using StructuredOutputExample.Models;

Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   AI SDK for .NET - Structured Output Example                ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

// Create a mock language model for demonstration
var model = new MockLanguageModel();

Console.WriteLine("Demonstration 1: Generating a Recipe");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Generate a structured recipe object
var recipeResult = await AiClient.GenerateObjectAsync<Recipe>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Generate a recipe for spaghetti carbonara"
    });

var recipe = recipeResult.Object;
Console.WriteLine($"Recipe: {recipe.Name}");
Console.WriteLine($"Description: {recipe.Description}");
Console.WriteLine($"Difficulty: {recipe.Difficulty} | Servings: {recipe.Servings}");
Console.WriteLine($"Time: {recipe.PrepTimeMinutes + recipe.CookTimeMinutes} minutes total");
Console.WriteLine();
Console.WriteLine("Ingredients:");
foreach (var ingredient in recipe.Ingredients)
{
    var prep = ingredient.Preparation != null ? $" ({ingredient.Preparation})" : "";
    Console.WriteLine($"  • {ingredient.Amount} {ingredient.Unit} {ingredient.Name}{prep}");
}
Console.WriteLine();
Console.WriteLine("Instructions:");
for (int i = 0; i < recipe.Instructions.Count; i++)
{
    Console.WriteLine($"  {i + 1}. {recipe.Instructions[i]}");
}
if (recipe.Nutrition != null)
{
    Console.WriteLine();
    Console.WriteLine($"Nutrition (per serving): {recipe.Nutrition.Calories} cal, " +
                     $"{recipe.Nutrition.ProteinGrams}g protein, " +
                     $"{recipe.Nutrition.CarbsGrams}g carbs, " +
                     $"{recipe.Nutrition.FatGrams}g fat");
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 2: Generating a Person Profile");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

var personResult = await AiClient.GenerateObjectAsync<PersonProfile>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Generate a professional profile for a software engineer"
    });

var person = personResult.Object;
Console.WriteLine($"Name: {person.Name}");
Console.WriteLine($"Age: {person.Age}");
Console.WriteLine($"Email: {person.Email}");
Console.WriteLine($"Phone: {person.Phone ?? "Not provided"}");
Console.WriteLine();
Console.WriteLine("Address:");
Console.WriteLine($"  {person.Address.Street}");
Console.WriteLine($"  {person.Address.City}, {person.Address.State} {person.Address.PostalCode}");
Console.WriteLine($"  {person.Address.Country}");
Console.WriteLine();
Console.WriteLine("Professional:");
Console.WriteLine($"  Title: {person.Professional.Title}");
Console.WriteLine($"  Company: {person.Professional.Company}");
Console.WriteLine($"  Industry: {person.Professional.Industry}");
Console.WriteLine($"  Experience: {person.Professional.YearsOfExperience} years");
if (person.Professional.Skills != null && person.Professional.Skills.Count > 0)
{
    Console.WriteLine($"  Skills: {string.Join(", ", person.Professional.Skills)}");
}
if (person.Hobbies != null && person.Hobbies.Count > 0)
{
    Console.WriteLine();
    Console.WriteLine($"Hobbies: {string.Join(", ", person.Hobbies)}");
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 3: Product Analysis");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

var productResult = await AiClient.GenerateObjectAsync<ProductAnalysis>(
    model,
    new GenerateObjectOptions
    {
        Prompt = "Analyze a smart home hub product for market viability",
        System = "You are a product analyst. Provide detailed, data-driven analysis."
    });

var product = productResult.Object;
Console.WriteLine($"Product: {product.ProductName}");
Console.WriteLine($"Category: {product.Category}");
Console.WriteLine($"Target Audience: {product.TargetAudience}");
Console.WriteLine();
Console.WriteLine("Key Features:");
foreach (var feature in product.KeyFeatures)
{
    Console.WriteLine($"  • {feature}");
}
Console.WriteLine();
Console.WriteLine("Strengths:");
foreach (var strength in product.Strengths)
{
    Console.WriteLine($"  ✓ {strength}");
}
Console.WriteLine();
Console.WriteLine("Weaknesses:");
foreach (var weakness in product.Weaknesses)
{
    Console.WriteLine($"  ✗ {weakness}");
}
Console.WriteLine();
Console.WriteLine("Competitive Advantages:");
foreach (var advantage in product.CompetitiveAdvantages)
{
    Console.WriteLine($"  ★ {advantage}");
}
Console.WriteLine();
Console.WriteLine("Pricing Strategy:");
Console.WriteLine($"  Strategy: {product.Pricing.Strategy}");
Console.WriteLine($"  Suggested Range: ${product.Pricing.SuggestedPriceMin:F2} - ${product.Pricing.SuggestedPriceMax:F2}");
Console.WriteLine($"  Reasoning: {product.Pricing.Reasoning}");
Console.WriteLine();
Console.WriteLine($"Market Opportunity Score: {product.MarketOpportunityScore}/10");
Console.WriteLine();
Console.WriteLine($"Recommendation: {product.Recommendation}");

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 4: Streaming Structured Output");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

Console.WriteLine("Generating recipe in real-time...");
Console.WriteLine();

Recipe? finalRecipe = null;

await foreach (var chunk in AiClient.StreamObjectAsync<Recipe>(
    model,
    new StreamObjectOptions
    {
        Prompt = "Generate a recipe for chocolate chip cookies",
        OnChunk = delta =>
        {
            // You can process each chunk of raw text here
            // Useful for showing progress indicators
        }
    }))
{
    if (chunk.IsComplete && chunk.Object != null)
    {
        finalRecipe = chunk.Object;
        Console.WriteLine("✓ Recipe generation complete!");
        Console.WriteLine();
        Console.WriteLine($"Recipe: {finalRecipe.Name}");
        Console.WriteLine($"Total Time: {finalRecipe.PrepTimeMinutes + finalRecipe.CookTimeMinutes} minutes");
        Console.WriteLine($"Ingredients: {finalRecipe.Ingredients.Count} items");
        Console.WriteLine($"Steps: {finalRecipe.Instructions.Count}");
    }
    else if (chunk.Error != null)
    {
        Console.WriteLine($"Error: {chunk.Error}");
    }
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("Demonstration 5: Working with Generated Objects");
Console.WriteLine("─────────────────────────────────────────────────────────────────");
Console.WriteLine();

// Demonstrate that generated objects are real .NET objects
Console.WriteLine("Generated objects are fully-typed C# objects:");
Console.WriteLine();

// Calculate total time
var totalTime = recipe.PrepTimeMinutes + recipe.CookTimeMinutes;
Console.WriteLine($"✓ Type-safe calculations: Total time = {totalTime} minutes");

// Filter ingredients
var dairyIngredients = recipe.Ingredients
    .Where(i => i.Name.Contains("cheese") || i.Name.Contains("milk") || i.Name.Contains("cream"))
    .ToList();
Console.WriteLine($"✓ LINQ queries: Found {dairyIngredients.Count} dairy ingredients");

// Serialize to JSON
var json = JsonSerializer.Serialize(recipe, new JsonSerializerOptions
{
    WriteIndented = true
});
Console.WriteLine($"✓ JSON serialization: {json.Length} characters");

// Calculate calories per serving
if (recipe.Nutrition != null)
{
    var caloriesPerServing = recipe.Nutrition.Calories;
    Console.WriteLine($"✓ Access nested properties: {caloriesPerServing} calories per serving");
}

Console.WriteLine();
Console.WriteLine();
Console.WriteLine("╔═══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║   Key Concepts Demonstrated:                                 ║");
Console.WriteLine("╠═══════════════════════════════════════════════════════════════╣");
Console.WriteLine("║   • Generating strongly-typed objects from prompts           ║");
Console.WriteLine("║   • Complex nested structures (recipes, profiles, analysis)  ║");
Console.WriteLine("║   • Streaming structured output with partial objects         ║");
Console.WriteLine("║   • Working with generated objects as native C# types        ║");
Console.WriteLine("║   • Automatic JSON schema generation from C# classes         ║");
Console.WriteLine("╚═══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine("Next Steps:");
Console.WriteLine("  1. Define custom model classes for your domain");
Console.WriteLine("  2. Add validation attributes to enforce constraints");
Console.WriteLine("  3. Implement custom JSON converters for special types");
Console.WriteLine("  4. Use generated objects with databases, APIs, etc.");
Console.WriteLine("  5. Combine structured output with function calling");
