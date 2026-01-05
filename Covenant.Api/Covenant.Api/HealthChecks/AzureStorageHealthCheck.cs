using Azure.Storage.Blobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

public class AzureStorageHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly string _name;

    public AzureStorageHealthCheck(string connectionString, string name)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _name = name;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
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