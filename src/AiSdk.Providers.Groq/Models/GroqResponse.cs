using System.Text.Json.Serialization;

namespace AiSdk.Providers.Groq.Models;

/// <summary>
/// Groq chat completion response.
/// </summary>
internal record GroqResponse
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
    public required List<GroqChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public GroqUsage? Usage { get; init; }
}

/// <summary>
/// Groq choice in a response.
/// </summary>
internal record GroqChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public GroqMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Groq usage statistics.
/// </summary>
internal record GroqUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Groq error response.
/// </summary>
internal record GroqErrorResponse
{
    [JsonPropertyName("error")]
    public required GroqError Error { get; init; }
}

/// <summary>
/// Groq error details.
/// </summary>
internal record GroqError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
