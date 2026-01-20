using AiSdk.Abstractions;

namespace AiSdk.Providers.AmazonBedrock.Exceptions;

/// <summary>
/// Exception thrown when an error occurs with the Amazon Bedrock API.
/// </summary>
public class AmazonBedrockException : ApiCallError
{
    private const string MarkerName = "AI_AmazonBedrockError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_AmazonBedrockError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the AWS error code if available.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the model ID that was being invoked when the error occurred.
    /// </summary>
    public string? ModelId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonBedrockException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public AmazonBedrockException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonBedrockException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public AmazonBedrockException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AmazonBedrockException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="errorCode">The AWS error code.</param>
    /// <param name="modelId">The model ID that was being invoked.</param>
    public AmazonBedrockException(string message, int? statusCode, string? errorCode, string? modelId = null) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ModelId = modelId;
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="AmazonBedrockException"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is an AmazonBedrockException; otherwise, false.</returns>
    public static new bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
