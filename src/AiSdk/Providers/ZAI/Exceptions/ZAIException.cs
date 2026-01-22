namespace AiSdk.Providers.ZAI.Exceptions;

/// <summary>
/// Exception thrown when Z.AI API operations fail.
/// </summary>
public class ZAIException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZAIException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public ZAIException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZAIException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ZAIException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
