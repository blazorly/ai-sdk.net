using AiSdk.Abstractions;

namespace AiSdk.Providers.Fal;

/// <summary>
/// Factory for creating Fal AI language models.
/// Fal AI provides fast inference for image generation, video generation, and LLMs.
/// </summary>
public class FalProvider
{
    private readonly FalConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="FalProvider"/> class.
    /// </summary>
    /// <param name="config">The Fal AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public FalProvider(FalConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Claude 3.5 Sonnet model via Fal AI.
    /// High-performance model with excellent reasoning capabilities.
    /// </summary>
    /// <returns>A Fal AI Claude 3.5 Sonnet language model.</returns>
    public ILanguageModel Claude35Sonnet()
    {
        return new FalChatLanguageModel("anthropic/claude-3.5-sonnet", _config, _httpClient);
    }

    /// <summary>
    /// Creates a GPT-4o model via Fal AI.
    /// OpenAI's flagship multimodal model with strong performance.
    /// </summary>
    /// <returns>A Fal AI GPT-4o language model.</returns>
    public ILanguageModel Gpt4o()
    {
        return new FalChatLanguageModel("openai/gpt-4o", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Gemini Flash 1.5 model via Fal AI.
    /// Fast and efficient model from Google.
    /// </summary>
    /// <returns>A Fal AI Gemini Flash 1.5 language model.</returns>
    public ILanguageModel GeminiFlash15()
    {
        return new FalChatLanguageModel("google/gemini-flash-1.5", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.2 3B Instruct model via Fal AI.
    /// Efficient open-source model for general tasks.
    /// </summary>
    /// <returns>A Fal AI Llama 3.2 3B Instruct language model.</returns>
    public ILanguageModel Llama323BInstruct()
    {
        return new FalChatLanguageModel("meta-llama/llama-3.2-3b-instruct", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Fal AI model ID.</param>
    /// <returns>A Fal AI chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new FalChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Fal AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Fal AI model ID (e.g., "anthropic/claude-3.5-sonnet", "openai/gpt-4o").</param>
    /// <param name="apiKey">The Fal AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://fal.run).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Fal AI chat language model.</returns>
    public static FalChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new FalConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://fal.run"
        };

        return new FalChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Fal AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Fal AI model ID.</param>
    /// <param name="config">The Fal AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Fal AI chat language model.</returns>
    public static FalChatLanguageModel CreateChatModel(
        string modelId,
        FalConfiguration config,
        HttpClient? httpClient = null)
    {
        return new FalChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Claude 3.5 Sonnet model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Fal AI API key.</param>
    /// <returns>A Fal AI Claude 3.5 Sonnet language model.</returns>
    public static FalChatLanguageModel Claude35Sonnet(string apiKey)
    {
        return CreateChatModel("anthropic/claude-3.5-sonnet", apiKey);
    }

    /// <summary>
    /// Creates a GPT-4o model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Fal AI API key.</param>
    /// <returns>A Fal AI GPT-4o language model.</returns>
    public static FalChatLanguageModel Gpt4o(string apiKey)
    {
        return CreateChatModel("openai/gpt-4o", apiKey);
    }

    /// <summary>
    /// Creates a Gemini Flash 1.5 model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Fal AI API key.</param>
    /// <returns>A Fal AI Gemini Flash 1.5 language model.</returns>
    public static FalChatLanguageModel GeminiFlash15(string apiKey)
    {
        return CreateChatModel("google/gemini-flash-1.5", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.2 3B Instruct model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Fal AI API key.</param>
    /// <returns>A Fal AI Llama 3.2 3B Instruct language model.</returns>
    public static FalChatLanguageModel Llama323BInstruct(string apiKey)
    {
        return CreateChatModel("meta-llama/llama-3.2-3b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Fal AI model ID.</param>
    /// <param name="apiKey">The Fal AI API key.</param>
    /// <returns>A Fal AI chat language model.</returns>
    public static FalChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
