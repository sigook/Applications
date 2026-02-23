using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Sigook.CognitiveServices.UI.HealthChecks;

public class SpeechConfigurationHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public SpeechConfigurationHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var issues = new List<string>();

        var subscriptionKey = _configuration["SpeechConfiguration:SubscriptionKey"];
        if (string.IsNullOrWhiteSpace(subscriptionKey))
            issues.Add("SpeechConfiguration:SubscriptionKey is missing");

        var region = _configuration["SpeechConfiguration:Region"];
        if (string.IsNullOrWhiteSpace(region))
            issues.Add("SpeechConfiguration:Region is missing");

        if (issues.Count > 0)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                "Speech configuration is incomplete.",
                data: new Dictionary<string, object> { ["issues"] = issues }));
        }

        return Task.FromResult(HealthCheckResult.Healthy(
            "Speech configuration is valid."));
    }
}
