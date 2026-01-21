namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Invalid request - missing headers, malformed data, etc.
/// </summary>
public class GatewayInvalidRequestError : GatewayError
{
    private const string MarkerName = "AI_GatewayInvalidRequestError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayInvalidRequestError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <inheritdoc/>
    public override string ErrorType => "invalid_request_error";

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayInvalidRequestError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="cause">The underlying cause of error.</param>
    public GatewayInvalidRequestError(
        string message = "Invalid request",
        int? statusCode = null,
        Exception? cause = null)
        : base(message, statusCode ?? 400, cause)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GatewayInvalidRequestError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GatewayInvalidRequestError; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => GatewayError.IsInstance(error) && HasMarker(error, MarkerName);
}
