using AiSdk.Abstractions;

namespace AiSdk.Providers.Luma;

/// <summary>
/// Factory for creating Luma AI models.
/// Note: Luma AI specializes in video generation (Dream Machine). This provider implements
/// a placeholder structure for future video generation features and basic language model compatibility.
/// </summary>
public class LumaProvider
{
    private readonly LumaConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="LumaProvider"/> class.
    /// </summary>
    /// <param name="config">The Luma AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public LumaProvider(LumaConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Luma Dream Machine model placeholder.
    /// Note: This is a placeholder for future video generation features.
    /// Dream Machine is Luma's primary text-to-video and image-to-video model.
    /// </summary>
    /// <returns>A Luma language model placeholder.</returns>
    public ILanguageModel DreamMachine()
    {
        return new LumaChatLanguageModel("dream-machine-1.0", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Note: This is a placeholder structure for future expansion.
    /// </summary>
    /// <param name="modelId">The Luma model ID.</param>
    /// <returns>A Luma chat language model placeholder.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new LumaChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Luma chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Luma model ID (e.g., "dream-machine-1.0").</param>
    /// <param name="apiKey">The Luma AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.lumalabs.ai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Luma chat language model placeholder.</returns>
    public static LumaChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new LumaConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.lumalabs.ai/v1"
        };

        return new LumaChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Luma chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Luma model ID.</param>
    /// <param name="config">The Luma configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Luma chat language model placeholder.</returns>
    public static LumaChatLanguageModel CreateChatModel(
        string modelId,
        LumaConfiguration config,
        HttpClient? httpClient = null)
    {
        return new LumaChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Luma Dream Machine model placeholder.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Luma AI API key.</param>
    /// <returns>A Luma Dream Machine model placeholder.</returns>
    public static LumaChatLanguageModel DreamMachine(string apiKey)
    {
        return CreateChatModel("dream-machine-1.0", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Luma model ID.</param>
    /// <param name="apiKey">The Luma AI API key.</param>
    /// <returns>A Luma chat language model placeholder.</returns>
    public static LumaChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
