namespace AiSdk.Abstractions;

/// <summary>
/// Represents a speech synthesis (text-to-speech) model.
/// </summary>
public interface ISpeechModel
{
    /// <summary>
    /// Gets the provider identifier (e.g., "openai", "elevenlabs").
    /// </summary>
    string Provider { get; }

    /// <summary>
    /// Gets the provider-specific model identifier (e.g., "tts-1", "eleven_multilingual_v2").
    /// </summary>
    string ModelId { get; }

    /// <summary>
    /// Generates speech audio from the given text.
    /// </summary>
    /// <param name="text">The text to convert to speech.</param>
    /// <param name="options">Optional speech generation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated speech result containing the audio data.</returns>
    Task<SpeechResult> GenerateSpeechAsync(
        string text,
        SpeechOptions? options = null,
        CancellationToken cancellationToken = default);
}
