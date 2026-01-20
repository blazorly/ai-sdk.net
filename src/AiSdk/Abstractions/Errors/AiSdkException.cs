namespace AiSdk.Abstractions;

/// <summary>
/// Base exception for all AI SDK errors.
/// Uses a marker pattern for reliable instanceof checks across module boundaries.
/// </summary>
public abstract class AiSdkException : Exception
{
    /// <summary>
    /// Gets the error name/code.
    /// </summary>
    public abstract string ErrorName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AiSdkException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    protected AiSdkException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Checks if an error has a specific marker.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <param name="marker">The marker string to look for.</param>
    /// <returns>True if the error has the marker; otherwise, false.</returns>
    public static bool HasMarker(object? error, string marker)
    {
        if (error is null)
        {
            return false;
        }

        var type = error.GetType();
        var field = type.GetField($"_marker_{marker}",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.NonPublic);

        return field is not null;
    }
}
