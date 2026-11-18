using System.Threading.RateLimiting;

namespace Covenant.Api.Configuration;

public static class RateLimitingConfiguration
{
    public const string PasswordResetPolicy = "password-reset";

    private const int PasswordResetPermitsPerWindow = 10;
    private const int PasswordResetSegmentsPerWindow = 5;
    private static readonly TimeSpan PasswordResetWindow = TimeSpan.FromMinutes(10);

    public const string CandidateApplicationPolicy = "candidate-application";

    private const int CandidateApplicationPermitsPerWindow = 30;
    private const int CandidateApplicationSegmentsPerWindow = 5;
    private static readonly TimeSpan CandidateApplicationWindow = TimeSpan.FromMinutes(10);

    public static IServiceCollection AddCovenantRateLimiting(this IServiceCollection services) =>
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, _) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(RateLimitingConfiguration));
                logger.LogWarning("Rate limit exceeded. Path={Path} ClientIp={ClientIp}", context.HttpContext.Request.Path, ClientIp(context.HttpContext));
                return ValueTask.CompletedTask;
            };
            options.AddPolicy(PasswordResetPolicy, httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(ClientIp(httpContext), _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = PasswordResetPermitsPerWindow,
                    Window = PasswordResetWindow,
                    SegmentsPerWindow = PasswordResetSegmentsPerWindow,
                    QueueLimit = 0
                }));
            options.AddPolicy(CandidateApplicationPolicy, httpContext =>
                RateLimitPartition.GetSlidingWindowLimiter(ClientIp(httpContext), _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = CandidateApplicationPermitsPerWindow,
                    Window = CandidateApplicationWindow,
                    SegmentsPerWindow = CandidateApplicationSegmentsPerWindow,
                    QueueLimit = 0
                }));
        });

    private static string ClientIp(HttpContext httpContext) =>
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
