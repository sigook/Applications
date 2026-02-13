using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

/// <summary>
/// Health check that validates Azure Storage connection strings configuration exists
/// </summary>
public class AzureStorageConfigurationHealthCheck : IHealthCheck
{
    private readonly string _accountingStorageConnection;
    private readonly string _fileStorageConnection;
    private readonly ILogger<AzureStorageConfigurationHealthCheck> _logger;

    public AzureStorageConfigurationHealthCheck(
        string accountingStorageConnection,
        string fileStorageConnection,
        ILogger<AzureStorageConfigurationHealthCheck> logger)
    {
        _accountingStorageConnection = accountingStorageConnection;
        _fileStorageConnection = fileStorageConnection;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var missingConfigurations = new List<string>();
            var data = new Dictionary<string, object>();

            if (string.IsNullOrWhiteSpace(_accountingStorageConnection))
            {
                missingConfigurations.Add("AccountingStorageConnection");
                data["accountingStorageConnection"] = "missing";
                _logger.LogWarning("Accounting storage connection string is missing");
            }
            else
            {
                data["accountingStorageConnection"] = "configured";
            }

            if (string.IsNullOrWhiteSpace(_fileStorageConnection))
            {
                missingConfigurations.Add("FileStorageConnection");
                data["fileStorageConnection"] = "missing";
                _logger.LogWarning("File storage connection string is missing");
            }
            else
            {
                data["fileStorageConnection"] = "configured";
            }

            if (missingConfigurations.Any())
            {
                data["missingConfigurations"] = missingConfigurations.ToArray();
                data["environmentVariables"] = new[]
                {
                    "CUSTOMCONNSTR_AccountingStorageConnection",
                    "CUSTOMCONNSTR_FileStorageConnection"
                };

                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Azure Storage configuration is incomplete. Missing: {string.Join(", ", missingConfigurations)}",
                    null,
                    data));
            }

            _logger.LogDebug("All Azure Storage connection strings are configured");
            return Task.FromResult(HealthCheckResult.Healthy(
                "All Azure Storage connection strings are configured",
                data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking Azure Storage configuration");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Failed to check Azure Storage configuration", ex));
        }
    }
}
