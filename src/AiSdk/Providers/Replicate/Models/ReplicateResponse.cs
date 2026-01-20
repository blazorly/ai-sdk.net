using System.Text.Json.Serialization;

namespace AiSdk.Providers.Replicate.Models;

/// <summary>
/// Replicate prediction response.
/// </summary>
internal record ReplicateResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("output")]
    public object? Output { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("logs")]
    public string? Logs { get; init; }

    [JsonPropertyName("metrics")]
    public ReplicateMetrics? Metrics { get; init; }
}

/// <summary>
/// Replicate prediction metrics.
/// </summary>
internal record ReplicateMetrics
{
    [JsonPropertyName("predict_time")]
    public double? PredictTime { get; init; }
}

/// <summary>
/// Replicate error response.
/// </summary>
internal record ReplicateErrorResponse
{
    [JsonPropertyName("detail")]
    public string? Detail { get; init; }

    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("status")]
    public int? Status { get; init; }
}
