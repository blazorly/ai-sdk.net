using System.Text.Json.Serialization;

namespace AiSdk.Providers.Replicate.Models;

/// <summary>
/// Replicate prediction request for chat models.
/// </summary>
internal record ReplicateRequest
{
    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("input")]
    public required ReplicateInput Input { get; init; }

    [JsonPropertyName("stream")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Stream { get; init; }
}

/// <summary>
/// Replicate input parameters for chat models.
/// </summary>
internal record ReplicateInput
{
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    [JsonPropertyName("max_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxTokens { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("top_p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stop_sequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StopSequences { get; init; }

    [JsonPropertyName("system_prompt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SystemPrompt { get; init; }
}
