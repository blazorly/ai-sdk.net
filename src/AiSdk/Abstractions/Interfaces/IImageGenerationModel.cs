namespace AiSdk.Abstractions;

/// <summary>
/// Represents an image generation model.
/// </summary>
public interface IImageGenerationModel
{
    /// <summary>
    /// Gets the provider identifier (e.g., "openai", "black-forest-labs").
    /// </summary>
    string Provider { get; }

    /// <summary>
    /// Gets the provider-specific model identifier (e.g., "dall-e-3", "flux-pro").
    /// </summary>
    string ModelId { get; }

    /// <summary>
    /// Generates an image from the given prompt.
    /// </summary>
    /// <param name="options">The image generation options including prompt, size, and quality.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated image result.</returns>
    Task<ImageGenerationResult> GenerateImageAsync(
        ImageGenerationOptions options,
        CancellationToken cancellationToken = default);
}
