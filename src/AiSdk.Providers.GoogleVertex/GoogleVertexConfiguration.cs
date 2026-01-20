namespace AiSdk.Providers.GoogleVertex;

/// <summary>
/// Configuration for Google Vertex AI API client.
/// Supports both Gemini (Google) and Claude (Anthropic) models through Vertex AI.
/// </summary>
public record GoogleVertexConfiguration
{
    /// <summary>
    /// Gets or sets the Google Cloud Project ID.
    /// </summary>
    public required string ProjectId { get; init; }

    /// <summary>
    /// Gets or sets the Google Cloud location/region (e.g., "us-central1", "europe-west1").
    /// </summary>
    public required string Location { get; init; }

    /// <summary>
    /// Gets or sets the Google Cloud OAuth2 access token.
    /// Either AccessToken or ServiceAccountJson must be provided.
    /// </summary>
    public string? AccessToken { get; init; }

    /// <summary>
    /// Gets or sets the service account JSON content for authentication.
    /// Either AccessToken or ServiceAccountJson must be provided.
    /// </summary>
    public string? ServiceAccountJson { get; init; }

    /// <summary>
    /// Gets or sets the base URL for the Vertex AI API.
    /// If not specified, defaults to https://{location}-aiplatform.googleapis.com/v1.
    /// </summary>
    public string? BaseUrl { get; init; }

    /// <summary>
    /// Gets or sets the request timeout in seconds (optional).
    /// If not set, the default HttpClient timeout (100 seconds) will be used.
    /// </summary>
    public int? TimeoutSeconds { get; init; }

    /// <summary>
    /// Validates the configuration and throws if invalid.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ProjectId))
            throw new ArgumentException("ProjectId is required", nameof(ProjectId));

        if (string.IsNullOrWhiteSpace(Location))
            throw new ArgumentException("Location is required", nameof(Location));

        if (string.IsNullOrWhiteSpace(AccessToken) && string.IsNullOrWhiteSpace(ServiceAccountJson))
            throw new ArgumentException("Either AccessToken or ServiceAccountJson must be provided");
    }
}
