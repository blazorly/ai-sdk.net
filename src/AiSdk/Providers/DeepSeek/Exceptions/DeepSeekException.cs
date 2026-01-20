using AiSdk.Abstractions;

namespace AiSdk.Providers.DeepSeek.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the DeepSeek API.
/// </summary>
public class DeepSeekException : ApiCallError
{
    private const string MarkerName = "AI_DeepSeekError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_DeepSeekError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from DeepSeek if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeepSeekException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public DeepSeekException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeepSeekException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public DeepSeekException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeepSeekException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The DeepSeek error code.</param>
    public DeepSeekException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="DeepSeekException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a DeepSeekException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
