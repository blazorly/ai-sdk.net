namespace AiSdk.Abstractions;

/// <summary>
/// Represents the reason why the model stopped generating tokens.
/// </summary>
public enum FinishReason
{
    /// <summary>
    /// The model reached a natural stopping point or provided a complete response.
    /// </summary>
    Stop,

    /// <summary>
    /// The model reached the maximum token limit.
    /// </summary>
    Length,

    /// <summary>
    /// Content was filtered due to safety/policy reasons.
    /// </summary>
    ContentFilter,

    /// <summary>
    /// The model called one or more tools/functions.
    /// </summary>
    ToolCalls,

    /// <summary>
    /// An error occurred during generation.
    /// </summary>
    Error,

    /// <summary>
    /// Unknown or provider-specific reason.
    /// </summary>
    Other
}
