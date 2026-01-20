using System.Text.Json.Serialization;

namespace AiSdk.Providers.Fal.Models;

/// <summary>
/// Fal AI chat completion response.
/// </summary>
internal record FalResponse
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
    public required List<FalChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public FalUsage? Usage { get; init; }
}

/// <summary>
/// Fal AI choice in a response.
/// </summary>
internal record FalChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public FalMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Fal AI usage statistics.
/// </summary>
internal record FalUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Fal AI error response.
/// </summary>
internal record FalErrorResponse
{
    [JsonPropertyName("error")]
    public required FalError Error { get; init; }
}

/// <summary>
/// Fal AI error details.
/// </summary>
internal record FalError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
