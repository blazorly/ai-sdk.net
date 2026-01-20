namespace AiSdk.Abstractions;

/// <summary>
/// Represents token usage statistics for an AI model call.
/// </summary>
/// <param name="InputTokens">Number of tokens in the input prompt.</param>
/// <param name="OutputTokens">Number of tokens in the generated output.</param>
/// <param name="TotalTokens">Total number of tokens (input + output).</param>
/// <param name="ReasoningTokens">Number of tokens used for reasoning (if supported by model).</param>
/// <param name="CachedInputTokens">Number of cached input tokens (for prompt caching).</param>
public record Usage(
    int? InputTokens = null,
    int? OutputTokens = null,
    int? TotalTokens = null,
    int? ReasoningTokens = null,
    int? CachedInputTokens = null);
