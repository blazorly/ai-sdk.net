using System.Text.Json.Serialization;

namespace AiSdk.Providers.GoogleVertex.Models;

/// <summary>
/// Vertex AI Gemini API request.
/// </summary>
internal record VertexGeminiRequest
{
    [JsonPropertyName("contents")]
    public required List<VertexGeminiContent> Contents { get; init; }

    [JsonPropertyName("generationConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexGeminiGenerationConfig? GenerationConfig { get; init; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<VertexGeminiTool>? Tools { get; init; }

    [JsonPropertyName("toolConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexGeminiToolConfig? ToolConfig { get; init; }
}

/// <summary>
/// Vertex AI Gemini content in a request.
/// </summary>
internal record VertexGeminiContent
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("parts")]
    public required List<VertexGeminiPart> Parts { get; init; }
}

/// <summary>
/// Vertex AI Gemini part (content element).
/// </summary>
internal record VertexGeminiPart
{
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    [JsonPropertyName("functionCall")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexGeminiFunctionCall? FunctionCall { get; init; }

    [JsonPropertyName("functionResponse")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public VertexGeminiFunctionResponse? FunctionResponse { get; init; }
}

/// <summary>
/// Vertex AI Gemini function call.
/// </summary>
internal record VertexGeminiFunctionCall
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("args")]
    public required object Args { get; init; }
}

/// <summary>
/// Vertex AI Gemini function response.
/// </summary>
internal record VertexGeminiFunctionResponse
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("response")]
    public required object Response { get; init; }
}

/// <summary>
/// Vertex AI Gemini generation configuration.
/// </summary>
internal record VertexGeminiGenerationConfig
{
    [JsonPropertyName("maxOutputTokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxOutputTokens { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("topP")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stopSequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? StopSequences { get; init; }
}

/// <summary>
/// Vertex AI Gemini tool definition.
/// </summary>
internal record VertexGeminiTool
{
    [JsonPropertyName("functionDeclarations")]
    public required List<VertexGeminiFunctionDeclaration> FunctionDeclarations { get; init; }
}

/// <summary>
/// Vertex AI Gemini function declaration.
/// </summary>
internal record VertexGeminiFunctionDeclaration
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    [JsonPropertyName("parameters")]
    public required object Parameters { get; init; }
}

/// <summary>
/// Vertex AI Gemini tool configuration.
/// </summary>
internal record VertexGeminiToolConfig
{
    [JsonPropertyName("functionCallingConfig")]
    public VertexGeminiFunctionCallingConfig? FunctionCallingConfig { get; init; }
}

/// <summary>
/// Vertex AI Gemini function calling configuration.
/// </summary>
internal record VertexGeminiFunctionCallingConfig
{
    [JsonPropertyName("mode")]
    public required string Mode { get; init; }

    [JsonPropertyName("allowedFunctionNames")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? AllowedFunctionNames { get; init; }
}

/// <summary>
/// Vertex AI Gemini API response.
/// </summary>
internal record VertexGeminiResponse
{
    [JsonPropertyName("candidates")]
    public required List<VertexGeminiCandidate> Candidates { get; init; }

    [JsonPropertyName("usageMetadata")]
    public VertexGeminiUsageMetadata? UsageMetadata { get; init; }

    [JsonPropertyName("promptFeedback")]
    public VertexGeminiPromptFeedback? PromptFeedback { get; init; }
}

/// <summary>
/// Vertex AI Gemini candidate in a response.
/// </summary>
internal record VertexGeminiCandidate
{
    [JsonPropertyName("content")]
    public VertexGeminiContent? Content { get; init; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("safetyRatings")]
    public List<VertexGeminiSafetyRating>? SafetyRatings { get; init; }
}

/// <summary>
/// Vertex AI Gemini usage metadata.
/// </summary>
internal record VertexGeminiUsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; init; }

    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; init; }

    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; init; }
}

/// <summary>
/// Vertex AI Gemini prompt feedback.
/// </summary>
internal record VertexGeminiPromptFeedback
{
    [JsonPropertyName("blockReason")]
    public string? BlockReason { get; init; }

    [JsonPropertyName("safetyRatings")]
    public List<VertexGeminiSafetyRating>? SafetyRatings { get; init; }
}

/// <summary>
/// Vertex AI Gemini safety rating.
/// </summary>
internal record VertexGeminiSafetyRating
{
    [JsonPropertyName("category")]
    public required string Category { get; init; }

    [JsonPropertyName("probability")]
    public required string Probability { get; init; }
}

/// <summary>
/// Vertex AI Gemini stream response.
/// </summary>
internal record VertexGeminiStreamResponse
{
    [JsonPropertyName("candidates")]
    public List<VertexGeminiCandidate>? Candidates { get; init; }

    [JsonPropertyName("usageMetadata")]
    public VertexGeminiUsageMetadata? UsageMetadata { get; init; }
}
