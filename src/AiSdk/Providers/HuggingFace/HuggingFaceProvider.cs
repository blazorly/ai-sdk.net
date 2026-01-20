namespace AiSdk.Providers.HuggingFace;

/// <summary>
/// Factory for creating Hugging Face language models.
/// </summary>
public static class HuggingFaceProvider
{
    /// <summary>
    /// Creates a Hugging Face chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Hugging Face model ID (e.g., "meta-llama/Llama-2-70b-chat-hf", "mistralai/Mistral-7B-Instruct-v0.2").</param>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api-inference.huggingface.co/models/).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Hugging Face chat language model.</returns>
    public static HuggingFaceChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new HuggingFaceConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api-inference.huggingface.co/models/"
        };

        return new HuggingFaceChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Hugging Face chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Hugging Face model ID.</param>
    /// <param name="config">The Hugging Face configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Hugging Face chat language model.</returns>
    public static HuggingFaceChatLanguageModel CreateChatModel(
        string modelId,
        HuggingFaceConfiguration config,
        HttpClient? httpClient = null)
    {
        return new HuggingFaceChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Llama 2 70B Chat model.
    /// Meta's large language model optimized for chat applications.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Llama 2 70B Chat model.</returns>
    public static HuggingFaceChatLanguageModel Llama2_70B_Chat(string apiKey)
    {
        return CreateChatModel("meta-llama/Llama-2-70b-chat-hf", apiKey);
    }

    /// <summary>
    /// Creates a Llama 2 13B Chat model.
    /// Meta's medium-sized language model optimized for chat applications.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Llama 2 13B Chat model.</returns>
    public static HuggingFaceChatLanguageModel Llama2_13B_Chat(string apiKey)
    {
        return CreateChatModel("meta-llama/Llama-2-13b-chat-hf", apiKey);
    }

    /// <summary>
    /// Creates a Llama 2 7B Chat model.
    /// Meta's smaller language model optimized for chat applications.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Llama 2 7B Chat model.</returns>
    public static HuggingFaceChatLanguageModel Llama2_7B_Chat(string apiKey)
    {
        return CreateChatModel("meta-llama/Llama-2-7b-chat-hf", apiKey);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct v0.2 model.
    /// Mistral AI's instruction-tuned model with extended context.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Mistral 7B Instruct v0.2 model.</returns>
    public static HuggingFaceChatLanguageModel Mistral7B_Instruct(string apiKey)
    {
        return CreateChatModel("mistralai/Mistral-7B-Instruct-v0.2", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct model.
    /// Mistral AI's mixture of experts model with strong performance.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Mixtral 8x7B Instruct model.</returns>
    public static HuggingFaceChatLanguageModel Mixtral8x7B_Instruct(string apiKey)
    {
        return CreateChatModel("mistralai/Mixtral-8x7B-Instruct-v0.1", apiKey);
    }

    /// <summary>
    /// Creates a Zephyr 7B Beta model.
    /// HuggingFaceH4's fine-tuned Mistral model for chat.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Zephyr 7B Beta model.</returns>
    public static HuggingFaceChatLanguageModel Zephyr7B_Beta(string apiKey)
    {
        return CreateChatModel("HuggingFaceH4/zephyr-7b-beta", apiKey);
    }

    /// <summary>
    /// Creates a CodeLlama 34B Instruct model.
    /// Meta's code-specialized language model.
    /// </summary>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A CodeLlama 34B Instruct model.</returns>
    public static HuggingFaceChatLanguageModel CodeLlama34B_Instruct(string apiKey)
    {
        return CreateChatModel("codellama/CodeLlama-34b-Instruct-hf", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any Hugging Face model.
    /// </summary>
    /// <param name="modelId">The Hugging Face model ID (e.g., "organization/model-name").</param>
    /// <param name="apiKey">The Hugging Face API key.</param>
    /// <returns>A Hugging Face chat language model.</returns>
    public static HuggingFaceChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
