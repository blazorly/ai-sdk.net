using System.Text.Json.Serialization;

namespace AiSdk.Providers.Google.Models;

/// <summary>
/// Google Gemini API response.
/// </summary>
internal record GoogleResponse
{
    [JsonPropertyName("candidates")]
    public required List<GoogleCandidate> Candidates { get; init; }

    [JsonPropertyName("usageMetadata")]
    public GoogleUsageMetadata? UsageMetadata { get; init; }

    [JsonPropertyName("promptFeedback")]
    public GooglePromptFeedback? PromptFeedback { get; init; }
}

/// <summary>
/// Google candidate in a response.
/// </summary>
internal record GoogleCandidate
{
    [JsonPropertyName("content")]
    public GoogleContent? Content { get; init; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; init; }

    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("safetyRatings")]
    public List<GoogleSafetyRating>? SafetyRatings { get; init; }
}

/// <summary>
/// Google usage metadata.
/// </summary>
internal record GoogleUsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; init; }

    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; init; }

    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; init; }
}

/// <summary>
/// Google prompt feedback.
/// </summary>
internal record GooglePromptFeedback
{
    [JsonPropertyName("blockReason")]
    public string? BlockReason { get; init; }

    [JsonPropertyName("safetyRatings")]
    public List<GoogleSafetyRating>? SafetyRatings { get; init; }
}

/// <summary>
/// Google safety rating.
/// </summary>
internal record GoogleSafetyRating
{
    [JsonPropertyName("category")]
    public required string Category { get; init; }

    [JsonPropertyName("probability")]
    public required string Probability { get; init; }
}

/// <summary>
/// Google error response.
/// </summary>
internal record GoogleErrorResponse
{
    [JsonPropertyName("error")]
    public required GoogleError Error { get; init; }
}

/// <summary>
/// Google error details.
/// </summary>
internal record GoogleError
{
    [JsonPropertyName("code")]
    public int Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }
}
