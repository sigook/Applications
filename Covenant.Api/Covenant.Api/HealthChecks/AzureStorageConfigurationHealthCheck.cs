using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

/// <summary>
/// Health check that validates the Azure Storage connection string configuration exists
/// </summary>
public class AzureStorageConfigurationHealthCheck(
    string fileStorageConnection,
    ILogger<AzureStorageConfigurationHealthCheck> logger) : IHealthCheck
{
    private readonly string _fileStorageConnection = fileStorageConnection;
    private readonly ILogger<AzureStorageConfigurationHealthCheck> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_fileStorageConnection))
            {
                _logger.LogWarning("File storage connection string is missing");
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Azure Storage configuration is incomplete. Missing: FileStorageConnection",
                    null,
                    new Dictionary<string, object>
                    {
                        ["fileStorageConnection"] = "missing",
                        ["environmentVariables"] = new[] { "CUSTOMCONNSTR_FileStorageConnection" }
                    }));
            }

            _logger.LogDebug("Azure Storage connection string is configured");
            return Task.FromResult(HealthCheckResult.Healthy(
                "Azure Storage connection string is configured",
                new Dictionary<string, object> { ["fileStorageConnection"] = "configured" }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking Azure Storage configuration");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Failed to check Azure Storage configuration", ex));
        }
    }
}
