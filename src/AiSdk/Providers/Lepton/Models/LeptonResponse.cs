using System.Text.Json.Serialization;

namespace AiSdk.Providers.Lepton.Models;

/// <summary>
/// Lepton AI chat completion response.
/// </summary>
internal record LeptonResponse
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
    public required List<LeptonChoice> Choices { get; init; }

    [JsonPropertyName("usage")]
    public LeptonUsage? Usage { get; init; }
}

/// <summary>
/// Lepton AI choice in a response.
/// </summary>
internal record LeptonChoice
{
    [JsonPropertyName("index")]
    public required int Index { get; init; }

    [JsonPropertyName("message")]
    public LeptonMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; init; }
}

/// <summary>
/// Lepton AI usage statistics.
/// </summary>
internal record LeptonUsage
{
    [JsonPropertyName("prompt_tokens")]
    public required int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public required int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public required int TotalTokens { get; init; }
}

/// <summary>
/// Lepton AI error response.
/// </summary>
internal record LeptonErrorResponse
{
    [JsonPropertyName("error")]
    public required LeptonError Error { get; init; }
}

/// <summary>
/// Lepton AI error details.
/// </summary>
internal record LeptonError
{
    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
