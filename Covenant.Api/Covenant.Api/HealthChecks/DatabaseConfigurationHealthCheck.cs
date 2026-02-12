using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

/// <summary>
/// Health check that validates database connection string configuration exists
/// </summary>
public class DatabaseConfigurationHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseConfigurationHealthCheck> _logger;

    public DatabaseConfigurationHealthCheck(
        string connectionString,
        ILogger<DatabaseConfigurationHealthCheck> logger)
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
                _logger.LogError("Database connection string is missing or empty");

                var data = new Dictionary<string, object>
                {
                    ["configurationKey"] = "ConnectionStrings:DefaultConnection",
                    ["environmentVariable"] = "POSTGRESQLCONNSTR_DefaultConnection",
                    ["message"] = "Database connection string is not configured"
                };

                return Task.FromResult(HealthCheckResult.Unhealthy(
                    "Database connection string is missing or empty. Application cannot connect to database.",
                    null,
                    data));
            }

            _logger.LogDebug("Database connection string is configured");
            return Task.FromResult(HealthCheckResult.Healthy("Database connection string is configured"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking database configuration");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Failed to check database configuration", ex));
        }
    }
}
