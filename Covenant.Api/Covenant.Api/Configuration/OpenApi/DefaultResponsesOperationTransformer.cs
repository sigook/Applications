using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Covenant.Api.Configuration.OpenApi;

public sealed class DefaultResponsesOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        operation.Responses ??= [];
        operation.Responses.TryAdd("500", new OpenApiResponse { Description = "Internal Server Error" });

        var allowsAnonymous = context.Description.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any();
        if (allowsAnonymous)
        {
            return Task.CompletedTask;
        }

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized — authentication required" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden — insufficient permissions" });
        return Task.CompletedTask;
    }
}
