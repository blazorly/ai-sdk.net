using AiSdk.Abstractions;

namespace AiSdk.Middleware;

/// <summary>
/// Middleware for intercepting and transforming language model calls.
/// Follows the same pattern as ASP.NET Core middleware.
/// </summary>
public interface ILanguageModelMiddleware
{
    /// <summary>
    /// Intercepts a non-streaming generation call.
    /// </summary>
    /// <param name="options">The call options (can be modified before forwarding).</param>
    /// <param name="model">The underlying language model.</param>
    /// <param name="next">The next handler in the middleware chain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generation result (can be modified before returning).</returns>
    Task<LanguageModelGenerateResult> GenerateAsync(
        LanguageModelCallOptions options,
        ILanguageModel model,
        Func<LanguageModelCallOptions, CancellationToken, Task<LanguageModelGenerateResult>> next,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Intercepts a streaming generation call.
    /// </summary>
    /// <param name="options">The call options (can be modified before forwarding).</param>
    /// <param name="model">The underlying language model.</param>
    /// <param name="next">The next handler in the middleware chain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of stream chunks (can be modified before returning).</returns>
    IAsyncEnumerable<LanguageModelStreamChunk> StreamAsync(
        LanguageModelCallOptions options,
        ILanguageModel model,
        Func<LanguageModelCallOptions, CancellationToken, IAsyncEnumerable<LanguageModelStreamChunk>> next,
        CancellationToken cancellationToken = default);
}
