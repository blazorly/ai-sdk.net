using AiSdk.Abstractions;

namespace AiSdk.Providers.LlamaFile.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the LlamaFile local server.
/// </summary>
public class LlamaFileException : ApiCallError
{
    private const string MarkerName = "AI_LlamaFileError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_LlamaFileError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from LlamaFile if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LlamaFileException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public LlamaFileException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LlamaFileException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public LlamaFileException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LlamaFileException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The LlamaFile error code.</param>
    public LlamaFileException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="LlamaFileException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a LlamaFileException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
