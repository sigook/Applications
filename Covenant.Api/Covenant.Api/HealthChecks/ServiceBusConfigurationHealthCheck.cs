using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

/// <summary>
/// Health check that validates Azure Service Bus connection string configuration exists
/// </summary>
public class ServiceBusConfigurationHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly ILogger<ServiceBusConfigurationHealthCheck> _logger;

    public ServiceBusConfigurationHealthCheck(
        string connectionString,
        ILogger<ServiceBusConfigurationHealthCheck> logger)
    {
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                _logger.LogWarning("Service Bus connection string is missing or empty");

                var data = new Dictionary<string, object>
                {
                    ["configurationKey"] = "ConnectionStrings:ServiceBusConnection",
                    ["environmentVariable"] = "CUSTOMCONNSTR_ServiceBusConnection",
                    ["message"] = "Service Bus connection string is not configured"
                };

                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Service Bus connection string is missing.",
                    null,
                    data));
            }

            _logger.LogDebug("Service Bus connection string is configured");
            return Task.FromResult(HealthCheckResult.Healthy("Service Bus connection string is configured"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking Service Bus configuration");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Failed to check Service Bus configuration", ex));
        }
    }
}
