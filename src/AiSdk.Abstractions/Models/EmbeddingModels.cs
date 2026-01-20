namespace AiSdk.Abstractions;

/// <summary>
/// Options for embedding generation.
/// </summary>
public record EmbeddingOptions
{
    /// <summary>
    /// Dimensions for the output embeddings (if supported by the model).
    /// </summary>
    public int? Dimensions { get; init; }

    /// <summary>
    /// Provider-specific options.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ProviderOptions { get; init; }
}

/// <summary>
/// Result from a single embedding generation.
/// </summary>
public record EmbeddingResult
{
    /// <summary>
    /// The embedding vector.
    /// </summary>
    public required float[] Embedding { get; init; }

    /// <summary>
    /// Token usage statistics.
    /// </summary>
    public required Usage Usage { get; init; }
}

/// <summary>
/// Result from batch embedding generation.
/// </summary>
public record BatchEmbeddingResult
{
    /// <summary>
    /// The embedding vectors for each input.
    /// </summary>
    public required IReadOnlyList<float[]> Embeddings { get; init; }

    /// <summary>
    /// Token usage statistics.
    /// </summary>
    public required Usage Usage { get; init; }
}
