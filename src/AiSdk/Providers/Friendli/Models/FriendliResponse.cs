using System.Text.Json.Serialization;

namespace AiSdk.Providers.Friendli.Models;

/// <summary>
/// Friendli chat completion response.
/// </summary>
internal record FriendliResponse
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
    public required List<FriendliChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public FriendliUsage? Usage { get; init; }
}

/// <summary>
/// Friendli choice in a response.
/// </summary>
internal record FriendliChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public FriendliMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Friendli usage statistics.
/// </summary>
internal record FriendliUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Friendli error response.
/// </summary>
internal record FriendliErrorResponse
{
    [JsonPropertyName("error")]
    public required FriendliError Error { get; init; }
}

/// <summary>
/// Friendli error details.
/// </summary>
internal record FriendliError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
