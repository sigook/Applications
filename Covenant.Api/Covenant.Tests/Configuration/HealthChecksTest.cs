using Covenant.Api.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Covenant.Tests.Configuration;

public class HealthChecksTest
{
    private const string Configured = "Host=localhost;Database=covenant";
    private const string Malformed = "not-a-connection-string";

    private static Task<HealthCheckResult> Check(IHealthCheck check) => check.CheckHealthAsync(new HealthCheckContext());

    public static TheoryData<string> Missing => [null, "", "   "];

    [Theory]
    [MemberData(nameof(Missing))]
    public async Task DatabaseConfiguration_Missing_IsUnhealthy(string connectionString)
    {
        var result = await Check(new DatabaseConfigurationHealthCheck(connectionString, NullLogger<DatabaseConfigurationHealthCheck>.Instance));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("ConnectionStrings:DefaultConnection", result.Data["configurationKey"]);
    }

    [Fact]
    public async Task DatabaseConfiguration_Configured_IsHealthy()
    {
        var result = await Check(new DatabaseConfigurationHealthCheck(Configured, NullLogger<DatabaseConfigurationHealthCheck>.Instance));

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Theory]
    [MemberData(nameof(Missing))]
    public async Task ServiceBusConfiguration_Missing_IsUnhealthy(string connectionString)
    {
        var result = await Check(new ServiceBusConfigurationHealthCheck(connectionString, NullLogger<ServiceBusConfigurationHealthCheck>.Instance));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("ConnectionStrings:ServiceBusConnection", result.Data["configurationKey"]);
    }

    [Fact]
    public async Task ServiceBusConfiguration_Configured_IsHealthy()
    {
        var result = await Check(new ServiceBusConfigurationHealthCheck(Configured, NullLogger<ServiceBusConfigurationHealthCheck>.Instance));

        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Theory]
    [MemberData(nameof(Missing))]
    public async Task AzureStorageConfiguration_Missing_IsUnhealthy(string connectionString)
    {
        var result = await Check(new AzureStorageConfigurationHealthCheck(connectionString, NullLogger<AzureStorageConfigurationHealthCheck>.Instance));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("missing", result.Data["fileStorageConnection"]);
    }

    [Fact]
    public async Task AzureStorageConfiguration_Configured_IsHealthy()
    {
        var result = await Check(new AzureStorageConfigurationHealthCheck(Configured, NullLogger<AzureStorageConfigurationHealthCheck>.Instance));

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("configured", result.Data["fileStorageConnection"]);
    }

    [Fact]
    public void ConfigurationChecks_RequireLogger()
    {
        Assert.Throws<ArgumentNullException>(() => new DatabaseConfigurationHealthCheck(Configured, null));
        Assert.Throws<ArgumentNullException>(() => new ServiceBusConfigurationHealthCheck(Configured, null));
        Assert.Throws<ArgumentNullException>(() => new AzureStorageConfigurationHealthCheck(Configured, null));
    }

    [Theory]
    [MemberData(nameof(Missing))]
    public async Task AzureStorage_Missing_IsUnhealthy(string connectionString)
    {
        var result = await Check(new AzureStorageHealthCheck(connectionString, "files"));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("files", result.Data["storage"]);
    }

    [Fact]
    public async Task AzureStorage_MalformedConnectionString_IsUnhealthy()
    {
        var result = await Check(new AzureStorageHealthCheck(Malformed, "files"));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Exception);
    }

    [Theory]
    [MemberData(nameof(Missing))]
    public async Task AzureServiceBus_Missing_IsUnhealthy(string connectionString)
    {
        var result = await Check(new AzureServiceBusHealthCheck(connectionString));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Null(result.Exception);
    }

    [Fact]
    public async Task AzureServiceBus_MalformedConnectionString_IsUnhealthy()
    {
        var result = await Check(new AzureServiceBusHealthCheck(Malformed));

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Exception);
    }
}
