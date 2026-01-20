using System.Text.Json.Serialization;

namespace AiSdk.Providers.Luma.Models;

/// <summary>
/// Luma AI request structure.
/// Note: This is a placeholder for future video generation features.
/// Actual Luma API would have different parameters for video generation.
/// </summary>
internal record LumaRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("messages")]
    public required List<LumaMessage> Messages { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }

    // Future video generation parameters could include:
    // [JsonPropertyName("prompt")]
    // public string? Prompt { get; init; }

    // [JsonPropertyName("image_url")]
    // public string? ImageUrl { get; init; }

    // [JsonPropertyName("aspect_ratio")]
    // public string? AspectRatio { get; init; }

    // [JsonPropertyName("duration")]
    // public int? Duration { get; init; }
}

/// <summary>
/// Luma message in a request.
/// Note: Placeholder structure for future expansion.
/// </summary>
internal record LumaMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Content { get; init; }
}
