namespace StructuredOutputExample.Models;

/// <summary>
/// Represents a recipe with structured data.
/// </summary>
public class Recipe
{
    /// <summary>
    /// The name of the recipe.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// A brief description of the dish.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Total preparation time in minutes.
    /// </summary>
    public required int PrepTimeMinutes { get; set; }

    /// <summary>
    /// Total cooking time in minutes.
    /// </summary>
    public required int CookTimeMinutes { get; set; }

    /// <summary>
    /// Number of servings this recipe makes.
    /// </summary>
    public required int Servings { get; set; }

    /// <summary>
    /// Difficulty level (easy, medium, hard).
    /// </summary>
    public required string Difficulty { get; set; }

    /// <summary>
    /// List of ingredients needed.
    /// </summary>
    public required List<Ingredient> Ingredients { get; set; }

    /// <summary>
    /// Step-by-step instructions.
    /// </summary>
    public required List<string> Instructions { get; set; }

    /// <summary>
    /// Nutritional information per serving.
    /// </summary>
    public NutritionInfo? Nutrition { get; set; }

    /// <summary>
    /// Tags for categorizing the recipe.
    /// </summary>
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Represents an ingredient in a recipe.
/// </summary>
public class Ingredient
{
    /// <summary>
    /// The name of the ingredient.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The amount needed.
    /// </summary>
    public required string Amount { get; set; }

    /// <summary>
    /// Unit of measurement (cups, grams, etc.).
    /// </summary>
    public required string Unit { get; set; }

    /// <summary>
    /// Optional preparation notes (diced, chopped, etc.).
    /// </summary>
    public string? Preparation { get; set; }
}

/// <summary>
/// Nutritional information for a recipe.
/// </summary>
public class NutritionInfo
{
    public required int Calories { get; set; }
    public required int ProteinGrams { get; set; }
    public required int CarbsGrams { get; set; }
    public required int FatGrams { get; set; }
    public required int FiberGrams { get; set; }
}
