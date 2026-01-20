using System.Text.Json.Serialization;

namespace AiSdk.Providers.Perplexity.Models;

/// <summary>
/// Perplexity chat completion response.
/// </summary>
internal record PerplexityResponse
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
    public required List<PerplexityChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public PerplexityUsage? Usage { get; init; }
}

/// <summary>
/// Perplexity choice in a response.
/// </summary>
internal record PerplexityChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public PerplexityMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Perplexity usage statistics.
/// </summary>
internal record PerplexityUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Perplexity error response.
/// </summary>
internal record PerplexityErrorResponse
{
    [JsonPropertyName("error")]
    public required PerplexityError Error { get; init; }
}

/// <summary>
/// Perplexity error details.
/// </summary>
internal record PerplexityError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
