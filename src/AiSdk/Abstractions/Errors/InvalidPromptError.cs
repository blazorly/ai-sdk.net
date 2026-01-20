namespace AiSdk.Abstractions;

/// <summary>
/// Exception thrown when the prompt is invalid or malformed.
/// </summary>
public class InvalidPromptError : AiSdkException
{
    private const string MarkerName = "AI_InvalidPromptError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_InvalidPromptError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the prompt that caused the error.
    /// </summary>
    public string? Prompt { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPromptError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidPromptError(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="InvalidPromptError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an InvalidPromptError; otherwise, false.</returns>
    public static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
