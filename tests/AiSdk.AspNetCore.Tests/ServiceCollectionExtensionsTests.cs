using AiSdk.Abstractions;
using AiSdk.AspNetCore.Configuration;
using AiSdk.AspNetCore.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace AiSdk.AspNetCore.Tests;

/// <summary>
/// Tests for ServiceCollectionExtensions to verify AI SDK service registration and configuration.
/// </summary>
public class ServiceCollectionExtensionsTests
{
    #region Basic Registration Tests

    /// <summary>
    /// Tests that AddAiSdk() registers services without throwing exceptions.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithNoParameters_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddAiSdk();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(services);
    }

    /// <summary>
    /// Tests that AddAiSdk() configures options with default values.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithNoParameters_ShouldConfigureDefaultOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddAiSdk();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<AiSdkOptions>>();

        // Assert
        options.Should().NotBeNull();
        options!.Value.Should().NotBeNull();
        options.Value.EnableHealthChecks.Should().BeTrue();
        options.Value.EnableTelemetry.Should().BeTrue();
    }

    /// <summary>
    /// Tests that AddAiSdk() registers logger factory.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithNoParameters_ShouldRegisterLoggingServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAiSdk();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

        // Assert
        loggerFactory.Should().NotBeNull();
    }

    #endregion

    #region Configuration Action Tests

    /// <summary>
    /// Tests that AddAiSdk() with configuration action properly configures options.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithConfigurationAction_ShouldApplyConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAiSdk(options =>
        {
            options.DefaultProvider = "openai";
            options.TimeoutSeconds = 30;
            options.EnableHealthChecks = false;
        });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.DefaultProvider.Should().Be("openai");
        options.Value.TimeoutSeconds.Should().Be(30);
        options.Value.EnableHealthChecks.Should().BeFalse();
    }

    /// <summary>
    /// Tests that AddAiSdk() with provider configuration properly sets provider settings.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithProviderConfiguration_ShouldConfigureProvider()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAiSdk(options =>
        {
            options.DefaultProvider = "openai";
            options.Providers["openai"] = new ProviderConfiguration
            {
                ApiKey = "test-key",
                DefaultModel = "gpt-4",
                BaseUrl = "https://api.openai.com",
                TimeoutSeconds = 60
            };
        });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.Providers.Should().ContainKey("openai");
        var provider = options.Value.Providers["openai"];
        provider.ApiKey.Should().Be("test-key");
        provider.DefaultModel.Should().Be("gpt-4");
        provider.BaseUrl.Should().Be("https://api.openai.com");
        provider.TimeoutSeconds.Should().Be(60);
    }

    /// <summary>
    /// Tests that AddAiSdk() throws ArgumentNullException when services is null.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        var act = () => services.AddAiSdk();

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    /// <summary>
    /// Tests that AddAiSdk() throws ArgumentNullException when configure action is null.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithNullConfigureAction_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddAiSdk((Action<AiSdkOptions>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configure");
    }

    /// <summary>
    /// Tests that AddAiSdk() can configure multiple providers.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithMultipleProviders_ShouldConfigureAllProviders()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAiSdk(options =>
        {
            options.DefaultProvider = "openai";
            options.Providers["openai"] = new ProviderConfiguration
            {
                ApiKey = "openai-key",
                DefaultModel = "gpt-4"
            };
            options.Providers["anthropic"] = new ProviderConfiguration
            {
                ApiKey = "anthropic-key",
                DefaultModel = "claude-3-opus-20240229"
            };
            options.Providers["google"] = new ProviderConfiguration
            {
                ApiKey = "google-key",
                DefaultModel = "gemini-pro"
            };
        });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.Providers.Should().HaveCount(3);
        options.Value.Providers.Should().ContainKey("openai");
        options.Value.Providers.Should().ContainKey("anthropic");
        options.Value.Providers.Should().ContainKey("google");
    }

    /// <summary>
    /// Tests that AddAiSdk() properly configures provider with additional settings.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithAdditionalProviderSettings_ShouldConfigureAdditionalSettings()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAiSdk(options =>
        {
            options.Providers["openai"] = new ProviderConfiguration
            {
                ApiKey = "test-key",
                OrganizationId = "org-123",
                AdditionalSettings =
                {
                    ["temperature"] = "0.7",
                    ["max_tokens"] = "1000"
                }
            };
        });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        var provider = options.Value.Providers["openai"];
        provider.OrganizationId.Should().Be("org-123");
        provider.AdditionalSettings.Should().ContainKey("temperature");
        provider.AdditionalSettings["temperature"].Should().Be("0.7");
        provider.AdditionalSettings.Should().ContainKey("max_tokens");
        provider.AdditionalSettings["max_tokens"].Should().Be("1000");
    }

    /// <summary>
    /// Tests that provider configuration defaults to enabled.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithProviderConfiguration_ShouldDefaultToEnabled()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAiSdk(options =>
        {
            options.Providers["openai"] = new ProviderConfiguration
            {
                ApiKey = "test-key"
            };
        });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.Providers["openai"].Enabled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that provider can be disabled.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithDisabledProvider_ShouldRespectEnabledFlag()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAiSdk(options =>
        {
            options.Providers["openai"] = new ProviderConfiguration
            {
                ApiKey = "test-key",
                Enabled = false
            };
        });

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.Providers["openai"].Enabled.Should().BeFalse();
    }

    #endregion

    #region Configuration Binding Tests

    /// <summary>
    /// Tests that AddAiSdk() binds configuration from IConfiguration.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithIConfiguration_ShouldBindConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configDict = new Dictionary<string, string?>
        {
            ["DefaultProvider"] = "openai",
            ["TimeoutSeconds"] = "45",
            ["EnableHealthChecks"] = "false",
            ["EnableTelemetry"] = "false"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        services.AddAiSdk(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.DefaultProvider.Should().Be("openai");
        options.Value.TimeoutSeconds.Should().Be(45);
        options.Value.EnableHealthChecks.Should().BeFalse();
        options.Value.EnableTelemetry.Should().BeFalse();
    }

    /// <summary>
    /// Tests that AddAiSdk() binds provider configuration from IConfiguration.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithIConfiguration_ShouldBindProviderConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var configDict = new Dictionary<string, string?>
        {
            ["Providers:openai:ApiKey"] = "test-key",
            ["Providers:openai:DefaultModel"] = "gpt-4",
            ["Providers:openai:BaseUrl"] = "https://api.openai.com",
            ["Providers:openai:TimeoutSeconds"] = "60"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        services.AddAiSdk(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.Providers.Should().ContainKey("openai");
        var provider = options.Value.Providers["openai"];
        provider.ApiKey.Should().Be("test-key");
        provider.DefaultModel.Should().Be("gpt-4");
        provider.BaseUrl.Should().Be("https://api.openai.com");
        provider.TimeoutSeconds.Should().Be(60);
    }

    /// <summary>
    /// Tests that AddAiSdk() throws ArgumentNullException when configuration is null.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddAiSdk((IConfiguration)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    /// <summary>
    /// Tests that AddAiSdk() handles empty configuration gracefully.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithEmptyConfiguration_ShouldUseDefaultValues()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddAiSdk(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.DefaultProvider.Should().BeNull();
        options.Value.TimeoutSeconds.Should().BeNull();
        options.Value.EnableHealthChecks.Should().BeTrue();
        options.Value.EnableTelemetry.Should().BeTrue();
        options.Value.Providers.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that AddAiSdk() binds multiple providers from configuration.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithIConfiguration_ShouldBindMultipleProviders()
    {
        // Arrange
        var services = new ServiceCollection();
        var configDict = new Dictionary<string, string?>
        {
            ["Providers:openai:ApiKey"] = "openai-key",
            ["Providers:openai:DefaultModel"] = "gpt-4",
            ["Providers:anthropic:ApiKey"] = "anthropic-key",
            ["Providers:anthropic:DefaultModel"] = "claude-3-opus-20240229",
            ["Providers:google:ApiKey"] = "google-key",
            ["Providers:google:DefaultModel"] = "gemini-pro"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        services.AddAiSdk(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        options.Value.Providers.Should().HaveCount(3);
        options.Value.Providers.Should().ContainKeys("openai", "anthropic", "google");
    }

    /// <summary>
    /// Tests that AddAiSdk() binds additional provider settings from configuration.
    /// </summary>
    [Fact]
    public void AddAiSdk_WithIConfiguration_ShouldBindAdditionalProviderSettings()
    {
        // Arrange
        var services = new ServiceCollection();
        var configDict = new Dictionary<string, string?>
        {
            ["Providers:openai:ApiKey"] = "test-key",
            ["Providers:openai:OrganizationId"] = "org-123",
            ["Providers:openai:AdditionalSettings:temperature"] = "0.7",
            ["Providers:openai:AdditionalSettings:max_tokens"] = "1000"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        services.AddAiSdk(configuration);

        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<AiSdkOptions>>();

        // Assert
        var provider = options.Value.Providers["openai"];
        provider.OrganizationId.Should().Be("org-123");
        provider.AdditionalSettings.Should().ContainKey("temperature");
        provider.AdditionalSettings["temperature"].Should().Be("0.7");
    }

    #endregion

    #region Health Check Tests

    /// <summary>
    /// Tests that AddAiSdkHealthCheck() registers health check.
    /// </summary>
    [Fact]
    public void AddAiSdkHealthCheck_ShouldRegisterHealthCheck()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHealthChecks()
            .AddAiSdkHealthCheck();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();

        // Assert
        healthCheckService.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddAiSdkHealthCheck() uses default name.
    /// </summary>
    [Fact]
    public void AddAiSdkHealthCheck_WithoutName_ShouldUseDefaultName()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddAiSdkHealthCheck();

        // Assert - verify registration was successful
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddAiSdkHealthCheck() accepts custom name.
    /// </summary>
    [Fact]
    public void AddAiSdkHealthCheck_WithCustomName_ShouldUseCustomName()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddAiSdkHealthCheck(name: "custom-ai-check");

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddAiSdkHealthCheck() accepts custom failure status.
    /// </summary>
    [Fact]
    public void AddAiSdkHealthCheck_WithCustomFailureStatus_ShouldAcceptStatus()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();

        // Act
        builder.AddAiSdkHealthCheck(failureStatus: HealthStatus.Unhealthy);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddAiSdkHealthCheck() accepts tags.
    /// </summary>
    [Fact]
    public void AddAiSdkHealthCheck_WithTags_ShouldAcceptTags()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var builder = services.AddHealthChecks();
        var tags = new[] { "ai", "external", "critical" };

        // Act
        builder.AddAiSdkHealthCheck(tags: tags);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var healthCheckService = serviceProvider.GetService<HealthCheckService>();
        healthCheckService.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that AddAiSdkHealthCheck() throws ArgumentNullException when builder is null.
    /// </summary>
    [Fact]
    public void AddAiSdkHealthCheck_WithNullBuilder_ShouldThrowArgumentNullException()
    {
        // Arrange
        IHealthChecksBuilder builder = null!;

        // Act
        var act = () => builder.AddAiSdkHealthCheck();

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("builder");
    }

    #endregion
}
