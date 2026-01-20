using System.Runtime.CompilerServices;
using System.Text.Json;
using AiSdk.Abstractions;

namespace StructuredOutputExample;

/// <summary>
/// A mock language model that generates structured JSON responses.
/// This allows the example to run without requiring an API key.
/// </summary>
public class MockLanguageModel : ILanguageModel
{
    public string SpecificationVersion => "v3";
    public string Provider => "mock";
    public string ModelId => "mock-structured-model";

    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
    {
        var empty = new Dictionary<string, IReadOnlyList<string>>();
        return Task.FromResult<IReadOnlyDictionary<string, IReadOnlyList<string>>>(empty);
    }

    public Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        // Extract prompt to determine what kind of structure to generate
        var userMessage = options.Messages?.FirstOrDefault(m => m.Role == MessageRole.User);
        var promptText = userMessage?.Content?.ToLowerInvariant() ?? "";

        string jsonResponse;

        // Generate appropriate JSON based on prompt
        if (promptText.Contains("recipe") || promptText.Contains("pasta") || promptText.Contains("cook"))
        {
            jsonResponse = GenerateRecipeJson();
        }
        else if (promptText.Contains("person") || promptText.Contains("profile") || promptText.Contains("contact"))
        {
            jsonResponse = GeneratePersonJson();
        }
        else if (promptText.Contains("product") || promptText.Contains("analysis") || promptText.Contains("market"))
        {
            jsonResponse = GenerateProductJson();
        }
        else
        {
            // Default simple structure
            jsonResponse = @"{
                ""message"": ""This is a structured response"",
                ""type"": ""example"",
                ""data"": {
                    ""key1"": ""value1"",
                    ""key2"": ""value2""
                }
            }";
        }

        // Wrap in markdown code fence to simulate common model behavior
        var result = new LanguageModelGenerateResult
        {
            Text = $"```json\n{jsonResponse}\n```",
            FinishReason = FinishReason.Stop,
            Usage = new Usage(
                InputTokens: 30,
                OutputTokens: 150,
                TotalTokens: 180
            )
        };

        return Task.FromResult(result);
    }

    public async IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Get the complete response
        var result = await GenerateAsync(options, cancellationToken);
        var text = result.Text ?? "";

        // Stream it character by character to simulate real streaming
        var charsPerChunk = 10;
        for (int i = 0; i < text.Length; i += charsPerChunk)
        {
            var chunk = text.Substring(i, Math.Min(charsPerChunk, text.Length - i));
            await Task.Delay(20, cancellationToken); // Simulate network delay

            yield return new LanguageModelStreamChunk
            {
                Type = ChunkType.TextDelta,
                Delta = chunk
            };
        }

        yield return new LanguageModelStreamChunk
        {
            Type = ChunkType.Finish,
            FinishReason = FinishReason.Stop,
            Usage = result.Usage
        };
    }

    private string GenerateRecipeJson()
    {
        return @"{
  ""name"": ""Classic Spaghetti Carbonara"",
  ""description"": ""A traditional Italian pasta dish with eggs, cheese, and pancetta"",
  ""prepTimeMinutes"": 10,
  ""cookTimeMinutes"": 20,
  ""servings"": 4,
  ""difficulty"": ""medium"",
  ""ingredients"": [
    {
      ""name"": ""spaghetti"",
      ""amount"": ""400"",
      ""unit"": ""grams"",
      ""preparation"": null
    },
    {
      ""name"": ""pancetta"",
      ""amount"": ""200"",
      ""unit"": ""grams"",
      ""preparation"": ""diced""
    },
    {
      ""name"": ""eggs"",
      ""amount"": ""4"",
      ""unit"": ""large"",
      ""preparation"": null
    },
    {
      ""name"": ""Pecorino Romano cheese"",
      ""amount"": ""100"",
      ""unit"": ""grams"",
      ""preparation"": ""grated""
    },
    {
      ""name"": ""black pepper"",
      ""amount"": ""2"",
      ""unit"": ""teaspoons"",
      ""preparation"": ""freshly ground""
    }
  ],
  ""instructions"": [
    ""Bring a large pot of salted water to boil and cook spaghetti according to package directions"",
    ""While pasta cooks, fry pancetta in a large skillet over medium heat until crispy, about 8-10 minutes"",
    ""In a bowl, whisk together eggs, grated Pecorino Romano, and black pepper"",
    ""Reserve 1 cup of pasta cooking water, then drain pasta"",
    ""Remove skillet from heat and add hot pasta to the pancetta"",
    ""Quickly stir in egg mixture, adding pasta water as needed to create a creamy sauce"",
    ""Serve immediately with additional cheese and pepper""
  ],
  ""nutrition"": {
    ""calories"": 520,
    ""proteinGrams"": 24,
    ""carbsGrams"": 62,
    ""fatGrams"": 18,
    ""fiberGrams"": 3
  },
  ""tags"": [""Italian"", ""Pasta"", ""Dinner"", ""Comfort Food""]
}";
    }

    private string GeneratePersonJson()
    {
        return @"{
  ""name"": ""Jane Smith"",
  ""age"": 32,
  ""email"": ""jane.smith@example.com"",
  ""phone"": ""+1-555-0123"",
  ""address"": {
    ""street"": ""123 Main Street"",
    ""city"": ""San Francisco"",
    ""state"": ""CA"",
    ""postalCode"": ""94102"",
    ""country"": ""USA""
  },
  ""professional"": {
    ""title"": ""Senior Software Engineer"",
    ""company"": ""Tech Innovations Inc."",
    ""industry"": ""Technology"",
    ""yearsOfExperience"": 8,
    ""skills"": [
      ""C#"",
      "".NET"",
      ""Azure"",
      ""Microservices"",
      ""Docker"",
      ""Kubernetes""
    ]
  },
  ""hobbies"": [
    ""Photography"",
    ""Hiking"",
    ""Playing Guitar"",
    ""Reading Sci-Fi""
  ],
  ""socialMedia"": {
    ""LinkedIn"": ""linkedin.com/in/janesmith"",
    ""GitHub"": ""github.com/janesmith"",
    ""Twitter"": ""@janesmith""
  }
}";
    }

    private string GenerateProductJson()
    {
        return @"{
  ""productName"": ""SmartHome Hub Pro"",
  ""category"": ""Consumer Electronics / Smart Home"",
  ""targetAudience"": ""Tech-savvy homeowners aged 25-45 interested in home automation and energy efficiency"",
  ""keyFeatures"": [
    ""Voice control with multiple assistant integrations"",
    ""Energy usage monitoring and optimization"",
    ""Supports 100+ smart device brands"",
    ""Advanced automation with AI-driven routines"",
    ""Built-in security and privacy features""
  ],
  ""strengths"": [
    ""Wide device compatibility"",
    ""User-friendly mobile app"",
    ""Strong privacy protections"",
    ""Energy-saving capabilities"",
    ""Regular software updates""
  ],
  ""weaknesses"": [
    ""Higher price point than competitors"",
    ""Requires stable internet connection"",
    ""Learning curve for advanced features"",
    ""Limited offline functionality""
  ],
  ""competitiveAdvantages"": [
    ""Superior AI-driven automation"",
    ""Industry-leading device support"",
    ""Enhanced privacy and security"",
    ""Comprehensive energy analytics""
  ],
  ""pricing"": {
    ""strategy"": ""Premium positioning with value justification"",
    ""suggestedPriceMin"": 199.99,
    ""suggestedPriceMax"": 249.99,
    ""reasoning"": ""Price reflects advanced features and broad compatibility. Target customers willing to pay premium for quality and comprehensive solution.""
  },
  ""marketOpportunityScore"": 8,
  ""recommendation"": ""Strong market opportunity. Position as premium solution for serious smart home enthusiasts. Focus marketing on energy savings ROI and superior automation capabilities.""
}";
    }
}
