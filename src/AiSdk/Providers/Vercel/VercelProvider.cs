namespace AiSdk.Providers.Vercel;

/// <summary>
/// Factory for creating Vercel AI Gateway language models.
/// Provides access to models from multiple providers through Vercel's unified API gateway.
/// </summary>
public static class VercelProvider
{
    /// <summary>
    /// Creates a Vercel AI Gateway chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The model ID in format "provider/model-name" (e.g., "openai/gpt-4", "anthropic/claude-3.5-sonnet").</param>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://ai-gateway.vercel.sh/v3/ai).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Vercel AI Gateway chat language model.</returns>
    public static VercelChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new VercelConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://ai-gateway.vercel.sh/v3/ai"
        };

        return new VercelChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Vercel AI Gateway chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The model ID in format "provider/model-name".</param>
    /// <param name="config">The Vercel AI Gateway configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Vercel AI Gateway chat language model.</returns>
    public static VercelChatLanguageModel CreateChatModel(
        string modelId,
        VercelConfiguration config,
        HttpClient? httpClient = null)
    {
        return new VercelChatLanguageModel(modelId, config, httpClient);
    }

    // OpenAI Models via Vercel AI Gateway

    /// <summary>
    /// Creates a GPT-4 Turbo model via Vercel AI Gateway.
    /// Most capable GPT-4 model with enhanced performance and longer context.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A GPT-4 Turbo chat model.</returns>
    public static VercelChatLanguageModel OpenAI_GPT4Turbo(string apiKey)
    {
        return CreateChatModel("openai/gpt-4-turbo", apiKey);
    }

    /// <summary>
    /// Creates a GPT-4o model via Vercel AI Gateway.
    /// Flagship multimodal model that's faster and cheaper than GPT-4 Turbo.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A GPT-4o chat model.</returns>
    public static VercelChatLanguageModel OpenAI_GPT4o(string apiKey)
    {
        return CreateChatModel("openai/gpt-4o", apiKey);
    }

    /// <summary>
    /// Creates a GPT-4o Mini model via Vercel AI Gateway.
    /// Affordable small model for fast, lightweight tasks.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A GPT-4o Mini chat model.</returns>
    public static VercelChatLanguageModel OpenAI_GPT4oMini(string apiKey)
    {
        return CreateChatModel("openai/gpt-4o-mini", apiKey);
    }

    /// <summary>
    /// Creates a GPT-3.5 Turbo model via Vercel AI Gateway.
    /// Fast, inexpensive model for simple tasks.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A GPT-3.5 Turbo chat model.</returns>
    public static VercelChatLanguageModel OpenAI_GPT35Turbo(string apiKey)
    {
        return CreateChatModel("openai/gpt-3.5-turbo", apiKey);
    }

    // Anthropic Models via Vercel AI Gateway

    /// <summary>
    /// Creates a Claude Opus 4.5 model via Vercel AI Gateway.
    /// Most powerful Claude model for complex tasks requiring deep analysis.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Claude Opus 4.5 chat model.</returns>
    public static VercelChatLanguageModel Anthropic_ClaudeOpus4_5(string apiKey)
    {
        return CreateChatModel("anthropic/claude-opus-4.5", apiKey);
    }

    /// <summary>
    /// Creates a Claude Sonnet 4.5 model via Vercel AI Gateway.
    /// Balanced Claude model for most tasks with excellent performance.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Claude Sonnet 4.5 chat model.</returns>
    public static VercelChatLanguageModel Anthropic_ClaudeSonnet4_5(string apiKey)
    {
        return CreateChatModel("anthropic/claude-sonnet-4.5", apiKey);
    }

    /// <summary>
    /// Creates a Claude Sonnet 3.5 model via Vercel AI Gateway.
    /// Previous generation balanced Claude model.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Claude Sonnet 3.5 chat model.</returns>
    public static VercelChatLanguageModel Anthropic_ClaudeSonnet3_5(string apiKey)
    {
        return CreateChatModel("anthropic/claude-3.5-sonnet", apiKey);
    }

    /// <summary>
    /// Creates a Claude Haiku 3.5 model via Vercel AI Gateway.
    /// Fastest and most compact Claude model for quick responses.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Claude Haiku 3.5 chat model.</returns>
    public static VercelChatLanguageModel Anthropic_ClaudeHaiku3_5(string apiKey)
    {
        return CreateChatModel("anthropic/claude-3.5-haiku", apiKey);
    }

    // Google Models via Vercel AI Gateway

    /// <summary>
    /// Creates a Gemini 2.0 Flash model via Vercel AI Gateway.
    /// Google's latest fast multimodal model with strong performance.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Gemini 2.0 Flash chat model.</returns>
    public static VercelChatLanguageModel Google_Gemini2Flash(string apiKey)
    {
        return CreateChatModel("google/gemini-2.0-flash", apiKey);
    }

    /// <summary>
    /// Creates a Gemini 1.5 Pro model via Vercel AI Gateway.
    /// Google's most capable model with excellent reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Gemini 1.5 Pro chat model.</returns>
    public static VercelChatLanguageModel Google_Gemini1_5Pro(string apiKey)
    {
        return CreateChatModel("google/gemini-1.5-pro", apiKey);
    }

    /// <summary>
    /// Creates a Gemini 1.5 Flash model via Vercel AI Gateway.
    /// Fast and efficient model for high-frequency tasks.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Gemini 1.5 Flash chat model.</returns>
    public static VercelChatLanguageModel Google_Gemini1_5Flash(string apiKey)
    {
        return CreateChatModel("google/gemini-1.5-flash", apiKey);
    }

    // Meta Models via Vercel AI Gateway

    /// <summary>
    /// Creates a Llama 3.3 70B Instruct model via Vercel AI Gateway.
    /// Latest Llama model with 70B parameters for high-quality responses.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Llama 3.3 70B Instruct chat model.</returns>
    public static VercelChatLanguageModel Meta_Llama3_3_70B(string apiKey)
    {
        return CreateChatModel("meta/llama-3.3-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 405B Instruct model via Vercel AI Gateway.
    /// Largest Llama model with exceptional capabilities for complex tasks.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Llama 3.1 405B Instruct chat model.</returns>
    public static VercelChatLanguageModel Meta_Llama3_1_405B(string apiKey)
    {
        return CreateChatModel("meta/llama-3.1-405b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 70B Instruct model via Vercel AI Gateway.
    /// Balanced Llama model suitable for most tasks.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Llama 3.1 70B Instruct chat model.</returns>
    public static VercelChatLanguageModel Meta_Llama3_1_70B(string apiKey)
    {
        return CreateChatModel("meta/llama-3.1-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 8B Instruct model via Vercel AI Gateway.
    /// Compact and efficient Llama model for lightweight tasks.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Llama 3.1 8B Instruct chat model.</returns>
    public static VercelChatLanguageModel Meta_Llama3_1_8B(string apiKey)
    {
        return CreateChatModel("meta/llama-3.1-8b-instruct", apiKey);
    }

    // Mistral Models via Vercel AI Gateway

    /// <summary>
    /// Creates a Mistral Large model via Vercel AI Gateway.
    /// Mistral's flagship model with strong reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Mistral Large chat model.</returns>
    public static VercelChatLanguageModel Mistral_Large(string apiKey)
    {
        return CreateChatModel("mistral/mistral-large", apiKey);
    }

    /// <summary>
    /// Creates a Mistral Small model via Vercel AI Gateway.
    /// Efficient Mistral model for cost-effective inference.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Mistral Small chat model.</returns>
    public static VercelChatLanguageModel Mistral_Small(string apiKey)
    {
        return CreateChatModel("mistral/mistral-small", apiKey);
    }

    // DeepSeek Models via Vercel AI Gateway

    /// <summary>
    /// Creates a DeepSeek V3 model via Vercel AI Gateway.
    /// Latest DeepSeek model with advanced reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A DeepSeek V3 chat model.</returns>
    public static VercelChatLanguageModel DeepSeek_V3(string apiKey)
    {
        return CreateChatModel("deepseek/deepseek-v3", apiKey);
    }

    /// <summary>
    /// Creates a DeepSeek Chat model via Vercel AI Gateway.
    /// General-purpose DeepSeek conversational model.
    /// </summary>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A DeepSeek Chat model.</returns>
    public static VercelChatLanguageModel DeepSeek_Chat(string apiKey)
    {
        return CreateChatModel("deepseek/deepseek-chat", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any model available through Vercel AI Gateway.
    /// </summary>
    /// <param name="modelId">The model ID in format "provider/model-name".</param>
    /// <param name="apiKey">The Vercel AI Gateway API key.</param>
    /// <returns>A Vercel AI Gateway chat language model.</returns>
    public static VercelChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
