using AiSdk.Abstractions;

namespace AiSdk.Providers.Vercel.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Vercel AI Gateway API.
/// </summary>
public class VercelException : ApiCallError
{
    private const string MarkerName = "AI_VercelError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_VercelError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from Vercel AI Gateway if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VercelException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public VercelException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VercelException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public VercelException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VercelException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The Vercel AI Gateway error code.</param>
    public VercelException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="VercelException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a VercelException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
