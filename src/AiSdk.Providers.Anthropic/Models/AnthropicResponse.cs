using System.Text.Json.Serialization;

namespace AiSdk.Providers.Anthropic.Models;

/// <summary>
/// Anthropic messages API response.
/// </summary>
internal record AnthropicResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required List<AnthropicContentBlock> Content { get; init; }

    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; init; }

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; init; }

    [JsonPropertyName("usage")]
    public required AnthropicUsage Usage { get; init; }
}

/// <summary>
/// Anthropic usage statistics.
/// </summary>
internal record AnthropicUsage
{
    [JsonPropertyName("input_tokens")]
    public required int InputTokens { get; init; }

    [JsonPropertyName("output_tokens")]
    public required int OutputTokens { get; init; }
}

/// <summary>
/// Anthropic error response.
/// </summary>
internal record AnthropicErrorResponse
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("error")]
    public required AnthropicError Error { get; init; }
}

/// <summary>
/// Anthropic error details.
/// </summary>
internal record AnthropicError
{
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
