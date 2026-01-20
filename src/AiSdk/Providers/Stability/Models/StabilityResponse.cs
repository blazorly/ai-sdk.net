using System.Text.Json.Serialization;

namespace AiSdk.Providers.Stability.Models;

/// <summary>
/// Stability AI chat completion response.
/// Uses OpenAI-compatible format for self-hosted StableLM models.
/// </summary>
internal record StabilityResponse
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
    public required List<StabilityChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public StabilityUsage? Usage { get; init; }
}

/// <summary>
/// Stability AI choice in a response.
/// </summary>
internal record StabilityChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public required StabilityMessage Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Stability AI usage statistics.
/// </summary>
internal record StabilityUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Stability AI error response.
/// </summary>
internal record StabilityErrorResponse
{
    [JsonPropertyName("error")]
    public required StabilityError Error { get; init; }
}

/// <summary>
/// Stability AI error details.
/// </summary>
internal record StabilityError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; init; }
}
