namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Internal server error from the Gateway.
/// </summary>
public class GatewayInternalServerError : GatewayError
{
    private const string MarkerName = "AI_GatewayInternalServerError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayInternalServerError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <inheritdoc/>
    public override string ErrorType => "internal_server_error";

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayInternalServerError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="cause">The underlying cause of error.</param>
    public GatewayInternalServerError(
        string message = "Internal server error",
        int? statusCode = null,
        Exception? cause = null)
        : base(message, statusCode ?? 500, cause)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GatewayInternalServerError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GatewayInternalServerError; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => GatewayError.IsInstance(error) && HasMarker(error, MarkerName);
}
