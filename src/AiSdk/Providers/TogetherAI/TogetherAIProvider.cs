using AiSdk.Abstractions;

namespace AiSdk.Providers.TogetherAI;

/// <summary>
/// Factory for creating Together AI language models.
/// </summary>
public class TogetherAIProvider
{
    private readonly TogetherAIConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TogetherAIProvider"/> class.
    /// </summary>
    /// <param name="config">The Together AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public TogetherAIProvider(TogetherAIConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 8B Instruct Turbo model.
    /// Fast and efficient model for general conversations and tasks.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Llama3_1_8B()
    {
        return new TogetherAIChatLanguageModel("meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 70B Instruct Turbo model.
    /// Powerful model for complex reasoning and advanced tasks.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Llama3_1_70B()
    {
        return new TogetherAIChatLanguageModel("meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 405B Instruct Turbo model.
    /// Largest Llama model with exceptional performance on complex tasks.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Llama3_1_405B()
    {
        return new TogetherAIChatLanguageModel("meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3.3 70B Instruct Turbo model.
    /// Latest generation Llama model with improved capabilities.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Llama3_3_70B()
    {
        return new TogetherAIChatLanguageModel("meta-llama/Llama-3.3-70B-Instruct-Turbo", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Qwen 2.5 7B Instruct Turbo model.
    /// Fast Chinese and English bilingual model for general tasks.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Qwen2_5_7B()
    {
        return new TogetherAIChatLanguageModel("Qwen/Qwen2.5-7B-Instruct-Turbo", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Qwen 2.5 72B Instruct Turbo model.
    /// High-performance Chinese and English bilingual model.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Qwen2_5_72B()
    {
        return new TogetherAIChatLanguageModel("Qwen/Qwen2.5-72B-Instruct-Turbo", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Qwen QwQ 32B Preview model.
    /// Advanced reasoning model with chain-of-thought capabilities.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel QwQ32B()
    {
        return new TogetherAIChatLanguageModel("Qwen/QwQ-32B-Preview", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct v0.1 model.
    /// Efficient mixture-of-experts model for general tasks.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Mixtral8x7B()
    {
        return new TogetherAIChatLanguageModel("mistralai/Mixtral-8x7B-Instruct-v0.1", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Mixtral 8x22B Instruct v0.1 model.
    /// Large mixture-of-experts model with enhanced capabilities.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel Mixtral8x22B()
    {
        return new TogetherAIChatLanguageModel("mistralai/Mixtral-8x22B-Instruct-v0.1", _config, _httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek V3 model.
    /// Advanced model with strong coding and reasoning capabilities.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel DeepSeekV3()
    {
        return new TogetherAIChatLanguageModel("deepseek-ai/DeepSeek-V3", _config, _httpClient);
    }

    /// <summary>
    /// Creates a DeepSeek R1 model.
    /// Reasoning-focused model with chain-of-thought capabilities.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel DeepSeekR1()
    {
        return new TogetherAIChatLanguageModel("deepseek-ai/DeepSeek-R1", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Nous Hermes 2 Mixtral 8x7B DPO model.
    /// Fine-tuned model optimized for instruction following.
    /// </summary>
    /// <returns>A Together AI language model.</returns>
    public ILanguageModel NousHermes2Mixtral()
    {
        return new TogetherAIChatLanguageModel("NousResearch/Nous-Hermes-2-Mixtral-8x7B-DPO", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Together AI model ID.</param>
    /// <returns>A Together AI chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new TogetherAIChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Together AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Together AI model ID (e.g., "meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo").</param>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.together.xyz/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Together AI chat language model.</returns>
    public static TogetherAIChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new TogetherAIConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.together.xyz/v1"
        };

        return new TogetherAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Together AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Together AI model ID.</param>
    /// <param name="config">The Together AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Together AI chat language model.</returns>
    public static TogetherAIChatLanguageModel CreateChatModel(
        string modelId,
        TogetherAIConfiguration config,
        HttpClient? httpClient = null)
    {
        return new TogetherAIChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 8B Instruct Turbo model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <returns>A Together AI language model.</returns>
    public static TogetherAIChatLanguageModel Llama3_1_8B(string apiKey)
    {
        return CreateChatModel("meta-llama/Meta-Llama-3.1-8B-Instruct-Turbo", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 70B Instruct Turbo model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <returns>A Together AI language model.</returns>
    public static TogetherAIChatLanguageModel Llama3_1_70B(string apiKey)
    {
        return CreateChatModel("meta-llama/Meta-Llama-3.1-70B-Instruct-Turbo", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 405B Instruct Turbo model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <returns>A Together AI language model.</returns>
    public static TogetherAIChatLanguageModel Llama3_1_405B(string apiKey)
    {
        return CreateChatModel("meta-llama/Meta-Llama-3.1-405B-Instruct-Turbo", apiKey);
    }

    /// <summary>
    /// Creates a Qwen 2.5 72B Instruct Turbo model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <returns>A Together AI language model.</returns>
    public static TogetherAIChatLanguageModel Qwen2_5_72B(string apiKey)
    {
        return CreateChatModel("Qwen/Qwen2.5-72B-Instruct-Turbo", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct v0.1 model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <returns>A Together AI language model.</returns>
    public static TogetherAIChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mistralai/Mixtral-8x7B-Instruct-v0.1", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Together AI model ID.</param>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <returns>A Together AI chat language model.</returns>
    public static TogetherAIChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
