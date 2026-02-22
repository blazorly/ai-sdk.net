using System.Text.Json.Serialization;

namespace AiSdk.Providers.Google.Models;

/// <summary>
/// Google Gemini API request.
/// </summary>
internal record GoogleRequest
{
    [JsonPropertyName("contents")]
    public required List<GoogleContent> Contents { get; init; }

    [JsonPropertyName("generationConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GoogleGenerationConfig? GenerationConfig { get; init; }

    [JsonPropertyName("tools")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GoogleTool>? Tools { get; init; }

    [JsonPropertyName("toolConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GoogleToolConfig? ToolConfig { get; init; }
}

/// <summary>
/// Google content in a request.
/// </summary>
internal record GoogleContent
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("parts")]
    public required List<GooglePart> Parts { get; init; }
}

/// <summary>
/// Google part (content element: text, inline data, file data, or function call/response).
/// </summary>
internal record GooglePart
{
    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; init; }

    [JsonPropertyName("inlineData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GoogleInlineData? InlineData { get; init; }

    [JsonPropertyName("fileData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GoogleFileData? FileData { get; init; }

    [JsonPropertyName("functionCall")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GoogleFunctionCall? FunctionCall { get; init; }

    [JsonPropertyName("functionResponse")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GoogleFunctionResponse? FunctionResponse { get; init; }
}

/// <summary>
/// Inline data for images/audio/video in Google Gemini.
/// </summary>
internal record GoogleInlineData
{
    [JsonPropertyName("mimeType")]
    public required string MimeType { get; init; }

    [JsonPropertyName("data")]
    public required string Data { get; init; }
}

/// <summary>
/// File data reference for Google Gemini (Cloud Storage or URL).
/// </summary>
internal record GoogleFileData
{
    [JsonPropertyName("mimeType")]
    public required string MimeType { get; init; }

    [JsonPropertyName("fileUri")]
    public required string FileUri { get; init; }
}

/// <summary>
/// Google function call.
/// </summary>
internal record GoogleFunctionCall
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("args")]
    public required object Args { get; init; }
}

/// <summary>
/// Google function response.
/// </summary>
internal record GoogleFunctionResponse
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("response")]
    public required object Response { get; init; }
}

/// <summary>
/// Google generation configuration.
/// </summary>
internal record GoogleGenerationConfig
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
/// Google tool definition.
/// </summary>
internal record GoogleTool
{
    [JsonPropertyName("functionDeclarations")]
    public required List<GoogleFunctionDeclaration> FunctionDeclarations { get; init; }
}

/// <summary>
/// Google function declaration.
/// </summary>
internal record GoogleFunctionDeclaration
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
/// Google tool configuration.
/// </summary>
internal record GoogleToolConfig
{
    [JsonPropertyName("functionCallingConfig")]
    public GoogleFunctionCallingConfig? FunctionCallingConfig { get; init; }
}

/// <summary>
/// Google function calling configuration.
/// </summary>
internal record GoogleFunctionCallingConfig
{
    [JsonPropertyName("mode")]
    public required string Mode { get; init; }

    [JsonPropertyName("allowedFunctionNames")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string>? AllowedFunctionNames { get; init; }
}
