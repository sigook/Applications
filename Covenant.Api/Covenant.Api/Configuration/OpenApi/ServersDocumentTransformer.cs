using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Covenant.Api.Configuration.OpenApi;

public sealed class ServersDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Servers =
        [
            new OpenApiServer { Url = "https://localhost:44307", Description = "Local" },
            new OpenApiServer { Url = "https://staging.api.sigook.ca", Description = "Staging" }
        ];
        return Task.CompletedTask;
    }
}
