using AiSdk.Abstractions;

namespace AiSdk.Providers.Cohere.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Cohere API.
/// </summary>
public class CohereException : ApiCallError
{
    private const string MarkerName = "AI_CohereError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_CohereError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public CohereException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public CohereException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CohereException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    public CohereException(string message, int? statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="CohereException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a CohereException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
