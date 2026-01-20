using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cerebras.Models;

/// <summary>
/// Cerebras chat completion response.
/// </summary>
internal record CerebrasResponse
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
    public required List<CerebrasChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public CerebrasUsage? Usage { get; init; }
}

/// <summary>
/// Cerebras choice in a response.
/// </summary>
internal record CerebrasChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public CerebrasMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Cerebras usage statistics.
/// </summary>
internal record CerebrasUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Cerebras error response.
/// </summary>
internal record CerebrasErrorResponse
{
    [JsonPropertyName("error")]
    public required CerebrasError Error { get; init; }
}

/// <summary>
/// Cerebras error details.
/// </summary>
internal record CerebrasError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
