using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

public class AzureStorageHealthCheck(string connectionString, string name) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return HealthCheckResult.Unhealthy(
                    $"Azure Storage ({name}) connection string is not configured",
                    null,
                    new Dictionary<string, object>
                    {
                        ["storage"] = name,
                        ["message"] = "Connection string is missing or empty"
                    });
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            var accountInfo = await blobServiceClient.GetAccountInfoAsync(cancellationToken);

            return HealthCheckResult.Healthy($"Azure Storage ({name}) is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Azure Storage ({name}) is not accessible", ex);
        }
    }
}