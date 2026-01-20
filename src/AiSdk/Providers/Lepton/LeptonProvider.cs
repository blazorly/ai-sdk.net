namespace AiSdk.Providers.Lepton;

/// <summary>
/// Factory for creating Lepton AI language models.
/// </summary>
public static class LeptonProvider
{
    /// <summary>
    /// Creates a Lepton AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Lepton AI model ID (e.g., "llama3-8b", "llama3-70b", "mixtral-8x7b").</param>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.lepton.ai/api/v1).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Lepton AI chat language model.</returns>
    public static LeptonChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new LeptonConfiguration
        {
            ApiKey = apiKey,
            BaseUrl = baseUrl ?? "https://api.lepton.ai/api/v1"
        };

        return new LeptonChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Lepton AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Lepton AI model ID.</param>
    /// <param name="config">The Lepton AI configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Lepton AI chat language model.</returns>
    public static LeptonChatLanguageModel CreateChatModel(
        string modelId,
        LeptonConfiguration config,
        HttpClient? httpClient = null)
    {
        return new LeptonChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3 8B model.
    /// Fast and efficient model suitable for most tasks.
    /// </summary>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <returns>A Llama 3 8B chat model.</returns>
    public static LeptonChatLanguageModel Llama3_8B(string apiKey)
    {
        return CreateChatModel("llama3-8b", apiKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3 70B model.
    /// More capable model for complex reasoning and generation tasks.
    /// </summary>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <returns>A Llama 3 70B chat model.</returns>
    public static LeptonChatLanguageModel Llama3_70B(string apiKey)
    {
        return CreateChatModel("llama3-70b", apiKey);
    }

    /// <summary>
    /// Creates a Mixtral 8x7B Instruct model.
    /// Mixture of experts model with strong reasoning capabilities.
    /// </summary>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <returns>A Mixtral 8x7B Instruct chat model.</returns>
    public static LeptonChatLanguageModel Mixtral8x7B(string apiKey)
    {
        return CreateChatModel("mixtral-8x7b", apiKey);
    }

    /// <summary>
    /// Creates a WizardLM-2 7B model.
    /// Instruction-tuned model optimized for following complex instructions.
    /// </summary>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <returns>A WizardLM-2 7B chat model.</returns>
    public static LeptonChatLanguageModel WizardLM2_7B(string apiKey)
    {
        return CreateChatModel("wizardlm-2-7b", apiKey);
    }

    /// <summary>
    /// Creates a Databricks DBRX model.
    /// High-performance open model from Databricks.
    /// </summary>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <returns>A Databricks DBRX chat model.</returns>
    public static LeptonChatLanguageModel DBRX(string apiKey)
    {
        return CreateChatModel("dbrx", apiKey);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience method for creating any Lepton AI model.
    /// </summary>
    /// <param name="modelId">The Lepton AI model ID.</param>
    /// <param name="apiKey">The Lepton AI API key.</param>
    /// <returns>A Lepton AI chat language model.</returns>
    public static LeptonChatLanguageModel ChatModel(string modelId, string apiKey)
    {
        return CreateChatModel(modelId, apiKey);
    }
}
