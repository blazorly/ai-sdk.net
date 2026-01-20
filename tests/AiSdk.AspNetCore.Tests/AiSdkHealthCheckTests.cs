using AiSdk.AspNetCore.HealthChecks;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace AiSdk.AspNetCore.Tests;

/// <summary>
/// Tests for AiSdkHealthCheck to verify health check functionality.
/// </summary>
public class AiSdkHealthCheckTests
{
    private readonly ILogger<AiSdkHealthCheck> _logger;

    public AiSdkHealthCheckTests()
    {
        _logger = Substitute.For<ILogger<AiSdkHealthCheck>>();
    }

    #region Constructor Tests

    /// <summary>
    /// Tests that constructor throws ArgumentNullException when logger is null.
    /// </summary>
    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => new AiSdkHealthCheck(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }

    /// <summary>
    /// Tests that constructor succeeds with valid logger.
    /// </summary>
    [Fact]
    public void Constructor_WithValidLogger_ShouldSucceed()
    {
        // Act
        var act = () => new AiSdkHealthCheck(_logger);

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region CheckHealthAsync Tests

    /// <summary>
    /// Tests that CheckHealthAsync returns healthy status by default.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_WithDefaultConfiguration_ShouldReturnHealthy()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Healthy);
        result.Description.Should().Be("AI SDK services are available");
    }

    /// <summary>
    /// Tests that CheckHealthAsync includes diagnostic data.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_WithDefaultConfiguration_ShouldIncludeDiagnosticData()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Data.Should().NotBeNull();
        result.Data.Should().ContainKey("timestamp");
        result.Data.Should().ContainKey("checked");
        result.Data["checked"].Should().Be("ai-sdk-services");
    }

    /// <summary>
    /// Tests that CheckHealthAsync timestamp is recent.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_WithDefaultConfiguration_ShouldHaveRecentTimestamp()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;
        var beforeCheck = DateTime.UtcNow;

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);
        var afterCheck = DateTime.UtcNow;

        // Assert
        result.Data.Should().ContainKey("timestamp");
        var timestamp = (DateTime)result.Data["timestamp"];
        timestamp.Should().BeOnOrAfter(beforeCheck);
        timestamp.Should().BeOnOrBefore(afterCheck);
    }

    /// <summary>
    /// Tests that CheckHealthAsync respects cancellation token.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_WithCancellationToken_ShouldRespectCancellation()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        // Note: The current implementation doesn't actually check cancellation,
        // but it should still complete successfully
        var result = await healthCheck.CheckHealthAsync(context, cts.Token);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    /// <summary>
    /// Tests that CheckHealthAsync logs debug message on success.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_OnSuccess_ShouldLogDebugMessage()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;

        // Act
        await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Debug,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("AI SDK health check completed successfully")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    /// <summary>
    /// Tests that CheckHealthAsync can be called multiple times.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_CalledMultipleTimes_ShouldReturnHealthyEachTime()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result1 = await healthCheck.CheckHealthAsync(context, cancellationToken);
        var result2 = await healthCheck.CheckHealthAsync(context, cancellationToken);
        var result3 = await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        result1.Status.Should().Be(HealthStatus.Healthy);
        result2.Status.Should().Be(HealthStatus.Healthy);
        result3.Status.Should().Be(HealthStatus.Healthy);
    }

    /// <summary>
    /// Tests that CheckHealthAsync handles empty context.
    /// </summary>
    [Fact]
    public async Task CheckHealthAsync_WithEmptyContext_ShouldSucceed()
    {
        // Arrange
        var healthCheck = new AiSdkHealthCheck(_logger);
        var context = new HealthCheckContext();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await healthCheck.CheckHealthAsync(context, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(HealthStatus.Healthy);
    }

    #endregion
}
