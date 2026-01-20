using System.Text.Json.Serialization;

namespace AiSdk.Providers.Mistral.Models;

/// <summary>
/// Mistral AI chat completion response.
/// </summary>
internal record MistralResponse
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
    public required List<MistralChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public MistralUsage? Usage { get; init; }
}

/// <summary>
/// Mistral AI choice in a response.
/// </summary>
internal record MistralChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public MistralMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Mistral AI usage statistics.
/// </summary>
internal record MistralUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Mistral AI error response.
/// </summary>
internal record MistralErrorResponse
{
    [JsonPropertyName("error")]
    public required MistralError Error { get; init; }
}

/// <summary>
/// Mistral AI error details.
/// </summary>
internal record MistralError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
