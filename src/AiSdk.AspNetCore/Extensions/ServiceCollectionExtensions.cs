using AiSdk.Abstractions;
using AiSdk.AspNetCore.Configuration;
using AiSdk.AspNetCore.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace AiSdk.AspNetCore.Extensions;

/// <summary>
/// Extension methods for configuring AI SDK services in ASP.NET Core applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AI SDK services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddAiSdk();
    /// </code>
    /// </example>
    public static IServiceCollection AddAiSdk(this IServiceCollection services)
    {
        return services.AddAiSdk(options => { });
    }

    /// <summary>
    /// Adds AI SDK services to the dependency injection container with custom configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An action to configure AI SDK options.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddAiSdk(options =>
    /// {
    ///     options.DefaultProvider = "openai";
    ///     options.Providers["openai"] = new ProviderConfiguration
    ///     {
    ///         ApiKey = "your-api-key",
    ///         DefaultModel = "gpt-4"
    ///     };
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddAiSdk(
        this IServiceCollection services,
        Action<AiSdkOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        // Configure options
        services.Configure(configure);

        // Register core services
        RegisterCoreServices(services);

        return services;
    }

    /// <summary>
    /// Adds AI SDK services to the dependency injection container with configuration binding.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section containing AI SDK settings.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// // In appsettings.json:
    /// {
    ///   "AiSdk": {
    ///     "DefaultProvider": "openai",
    ///     "Providers": {
    ///       "openai": {
    ///         "ApiKey": "your-api-key",
    ///         "DefaultModel": "gpt-4"
    ///       }
    ///     }
    ///   }
    /// }
    ///
    /// // In Program.cs:
    /// services.AddAiSdk(builder.Configuration.GetSection("AiSdk"));
    /// </code>
    /// </example>
    public static IServiceCollection AddAiSdk(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Bind configuration to options
        services.Configure<AiSdkOptions>(configuration);

        // Register core services
        RegisterCoreServices(services);

        return services;
    }

    /// <summary>
    /// Adds AI SDK health checks to the application.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The name of the health check. Defaults to "ai-sdk".</param>
    /// <param name="failureStatus">The status to report when the check fails. Defaults to Degraded.</param>
    /// <param name="tags">Optional tags for the health check.</param>
    /// <returns>The health checks builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddHealthChecks()
    ///     .AddAiSdkHealthCheck();
    /// </code>
    /// </example>
    public static IHealthChecksBuilder AddAiSdkHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "ai-sdk",
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.Add(new HealthCheckRegistration(
            name,
            sp => new AiSdkHealthCheck(
                sp.GetRequiredService<ILogger<AiSdkHealthCheck>>()),
            failureStatus ?? HealthStatus.Degraded,
            tags));
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        // Note: ILanguageModel implementations should be registered by provider-specific packages
        // This package only provides the infrastructure for ASP.NET Core integration

        // Register factory for creating language models (if needed)
        // For now, we expect consumers to register their own ILanguageModel implementations

        // Add logging
        services.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));
    }
}
