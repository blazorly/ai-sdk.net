namespace AiSdk.Providers.Gateway.Exceptions;

/// <summary>
/// Authentication failed - invalid API key or OIDC token.
/// </summary>
public class GatewayAuthenticationError : GatewayError
{
    private const string MarkerName = "AI_GatewayAuthenticationError";
#pragma warning disable CS0414
    private readonly bool _marker_AI_GatewayAuthenticationError = true;
#pragma warning restore CS0414

    /// <inheritdoc/>
    public override string ErrorName => MarkerName;

    /// <inheritdoc/>
    public override string ErrorType => "authentication_error";

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayAuthenticationError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="cause">The underlying cause of error.</param>
    public GatewayAuthenticationError(
        string message = "Authentication failed",
        int? statusCode = null,
        Exception? cause = null)
        : base(message, statusCode ?? 401, cause)
    {
    }

    /// <summary>
    /// Checks if an exception is an instance of <see cref="GatewayAuthenticationError"/>.
    /// </summary>
    /// <param name="error">The error to check.</param>
    /// <returns>True if the error is a GatewayAuthenticationError; otherwise, false.</returns>
    public new static bool IsInstance(object? error)
        => GatewayError.IsInstance(error) && HasMarker(error, MarkerName);

    /// <summary>
    /// Creates a contextual error message when authentication fails.
    /// </summary>
    /// <param name="apiKeyProvided">Whether an API key was provided.</param>
    /// <param name="oidcTokenProvided">Whether an OIDC token was provided.</param>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="cause">The underlying cause of error.</param>
    /// <returns>A contextual authentication error.</returns>
    public static GatewayAuthenticationError CreateContextualError(
        bool apiKeyProvided,
        bool oidcTokenProvided,
        string message = "Authentication failed",
        int? statusCode = null,
        Exception? cause = null)
    {
        string contextualMessage;

        if (apiKeyProvided)
        {
            contextualMessage = "AI Gateway authentication failed: Invalid API key.\n\n" +
                "Create a new API key: https://vercel.com/d?to=%2F%5Bteam%5D%2F%7E%2Fai%2Fapi-keys\n\n" +
                "Provide via 'ApiKey' option or 'AI_GATEWAY_API_KEY' environment variable.";
        }
        else if (oidcTokenProvided)
        {
            contextualMessage = "AI Gateway authentication failed: Invalid OIDC token.\n\n" +
                "Run 'npx vercel link' to link your project, then 'vc env pull' to fetch token.\n\n" +
                "Alternatively, use an API key: https://vercel.com/d?to=%2F%5Bteam%5D%2F%7E%2Fai%2Fapi-keys";
        }
        else
        {
            contextualMessage = "AI Gateway authentication failed: No authentication provided.\n\n" +
                "Option 1 - API key:\n" +
                "Create an API key: https://vercel.com/d?to=%2F%5Bteam%5D%2F%7E%2Fai%2Fapi-keys\n" +
                "Provide via 'ApiKey' option or 'AI_GATEWAY_API_KEY' environment variable.\n\n" +
                "Option 2 - OIDC token:\n" +
                "Run 'npx vercel link' to link your project, then 'vc env pull' to fetch the token.";
        }

        return new GatewayAuthenticationError(contextualMessage, statusCode ?? 401, cause);
    }
}
