using System.Text.Json.Serialization;

namespace AiSdk.Providers.OpenAICompatible.Models;

/// <summary>
/// OpenAI-compatible chat completion response.
/// </summary>
internal record OpenAICompatibleResponse
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
    public required List<OpenAICompatibleChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public OpenAICompatibleUsage? Usage { get; init; }
}

/// <summary>
/// OpenAI-compatible choice in a response.
/// </summary>
internal record OpenAICompatibleChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public OpenAICompatibleMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// OpenAI-compatible usage statistics.
/// </summary>
internal record OpenAICompatibleUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// OpenAI-compatible error response.
/// </summary>
internal record OpenAICompatibleErrorResponse
{
    [JsonPropertyName("error")]
    public required OpenAICompatibleError Error { get; init; }
}

/// <summary>
/// OpenAI-compatible error details.
/// </summary>
internal record OpenAICompatibleError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
