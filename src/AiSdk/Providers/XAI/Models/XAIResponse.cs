using System.Text.Json.Serialization;

namespace AiSdk.Providers.XAI.Models;

/// <summary>
/// xAI chat completion response.
/// </summary>
internal record XAIResponse
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
    public required List<XAIChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public XAIUsage? Usage { get; init; }
}

/// <summary>
/// xAI choice in a response.
/// </summary>
internal record XAIChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public XAIMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// xAI usage statistics.
/// </summary>
internal record XAIUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// xAI error response.
/// </summary>
internal record XAIErrorResponse
{
    [JsonPropertyName("error")]
    public required XAIError Error { get; init; }
}

/// <summary>
/// xAI error details.
/// </summary>
internal record XAIError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
