using AiSdk.Abstractions;

namespace AiSdk.Providers.Writer;

/// <summary>
/// Factory for creating Writer language models.
/// </summary>
public class WriterProvider
{
    private readonly WriterConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="WriterProvider"/> class.
    /// </summary>
    /// <param name="config">The Writer configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public WriterProvider(WriterConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Palmyra X 004 model (latest flagship).
    /// Most capable Palmyra model with advanced reasoning and long context.
    /// </summary>
    /// <returns>A Writer Palmyra X 004 language model.</returns>
    public ILanguageModel PalmyraX_004()
    {
        return new WriterChatLanguageModel("palmyra-x-004", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Palmyra X 003 model.
    /// Previous generation flagship model with strong performance.
    /// </summary>
    /// <returns>A Writer Palmyra X 003 language model.</returns>
    public ILanguageModel PalmyraX_003()
    {
        return new WriterChatLanguageModel("palmyra-x-003", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Palmyra 2 model.
    /// Efficient model for general-purpose tasks.
    /// </summary>
    /// <returns>A Writer Palmyra 2 language model.</returns>
    public ILanguageModel Palmyra2()
    {
        return new WriterChatLanguageModel("palmyra-2", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Writer model ID.</param>
    /// <returns>A Writer chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new WriterChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Writer chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Writer model ID (e.g., "palmyra-x-004", "palmyra-x-003", "palmyra-2").</param>
    /// <param name="apiKey">The Writer API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.writer.com/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Writer chat language model.</returns>
    public static WriterChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new WriterConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.writer.com/v1"
        };

        return new WriterChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Writer chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Writer model ID.</param>
    /// <param name="config">The Writer configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Writer chat language model.</returns>
    public static WriterChatLanguageModel CreateChatModel(
        string modelId,
        WriterConfiguration config,
        HttpClient? httpClient = null)
    {
        return new WriterChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Palmyra X 004 model (latest flagship).
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Writer API key.</param>
    /// <returns>A Writer Palmyra X 004 language model.</returns>
    public static WriterChatLanguageModel PalmyraX_004(string apiKey)
    {
        return CreateChatModel("palmyra-x-004", apiKey);
    }

    /// <summary>
    /// Creates a Palmyra X 003 model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Writer API key.</param>
    /// <returns>A Writer Palmyra X 003 language model.</returns>
    public static WriterChatLanguageModel PalmyraX_003(string apiKey)
    {
        return CreateChatModel("palmyra-x-003", apiKey);
    }

    /// <summary>
    /// Creates a Palmyra 2 model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Writer API key.</param>
    /// <returns>A Writer Palmyra 2 language model.</returns>
    public static WriterChatLanguageModel Palmyra2(string apiKey)
    {
        return CreateChatModel("palmyra-2", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Writer model ID.</param>
    /// <param name="apiKey">The Writer API key.</param>
    /// <returns>A Writer chat language model.</returns>
    public static WriterChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
