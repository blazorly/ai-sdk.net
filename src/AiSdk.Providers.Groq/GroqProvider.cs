namespace AiSdk.Providers.Groq;

/// <summary>
/// Factory for creating Groq language models.
/// </summary>
public static class GroqProvider
{
    /// <summary>
    /// Creates a Groq chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Groq model ID (e.g., "llama-3.1-70b-versatile", "llama-3.1-8b-instant").</param>
    /// <param name="apiKey">The Groq API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.groq.com/openai/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Groq chat language model.</returns>
    public static GroqChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new GroqConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.groq.com/openai/v1"
        };

        return new GroqChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Groq chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Groq model ID.</param>
    /// <param name="config">The Groq configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Groq chat language model.</returns>
    public static GroqChatLanguageModel CreateChatModel(
        string modelId,
        GroqConfiguration config,
        HttpClient? httpClient = null)
    {
        return new GroqChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Llama 3.1 70B Versatile model.
    /// Known for balanced performance across diverse tasks with high quality output.
    /// </summary>
    /// <param name="apiKey">The Groq API key.</param>
    /// <returns>A Llama 3.1 70B Versatile chat model.</returns>
    public static GroqChatLanguageModel Llama3_1_70B(string apiKey)
    {
        return CreateChatModel("llama-3.1-70b-versatile", apiKey);
    }

    /// <summary>
    /// Creates a Llama 3.1 8B Instant model.
    /// Optimized for speed with lower latency, suitable for real-time applications.
    /// </summary>
    /// <param name="apiKey">The Groq API key.</param>
    /// <returns>A Llama 3.1 8B Instant chat model.</returns>
    public static GroqChatLanguageModel Llama3_1_8B(string apiKey)
    {
        return CreateChatModel("llama-3.1-8b-instant", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B model.
    /// Mixture of experts model with 32K context, excellent for complex reasoning.
    /// </summary>
    /// <param name="apiKey">The Groq API key.</param>
    /// <returns>A Mixtral 8x7B chat model.</returns>
    public static GroqChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mixtral-8x7b-32768", apiKey);
    }

    /// <summary>
    /// Creates a Gemma 7B IT model.
    /// Google's open-source instruction-tuned model.
    /// </summary>
    /// <param name="apiKey">The Groq API key.</param>
    /// <returns>A Gemma 7B IT chat model.</returns>
    public static GroqChatLanguageModel Gemma7B(string apiKey)
    {
        return CreateChatModel("gemma-7b-it", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any Groq model.
    /// </summary>
    /// <param name="modelId">The Groq model ID.</param>
    /// <param name="apiKey">The Groq API key.</param>
    /// <returns>A Groq chat language model.</returns>
    public static GroqChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
