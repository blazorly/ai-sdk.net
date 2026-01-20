using AiSdk.Abstractions;

namespace AiSdk.Providers.Cerebras;

/// <summary>
/// Factory for creating Cerebras language models.
/// Cerebras provides ultra-fast inference powered by the world's largest AI processor.
/// </summary>
public class CerebrasProvider
{
    private readonly CerebrasConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CerebrasProvider"/> class.
    /// </summary>
    /// <param name="config">The Cerebras configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public CerebrasProvider(CerebrasConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Llama 3.3 70B model.
    /// High-performance model optimized for ultra-fast inference with excellent capabilities.
    /// </summary>
    /// <returns>A Cerebras Llama 3.3 70B language model.</returns>
    public ILanguageModel Llama33_70B()
    {
        return new CerebrasChatLanguageModel("llama-3.3-70b", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.1 8B model.
    /// Fast, efficient model for general-purpose tasks with reduced latency.
    /// </summary>
    /// <returns>A Cerebras Llama 3.1 8B language model.</returns>
    public ILanguageModel Llama31_8B()
    {
        return new CerebrasChatLanguageModel("llama3.1-8b", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.1 70B model.
    /// Powerful model for complex tasks with ultra-fast inference.
    /// </summary>
    /// <returns>A Cerebras Llama 3.1 70B language model.</returns>
    public ILanguageModel Llama31_70B()
    {
        return new CerebrasChatLanguageModel("llama3.1-70b", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Cerebras model ID.</param>
    /// <returns>A Cerebras chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new CerebrasChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Cerebras chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Cerebras model ID (e.g., "llama-3.3-70b", "llama3.1-8b").</param>
    /// <param name="apiKey">The Cerebras API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.cerebras.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cerebras chat language model.</returns>
    public static CerebrasChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new CerebrasConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.cerebras.ai/v1"
        };

        return new CerebrasChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Cerebras chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Cerebras model ID.</param>
    /// <param name="config">The Cerebras configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cerebras chat language model.</returns>
    public static CerebrasChatLanguageModel CreateChatModel(
        string modelId,
        CerebrasConfiguration config,
        HttpClient? httpClient = null)
    {
        return new CerebrasChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.3 70B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cerebras API key.</param>
    /// <returns>A Cerebras Llama 3.3 70B language model.</returns>
    public static CerebrasChatLanguageModel Llama33_70B(string apiKey)
    {
        return CreateChatModel("llama-3.3-70b", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 8B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cerebras API key.</param>
    /// <returns>A Cerebras Llama 3.1 8B language model.</returns>
    public static CerebrasChatLanguageModel Llama31_8B(string apiKey)
    {
        return CreateChatModel("llama3.1-8b", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 70B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cerebras API key.</param>
    /// <returns>A Cerebras Llama 3.1 70B language model.</returns>
    public static CerebrasChatLanguageModel Llama31_70B(string apiKey)
    {
        return CreateChatModel("llama3.1-70b", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Cerebras model ID.</param>
    /// <param name="apiKey">The Cerebras API key.</param>
    /// <returns>A Cerebras chat language model.</returns>
    public static CerebrasChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
