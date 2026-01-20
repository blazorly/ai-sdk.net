using AiSdk.Abstractions;

namespace AiSdk.Providers.Replicate.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Replicate API.
/// </summary>
public class ReplicateException : ApiCallError
{
    private const string MarkerName = "AI_ReplicateError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_ReplicateError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from Replicate if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplicateException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ReplicateException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplicateException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ReplicateException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplicateException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The Replicate error code.</param>
    public ReplicateException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="ReplicateException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a ReplicateException; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
