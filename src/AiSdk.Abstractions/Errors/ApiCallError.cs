namespace AiSdk.Abstractions;

/// <summary>
/// Exception thrown when an API call to a provider fails.
/// </summary>
public class ApiCallError : AiSdkException
{
    private const string MarkerName = "AI_ApiCallError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_ApiCallError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the HTTP status code (if applicable).
    /// </summary>
    public int? StatusCode { get; init; }

    /// <summary>
    /// Gets the response body from the failed API call.
    /// </summary>
    public string? ResponseBody { get; init; }

    /// <summary>
    /// Gets the URL that was called.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiCallError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiCallError(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="ApiCallError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an ApiCallError; otherwise, false.</returns>
    public static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
