using AiSdk.Abstractions;

namespace AiSdk.Providers.Google.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Google Gemini API.
/// </summary>
public class GoogleException : ApiCallError
{
    private const string MarkerName = "AI_GoogleError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_GoogleError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error status from Google if available.
    /// </summary>
    public string? ErrorStatus { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public GoogleException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public GoogleException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorStatus">The Google error status.</param>
    public GoogleException(string message, int? statusCode, string? errorStatus) : base(message)
    {
        StatusCode = statusCode;
        ErrorStatus = errorStatus;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GoogleException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GoogleException; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
