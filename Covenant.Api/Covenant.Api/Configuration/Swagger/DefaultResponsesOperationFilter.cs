using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Covenant.Api.Configuration.Swagger;

/// <summary>
/// Adds the standard error responses (401, 403, 500) to every operation so the
/// generated OpenAPI document reflects the API's actual error contract without
/// requiring each controller action to declare them explicitly.
/// </summary>
public class DefaultResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Responses.TryAdd("500", new OpenApiResponse { Description = "Internal Server Error" });

        var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>().Any()
            || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any() ?? false);

        if (hasAllowAnonymous)
        {
            return;
        }

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized — authentication required" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden — insufficient permissions" });
    }
}
