namespace AiSdk.Providers.OpenRouter;

/// <summary>
/// Factory for creating OpenRouter language models.
/// Provides access to 100+ models from multiple providers through OpenRouter's unified API.
/// </summary>
public static class OpenRouterProvider
{
    /// <summary>
    /// Creates an OpenRouter chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "openai/gpt-4-turbo", "anthropic/claude-3.5-sonnet").</param>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://openrouter.ai/api/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenRouter chat language model.</returns>
    public static OpenRouterChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenRouterConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://openrouter.ai/api/v1"
        };

        return new OpenRouterChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an OpenRouter chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="config">The OpenRouter configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenRouter chat language model.</returns>
    public static OpenRouterChatLanguageModel CreateChatModel(
        string modelId,
        OpenRouterConfiguration config,
        HttpClient? httpClient = null)
    {
        return new OpenRouterChatLanguageModel(modelId, config, httpClient);
    }

    // OpenAI Models via OpenRouter

    /// <summary>
    /// Creates a GPT-4 Turbo model via OpenRouter.
    /// Most capable GPT-4 model with enhanced performance and longer context.
    /// Model ID: openai/gpt-4-turbo
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A GPT-4 Turbo chat model.</returns>
    public static OpenRouterChatLanguageModel GPT4_Turbo(string apiKey)
    {
        return CreateChatModel("openai/gpt-4-turbo", apiKey);
    }

    /// <summary>
    /// Creates a GPT-4o model via OpenRouter.
    /// Flagship multimodal model that's faster and cheaper than GPT-4 Turbo.
    /// Model ID: openai/gpt-4o
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A GPT-4o chat model.</returns>
    public static OpenRouterChatLanguageModel GPT4o(string apiKey)
    {
        return CreateChatModel("openai/gpt-4o", apiKey);
    }

    // Anthropic Models via OpenRouter

    /// <summary>
    /// Creates a Claude 3.5 Sonnet model via OpenRouter.
    /// Balanced Claude model with excellent performance for most tasks.
    /// Model ID: anthropic/claude-3.5-sonnet
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Claude 3.5 Sonnet chat model.</returns>
    public static OpenRouterChatLanguageModel Claude3_5_Sonnet(string apiKey)
    {
        return CreateChatModel("anthropic/claude-3.5-sonnet", apiKey);
    }

    /// <summary>
    /// Creates a Claude Opus 4.5 model via OpenRouter.
    /// Most powerful Claude model for complex tasks requiring deep analysis.
    /// Model ID: anthropic/claude-opus-4.5
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Claude Opus 4.5 chat model.</returns>
    public static OpenRouterChatLanguageModel ClaudeOpus4_5(string apiKey)
    {
        return CreateChatModel("anthropic/claude-opus-4.5", apiKey);
    }

    /// <summary>
    /// Creates a Claude Haiku 3.5 model via OpenRouter.
    /// Fastest and most compact Claude model for quick responses.
    /// Model ID: anthropic/claude-3.5-haiku
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Claude Haiku 3.5 chat model.</returns>
    public static OpenRouterChatLanguageModel ClaudeHaiku3_5(string apiKey)
    {
        return CreateChatModel("anthropic/claude-3.5-haiku", apiKey);
    }

    // Google Models via OpenRouter

    /// <summary>
    /// Creates a Gemini Pro model via OpenRouter.
    /// Google's powerful model with excellent reasoning capabilities.
    /// Model ID: google/gemini-pro
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Gemini Pro chat model.</returns>
    public static OpenRouterChatLanguageModel GeminiPro(string apiKey)
    {
        return CreateChatModel("google/gemini-pro", apiKey);
    }

    /// <summary>
    /// Creates a Gemini 1.5 Pro model via OpenRouter.
    /// Google's most capable model with excellent reasoning capabilities.
    /// Model ID: google/gemini-1.5-pro
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Gemini 1.5 Pro chat model.</returns>
    public static OpenRouterChatLanguageModel Gemini1_5Pro(string apiKey)
    {
        return CreateChatModel("google/gemini-1.5-pro", apiKey);
    }

    /// <summary>
    /// Creates a Gemini 2.0 Flash model via OpenRouter.
    /// Google's latest fast multimodal model with strong performance.
    /// Model ID: google/gemini-2.0-flash
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Gemini 2.0 Flash chat model.</returns>
    public static OpenRouterChatLanguageModel Gemini2Flash(string apiKey)
    {
        return CreateChatModel("google/gemini-2.0-flash", apiKey);
    }

    // Meta Models via OpenRouter

    /// <summary>
    /// Creates a Llama 3 70B Instruct model via OpenRouter.
    /// Meta's powerful open-source model with 70B parameters.
    /// Model ID: meta-llama/llama-3-70b-instruct
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Llama 3 70B Instruct chat model.</returns>
    public static OpenRouterChatLanguageModel Llama3_70B(string apiKey)
    {
        return CreateChatModel("meta-llama/llama-3-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 405B Instruct model via OpenRouter.
    /// Largest Llama model with exceptional capabilities for complex tasks.
    /// Model ID: meta-llama/llama-3.1-405b-instruct
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Llama 3.1 405B Instruct chat model.</returns>
    public static OpenRouterChatLanguageModel Llama3_1_405B(string apiKey)
    {
        return CreateChatModel("meta-llama/llama-3.1-405b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.3 70B Instruct model via OpenRouter.
    /// Latest Llama model with 70B parameters for high-quality responses.
    /// Model ID: meta-llama/llama-3.3-70b-instruct
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Llama 3.3 70B Instruct chat model.</returns>
    public static OpenRouterChatLanguageModel Llama3_3_70B(string apiKey)
    {
        return CreateChatModel("meta-llama/llama-3.3-70b-instruct", apiKey);
    }

    // Mistral Models via OpenRouter

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct model via OpenRouter.
    /// Mistral's efficient mixture-of-experts model with strong performance.
    /// Model ID: mistralai/mixtral-8x7b-instruct
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Mixtral 8x7B Instruct chat model.</returns>
    public static OpenRouterChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mistralai/mixtral-8x7b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Mistral Large model via OpenRouter.
    /// Mistral's flagship model with strong reasoning capabilities.
    /// Model ID: mistralai/mistral-large
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A Mistral Large chat model.</returns>
    public static OpenRouterChatLanguageModel MistralLarge(string apiKey)
    {
        return CreateChatModel("mistralai/mistral-large", apiKey);
    }

    // DeepSeek Models via OpenRouter

    /// <summary>
    /// Creates a DeepSeek V3 model via OpenRouter.
    /// Latest DeepSeek model with advanced reasoning capabilities.
    /// Model ID: deepseek/deepseek-v3
    /// </summary>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>A DeepSeek V3 chat model.</returns>
    public static OpenRouterChatLanguageModel DeepSeekV3(string apiKey)
    {
        return CreateChatModel("deepseek/deepseek-v3", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any model available through OpenRouter.
    /// Visit https://openrouter.ai/models for a complete list of available models.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "openai/gpt-4-turbo", "anthropic/claude-3.5-sonnet").</param>
    /// <param name="apiKey">The OpenRouter API key.</param>
    /// <returns>An OpenRouter chat language model.</returns>
    public static OpenRouterChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
