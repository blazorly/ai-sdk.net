using AiSdk.Abstractions;

namespace AiSdk.Providers.Cloudflare.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Cloudflare Workers AI API.
/// </summary>
public class CloudflareException : ApiCallError
{
    private const string MarkerName = "AI_CloudflareError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_CloudflareError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from Cloudflare if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudflareException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public CloudflareException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudflareException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public CloudflareException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudflareException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The Cloudflare error code.</param>
    public CloudflareException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="CloudflareException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a CloudflareException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
