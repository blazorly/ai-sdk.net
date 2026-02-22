using AiSdk.Abstractions;

namespace AiSdk.Middleware;

/// <summary>
/// Extension methods for adding middleware to language models.
/// </summary>
public static class LanguageModelExtensions
{
    /// <summary>
    /// Wraps a language model with middleware.
    /// Multiple calls can be chained: model.WithMiddleware(a).WithMiddleware(b).
    /// Middleware executes in the order added (a runs before b).
    /// </summary>
    /// <param name="model">The language model to wrap.</param>
    /// <param name="middleware">The middleware to add.</param>
    /// <returns>A new language model that passes calls through the middleware.</returns>
    public static ILanguageModel WithMiddleware(this ILanguageModel model, ILanguageModelMiddleware middleware)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(middleware);

        // If already wrapped, append to existing chain
        if (model is MiddlewareLanguageModel wrapped)
        {
            var existing = new List<ILanguageModelMiddleware>(wrapped.GetMiddlewares());
            existing.Add(middleware);
            return new MiddlewareLanguageModel(wrapped.GetInnerModel(), existing.AsReadOnly());
        }

        return new MiddlewareLanguageModel(model, new[] { middleware });
    }

    /// <summary>
    /// Wraps a language model with multiple middlewares at once.
    /// </summary>
    /// <param name="model">The language model to wrap.</param>
    /// <param name="middlewares">The middlewares to add (executed in order).</param>
    /// <returns>A new language model that passes calls through all middlewares.</returns>
    public static ILanguageModel WithMiddleware(this ILanguageModel model, params ILanguageModelMiddleware[] middlewares)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (middlewares.Length == 0)
            return model;

        if (model is MiddlewareLanguageModel wrapped)
        {
            var existing = new List<ILanguageModelMiddleware>(wrapped.GetMiddlewares());
            existing.AddRange(middlewares);
            return new MiddlewareLanguageModel(wrapped.GetInnerModel(), existing.AsReadOnly());
        }

        return new MiddlewareLanguageModel(model, middlewares);
    }
}
