namespace AiSdk.Providers.Baseten;

/// <summary>
/// Factory for creating Baseten language models.
/// </summary>
public static class BasetenProvider
{
    /// <summary>
    /// Creates a Baseten chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Baseten model ID (e.g., "meta-llama-3-8b-instruct", "mistral-7b-instruct").</param>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.baseten.co/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Baseten chat language model.</returns>
    public static BasetenChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new BasetenConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.baseten.co/v1"
        };

        return new BasetenChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Baseten chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Baseten model ID.</param>
    /// <param name="config">The Baseten configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Baseten chat language model.</returns>
    public static BasetenChatLanguageModel CreateChatModel(
        string modelId,
        BasetenConfiguration config,
        HttpClient? httpClient = null)
    {
        return new BasetenChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3 8B Instruct model.
    /// Fast and efficient model suitable for most conversational tasks.
    /// </summary>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <returns>A Llama 3 8B chat model.</returns>
    public static BasetenChatLanguageModel Llama3_8B(string apiKey)
    {
        return CreateChatModel("meta-llama-3-8b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3 70B Instruct model.
    /// More powerful model with better reasoning and instruction following.
    /// </summary>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <returns>A Llama 3 70B chat model.</returns>
    public static BasetenChatLanguageModel Llama3_70B(string apiKey)
    {
        return CreateChatModel("meta-llama-3-70b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct model.
    /// Efficient model with strong performance on instruction-following tasks.
    /// </summary>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <returns>A Mistral 7B chat model.</returns>
    public static BasetenChatLanguageModel Mistral7B(string apiKey)
    {
        return CreateChatModel("mistral-7b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a WizardLM-2 8x22B model.
    /// Mixture of experts model with excellent reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <returns>A WizardLM-2 8x22B chat model.</returns>
    public static BasetenChatLanguageModel WizardLM2_8x22B(string apiKey)
    {
        return CreateChatModel("wizardlm-2-8x22b", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B model.
    /// Mixture of experts model with balanced performance and efficiency.
    /// </summary>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <returns>A Mixtral 8x7B chat model.</returns>
    public static BasetenChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mixtral-8x7b-instruct", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any Baseten model.
    /// </summary>
    /// <param name="modelId">The Baseten model ID.</param>
    /// <param name="apiKey">The Baseten API key.</param>
    /// <returns>A Baseten chat language model.</returns>
    public static BasetenChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
