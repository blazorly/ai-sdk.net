namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Model not found or not available.
/// </summary>
public class GatewayModelNotFoundError : GatewayError
{
    private const string MarkerName = "AI_GatewayModelNotFoundError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayModelNotFoundError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <inheritdoc/>
    public override string ErrorType => "model_not_found";

    /// <summary>
    /// Gets model ID that was not found, if available.
    /// </summary>
    public string? ModelId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayModelNotFoundError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="modelId">The model ID that was not found.</param>
    /// <param name="cause">The underlying cause of error.</param>
    public GatewayModelNotFoundError(
        string message = "Model not found",
        int? statusCode = null,
        string? modelId = null,
        Exception? cause = null)
        : base(message, statusCode ?? 404, cause)
    {
        ModelId = modelId;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GatewayModelNotFoundError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GatewayModelNotFoundError; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => GatewayError.IsInstance(error) && HasMarker(error, MarkerName);
}
