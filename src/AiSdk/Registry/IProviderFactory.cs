using AiSdk.Abstractions;

namespace AiSdk.Registry;

/// <summary>
/// Factory interface for creating language and embedding models from a provider.
/// Each provider implements this to register itself with the ProviderRegistry.
/// </summary>
public interface IProviderFactory
{
    /// <summary>
    /// Gets the provider identifier (e.g., "openai", "google", "azure-openai").
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    /// Creates a language model for the specified model ID.
    /// </summary>
    /// <param name="modelId">The provider-specific model ID (e.g., "gpt-4o", "gemini-1.5-pro").</param>
    /// <returns>A language model instance.</returns>
    ILanguageModel CreateLanguageModel(string modelId);

    /// <summary>
    /// Creates an embedding model for the specified model ID.
    /// </summary>
    /// <param name="modelId">The provider-specific embedding model ID (e.g., "text-embedding-3-small").</param>
    /// <returns>An embedding model instance.</returns>
    IEmbeddingModel CreateEmbeddingModel(string modelId);
}
