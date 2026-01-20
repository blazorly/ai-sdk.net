using AiSdk.Abstractions;

namespace AiSdk.Providers.OpenAI.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the OpenAI API.
/// </summary>
public class OpenAIException : ApiCallError
{
    private const string MarkerName = "AI_OpenAIError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_OpenAIError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from OpenAI if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OpenAIException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OpenAIException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The OpenAI error code.</param>
    public OpenAIException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="OpenAIException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an OpenAIException; otherwise, false.</returns>
    public static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
