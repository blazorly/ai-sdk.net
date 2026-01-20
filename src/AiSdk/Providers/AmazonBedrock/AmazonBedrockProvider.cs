namespace AiSdk.Providers.AmazonBedrock;

/// <summary>
/// Factory for creating Amazon Bedrock language models.
/// Bedrock is a multi-provider aggregator offering access to models from:
/// - Anthropic (Claude)
/// - Amazon (Titan)
/// - Meta (Llama)
/// - Cohere (Command)
/// - Mistral
/// And more.
/// </summary>
public static class AmazonBedrockProvider
{
    /// <summary>
    /// Creates an Amazon Bedrock model with the specified model ID.
    /// </summary>
    /// <param name="modelId">The Bedrock model ID (e.g., "anthropic.claude-3-5-sonnet-20241022-v2:0").</param>
    /// <param name="region">The AWS region (e.g., "us-east-1", "us-west-2").</param>
    /// <param name="accessKeyId">Optional AWS access key ID. If not provided, uses default AWS credentials.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <param name="sessionToken">Optional AWS session token for temporary credentials.</param>
    /// <param name="timeoutSeconds">Optional timeout in seconds.</param>
    /// <returns>An Amazon Bedrock language model.</returns>
    public static AmazonBedrockLanguageModel CreateModel(
        string modelId,
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null,
        string? sessionToken = null,
        int? timeoutSeconds = null)
    {
        var config = new AmazonBedrockConfiguration
        {
            Region = region,
            AccessKeyId = accessKeyId,
            SecretAccessKey = secretAccessKey,
            SessionToken = sessionToken,
            TimeoutSeconds = timeoutSeconds
        };

        return new AmazonBedrockLanguageModel(modelId, config);
    }

    /// <summary>
    /// Creates an Amazon Bedrock model with the specified configuration.
    /// </summary>
    /// <param name="modelId">The Bedrock model ID.</param>
    /// <param name="config">The Amazon Bedrock configuration.</param>
    /// <returns>An Amazon Bedrock language model.</returns>
    public static AmazonBedrockLanguageModel CreateModel(
        string modelId,
        AmazonBedrockConfiguration config)
    {
        return new AmazonBedrockLanguageModel(modelId, config);
    }

    #region Anthropic Claude Models

    /// <summary>
    /// Creates a Claude 3.5 Sonnet v2 model on Bedrock (latest version).
    /// Excellent for complex tasks, coding, and analysis.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Claude 3.5 Sonnet model.</returns>
    public static AmazonBedrockLanguageModel Claude35Sonnet(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("anthropic.claude-3-5-sonnet-20241022-v2:0", region, accessKeyId, secretAccessKey);
    }

    /// <summary>
    /// Creates a Claude 3 Opus model on Bedrock.
    /// Most capable Claude 3 model for highly complex tasks.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Claude 3 Opus model.</returns>
    public static AmazonBedrockLanguageModel Claude3Opus(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("anthropic.claude-3-opus-20240229-v1:0", region, accessKeyId, secretAccessKey);
    }

    /// <summary>
    /// Creates a Claude 3 Sonnet model on Bedrock.
    /// Balanced intelligence and speed.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Claude 3 Sonnet model.</returns>
    public static AmazonBedrockLanguageModel Claude3Sonnet(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("anthropic.claude-3-sonnet-20240229-v1:0", region, accessKeyId, secretAccessKey);
    }

    /// <summary>
    /// Creates a Claude 3 Haiku model on Bedrock.
    /// Fastest and most compact Claude 3 model.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Claude 3 Haiku model.</returns>
    public static AmazonBedrockLanguageModel Claude3Haiku(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("anthropic.claude-3-haiku-20240307-v1:0", region, accessKeyId, secretAccessKey);
    }

    #endregion

    #region Amazon Titan Models

    /// <summary>
    /// Creates an Amazon Titan Text Express v1 model.
    /// Optimized for English text generation tasks with high speed.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Titan Text Express model.</returns>
    public static AmazonBedrockLanguageModel TitanTextExpress(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("amazon.titan-text-express-v1", region, accessKeyId, secretAccessKey);
    }

    /// <summary>
    /// Creates an Amazon Titan Text Lite v1 model.
    /// Lightweight model for simple text generation tasks.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Titan Text Lite model.</returns>
    public static AmazonBedrockLanguageModel TitanTextLite(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("amazon.titan-text-lite-v1", region, accessKeyId, secretAccessKey);
    }

    #endregion

    #region Meta Llama Models

    /// <summary>
    /// Creates a Meta Llama 3.1 70B Instruct model on Bedrock.
    /// High-performance instruction-following model.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Llama 3.1 70B model.</returns>
    public static AmazonBedrockLanguageModel Llama31_70B(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("meta.llama3-1-70b-instruct-v1:0", region, accessKeyId, secretAccessKey);
    }

    /// <summary>
    /// Creates a Meta Llama 3.1 8B Instruct model on Bedrock.
    /// Compact and efficient instruction-following model.
    /// </summary>
    /// <param name="region">The AWS region.</param>
    /// <param name="accessKeyId">Optional AWS access key ID.</param>
    /// <param name="secretAccessKey">Optional AWS secret access key.</param>
    /// <returns>A Llama 3.1 8B model.</returns>
    public static AmazonBedrockLanguageModel Llama31_8B(
        string region,
        string? accessKeyId = null,
        string? secretAccessKey = null)
    {
        return CreateModel("meta.llama3-1-8b-instruct-v1:0", region, accessKeyId, secretAccessKey);
    }

    #endregion
}
