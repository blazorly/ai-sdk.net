using System.Text.Json.Serialization;

namespace AiSdk.Providers.DeepSeek.Models;

/// <summary>
/// DeepSeek chat completion response.
/// </summary>
internal record DeepSeekResponse
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
    public required List<DeepSeekChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public DeepSeekUsage? Usage { get; init; }
}

/// <summary>
/// DeepSeek choice in a response.
/// </summary>
internal record DeepSeekChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public DeepSeekMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// DeepSeek usage statistics.
/// </summary>
internal record DeepSeekUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// DeepSeek error response.
/// </summary>
internal record DeepSeekErrorResponse
{
    [JsonPropertyName("error")]
    public required DeepSeekError Error { get; init; }
}

/// <summary>
/// DeepSeek error details.
/// </summary>
internal record DeepSeekError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
