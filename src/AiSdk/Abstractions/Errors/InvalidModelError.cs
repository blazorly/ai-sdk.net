namespace AiSdk.Abstractions;

/// <summary>
/// Exception thrown when an invalid or unsupported model is specified.
/// </summary>
public class InvalidModelError : AiSdkException
{
    private const string MarkerName = "AI_InvalidModelError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_InvalidModelError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the model ID that was invalid.
    /// </summary>
    public string? ModelId { get; init; }

    /// <summary>
    /// Gets the provider ID.
    /// </summary>
    public string? Provider { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidModelError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidModelError(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="InvalidModelError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an InvalidModelError; otherwise, false.</returns>
    public static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
