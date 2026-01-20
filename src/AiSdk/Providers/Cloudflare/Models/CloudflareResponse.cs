using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cloudflare.Models;

/// <summary>
/// Cloudflare Workers AI response wrapper.
/// </summary>
internal record CloudflareResponse
{
    [JsonPropertyName("result")]
    public required CloudflareResult Result { get; init; }

    [JsonPropertyName("success")]
    public required bool Success { get; init; }

    [JsonPropertyName("errors")]
    public required List<CloudflareError> Errors { get; init; }

    [JsonPropertyName("messages")]
    public required List<string> Messages { get; init; }
}

/// <summary>
/// Cloudflare Workers AI result.
/// </summary>
internal record CloudflareResult
{
    [JsonPropertyName("response")]
    public string? Response { get; init; }
}

/// <summary>
/// Cloudflare Workers AI error response.
/// </summary>
internal record CloudflareErrorResponse
{
    [JsonPropertyName("success")]
    public required bool Success { get; init; }

    [JsonPropertyName("errors")]
    public required List<CloudflareError> Errors { get; init; }

    [JsonPropertyName("messages")]
    public required List<string> Messages { get; init; }
}

/// <summary>
/// Cloudflare Workers AI error details.
/// </summary>
internal record CloudflareError
{
    [JsonPropertyName("code")]
    public required int Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
