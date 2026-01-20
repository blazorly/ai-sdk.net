using System.Text.Json.Serialization;

namespace AiSdk.Providers.TogetherAI.Models;

/// <summary>
/// Together AI chat completion response.
/// </summary>
internal record TogetherAIResponse
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
    public required List<TogetherAIChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public TogetherAIUsage? Usage { get; init; }
}

/// <summary>
/// Together AI choice in a response.
/// </summary>
internal record TogetherAIChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public TogetherAIMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Together AI usage statistics.
/// </summary>
internal record TogetherAIUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Together AI error response.
/// </summary>
internal record TogetherAIErrorResponse
{
    [JsonPropertyName("error")]
    public required TogetherAIError Error { get; init; }
}

/// <summary>
/// Together AI error details.
/// </summary>
internal record TogetherAIError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
