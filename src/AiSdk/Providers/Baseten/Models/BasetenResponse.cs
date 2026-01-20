using System.Text.Json.Serialization;

namespace AiSdk.Providers.Baseten.Models;

/// <summary>
/// Baseten chat completion response.
/// </summary>
internal record BasetenResponse
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
    public required List<BasetenChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public BasetenUsage? Usage { get; init; }
}

/// <summary>
/// Baseten choice in a response.
/// </summary>
internal record BasetenChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public BasetenMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Baseten usage statistics.
/// </summary>
internal record BasetenUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Baseten error response.
/// </summary>
internal record BasetenErrorResponse
{
    [JsonPropertyName("error")]
    public required BasetenError Error { get; init; }
}

/// <summary>
/// Baseten error details.
/// </summary>
internal record BasetenError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
