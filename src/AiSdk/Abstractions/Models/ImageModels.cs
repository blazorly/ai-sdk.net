namespace AiSdk.Abstractions;

/// <summary>
/// Options for image generation.
/// </summary>
public record ImageGenerationOptions
{
    /// <summary>
    /// The text prompt describing the desired image.
    /// </summary>
    public required string Prompt { get; init; }

    /// <summary>
    /// Negative prompt (what to avoid in the image).
    /// </summary>
    public string? NegativePrompt { get; init; }

    /// <summary>
    /// Image size (format depends on provider, e.g., "1024x1024", "square_hd").
    /// </summary>
    public string? Size { get; init; }

    /// <summary>
    /// Number of images to generate.
    /// </summary>
    public int? N { get; init; }

    /// <summary>
    /// Quality setting (e.g., "standard", "hd").
    /// </summary>
    public string? Quality { get; init; }

    /// <summary>
    /// Style setting (e.g., "vivid", "natural").
    /// </summary>
    public string? Style { get; init; }

    /// <summary>
    /// Provider-specific options.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ProviderOptions { get; init; }
}

/// <summary>
/// Result from image generation.
/// </summary>
public record ImageGenerationResult
{
    /// <summary>
    /// The generated images.
    /// </summary>
    public required IReadOnlyList<GeneratedImage> Images { get; init; }

    /// <summary>
    /// Provider-specific metadata.
    /// </summary>
    public object? ProviderMetadata { get; init; }
}

/// <summary>
/// A generated image.
/// </summary>
public record GeneratedImage
{
    /// <summary>
    /// The image data as a byte array (if available).
    /// </summary>
    public byte[]? Data { get; init; }

    /// <summary>
    /// URL to the generated image (if available).
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Revised prompt used to generate the image (if provided by the model).
    /// </summary>
    public string? RevisedPrompt { get; init; }
}
