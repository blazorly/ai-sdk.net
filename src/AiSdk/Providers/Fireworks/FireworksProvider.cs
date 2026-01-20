namespace AiSdk.Providers.Fireworks;

/// <summary>
/// Factory for creating Fireworks AI language models.
/// Fireworks AI offers fast inference for open-source LLMs with OpenAI-compatible API.
/// </summary>
public static class FireworksProvider
{
    /// <summary>
    /// Creates a Fireworks chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Fireworks model ID (e.g., "accounts/fireworks/models/llama-v3p1-70b-instruct").</param>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.fireworks.ai/inference/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Fireworks chat language model.</returns>
    public static FireworksChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new FireworksConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.fireworks.ai/inference/v1"
        };

        return new FireworksChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Fireworks chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Fireworks model ID.</param>
    /// <param name="config">The Fireworks configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Fireworks chat language model.</returns>
    public static FireworksChatLanguageModel CreateChatModel(
        string modelId,
        FireworksConfiguration config,
        HttpClient? httpClient = null)
    {
        return new FireworksChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.1 70B Instruct model.
    /// Meta's flagship large language model with excellent reasoning and instruction following.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Llama 3.1 70B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Llama3_1_70B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/llama-v3p1-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 8B Instruct model.
    /// Efficient version of Llama 3.1 optimized for speed with strong capabilities.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Llama 3.1 8B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Llama3_1_8B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/llama-v3p1-8b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.3 70B Instruct model.
    /// Latest version of Meta's Llama 3 with improved performance and capabilities.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Llama 3.3 70B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Llama3_3_70B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/llama-v3p3-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a FireFunction V2 model.
    /// Fireworks' state-of-the-art function calling model based on Llama 3.
    /// Excellent for orchestrating multiple models, APIs, and external data sources.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A FireFunction V2 chat model.</returns>
    public static FireworksChatLanguageModel FireFunction_V2(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/firefunction-v2", apiKey);
    }

    /// <summary>
    /// Creates a Qwen 2.5 72B Instruct model.
    /// Alibaba's powerful multilingual model with strong coding and reasoning abilities.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Qwen 2.5 72B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Qwen2_5_72B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/qwen2p5-72b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Qwen 2.5 Coder 32B Instruct model.
    /// Specialized model for code generation and programming tasks.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Qwen 2.5 Coder 32B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Qwen2_5_Coder_32B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/qwen2p5-coder-32b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct model.
    /// Mistral AI's Sparse Mixture of Experts model with 32K context window.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Mixtral 8x7B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Mixtral_8x7B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/mixtral-8x7b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x22B Instruct model.
    /// Larger version of Mixtral with enhanced capabilities and 64K context window.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Mixtral 8x22B Instruct chat model.</returns>
    public static FireworksChatLanguageModel Mixtral_8x22B(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/mixtral-8x22b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a DeepSeek V3 model.
    /// DeepSeek's latest powerful language model with strong reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A DeepSeek V3 chat model.</returns>
    public static FireworksChatLanguageModel DeepSeek_V3(string apiKey)
    {
        return CreateChatModel("accounts/fireworks/models/deepseek-v3", apiKey);
    }

    /// <summary>
    /// Creates a chat model with a custom model ID.
    /// Use this for account-based models or any model not listed in the convenience methods.
    /// </summary>
    /// <param name="modelId">The Fireworks model ID (e.g., "accounts/YOUR_ACCOUNT/models/YOUR_MODEL").</param>
    /// <param name="apiKey">The Fireworks AI API key.</param>
    /// <returns>A Fireworks chat language model.</returns>
    public static FireworksChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
