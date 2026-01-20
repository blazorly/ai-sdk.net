using System.Text.Json.Serialization;

namespace AiSdk.Providers.Writer.Models;

/// <summary>
/// Writer chat completion response.
/// </summary>
internal record WriterResponse
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
    public required List<WriterChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public WriterUsage? Usage { get; init; }
}

/// <summary>
/// Writer choice in a response.
/// </summary>
internal record WriterChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public WriterMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Writer usage statistics.
/// </summary>
internal record WriterUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Writer error response.
/// </summary>
internal record WriterErrorResponse
{
    [JsonPropertyName("error")]
    public required WriterError Error { get; init; }
}

/// <summary>
/// Writer error details.
/// </summary>
internal record WriterError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
