using AiSdk.Abstractions;

namespace AiSdk.Providers.Anthropic.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Anthropic API.
/// </summary>
public class AnthropicException : ApiCallError
{
    private const string MarkerName = "AI_AnthropicError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_AnthropicError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error type from Anthropic if available.
    /// </summary>
    public string? ErrorType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AnthropicException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AnthropicException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnthropicException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorType">The Anthropic error type.</param>
    public AnthropicException(string message, int? statusCode, string? errorType) : base(message)
    {
        StatusCode = statusCode;
        ErrorType = errorType;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="AnthropicException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an AnthropicException; otherwise, false.</returns>
    public static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
