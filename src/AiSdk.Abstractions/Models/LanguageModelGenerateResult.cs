namespace AiSdk.Abstractions;

/// <summary>
/// Result from a non-streaming language model generation call.
/// </summary>
public record LanguageModelGenerateResult
{
    /// <summary>
    /// The generated text content.
    /// </summary>
    public string? Text { get; init; }

    /// <summary>
    /// The reason the model stopped generating.
    /// </summary>
    public required FinishReason FinishReason { get; init; }

    /// <summary>
    /// Token usage statistics.
    /// </summary>
    public required Usage Usage { get; init; }

    /// <summary>
    /// Tool calls made by the model.
    /// </summary>
    public IReadOnlyList<ToolCall>? ToolCalls { get; init; }

    /// <summary>
    /// Provider-specific metadata.
    /// </summary>
    public object? ProviderMetadata { get; init; }

    /// <summary>
    /// Warnings from the provider.
    /// </summary>
    public IReadOnlyList<string>? Warnings { get; init; }

    /// <summary>
    /// Raw finish reason from the provider (before normalization).
    /// </summary>
    public string? RawFinishReason { get; init; }
}
