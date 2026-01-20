using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cloudflare.Models;

/// <summary>
/// Cloudflare Workers AI streaming response chunk.
/// </summary>
internal record CloudflareStreamResponse
{
    [JsonPropertyName("response")]
    public string? Response { get; init; }
}

/// <summary>
/// Cloudflare Workers AI streaming delta.
/// </summary>
internal record CloudflareDelta
{
    [JsonPropertyName("content")]
    public string? Content { get; init; }
}
