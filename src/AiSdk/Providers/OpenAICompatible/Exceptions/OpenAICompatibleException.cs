using AiSdk.Abstractions;

namespace AiSdk.Providers.OpenAICompatible.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with an OpenAI-compatible API.
/// </summary>
public class OpenAICompatibleException : ApiCallError
{
    private const string MarkerName = "AI_OpenAICompatibleError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_OpenAICompatibleError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from the API if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAICompatibleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OpenAICompatibleException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAICompatibleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OpenAICompatibleException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAICompatibleException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The API error code.</param>
    public OpenAICompatibleException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="OpenAICompatibleException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an OpenAICompatibleException; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
