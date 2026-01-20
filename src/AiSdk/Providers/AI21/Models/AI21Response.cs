using System.Text.Json.Serialization;

namespace AiSdk.Providers.AI21.Models;

/// <summary>
/// AI21 Labs chat completion response.
/// </summary>
internal record AI21Response
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
    public required List<AI21Choice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public AI21Usage? Usage { get; init; }
}

/// <summary>
/// AI21 Labs choice in a response.
/// </summary>
internal record AI21Choice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public AI21Message? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// AI21 Labs usage statistics.
/// </summary>
internal record AI21Usage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// AI21 Labs error response.
/// </summary>
internal record AI21ErrorResponse
{
    [JsonPropertyName("error")]
    public required AI21Error Error { get; init; }
}

/// <summary>
/// AI21 Labs error details.
/// </summary>
internal record AI21Error
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
