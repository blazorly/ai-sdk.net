using AiSdk.AspNetCore.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AiSdk.AspNetCore.Tests;

/// <summary>
/// Tests for AiSdkOptions and ProviderConfiguration to verify configuration binding and validation.
/// </summary>
public class ConfigurationTests
{
    #region AiSdkOptions Tests

    /// <summary>
    /// Tests that AiSdkOptions has correct default values.
    /// </summary>
    [Fact]
    public void AiSdkOptions_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var options = new AiSdkOptions();

        // Assert
        options.DefaultProvider.Should().BeNull();
        options.Providers.Should().NotBeNull().And.BeEmpty();
        options.TimeoutSeconds.Should().BeNull();
        options.EnableHealthChecks.Should().BeTrue();
        options.EnableTelemetry.Should().BeTrue();
    }

    /// <summary>
    /// Tests that AiSdkOptions can be instantiated with custom values.
    /// </summary>
    [Fact]
    public void AiSdkOptions_WithCustomValues_ShouldSetProperties()
    {
        // Act
        var options = new AiSdkOptions
        {
            DefaultProvider = "openai",
            TimeoutSeconds = 30,
            EnableHealthChecks = false,
            EnableTelemetry = false
        };

        // Assert
        options.DefaultProvider.Should().Be("openai");
        options.TimeoutSeconds.Should().Be(30);
        options.EnableHealthChecks.Should().BeFalse();
        options.EnableTelemetry.Should().BeFalse();
    }

    /// <summary>
    /// Tests that AiSdkOptions Providers dictionary can be modified.
    /// </summary>
    [Fact]
    public void AiSdkOptions_Providers_ShouldBeModifiable()
    {
        // Arrange
        var options = new AiSdkOptions();
        var providerConfig = new ProviderConfiguration { ApiKey = "test-key" };

        // Act
        options.Providers.Add("openai", providerConfig);

        // Assert
        options.Providers.Should().ContainKey("openai");
        options.Providers["openai"].Should().BeSameAs(providerConfig);
    }

    /// <summary>
    /// Tests that AiSdkOptions can bind from configuration.
    /// </summary>
    [Fact]
    public void AiSdkOptions_FromConfiguration_ShouldBindCorrectly()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["DefaultProvider"] = "anthropic",
            ["TimeoutSeconds"] = "60",
            ["EnableHealthChecks"] = "false",
            ["EnableTelemetry"] = "true"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var options = new AiSdkOptions();
        configuration.Bind(options);

        // Assert
        options.DefaultProvider.Should().Be("anthropic");
        options.TimeoutSeconds.Should().Be(60);
        options.EnableHealthChecks.Should().BeFalse();
        options.EnableTelemetry.Should().BeTrue();
    }

    /// <summary>
    /// Tests that AiSdkOptions binds providers from configuration.
    /// </summary>
    [Fact]
    public void AiSdkOptions_FromConfiguration_ShouldBindProviders()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["Providers:openai:ApiKey"] = "sk-test-123",
            ["Providers:openai:DefaultModel"] = "gpt-4",
            ["Providers:anthropic:ApiKey"] = "sk-ant-test-456",
            ["Providers:anthropic:DefaultModel"] = "claude-3-opus-20240229"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var options = new AiSdkOptions();
        configuration.Bind(options);

        // Assert
        options.Providers.Should().HaveCount(2);
        options.Providers["openai"].ApiKey.Should().Be("sk-test-123");
        options.Providers["openai"].DefaultModel.Should().Be("gpt-4");
        options.Providers["anthropic"].ApiKey.Should().Be("sk-ant-test-456");
        options.Providers["anthropic"].DefaultModel.Should().Be("claude-3-opus-20240229");
    }

    /// <summary>
    /// Tests that AiSdkOptions handles null timeout gracefully.
    /// </summary>
    [Fact]
    public void AiSdkOptions_WithNullTimeout_ShouldBeValid()
    {
        // Act
        var options = new AiSdkOptions
        {
            TimeoutSeconds = null
        };

        // Assert
        options.TimeoutSeconds.Should().BeNull();
    }

    #endregion

    #region ProviderConfiguration Tests

    /// <summary>
    /// Tests that ProviderConfiguration has correct default values.
    /// </summary>
    [Fact]
    public void ProviderConfiguration_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var config = new ProviderConfiguration();

        // Assert
        config.ApiKey.Should().BeNull();
        config.BaseUrl.Should().BeNull();
        config.DefaultModel.Should().BeNull();
        config.OrganizationId.Should().BeNull();
        config.TimeoutSeconds.Should().BeNull();
        config.AdditionalSettings.Should().NotBeNull().And.BeEmpty();
        config.Enabled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ProviderConfiguration can be instantiated with custom values.
    /// </summary>
    [Fact]
    public void ProviderConfiguration_WithCustomValues_ShouldSetProperties()
    {
        // Act
        var config = new ProviderConfiguration
        {
            ApiKey = "test-key",
            BaseUrl = "https://api.example.com",
            DefaultModel = "test-model",
            OrganizationId = "org-123",
            TimeoutSeconds = 45,
            Enabled = false
        };

        // Assert
        config.ApiKey.Should().Be("test-key");
        config.BaseUrl.Should().Be("https://api.example.com");
        config.DefaultModel.Should().Be("test-model");
        config.OrganizationId.Should().Be("org-123");
        config.TimeoutSeconds.Should().Be(45);
        config.Enabled.Should().BeFalse();
    }

    /// <summary>
    /// Tests that ProviderConfiguration AdditionalSettings can be modified.
    /// </summary>
    [Fact]
    public void ProviderConfiguration_AdditionalSettings_ShouldBeModifiable()
    {
        // Arrange
        var config = new ProviderConfiguration();

        // Act
        config.AdditionalSettings.Add("temperature", "0.7");
        config.AdditionalSettings.Add("max_tokens", "1000");

        // Assert
        config.AdditionalSettings.Should().HaveCount(2);
        config.AdditionalSettings["temperature"].Should().Be("0.7");
        config.AdditionalSettings["max_tokens"].Should().Be("1000");
    }

    /// <summary>
    /// Tests that ProviderConfiguration can bind from configuration.
    /// </summary>
    [Fact]
    public void ProviderConfiguration_FromConfiguration_ShouldBindCorrectly()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["ApiKey"] = "sk-test-key",
            ["BaseUrl"] = "https://api.openai.com/v1",
            ["DefaultModel"] = "gpt-4-turbo",
            ["OrganizationId"] = "org-xyz",
            ["TimeoutSeconds"] = "90",
            ["Enabled"] = "true"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var config = new ProviderConfiguration();
        configuration.Bind(config);

        // Assert
        config.ApiKey.Should().Be("sk-test-key");
        config.BaseUrl.Should().Be("https://api.openai.com/v1");
        config.DefaultModel.Should().Be("gpt-4-turbo");
        config.OrganizationId.Should().Be("org-xyz");
        config.TimeoutSeconds.Should().Be(90);
        config.Enabled.Should().BeTrue();
    }

    /// <summary>
    /// Tests that ProviderConfiguration binds additional settings from configuration.
    /// </summary>
    [Fact]
    public void ProviderConfiguration_FromConfiguration_ShouldBindAdditionalSettings()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["ApiKey"] = "sk-test",
            ["AdditionalSettings:temperature"] = "0.8",
            ["AdditionalSettings:max_tokens"] = "2000",
            ["AdditionalSettings:top_p"] = "0.9"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var config = new ProviderConfiguration();
        configuration.Bind(config);

        // Assert
        config.AdditionalSettings.Should().HaveCount(3);
        config.AdditionalSettings["temperature"].Should().Be("0.8");
        config.AdditionalSettings["max_tokens"].Should().Be("2000");
        config.AdditionalSettings["top_p"].Should().Be("0.9");
    }

    /// <summary>
    /// Tests that ProviderConfiguration handles disabled state.
    /// </summary>
    [Fact]
    public void ProviderConfiguration_WithDisabled_ShouldSetEnabledToFalse()
    {
        // Act
        var config = new ProviderConfiguration
        {
            ApiKey = "test-key",
            Enabled = false
        };

        // Assert
        config.Enabled.Should().BeFalse();
    }

    #endregion

    #region Integration Tests

    /// <summary>
    /// Tests complete configuration binding from JSON-like structure.
    /// </summary>
    [Fact]
    public void Configuration_CompleteStructure_ShouldBindCorrectly()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["AiSdk:DefaultProvider"] = "openai",
            ["AiSdk:TimeoutSeconds"] = "120",
            ["AiSdk:EnableHealthChecks"] = "true",
            ["AiSdk:EnableTelemetry"] = "true",
            ["AiSdk:Providers:openai:ApiKey"] = "sk-openai-key",
            ["AiSdk:Providers:openai:DefaultModel"] = "gpt-4",
            ["AiSdk:Providers:openai:BaseUrl"] = "https://api.openai.com/v1",
            ["AiSdk:Providers:openai:TimeoutSeconds"] = "60",
            ["AiSdk:Providers:anthropic:ApiKey"] = "sk-ant-key",
            ["AiSdk:Providers:anthropic:DefaultModel"] = "claude-3-opus-20240229",
            ["AiSdk:Providers:anthropic:TimeoutSeconds"] = "90"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var options = new AiSdkOptions();
        configuration.GetSection("AiSdk").Bind(options);

        // Assert
        options.DefaultProvider.Should().Be("openai");
        options.TimeoutSeconds.Should().Be(120);
        options.EnableHealthChecks.Should().BeTrue();
        options.EnableTelemetry.Should().BeTrue();
        options.Providers.Should().HaveCount(2);

        var openai = options.Providers["openai"];
        openai.ApiKey.Should().Be("sk-openai-key");
        openai.DefaultModel.Should().Be("gpt-4");
        openai.BaseUrl.Should().Be("https://api.openai.com/v1");
        openai.TimeoutSeconds.Should().Be(60);

        var anthropic = options.Providers["anthropic"];
        anthropic.ApiKey.Should().Be("sk-ant-key");
        anthropic.DefaultModel.Should().Be("claude-3-opus-20240229");
        anthropic.TimeoutSeconds.Should().Be(90);
    }

    /// <summary>
    /// Tests configuration with minimal settings.
    /// </summary>
    [Fact]
    public void Configuration_MinimalSettings_ShouldUseDefaults()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["Providers:openai:ApiKey"] = "sk-test"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var options = new AiSdkOptions();
        configuration.Bind(options);

        // Assert
        options.DefaultProvider.Should().BeNull();
        options.TimeoutSeconds.Should().BeNull();
        options.EnableHealthChecks.Should().BeTrue();
        options.EnableTelemetry.Should().BeTrue();
        options.Providers.Should().ContainKey("openai");
        options.Providers["openai"].ApiKey.Should().Be("sk-test");
        options.Providers["openai"].Enabled.Should().BeTrue();
    }

    /// <summary>
    /// Tests configuration with all features disabled.
    /// </summary>
    [Fact]
    public void Configuration_AllFeaturesDisabled_ShouldRespectSettings()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["EnableHealthChecks"] = "false",
            ["EnableTelemetry"] = "false",
            ["Providers:openai:ApiKey"] = "sk-test",
            ["Providers:openai:Enabled"] = "false"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var options = new AiSdkOptions();
        configuration.Bind(options);

        // Assert
        options.EnableHealthChecks.Should().BeFalse();
        options.EnableTelemetry.Should().BeFalse();
        options.Providers["openai"].Enabled.Should().BeFalse();
    }

    /// <summary>
    /// Tests configuration with environment variable style keys.
    /// </summary>
    [Fact]
    public void Configuration_EnvironmentVariableStyle_ShouldBindCorrectly()
    {
        // Arrange
        var configDict = new Dictionary<string, string?>
        {
            ["AiSdk:DefaultProvider"] = "google",
            ["AiSdk:Providers:google:ApiKey"] = "google-key",
            ["AiSdk:Providers:google:DefaultModel"] = "gemini-pro"
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        // Act
        var options = new AiSdkOptions();
        configuration.GetSection("AiSdk").Bind(options);

        // Assert
        options.DefaultProvider.Should().Be("google");
        options.Providers["google"].ApiKey.Should().Be("google-key");
        options.Providers["google"].DefaultModel.Should().Be("gemini-pro");
    }

    #endregion
}
