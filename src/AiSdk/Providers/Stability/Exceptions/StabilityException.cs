using AiSdk.Abstractions;

namespace AiSdk.Providers.Stability.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Stability AI API.
/// </summary>
public class StabilityException : ApiCallError
{
    private const string MarkerName = "AI_StabilityError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_StabilityError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error type from Stability AI if available.
    /// </summary>
    public string? ErrorType { get; }

    /// <summary>
    /// Gets the error code from Stability AI if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StabilityException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public StabilityException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StabilityException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public StabilityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StabilityException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorType">The Stability AI error type.</param>
    /// <param name="errorCode">The Stability AI error code.</param>
    public StabilityException(string message, int? statusCode, string? errorType, string? errorCode = null) : base(message)
    {
        StatusCode = statusCode;
        ErrorType = errorType;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="StabilityException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a StabilityException; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
