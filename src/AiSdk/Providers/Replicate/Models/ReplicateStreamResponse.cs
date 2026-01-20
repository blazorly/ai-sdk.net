using System.Text.Json.Serialization;

namespace AiSdk.Providers.Replicate.Models;

/// <summary>
/// Replicate streaming response event.
/// </summary>
internal record ReplicateStreamResponse
{
    [JsonPropertyName("event")]
    public string? Event { get; init; }

    [JsonPropertyName("data")]
    public string? Data { get; init; }

    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("retry")]
    public int? Retry { get; init; }
}

/// <summary>
/// Replicate stream output event.
/// </summary>
internal record ReplicateStreamOutput
{
    [JsonPropertyName("output")]
    public required string Output { get; init; }
}

/// <summary>
/// Replicate stream done event with metrics.
/// </summary>
internal record ReplicateStreamDone
{
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    [JsonPropertyName("metrics")]
    public ReplicateMetrics? Metrics { get; init; }

    [JsonPropertyName("output")]
    public object? Output { get; init; }
}
