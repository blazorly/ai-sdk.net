using AiSdk.AspNetCore.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiSdk.AspNetCore.HealthChecks;

/// <summary>
/// Health check for AI SDK providers and services.
/// </summary>
/// <remarks>
/// This health check verifies that AI SDK services are properly configured and available.
/// It checks provider configuration, API keys, and validates that at least one provider is ready.
/// </remarks>
public class AiSdkHealthCheck : IHealthCheck
{
    private readonly ILogger<AiSdkHealthCheck> _logger;
    private readonly IOptions<AiSdkOptions>? _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="AiSdkHealthCheck"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">Optional AI SDK options.</param>
    public AiSdkHealthCheck(
        ILogger<AiSdkHealthCheck> logger,
        IOptions<AiSdkOptions>? options = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options;
    }

    /// <summary>
    /// Performs the health check asynchronously.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A health check result indicating the status of AI SDK services.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["checked"] = "ai-sdk-services"
            };

            // If no options are configured, return degraded
            if (_options == null)
            {
                _logger.LogWarning("AI SDK options not configured");
                return HealthCheckResult.Degraded(
                    "AI SDK options not configured",
                    null,
                    data);
            }

            var options = _options.Value;

            // Check if any providers are configured
            if (options.Providers == null || options.Providers.Count == 0)
            {
                _logger.LogWarning("No AI providers configured");
                data["providersConfigured"] = 0;
                return HealthCheckResult.Degraded(
                    "No AI providers configured",
                    null,
                    data);
            }

            // Validate provider configurations
            var enabledProviders = 0;
            var providersWithoutApiKey = new List<string>();
            var disabledProviders = new List<string>();

            foreach (var (providerName, providerConfig) in options.Providers)
            {
                if (providerConfig == null)
                {
                    continue;
                }

                if (!providerConfig.Enabled)
                {
                    disabledProviders.Add(providerName);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(providerConfig.ApiKey))
                {
                    providersWithoutApiKey.Add(providerName);
                    _logger.LogWarning("Provider {ProviderName} is enabled but has no API key configured", providerName);
                    continue;
                }

                enabledProviders++;
            }

            data["totalProvidersConfigured"] = options.Providers.Count;
            data["enabledProviders"] = enabledProviders;
            data["disabledProviders"] = disabledProviders.Count;
            data["providersWithoutApiKey"] = providersWithoutApiKey.Count;

            if (disabledProviders.Count > 0)
            {
                data["disabledProvidersList"] = string.Join(", ", disabledProviders);
            }

            if (providersWithoutApiKey.Count > 0)
            {
                data["providersWithoutApiKeyList"] = string.Join(", ", providersWithoutApiKey);
            }

            // If no providers are enabled with API keys, return Unhealthy
            if (enabledProviders == 0)
            {
                _logger.LogError("No providers are properly configured with API keys");
                return HealthCheckResult.Unhealthy(
                    "No providers are properly configured with API keys",
                    null,
                    data);
            }

            // If some providers are missing API keys, return Degraded
            if (providersWithoutApiKey.Count > 0)
            {
                _logger.LogWarning("Some providers are missing API keys: {Providers}",
                    string.Join(", ", providersWithoutApiKey));
                return HealthCheckResult.Degraded(
                    $"{enabledProviders} provider(s) ready, {providersWithoutApiKey.Count} provider(s) missing API keys",
                    null,
                    data);
            }

            // All checks passed
            _logger.LogDebug("AI SDK health check completed successfully with {Count} enabled provider(s)", enabledProviders);

            return await Task.FromResult(
                HealthCheckResult.Healthy(
                    $"AI SDK services are available with {enabledProviders} provider(s) ready",
                    data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI SDK health check encountered an error");

            return HealthCheckResult.Unhealthy(
                "AI SDK health check encountered an error",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["errorType"] = ex.GetType().Name,
                    ["timestamp"] = DateTime.UtcNow
                });
        }
    }
}
