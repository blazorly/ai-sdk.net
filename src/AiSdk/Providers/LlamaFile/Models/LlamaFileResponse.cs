using System.Text.Json.Serialization;

namespace AiSdk.Providers.LlamaFile.Models;

/// <summary>
/// LlamaFile chat completion response.
/// </summary>
internal record LlamaFileResponse
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
    public required List<LlamaFileChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public LlamaFileUsage? Usage { get; init; }
}

/// <summary>
/// LlamaFile choice in a response.
/// </summary>
internal record LlamaFileChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public LlamaFileMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// LlamaFile usage statistics.
/// </summary>
internal record LlamaFileUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// LlamaFile error response.
/// </summary>
internal record LlamaFileErrorResponse
{
    [JsonPropertyName("error")]
    public required LlamaFileError Error { get; init; }
}

/// <summary>
/// LlamaFile error details.
/// </summary>
internal record LlamaFileError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
