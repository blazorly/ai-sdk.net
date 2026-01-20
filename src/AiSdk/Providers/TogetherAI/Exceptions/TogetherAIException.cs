using AiSdk.Abstractions;

namespace AiSdk.Providers.TogetherAI.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Together AI API.
/// </summary>
public class TogetherAIException : ApiCallError
{
    private const string MarkerName = "AI_TogetherAIError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_TogetherAIError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from Together AI if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TogetherAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public TogetherAIException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TogetherAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public TogetherAIException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TogetherAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The Together AI error code.</param>
    public TogetherAIException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="TogetherAIException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a TogetherAIException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
