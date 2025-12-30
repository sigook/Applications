using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Covenant.Api.Middlewares;

public static class HealthCheckExtensions
{
    public static void UseCovenantHealthChecks(this WebApplication app)
    {
        app.UseHealthChecks("/health");

        app.UseHealthChecks("/healthz", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                
                var response = new
                {
                    status = report.Status.ToString(),
                    totalDuration = report.TotalDuration.TotalMilliseconds,
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        exception = x.Value.Exception?.Message,
                        duration = x.Value.Duration.TotalMilliseconds,
                        description = x.Value.Description,
                        tags = x.Value.Tags
                    }).ToArray()
                };
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                }));
            }
        });

        app.UseHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { status = report.Status.ToString() }));
            }
        });

        app.UseHealthChecks("/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { status = report.Status.ToString() }));
            }
        });
    }
}