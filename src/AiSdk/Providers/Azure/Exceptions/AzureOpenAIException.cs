using AiSdk.Abstractions;

namespace AiSdk.Providers.Azure.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Azure OpenAI API.
/// </summary>
public class AzureOpenAIException : ApiCallError
{
    private const string MarkerName = "AI_AzureOpenAIError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_AzureOpenAIError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the error code from Azure OpenAI if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AzureOpenAIException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AzureOpenAIException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureOpenAIException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The Azure OpenAI error code.</param>
    public AzureOpenAIException(string message, int? statusCode, string? errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="AzureOpenAIException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an AzureOpenAIException; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
