using AiSdk.Abstractions;

namespace AiSdk.Providers.Cloudflare;

/// <summary>
/// Factory for creating Cloudflare Workers AI language models.
/// </summary>
public class CloudflareProvider
{
    private readonly CloudflareConfiguration _config;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudflareProvider"/> class.
    /// </summary>
    /// <param name="config">The Cloudflare configuration.</param>
    /// <param name="httpClient">Optional HTTP client to use.</param>
    public CloudflareProvider(CloudflareConfiguration config, HttpClient? httpClient = null)
    {
        ArgumentNullException.ThrowIfNull(config);
        _config = config;
        _httpClient = httpClient;
    }

    /// <summary>
    /// Creates a Meta Llama 3 8B model.
    /// Fast and efficient model for general-purpose conversations and text generation.
    /// </summary>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public ILanguageModel Llama3_8B()
    {
        return new CloudflareChatLanguageModel("@cf/meta/llama-3-8b-instruct", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3 70B model.
    /// Large, powerful model for complex reasoning and high-quality text generation.
    /// </summary>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public ILanguageModel Llama3_70B()
    {
        return new CloudflareChatLanguageModel("@cf/meta/llama-3-70b-instruct", _config, _httpClient);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct model.
    /// Efficient model optimized for instruction-following tasks.
    /// </summary>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public ILanguageModel Mistral7B()
    {
        return new CloudflareChatLanguageModel("@cf/mistral/mistral-7b-instruct-v0.1", _config, _httpClient);
    }

    /// <summary>
    /// Creates an Intel Neural Chat 7B model.
    /// Specialized model for natural conversations and chat interactions.
    /// </summary>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public ILanguageModel NeuralChat7B()
    {
        return new CloudflareChatLanguageModel("@cf/intel/neural-chat-7b", _config, _httpClient);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Cloudflare Workers AI model ID (e.g., "@cf/meta/llama-3-8b-instruct").</param>
    /// <returns>A Cloudflare Workers AI chat language model.</returns>
    public ILanguageModel ChatModel(string modelId)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        return new CloudflareChatLanguageModel(modelId, _config, _httpClient);
    }

    /// <summary>
    /// Creates a Cloudflare Workers AI chat model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Cloudflare Workers AI model ID.</param>
    /// <param name="apiKey">The Cloudflare API key.</param>
    /// <param name="accountId">The Cloudflare account ID.</param>
    /// <param name="baseUrl">Optional base URL (defaults to https://api.cloudflare.com/client/v4).</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cloudflare Workers AI chat language model.</returns>
    public static CloudflareChatLanguageModel CreateChatModel(
        string modelId,
        string apiKey,
        string accountId,
        string? baseUrl = null,
        HttpClient? httpClient = null)
    {
        var config = new CloudflareConfiguration
        {
            ApiKey = apiKey,
            AccountId = accountId,
            BaseUrl = baseUrl ?? "https://api.cloudflare.com/client/v4"
        };

        return new CloudflareChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Cloudflare Workers AI chat model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Cloudflare Workers AI model ID.</param>
    /// <param name="config">The Cloudflare configuration.</param>
    /// <param name="httpClient">Optional HTTP client.</param>
    /// <returns>A Cloudflare Workers AI chat language model.</returns>
    public static CloudflareChatLanguageModel CreateChatModel(
        string modelId,
        CloudflareConfiguration config,
        HttpClient? httpClient = null)
    {
        return new CloudflareChatLanguageModel(modelId, config, httpClient);
    }

    /// <summary>
    /// Creates a Meta Llama 3 8B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cloudflare API key.</param>
    /// <param name="accountId">The Cloudflare account ID.</param>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public static CloudflareChatLanguageModel Llama3_8B(string apiKey, string accountId)
    {
        return CreateChatModel("@cf/meta/llama-3-8b-instruct", apiKey, accountId);
    }

    /// <summary>
    /// Creates a Meta Llama 3 70B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cloudflare API key.</param>
    /// <param name="accountId">The Cloudflare account ID.</param>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public static CloudflareChatLanguageModel Llama3_70B(string apiKey, string accountId)
    {
        return CreateChatModel("@cf/meta/llama-3-70b-instruct", apiKey, accountId);
    }

    /// <summary>
    /// Creates a Mistral 7B Instruct model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cloudflare API key.</param>
    /// <param name="accountId">The Cloudflare account ID.</param>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public static CloudflareChatLanguageModel Mistral7B(string apiKey, string accountId)
    {
        return CreateChatModel("@cf/mistral/mistral-7b-instruct-v0.1", apiKey, accountId);
    }

    /// <summary>
    /// Creates an Intel Neural Chat 7B model.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="apiKey">The Cloudflare API key.</param>
    /// <param name="accountId">The Cloudflare account ID.</param>
    /// <returns>A Cloudflare Workers AI language model.</returns>
    public static CloudflareChatLanguageModel NeuralChat7B(string apiKey, string accountId)
    {
        return CreateChatModel("@cf/intel/neural-chat-7b", apiKey, accountId);
    }

    /// <summary>
    /// Creates a chat model with the specified model ID.
    /// Convenience static method for quick initialization.
    /// </summary>
    /// <param name="modelId">The Cloudflare Workers AI model ID.</param>
    /// <param name="apiKey">The Cloudflare API key.</param>
    /// <param name="accountId">The Cloudflare account ID.</param>
    /// <returns>A Cloudflare Workers AI chat language model.</returns>
    public static CloudflareChatLanguageModel ChatModel(string modelId, string apiKey, string accountId)
    {
        return CreateChatModel(modelId, apiKey, accountId);
    }
}
