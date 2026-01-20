namespace AiSdk.Abstractions;

/// <summary>
/// Options for speech generation.
/// </summary>
public record SpeechOptions
{
    /// <summary>
    /// Voice to use for speech synthesis.
    /// </summary>
    public string? Voice { get; init; }

    /// <summary>
    /// Speed of speech (typically 0.25 to 4.0).
    /// </summary>
    public double? Speed { get; init; }

    /// <summary>
    /// Audio format (e.g., "mp3", "opus", "aac", "flac").
    /// </summary>
    public string? Format { get; init; }

    /// <summary>
    /// Provider-specific options.
    /// </summary>
    public IReadOnlyDictionary<string, object>? ProviderOptions { get; init; }
}

/// <summary>
/// Result from speech generation.
/// </summary>
public record SpeechResult
{
    /// <summary>
    /// The generated audio data.
    /// </summary>
    public required byte[] AudioData { get; init; }

    /// <summary>
    /// Audio format/content type (e.g., "audio/mpeg", "audio/opus").
    /// </summary>
    public required string ContentType { get; init; }

    /// <summary>
    /// Provider-specific metadata.
    /// </summary>
    public object? ProviderMetadata { get; init; }
}
