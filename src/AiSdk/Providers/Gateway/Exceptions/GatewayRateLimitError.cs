namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Rate limit exceeded.
/// </summary>
public class GatewayRateLimitError : GatewayError
{
    private const string MarkerName = "AI_GatewayRateLimitError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayRateLimitError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <inheritdoc/>
    public override string ErrorType => "rate_limit_exceeded";

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayRateLimitError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="cause">The underlying cause of error.</param>
    public GatewayRateLimitError(
        string message = "Rate limit exceeded",
        int? statusCode = null,
        Exception? cause = null)
        : base(message, statusCode ?? 429, cause)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GatewayRateLimitError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GatewayRateLimitError; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => GatewayError.IsInstance(error) && HasMarker(error, MarkerName);
}
