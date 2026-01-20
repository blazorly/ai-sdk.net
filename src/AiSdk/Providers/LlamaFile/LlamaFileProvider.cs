namespace AiSdk.Providers.LlamaFile;

/// <summary>
/// Factory for creating LlamaFile language models.
/// LlamaFile provides local LLM execution using llamafile single-file executables.
/// </summary>
public static class LlamaFileProvider
{
    /// <summary>
    /// Creates a LlamaFile chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The model ID to use (e.g., "llama-3", "mistral-7b-instruct").</param>
    /// <param name="baseUrl">Optional base URL (defaults to http://localhost:8080/v1).</param>
    /// <param name="apiKey">Optional API key (defaults to empty string for local use).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A LlamaFile chat language model.</returns>
    public static LlamaFileChatLanguageModel CreateChatModel(
        string modelId,
        string? baseUrl = null,
        string? apiKey = null,
        HttpClient? httpClient = null)
    {
        var config = new LlamaFileConfiguration
        {
            ApiKey = apiKey ?? string.Empty,
            BaseUrl = baseUrl ?? "http://localhost:8080/v1"
        };

        return new LlamaFileChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a LlamaFile chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The model ID.</param>
    /// <param name="config">The LlamaFile configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A LlamaFile chat language model.</returns>
    public static LlamaFileChatLanguageModel CreateChatModel(
        string modelId,
        LlamaFileConfiguration config,
        HttpClient? httpClient = null)
    {
        return new LlamaFileChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a generic local model instance.
    /// Use this method when you're running any llamafile model locally.
    /// The model name will be automatically detected by the llamafile server.
    /// </summary>
    /// <param name="baseUrl">Optional base URL (defaults to http://localhost:8080/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A LlamaFile chat language model.</returns>
    public static LlamaFileChatLanguageModel LocalModel(
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        return CreateChatModel("local-model", baseUrl, null, httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID and optional custom endpoint.
    /// Convenience method for creating any LlamaFile model.
    /// </summary>
    /// <param name="modelId">The model ID to use.</param>
    /// <param name="baseUrl">Optional base URL (defaults to http://localhost:8080/v1).</param>
    /// <returns>A LlamaFile chat language model.</returns>
    public static LlamaFileChatLanguageModel ChatModel(
        string modelId,
        string? baseUrl = null)
    {
        return CreateChatModel(modelId, baseUrl);
    }
}
