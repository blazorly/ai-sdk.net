using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenAI.Models;

/// <summary>
/// OpenAI chat completion response.
/// </summary>
internal record OpenAIResponse
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
    public required List<OpenAIChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public OpenAIUsage? Usage { get; init; }
}

/// <summary>
/// OpenAI choice in a response.
/// </summary>
internal record OpenAIChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public OpenAIMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// OpenAI usage statistics.
/// </summary>
internal record OpenAIUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// OpenAI error response.
/// </summary>
internal record OpenAIErrorResponse
{
    [JsonPropertyName("error")]
    public required OpenAIError Error { get; init; }
}

/// <summary>
/// OpenAI error details.
/// </summary>
internal record OpenAIError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
