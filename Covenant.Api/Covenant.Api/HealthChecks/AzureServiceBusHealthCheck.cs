using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

public class AzureServiceBusHealthCheck : IHealthCheck
{
    private readonly string _connectionString;

    public AzureServiceBusHealthCheck(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var client = new ServiceBusClient(_connectionString);
            await using var sender = client.CreateSender("test-queue");
            
            return HealthCheckResult.Healthy("Azure Service Bus is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Azure Service Bus is not accessible", ex);
        }
    }
}