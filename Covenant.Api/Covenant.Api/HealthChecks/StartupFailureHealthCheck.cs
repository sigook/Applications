using Covenant.Common.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Covenant.Api.HealthChecks;

public class StartupFailureHealthCheck : IHealthCheck
{
    private readonly IStartupFailureService _startupFailureService;
    private readonly ILogger<StartupFailureHealthCheck> _logger;

    public StartupFailureHealthCheck(
        IStartupFailureService startupFailureService,
        ILogger<StartupFailureHealthCheck> logger)
    {
        _startupFailureService = startupFailureService ?? throw new ArgumentNullException(nameof(startupFailureService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var failure = _startupFailureService.GetStartupFailure();
            
            if (failure != null)
            {
                _logger.LogWarning("Startup failure detected: {Message} at {OccurredAt}", 
                    failure.Message, failure.OccurredAt);
                
                var data = new Dictionary<string, object>
                {
                    ["startupError"] = failure.Message,
                    ["occurredAt"] = failure.OccurredAt,
                    ["exceptionType"] = failure.Exception.GetType().Name
                };
                
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Application startup failed: {failure.Message}", 
                    failure.Exception,
                    data));
            }

            _logger.LogDebug("No startup failure detected");
            return Task.FromResult(HealthCheckResult.Healthy("Application started successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking startup failure status");
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Failed to check startup status", ex));
        }
    }
}

