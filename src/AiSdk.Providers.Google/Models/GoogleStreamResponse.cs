using System.Text.Json.Serialization;

namespace AiSdk.Providers.Google.Models;

/// <summary>
/// Google Gemini API streaming response chunk.
/// This is essentially the same as GoogleResponse but may have partial content.
/// </summary>
internal record GoogleStreamResponse
{
    [JsonPropertyName("candidates")]
    public required List<GoogleCandidate> Candidates { get; init; }

    [JsonPropertyName("usageMetadata")]
    public GoogleUsageMetadata? UsageMetadata { get; init; }

    [JsonPropertyName("promptFeedback")]
    public GooglePromptFeedback? PromptFeedback { get; init; }
}
