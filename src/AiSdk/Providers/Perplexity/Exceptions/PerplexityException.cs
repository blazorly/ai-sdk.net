using AiSdk.Abstractions;

namespace AiSdk.Providers.Perplexity.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Perplexity API.
/// </summary>
public class PerplexityException : ApiCallError
{
    private const string MarkerName = "AI_PerplexityError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_PerplexityError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from Perplexity if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public PerplexityException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PerplexityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PerplexityException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The Perplexity error code.</param>
    public PerplexityException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="PerplexityException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a PerplexityException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
