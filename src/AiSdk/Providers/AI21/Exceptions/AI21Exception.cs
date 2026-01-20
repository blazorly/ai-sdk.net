using AiSdk.Abstractions;

namespace AiSdk.Providers.AI21.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the AI21 Labs API.
/// </summary>
public class AI21Exception : ApiCallError
{
    private const string MarkerName = "AI_AI21Error";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_AI21Error = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from the API if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AI21Exception"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AI21Exception(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AI21Exception"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AI21Exception(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AI21Exception"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The API error code.</param>
    public AI21Exception(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="AI21Exception"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an AI21Exception; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
