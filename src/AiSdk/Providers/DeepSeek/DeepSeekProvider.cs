using AiSdk.Abstractions;

namespace AiSdk.Providers.DeepSeek;

/// <summary>
/// Factory for creating DeepSeek language models.
/// </summary>
public class DeepSeekProvider
{
    private readonly DeepSeekConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeepSeekProvider"/> class.
    /// </summary>
    /// <param name="config">The DeepSeek configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public DeepSeekProvider(DeepSeekConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a DeepSeek Chat model (general purpose).
    /// Versatile model for general conversations, writing, and analysis.
    /// </summary>
    /// <returns>A DeepSeek Chat language model.</returns>
    public ILanguageModel Chat()
    {
        return new DeepSeekChatLanguageModel("deepseek-chat", _config, _httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek Coder model (code-focused).
    /// Specialized model optimized for code generation, debugging, and technical tasks.
    /// </summary>
    /// <returns>A DeepSeek Coder language model.</returns>
    public ILanguageModel Coder()
    {
        return new DeepSeekChatLanguageModel("deepseek-coder", _config, _httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek Reasoner model (DeepSeek-R1).
    /// Advanced reasoning model with chain-of-thought capabilities for complex problem-solving.
    /// </summary>
    /// <returns>A DeepSeek Reasoner language model.</returns>
    public ILanguageModel Reasoner()
    {
        return new DeepSeekChatLanguageModel("deepseek-reasoner", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The DeepSeek model ID.</param>
    /// <returns>A DeepSeek chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new DeepSeekChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The DeepSeek model ID (e.g., "deepseek-chat", "deepseek-coder", "deepseek-reasoner").</param>
    /// <param name="apiKey">The DeepSeek API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.deepseek.com).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A DeepSeek chat language model.</returns>
    public static DeepSeekChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new DeepSeekConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.deepseek.com"
        };

        return new DeepSeekChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The DeepSeek model ID.</param>
    /// <param name="config">The DeepSeek configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A DeepSeek chat language model.</returns>
    public static DeepSeekChatLanguageModel CreateChatModel(
        string modelId,
        DeepSeekConfiguration config,
        HttpClient? httpClient = null)
    {
        return new DeepSeekChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek Chat model (general purpose).
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The DeepSeek API key.</param>
    /// <returns>A DeepSeek Chat language model.</returns>
    public static DeepSeekChatLanguageModel Chat(string apiKey)
    {
        return CreateChatModel("deepseek-chat", apiKey);
    }

    /// <summary>
    /// Creates a DeepSeek Coder model (code-focused).
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The DeepSeek API key.</param>
    /// <returns>A DeepSeek Coder language model.</returns>
    public static DeepSeekChatLanguageModel Coder(string apiKey)
    {
        return CreateChatModel("deepseek-coder", apiKey);
    }

    /// <summary>
    /// Creates a DeepSeek Reasoner model (DeepSeek-R1).
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The DeepSeek API key.</param>
    /// <returns>A DeepSeek Reasoner language model.</returns>
    public static DeepSeekChatLanguageModel Reasoner(string apiKey)
    {
        return CreateChatModel("deepseek-reasoner", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The DeepSeek model ID.</param>
    /// <param name="apiKey">The DeepSeek API key.</param>
    /// <returns>A DeepSeek chat language model.</returns>
    public static DeepSeekChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
