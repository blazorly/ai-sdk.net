using System.Text.Json.Serialization;

namespace AiSdk.Providers.Vercel.Models;

/// <summary>
/// Vercel AI Gateway chat completion response.
/// </summary>
internal record VercelResponse
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
    public required List<VercelChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public VercelUsage? Usage { get; init; }
}

/// <summary>
/// Vercel choice in a response.
/// </summary>
internal record VercelChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public VercelMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Vercel usage statistics.
/// </summary>
internal record VercelUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Vercel error response.
/// </summary>
internal record VercelErrorResponse
{
    [JsonPropertyName("error")]
    public required VercelError Error { get; init; }
}

/// <summary>
/// Vercel error details.
/// </summary>
internal record VercelError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
