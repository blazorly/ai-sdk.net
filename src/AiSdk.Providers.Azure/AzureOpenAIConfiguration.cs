namespace AiSdk.Providers.Azure;

/// <summary>
/// Configuration for Azure OpenAI Service.
/// </summary>
public record AzureOpenAIConfiguration
{
    /// <summary>
    /// Gets or sets the Azure OpenAI endpoint URL.
    /// Example: https://{resource-name}.openai.azure.com
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// Gets or sets the deployment name (model deployment ID in Azure).
    /// </summary>
    public required string DeploymentName { get; init; }

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// Required if AzureAdToken is not provided.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gets or sets the Azure AD token for authentication.
    /// Optional alternative to API key authentication.
    /// </summary>
    public string? AzureAdToken { get; init; }

    /// <summary>
    /// Gets or sets the API version.
    /// </summary>
    public string ApiVersion { get; init; } = "2024-02-15-preview";

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(Endpoint))
        {
            throw new ArgumentException("Endpoint is required.", nameof(Endpoint));
        }

        if (string.IsNullOrWhiteSpace(DeploymentName))
        {
            throw new ArgumentException("DeploymentName is required.", nameof(DeploymentName));
        }

        if (string.IsNullOrWhiteSpace(ApiKey) && string.IsNullOrWhiteSpace(AzureAdToken))
        {
            throw new ArgumentException("Either ApiKey or AzureAdToken must be provided.");
        }
    }
}
