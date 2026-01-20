namespace AiSdk.Providers.AmazonBedrock;

/// <summary>
/// Configuration for Amazon Bedrock API client.
/// </summary>
public record AmazonBedrockConfiguration
{
    /// <summary>
    /// Gets or sets the AWS region for Bedrock API (e.g., "us-east-1", "us-west-2").
    /// </summary>
    public required string Region { get; init; }

    /// <summary>
    /// Gets or sets the AWS Access Key ID.
    /// If not provided, the AWS SDK will attempt to use credentials from environment variables,
    /// AWS credentials file, or IAM role.
    /// </summary>
    public string? AccessKeyId { get; init; }

    /// <summary>
    /// Gets or sets the AWS Secret Access Key.
    /// Required if AccessKeyId is provided.
    /// </summary>
    public string? SecretAccessKey { get; init; }

    /// <summary>
    /// Gets or sets the AWS Session Token (for temporary credentials).
    /// </summary>
    public string? SessionToken { get; init; }

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default AWS SDK timeout will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }
}
