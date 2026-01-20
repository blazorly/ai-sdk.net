using AiSdk.AspNetCore.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AiSdk.AspNetCore.Tests;

/// <summary>
/// Tests for StreamingMiddleware to verify SSE streaming configuration.
/// </summary>
public class StreamingMiddlewareTests
{
    private readonly ILogger<StreamingMiddleware> _logger;

    public StreamingMiddlewareTests()
    {
        _logger = Substitute.For<ILogger<StreamingMiddleware>>();
    }

    #region Constructor Tests

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when next is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullNext_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new StreamingMiddleware(null!, _logger);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("next");
    }

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        var act = () => new StreamingMiddleware(next, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    /// <summary>
    /// Tests that constructor succeeds with valid parameters.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldSucceed()
    {
        // Arrange
        RequestDelegate next = _ => Task.CompletedTask;

        // Act
        var act = () => new StreamingMiddleware(next, _logger);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region InvokeAsync Tests

    /// <summary>
    /// Tests that InvokeAsync sets SSE headers for streaming paths.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WithStreamingPath_ShouldSetSseHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/stream/chat";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["Content-Type"].ToString().Should().Be("text/event-stream");
        context.Response.Headers["Cache-Control"].ToString().Should().Be("no-cache");
        context.Response.Headers["Connection"].ToString().Should().Be("keep-alive");
        nextCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that InvokeAsync sets SSE headers when Accept header contains text/event-stream.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WithEventStreamAcceptHeader_ShouldSetSseHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/chat";
        context.Request.Headers["Accept"] = "text/event-stream";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["Content-Type"].ToString().Should().Be("text/event-stream");
        context.Response.Headers["Cache-Control"].ToString().Should().Be("no-cache");
        context.Response.Headers["Connection"].ToString().Should().Be("keep-alive");
        nextCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that InvokeAsync does not set SSE headers for non-streaming paths.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WithNonStreamingPath_ShouldNotSetSseHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/chat";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers.Should().NotContainKey("Content-Type");
        context.Response.Headers.Should().NotContainKey("Cache-Control");
        context.Response.Headers.Should().NotContainKey("Connection");
        nextCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that InvokeAsync handles case-insensitive Accept header.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WithMixedCaseAcceptHeader_ShouldSetSseHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/chat";
        context.Request.Headers["Accept"] = "Text/Event-Stream";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Headers["Content-Type"].ToString().Should().Be("text/event-stream");
        nextCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that InvokeAsync throws ArgumentNullException when context is null.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange
        RequestDelegate next = _ => Task.CompletedTask;
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        var act = async () => await middleware.InvokeAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that InvokeAsync calls next middleware.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_Always_ShouldCallNextMiddleware()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that InvokeAsync logs debug message for streaming requests.
    /// </summary>
    [Fact]
    public async Task InvokeAsync_WithStreamingPath_ShouldLogDebugMessage()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/stream/chat";
        var nextCalled = false;
        RequestDelegate next = _ => { nextCalled = true; return Task.CompletedTask; };
        var middleware = new StreamingMiddleware(next, _logger);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
        _logger.Received().Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Configuring response for SSE streaming")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    #endregion

    #region Extension Method Tests

    /// <summary>
    /// Tests that UseAiSdkStreaming extension method throws ArgumentNullException when builder is null.
    /// </summary>
    [Fact]
    public void UseAiSdkStreaming_WithNullBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange
        IApplicationBuilder builder = null!;

        // Act
        var act = () => builder.UseAiSdkStreaming();

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    /// <summary>
    /// Tests that UseAiSdkStreaming extension method returns the builder.
    /// </summary>
    [Fact]
    public void UseAiSdkStreaming_WithValidBuilder_ShouldReturnBuilder()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var serviceProvider = services.BuildServiceProvider();
        var builder = new ApplicationBuilder(serviceProvider);

        // Act
        var result = builder.UseAiSdkStreaming();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(builder);
    }

    #endregion
}
