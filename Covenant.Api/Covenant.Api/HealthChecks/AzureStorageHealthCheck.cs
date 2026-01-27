using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

public class AzureStorageHealthCheck : IHealthCheck
{
    private readonly string? _connectionString;
    private readonly string _name;

    public AzureStorageHealthCheck(string? connectionString, string name)
    {
        _connectionString = connectionString;
        _name = name;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                return HealthCheckResult.Unhealthy(
                    $"Azure Storage ({_name}) connection string is not configured",
                    null,
                    new Dictionary<string, object>
                    {
                        ["storage"] = _name,
                        ["message"] = "Connection string is missing or empty"
                    });
            }

            var blobServiceClient = new BlobServiceClient(_connectionString);
            var accountInfo = await blobServiceClient.GetAccountInfoAsync(cancellationToken);

            return HealthCheckResult.Healthy($"Azure Storage ({_name}) is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Azure Storage ({_name}) is not accessible", ex);
        }
    }
}