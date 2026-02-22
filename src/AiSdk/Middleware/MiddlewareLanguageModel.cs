using System.Runtime.CompilerServices;
using AiSdk.Abstractions;

namespace AiSdk.Middleware;

/// <summary>
/// Wraps an ILanguageModel with a middleware chain.
/// Each call passes through all middleware before reaching the inner model.
/// </summary>
public class MiddlewareLanguageModel : ILanguageModel
{
    private readonly ILanguageModel _innerModel;
    private readonly IReadOnlyList<ILanguageModelMiddleware> _middlewares;

    /// <inheritdoc />
    public string SpecificationVersion => _innerModel.SpecificationVersion;

    /// <inheritdoc />
    public string Provider => _innerModel.Provider;

    /// <inheritdoc />
    public string ModelId => _innerModel.ModelId;

    /// <summary>
    /// Creates a new MiddlewareLanguageModel wrapping the given model with the specified middleware chain.
    /// </summary>
    /// <param name="innerModel">The underlying language model.</param>
    /// <param name="middlewares">The middleware chain (executed in order).</param>
    public MiddlewareLanguageModel(ILanguageModel innerModel, IReadOnlyList<ILanguageModelMiddleware> middlewares)
    {
        _innerModel = innerModel ?? throw new ArgumentNullException(nameof(innerModel));
        _middlewares = middlewares ?? throw new ArgumentNullException(nameof(middlewares));
    }

    /// <summary>
    /// Gets the inner (unwrapped) language model.
    /// </summary>
    internal ILanguageModel GetInnerModel() => _innerModel;

    /// <summary>
    /// Gets the current middleware chain.
    /// </summary>
    internal IReadOnlyList<ILanguageModelMiddleware> GetMiddlewares() => _middlewares;

    /// <inheritdoc />
    public Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> GetSupportedUrlsAsync(
        CancellationToken cancellationToken = default)
        => _innerModel.GetSupportedUrlsAsync(cancellationToken);

    /// <inheritdoc />
    public Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        // Build the chain from the innermost (model) outward
        Func<LanguageModelCallOptions, CancellationToken, Task<LanguageModelGenerateResult>> chain =
            (opts, ct) => _innerModel.GenerateAsync(opts, ct);

        // Wrap each middleware around the chain, from last to first
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewares[i];
            var next = chain;
            chain = (opts, ct) => middleware.GenerateAsync(opts, _innerModel, next, ct);
        }

        return chain(options, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        CancellationToken cancellationToken = default)
    {
        // Build the chain from the innermost (model) outward
        Func<LanguageModelCallOptions, CancellationToken, IAsyncEnumerable<LanguageModelStreamChunk>> chain =
            (opts, ct) => _innerModel.StreamAsync(opts, ct);

        // Wrap each middleware around the chain, from last to first
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewares[i];
            var next = chain;
            chain = (opts, ct) => middleware.StreamAsync(opts, _innerModel, next, ct);
        }

        return chain(options, cancellationToken);
    }
}
