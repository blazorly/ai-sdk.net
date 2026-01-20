using System.Text.Json.Serialization;

namespace AiSdk.Providers.HuggingFace.Models;

/// <summary>
/// Hugging Face Inference API request.
/// </summary>
internal record HuggingFaceRequest
{
    [JsonPropertyName("inputs")]
    public required string Inputs { get; init; }

    [JsonPropertyName("parameters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HuggingFaceParameters? Parameters { get; init; }

    [JsonPropertyName("stream")]
    public bool Stream { get; init; }
}

/// <summary>
/// Hugging Face request parameters.
/// </summary>
internal record HuggingFaceParameters
{
    [JsonPropertyName("max_new_tokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MaxNewTokens { get; init; }

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Temperature { get; init; }

    [JsonPropertyName("top_p")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? TopP { get; init; }

    [JsonPropertyName("stop")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Stop { get; init; }

    [JsonPropertyName("return_full_text")]
    public bool ReturnFullText { get; init; } = false;
}
