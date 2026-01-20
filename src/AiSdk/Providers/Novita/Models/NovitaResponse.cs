using System.Text.Json.Serialization;

namespace AiSdk.Providers.Novita.Models;

/// <summary>
/// Novita chat completion response.
/// </summary>
internal record NovitaResponse
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
    public required List<NovitaChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public NovitaUsage? Usage { get; init; }
}

/// <summary>
/// Novita choice in a response.
/// </summary>
internal record NovitaChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public NovitaMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Novita usage statistics.
/// </summary>
internal record NovitaUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Novita error response.
/// </summary>
internal record NovitaErrorResponse
{
    [JsonPropertyName("error")]
    public required NovitaError Error { get; init; }
}

/// <summary>
/// Novita error details.
/// </summary>
internal record NovitaError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
