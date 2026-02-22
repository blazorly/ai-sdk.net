namespace AiSdk.Abstractions;

/// <summary>
/// Represents a speech-to-text transcription model.
/// </summary>
public interface ITranscriptionModel
{
    /// <summary>
    /// Gets the provider identifier (e.g., "openai").
    /// </summary>
    string Provider { get; }

    /// <summary>
    /// Gets the provider-specific model identifier (e.g., "whisper-1").
    /// </summary>
    string ModelId { get; }

    /// <summary>
    /// Transcribes audio data to text.
    /// </summary>
    /// <param name="audioData">The audio data to transcribe.</param>
    /// <param name="options">Optional transcription options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The transcription result.</returns>
    Task<TranscriptionResult> TranscribeAsync(
        byte[] audioData,
        TranscriptionOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for audio transcription.
/// </summary>
public record TranscriptionOptions
{
    /// <summary>
    /// Language of the audio (ISO 639-1 code, e.g., "en", "es", "fr").
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Prompt to guide the transcription (helps with context, spelling, etc.).
    /// </summary>
    public string? Prompt { get; init; }

    /// <summary>
    /// Temperature for sampling (0.0 to 1.0).
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Audio file format (e.g., "mp3", "wav", "webm").
    /// </summary>
    public string? FileFormat { get; init; }

    /// <summary>
    /// Provider-specific options.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ProviderOptions { get; init; }
}

/// <summary>
/// Result from audio transcription.
/// </summary>
public record TranscriptionResult
{
    /// <summary>
    /// The transcribed text.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// The detected or specified language.
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Duration of the audio in seconds.
    /// </summary>
    public double? Duration { get; init; }

    /// <summary>
    /// Provider-specific metadata.
    /// </summary>
    public object? ProviderMetadata { get; init; }
}
