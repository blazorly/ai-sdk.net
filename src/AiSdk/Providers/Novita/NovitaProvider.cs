using AiSdk.Abstractions;

namespace AiSdk.Providers.Novita;

/// <summary>
/// Factory for creating Novita language models.
/// </summary>
public class NovitaProvider
{
    private readonly NovitaConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="NovitaProvider"/> class.
    /// </summary>
    /// <param name="config">The Novita configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public NovitaProvider(NovitaConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Meta Llama 3 8B model.
    /// Fast and efficient model suitable for general-purpose tasks.
    /// </summary>
    /// <returns>A Novita Llama 3 8B language model.</returns>
    public ILanguageModel Llama3_8B()
    {
        return new NovitaChatLanguageModel("meta-llama/llama-3-8b-instruct", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3 70B model.
    /// Powerful large model for complex reasoning and advanced tasks.
    /// </summary>
    /// <returns>A Novita Llama 3 70B language model.</returns>
    public ILanguageModel Llama3_70B()
    {
        return new NovitaChatLanguageModel("meta-llama/llama-3-70b-instruct", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct model.
    /// Efficient instruction-following model optimized for chat and task completion.
    /// </summary>
    /// <returns>A Novita Mistral 7B language model.</returns>
    public ILanguageModel Mistral7B()
    {
        return new NovitaChatLanguageModel("mistralai/mistral-7b-instruct-v0.3", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Qwen 2 72B Instruct model.
    /// Advanced bilingual model with strong instruction-following capabilities.
    /// </summary>
    /// <returns>A Novita Qwen 2 72B language model.</returns>
    public ILanguageModel Qwen2_72B()
    {
        return new NovitaChatLanguageModel("qwen/qwen-2-72b-instruct", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Novita model ID.</param>
    /// <returns>A Novita chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new NovitaChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Novita chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Novita model ID (e.g., "meta-llama/llama-3-8b-instruct").</param>
    /// <param name="apiKey">The Novita API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.novita.ai/v3/openai).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Novita chat language model.</returns>
    public static NovitaChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new NovitaConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.novita.ai/v3/openai"
        };

        return new NovitaChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Novita chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Novita model ID.</param>
    /// <param name="config">The Novita configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Novita chat language model.</returns>
    public static NovitaChatLanguageModel CreateChatModel(
        string modelId,
        NovitaConfiguration config,
        HttpClient? httpClient = null)
    {
        return new NovitaChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3 8B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Novita API key.</param>
    /// <returns>A Novita Llama 3 8B language model.</returns>
    public static NovitaChatLanguageModel Llama3_8B(string apiKey)
    {
        return CreateChatModel("meta-llama/llama-3-8b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3 70B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Novita API key.</param>
    /// <returns>A Novita Llama 3 70B language model.</returns>
    public static NovitaChatLanguageModel Llama3_70B(string apiKey)
    {
        return CreateChatModel("meta-llama/llama-3-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Novita API key.</param>
    /// <returns>A Novita Mistral 7B language model.</returns>
    public static NovitaChatLanguageModel Mistral7B(string apiKey)
    {
        return CreateChatModel("mistralai/mistral-7b-instruct-v0.3", apiKey);
    }

    /// <summary>
    /// Creates a Qwen 2 72B Instruct model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Novita API key.</param>
    /// <returns>A Novita Qwen 2 72B language model.</returns>
    public static NovitaChatLanguageModel Qwen2_72B(string apiKey)
    {
        return CreateChatModel("qwen/qwen-2-72b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Novita model ID.</param>
    /// <param name="apiKey">The Novita API key.</param>
    /// <returns>A Novita chat language model.</returns>
    public static NovitaChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
