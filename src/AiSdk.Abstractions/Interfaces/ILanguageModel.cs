using System.Runtime.CompilerServices;

namespace AiSdk.Abstractions;

/// <summary>
/// Represents a language model that can generate text and handle streaming responses.
/// This interface follows the LanguageModelV3 specification from the Vercel AI SDK.
/// </summary>
public interface ILanguageModel
{
    /// <summary>
    /// Gets the specification version this model implements.
    /// </summary>
    string SpecificationVersion { get; }

    /// <summary>
    /// Gets the provider identifier (e.g., "openai", "anthropic", "google").
    /// </summary>
    string Provider { get; }

    /// <summary>
    /// Gets the provider-specific model identifier (e.g., "gpt-4", "claude-3-opus-20240229").
    /// </summary>
    string ModelId { get; }

    /// <summary>
    /// Gets the supported URL patterns by media type for this provider.
    /// </summary>
    /// <returns>
    /// A dictionary where keys are media type patterns (e.g., "*/*", "audio/*", "video/*", "application/pdf")
    /// and values are arrays of regular expressions that match supported URL paths.
    /// Matched URLs are supported natively by the model and are not downloaded.
    /// </returns>
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a language model output (non-streaming).
    /// </summary>
    /// <param name="options">The call options including prompt, tools, and settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated result including text, finish reason, and usage statistics.</returns>
    Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a language model output (streaming).
    /// </summary>
    /// <param name="options">The call options including prompt, tools, and settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async stream of chunks containing partial text, tool calls, and other content.</returns>
    IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default);
}
