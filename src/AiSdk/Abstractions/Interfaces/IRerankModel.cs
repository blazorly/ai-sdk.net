namespace AiSdk.Abstractions;

/// <summary>
/// Represents a reranking model that scores and reorders documents by relevance to a query.
/// Used in RAG pipelines to improve retrieval quality.
/// </summary>
public interface IRerankModel
{
    /// <summary>
    /// Gets the provider identifier (e.g., "cohere").
    /// </summary>
    string Provider { get; }

    /// <summary>
    /// Gets the provider-specific model identifier (e.g., "rerank-v3.5").
    /// </summary>
    string ModelId { get; }

    /// <summary>
    /// Reranks a list of documents based on their relevance to a query.
    /// </summary>
    /// <param name="query">The search query to rank against.</param>
    /// <param name="documents">The documents to rerank.</param>
    /// <param name="options">Optional reranking options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The reranking result with scored and sorted documents.</returns>
    Task<RerankResult> RerankAsync(
        string query,
        IEnumerable<string> documents,
        RerankOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for reranking.
/// </summary>
public record RerankOptions
{
    /// <summary>
    /// Maximum number of results to return (top-k).
    /// </summary>
    public int? TopN { get; init; }

    /// <summary>
    /// Whether to return the document text in results.
    /// </summary>
    public bool ReturnDocuments { get; init; } = true;

    /// <summary>
    /// Provider-specific options.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ProviderOptions { get; init; }
}

/// <summary>
/// Result from reranking.
/// </summary>
public record RerankResult
{
    /// <summary>
    /// The reranked results, sorted by relevance score (highest first).
    /// </summary>
    public required IReadOnlyList<RankedDocument> Results { get; init; }

    /// <summary>
    /// Token usage statistics (if available).
    /// </summary>
    public Usage? Usage { get; init; }
}

/// <summary>
/// A document with its relevance score from reranking.
/// </summary>
public record RankedDocument
{
    /// <summary>
    /// The original index of this document in the input list.
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// The relevance score (higher is more relevant).
    /// </summary>
    public required double RelevanceScore { get; init; }

    /// <summary>
    /// The document text (if ReturnDocuments was true).
    /// </summary>
    public string? Document { get; init; }
}
