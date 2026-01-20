namespace AiSdk.Models;

/// <summary>
/// Represents a chunk of a streaming object response.
/// </summary>
/// <typeparam name="T">The type of the object being streamed.</typeparam>
public record ObjectChunk<T>
{
    /// <summary>
    /// The partial object, if available.
    /// </summary>
    public T? Object { get; init; }

    /// <summary>
    /// The raw text delta for this chunk.
    /// </summary>
    public string? Delta { get; init; }

    /// <summary>
    /// The accumulated raw text up to this point.
    /// </summary>
    public string? AccumulatedText { get; init; }

    /// <summary>
    /// Whether this is the final chunk in the stream.
    /// </summary>
    public bool IsComplete { get; init; }

    /// <summary>
    /// Error information if the chunk encountered an error.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Token usage information if available.
    /// </summary>
    public Abstractions.Usage? Usage { get; init; }
}
