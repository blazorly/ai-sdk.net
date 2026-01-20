using AiSdk.Abstractions;

namespace AiSdk.Providers.HuggingFace.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Hugging Face Inference API.
/// </summary>
public class HuggingFaceException : ApiCallError
{
    private const string MarkerName = "AI_HuggingFaceError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_HuggingFaceError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error type from Hugging Face if available.
    /// </summary>
    public string? ErrorType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public HuggingFaceException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public HuggingFaceException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorType">The Hugging Face error type.</param>
    public HuggingFaceException(string message, int? statusCode, string? errorType) : base(message)
    {
        StatusCode = statusCode;
        ErrorType = errorType;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="HuggingFaceException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a HuggingFaceException; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
