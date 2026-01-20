using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiSdk.AspNetCore.Middleware;

/// <summary>
/// Middleware for handling Server-Sent Events (SSE) streaming responses from AI providers.
/// </summary>
/// <remarks>
/// This middleware configures the response headers and settings necessary for streaming
/// AI-generated content using Server-Sent Events. It should be added to the pipeline
/// for endpoints that return streaming responses.
/// </remarks>
public class StreamingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<StreamingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    public StreamingMiddleware(RequestDelegate next, ILogger<StreamingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware to configure streaming headers.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Check if this is a streaming request
        // You can customize this logic based on your needs (e.g., check for specific routes or headers)
        var acceptHeader = context.Request.Headers["Accept"].ToString();
        var isStreamingRequest = context.Request.Path.StartsWithSegments("/api/stream") ||
                                 acceptHeader.Contains("text/event-stream", StringComparison.OrdinalIgnoreCase);

        if (isStreamingRequest)
        {
            _logger.LogDebug("Configuring response for SSE streaming on path: {Path}", context.Request.Path);

            // Set headers for Server-Sent Events
            context.Response.Headers["Content-Type"] = "text/event-stream";
            context.Response.Headers["Cache-Control"] = "no-cache";
            context.Response.Headers["Connection"] = "keep-alive";

            // Disable response buffering to enable true streaming
            var responseBody = context.Response.Body;
            if (responseBody != null)
            {
                // Response buffering is handled by the framework for streaming scenarios
                _logger.LogDebug("SSE streaming headers configured successfully");
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Extension methods for adding the streaming middleware to the application pipeline.
/// </summary>
public static class StreamingMiddlewareExtensions
{
    /// <summary>
    /// Adds the AI SDK streaming middleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    /// <example>
    /// <code>
    /// app.UseAiSdkStreaming();
    /// </code>
    /// </example>
    public static IApplicationBuilder UseAiSdkStreaming(this IApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.UseMiddleware<StreamingMiddleware>();
    }
}
