using AiSdk.Abstractions;

namespace AiSdk.Registry;

/// <summary>
/// Registry for resolving AI models from configuration strings.
/// Parses strings like "openai/gpt-4o" into provider + model ID
/// and creates the appropriate model instance.
/// </summary>
public class ProviderRegistry
{
    private readonly Dictionary<string, IProviderFactory> _providers = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Registers a provider factory.
    /// </summary>
    /// <param name="factory">The provider factory to register.</param>
    /// <returns>This registry instance for chaining.</returns>
    public ProviderRegistry Register(IProviderFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _providers[factory.ProviderId] = factory;
        return this;
    }

    /// <summary>
    /// Creates a language model from a model string like "openai/gpt-4o".
    /// </summary>
    /// <param name="modelString">The model string in "provider/model-id" format.</param>
    /// <returns>A language model instance.</returns>
    /// <exception cref="ArgumentException">If the model string format is invalid.</exception>
    /// <exception cref="InvalidOperationException">If the provider is not registered.</exception>
    public ILanguageModel LanguageModel(string modelString)
    {
        var (providerId, modelId) = ParseModelString(modelString);
        var factory = GetFactory(providerId);
        return factory.CreateLanguageModel(modelId);
    }

    /// <summary>
    /// Creates an embedding model from a model string like "openai/text-embedding-3-small".
    /// </summary>
    /// <param name="modelString">The model string in "provider/model-id" format.</param>
    /// <returns>An embedding model instance.</returns>
    /// <exception cref="ArgumentException">If the model string format is invalid.</exception>
    /// <exception cref="InvalidOperationException">If the provider is not registered.</exception>
    public IEmbeddingModel EmbeddingModel(string modelString)
    {
        var (providerId, modelId) = ParseModelString(modelString);
        var factory = GetFactory(providerId);
        return factory.CreateEmbeddingModel(modelId);
    }

    /// <summary>
    /// Checks if a provider is registered.
    /// </summary>
    /// <param name="providerId">The provider identifier.</param>
    /// <returns>True if the provider is registered.</returns>
    public bool HasProvider(string providerId) => _providers.ContainsKey(providerId);

    /// <summary>
    /// Gets all registered provider IDs.
    /// </summary>
    /// <returns>The registered provider IDs.</returns>
    public IReadOnlyCollection<string> GetProviderIds() => _providers.Keys;

    private IProviderFactory GetFactory(string providerId)
    {
        if (!_providers.TryGetValue(providerId, out var factory))
        {
            var available = _providers.Count > 0
                ? string.Join(", ", _providers.Keys)
                : "none";
            throw new InvalidOperationException(
                $"Provider '{providerId}' is not registered. Available providers: {available}. " +
                $"Register it with registry.Register(new {providerId}ProviderFactory(...)).");
        }

        return factory;
    }

    private static (string ProviderId, string ModelId) ParseModelString(string modelString)
    {
        ArgumentNullException.ThrowIfNull(modelString);

        var separatorIndex = modelString.IndexOf('/');
        if (separatorIndex <= 0 || separatorIndex >= modelString.Length - 1)
        {
            throw new ArgumentException(
                $"Invalid model string '{modelString}'. Expected format: 'provider/model-id' (e.g., 'openai/gpt-4o').",
                nameof(modelString));
        }

        var providerId = modelString[..separatorIndex];
        var modelId = modelString[(separatorIndex + 1)..];

        return (providerId, modelId);
    }
}
