using AiSdk.Abstractions;

namespace AiSdk.Providers.GoogleVertex.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Google Vertex AI API.
/// </summary>
public class GoogleVertexException : ApiCallError
{
    private const string MarkerName = "AI_GoogleVertexError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_GoogleVertexError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error status from Google Vertex AI if available.
    /// </summary>
    public string? ErrorStatus { get; }

    /// <summary>
    /// Gets the model type that was being accessed (Gemini or Claude).
    /// </summary>
    public string? ModelType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleVertexException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public GoogleVertexException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleVertexException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public GoogleVertexException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleVertexException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorStatus">The Google Vertex AI error status.</param>
    /// <param name="modelType">The model type (Gemini or Claude).</param>
    public GoogleVertexException(string message, int? statusCode, string? errorStatus, string? modelType = null) : base(message)
    {
        StatusCode = statusCode;
        ErrorStatus = errorStatus;
        ModelType = modelType;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GoogleVertexException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GoogleVertexException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
