namespace AiSdk.Providers.OpenAICompatible;

/// <summary>
/// Factory for creating OpenAI-compatible language models.
/// Supports Ollama, LocalAI, vLLM, LM Studio, and any other OpenAI-compatible endpoint.
/// </summary>
public static class OpenAICompatibleProvider
{
    /// <summary>
    /// Creates an OpenAI-compatible chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The model ID (e.g., "llama2", "mistral", "gpt-3.5-turbo").</param>
    /// <param name="config">The OpenAI-compatible configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenAI-compatible chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel CreateChatModel(
        string modelId,
        OpenAICompatibleConfiguration config,
        HttpClient? httpClient = null)
    {
        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates an OpenAI-compatible chat model with the specified base URL.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="baseUrl">The base URL for the OpenAI-compatible API.</param>
    /// <param name="apiKey">Optional API key (not required for most local endpoints).</param>
    /// <param name="timeoutSeconds">Optional timeout in seconds.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An OpenAI-compatible chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel CreateChatModel(
        string modelId,
        string baseUrl,
        string? apiKey = null,
        int? timeoutSeconds = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey,
            TimeoutSeconds = timeoutSeconds
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for Ollama (default: http://localhost:11434/v1).
    /// </summary>
    /// <param name="modelId">The Ollama model ID (e.g., "llama2", "mistral", "codellama").</param>
    /// <param name="baseUrl">Optional custom base URL (defaults to http://localhost:11434/v1).</param>
    /// <param name="apiKey">Optional API key (Ollama doesn't require one by default).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An Ollama chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForOllama(
        string modelId,
        string? baseUrl = null,
        string? apiKey = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = baseUrl ?? "http://localhost:11434/v1",
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for LocalAI.
    /// </summary>
    /// <param name="modelId">The LocalAI model ID.</param>
    /// <param name="baseUrl">The LocalAI base URL (e.g., "http://localhost:8080/v1").</param>
    /// <param name="apiKey">Optional API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A LocalAI chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForLocalAI(
        string modelId,
        string baseUrl,
        string? apiKey = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for vLLM.
    /// </summary>
    /// <param name="modelId">The vLLM model ID.</param>
    /// <param name="baseUrl">The vLLM base URL (e.g., "http://localhost:8000/v1").</param>
    /// <param name="apiKey">Optional API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A vLLM chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForVLLM(
        string modelId,
        string baseUrl,
        string? apiKey = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for LM Studio (default: http://localhost:1234/v1).
    /// </summary>
    /// <param name="modelId">The LM Studio model ID.</param>
    /// <param name="baseUrl">Optional custom base URL (defaults to http://localhost:1234/v1).</param>
    /// <param name="apiKey">Optional API key (LM Studio doesn't require one by default).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>An LM Studio chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForLMStudio(
        string modelId,
        string? baseUrl = null,
        string? apiKey = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = baseUrl ?? "http://localhost:1234/v1",
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for Text Generation WebUI (default: http://localhost:5000/v1).
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="baseUrl">Optional custom base URL (defaults to http://localhost:5000/v1).</param>
    /// <param name="apiKey">Optional API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Text Generation WebUI chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForTextGenerationWebUI(
        string modelId,
        string? baseUrl = null,
        string? apiKey = null,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = baseUrl ?? "http://localhost:5000/v1",
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for Groq (cloud service with OpenAI-compatible API).
    /// </summary>
    /// <param name="modelId">The Groq model ID (e.g., "llama2-70b-4096", "mixtral-8x7b-32768").</param>
    /// <param name="apiKey">The Groq API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Groq chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForGroq(
        string modelId,
        string apiKey,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = "https://api.groq.com/openai/v1",
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a chat model for Together AI (cloud service with OpenAI-compatible API).
    /// </summary>
    /// <param name="modelId">The Together AI model ID.</param>
    /// <param name="apiKey">The Together AI API key.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Together AI chat language model.</returns>
    public static OpenAICompatibleChatLanguageModel ForTogetherAI(
        string modelId,
        string apiKey,
        HttpClient? httpClient = null)
    {
        var config = new OpenAICompatibleConfiguration
        {
            BaseUrl = "https://api.together.xyz/v1",
            ApiKey = apiKey
        };

        return new OpenAICompatibleChatLanguageModel(modelId, config, httpClient);
    }
}
