using System.Text.Json;

namespace AiSdk.Abstractions;

/// <summary>
/// Options for calling a language model.
/// </summary>
public record LanguageModelCallOptions
{
    /// <summary>
    /// The messages to send to the model.
    /// </summary>
    public required IReadOnlyList<Message> Messages { get; init; }

    /// <summary>
    /// Available tools that the model can call.
    /// </summary>
    public IReadOnlyList<ToolDefinition>? Tools { get; init; }

    /// <summary>
    /// Tool choice strategy ("auto", "none", "required", or specific tool name).
    /// </summary>
    public string? ToolChoice { get; init; }

    /// <summary>
    /// Maximum number of tokens to generate.
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// Temperature for sampling (typically 0.0 to 2.0).
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Top-p nucleus sampling parameter.
    /// </summary>
    public double? TopP { get; init; }

    /// <summary>
    /// Top-k sampling parameter.
    /// </summary>
    public int? TopK { get; init; }

    /// <summary>
    /// Presence penalty (-2.0 to 2.0).
    /// </summary>
    public double? PresencePenalty { get; init; }

    /// <summary>
    /// Frequency penalty (-2.0 to 2.0).
    /// </summary>
    public double? FrequencyPenalty { get; init; }

    /// <summary>
    /// Stop sequences where the model will stop generating.
    /// </summary>
    public IReadOnlyList<string>? StopSequences { get; init; }

    /// <summary>
    /// Seed for deterministic sampling.
    /// </summary>
    public int? Seed { get; init; }

    /// <summary>
    /// Response format specification (e.g., for JSON mode).
    /// </summary>
    public JsonDocument? ResponseFormat { get; init; }

    /// <summary>
    /// Provider-specific options.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ProviderOptions { get; init; }

    /// <summary>
    /// Additional HTTP headers to send with the request.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Headers { get; init; }
}
