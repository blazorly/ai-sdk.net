using System.Text.Json.Serialization;

namespace AiSdk.Providers.Fireworks.Models;

/// <summary>
/// Fireworks AI chat completion response.
/// </summary>
internal record FireworksResponse
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
    public required List<FireworksChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public FireworksUsage? Usage { get; init; }
}

/// <summary>
/// Fireworks AI choice in a response.
/// </summary>
internal record FireworksChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public FireworksMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Fireworks AI usage statistics.
/// </summary>
internal record FireworksUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Fireworks AI error response.
/// </summary>
internal record FireworksErrorResponse
{
    [JsonPropertyName("error")]
    public required FireworksError Error { get; init; }
}

/// <summary>
/// Fireworks AI error details.
/// </summary>
internal record FireworksError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
