using AiSdk.Abstractions;

namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Base exception for all Gateway errors.
/// </summary>
public abstract class GatewayError : ApiCallError
{
    private const string MarkerName = "AI_GatewayError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error type identifier.
    /// </summary>
    public abstract string ErrorType { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="GatewayError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="cause">The underlying cause of error.</param>
    protected GatewayError(string message, Exception? cause = null)
        : base(message, cause)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="GatewayError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="cause">The underlying cause of error.</param>
    protected GatewayError(string message, int? statusCode, Exception? cause = null)
        : base(message, cause)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Checks if the given error is a Gateway Error.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a Gateway Error; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
