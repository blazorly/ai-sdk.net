using System.Text.Json.Serialization;

namespace AiSdk.Providers.Cloudflare.Models;

/// <summary>
/// Cloudflare Workers AI chat completion request.
/// </summary>
internal record CloudflareRequest
{
    [JsonPropertyName("messages")]
    public required List<CloudflareMessage> Messages { get; init; }

    [JsonPropertyName("max_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("top_p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stream")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Stream { get; init; }
}

/// <summary>
/// Cloudflare Workers AI message in a request.
/// </summary>
internal record CloudflareMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
