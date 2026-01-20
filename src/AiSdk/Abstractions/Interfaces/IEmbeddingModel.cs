namespace AiSdk.Abstractions;

/// <summary>
/// Represents an embedding model that can convert text into vector representations.
/// </summary>
public interface IEmbeddingModel
{
    /// <summary>
    /// Gets the provider identifier (e.g., "openai", "cohere", "google").
    /// </summary>
    string Provider { get; }

    /// <summary>
    /// Gets the provider-specific model identifier (e.g., "text-embedding-3-small").
    /// </summary>
    string ModelId { get; }

    /// <summary>
    /// Generates an embedding for a single text input.
    /// </summary>
    /// <param name="input">The text to embed.</param>
    /// <param name="options">Optional embedding options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The embedding result containing the vector and usage statistics.</returns>
    Task<EmbeddingResult> EmbedAsync(
        string input,
        EmbeddingOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for multiple text inputs in a batch.
    /// </summary>
    /// <param name="inputs">The texts to embed.</param>
    /// <param name="options">Optional embedding options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The batch embedding result containing all vectors and usage statistics.</returns>
    Task<BatchEmbeddingResult> EmbedManyAsync(
        IEnumerable<string> inputs,
        EmbeddingOptions? options = null,
        CancellationToken cancellationToken = default);
}
