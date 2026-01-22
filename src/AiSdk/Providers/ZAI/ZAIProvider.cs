using AiSdk.Abstractions;

namespace AiSdk.Providers.ZAI;

/// <summary>
/// Factory for creating Z.AI language models.
/// </summary>
public class ZAIProvider
{
    private readonly ZAIConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ZAIProvider"/> class.
    /// </summary>
    /// <param name="config">The Z.AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public ZAIProvider(ZAIConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a GLM-4.7 model (latest general-purpose model).
    /// Versatile model for general conversations, writing, and analysis.
    /// </summary>
    /// <returns>A Z.AI GLM-4.7 language model.</returns>
    public ILanguageModel GLM47()
    {
        return new ZAIChatLanguageModel("glm-4.7", _config, _httpClient);
    }

    /// <summary>
    /// Creates a GLM-4.6 model.
    /// Previous generation model for general conversations and tasks.
    /// </summary>
    /// <returns>A Z.AI GLM-4.6 language model.</returns>
    public ILanguageModel GLM46()
    {
        return new ZAIChatLanguageModel("glm-4.6", _config, _httpClient);
    }

    /// <summary>
    /// Creates a GLM-4-32B model with 128K context window.
    /// Extended context model for long document processing and analysis.
    /// </summary>
    /// <returns>A Z.AI GLM-4-32B language model.</returns>
    public ILanguageModel GLM432B128K()
    {
        return new ZAIChatLanguageModel("glm-4-32b-0414-128k", _config, _httpClient);
    }

    /// <summary>
    /// Creates a CodeGeeX-4 model (code-focused).
    /// Specialized model optimized for code generation, debugging, and technical tasks.
    /// </summary>
    /// <returns>A Z.AI CodeGeeX-4 language model.</returns>
    public ILanguageModel CodeGeeX4()
    {
        return new ZAIChatLanguageModel("codegeex-4", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Z.AI model ID.</param>
    /// <returns>A Z.AI chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new ZAIChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Z.AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Z.AI model ID (e.g., "glm-4.7", "codegeex-4", "glm-4-32b-0414-128k").</param>
    /// <param name="apiKey">The Z.AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.z.ai/api/paas/v4/).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Z.AI chat language model.</returns>
    public static ZAIChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new ZAIConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.z.ai/api/paas/v4/"
        };

        return new ZAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Z.AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Z.AI model ID.</param>
    /// <param name="config">The Z.AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Z.AI chat language model.</returns>
    public static ZAIChatLanguageModel CreateChatModel(
        string modelId,
        ZAIConfiguration config,
        HttpClient? httpClient = null)
    {
        return new ZAIChatLanguageModel(modelId, config, httpClient);
    }
}
