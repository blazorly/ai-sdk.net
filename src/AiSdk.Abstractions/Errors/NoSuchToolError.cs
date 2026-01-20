namespace AiSdk.Abstractions;

/// <summary>
/// Exception thrown when a tool is called that doesn't exist.
/// </summary>
public class NoSuchToolError : AiSdkException
{
    private const string MarkerName = "AI_NoSuchToolError";
#pragma warning disable CS0414 // Field is assigned but never used - used via reflection for type checking
    private readonly bool _marker_AI_NoSuchToolError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <summary>
    /// Gets the tool name that was not found.
    /// </summary>
    public required string ToolName { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoSuchToolError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public NoSuchToolError(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="NoSuchToolError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a NoSuchToolError; otherwise, false.</returns>
    public static bool IsInstance(object? error)
        => HasMarker(error, MarkerName);
}
