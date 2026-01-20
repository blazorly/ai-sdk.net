using System.Text.Json.Serialization;

namespace AiSdk.Providers.Luma.Models;

/// <summary>
/// Luma AI response structure.
/// Note: This is a placeholder for future video generation features.
/// </summary>
internal record LumaResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("object")]
    public required string Object { get; init; }

    [JsonPropertyName("created")]
    public required long Created { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("choices")]
    public required List<LumaChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public LumaUsage? Usage { get; init; }
}

/// <summary>
/// Luma choice in a response.
/// </summary>
internal record LumaChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public LumaMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Luma usage statistics.
/// Note: Placeholder for future token usage tracking.
/// </summary>
internal record LumaUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Luma error response.
/// </summary>
internal record LumaErrorResponse
{
    [JsonPropertyName("error")]
    public required LumaError Error { get; init; }
}

/// <summary>
/// Luma error details.
/// </summary>
internal record LumaError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
