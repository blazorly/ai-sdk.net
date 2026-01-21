using AiSdk.Abstractions;

namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Gateway response parsing error.
/// </summary>
public class GatewayResponseError : GatewayError
{
    private const string MarkerName = "AI_GatewayResponseError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayResponseError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <inheritdoc/>
    public override string ErrorType => "response_error";

    /// <summary>
    /// Gets the response data if available.
    /// </summary>
    public object? Response { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayResponseError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="response">The response data.</param>
    /// <param name="cause">The underlying cause of error.</param>
    public GatewayResponseError(
        string message = "Invalid response from Gateway",
        int? statusCode = null,
        object? response = null,
        Exception? cause = null)
        : base(message, statusCode ?? 502, cause)
    {
        Response = response;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GatewayResponseError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GatewayResponseError; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => GatewayError.IsInstance(error) && HasMarker(error, MarkerName);
}
