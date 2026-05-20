using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Covenant.Api.Configuration.Swagger;

/// <summary>
/// Declares the deployment servers on the OpenAPI document. Implemented as a
/// document filter (instead of a <c>UseSwagger</c> pre-serialize filter) so the
/// servers are also present in the document generated at build time by
/// Microsoft.Extensions.ApiDescription.Server.
/// </summary>
public class ServersDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Servers =
        [
            new OpenApiServer { Url = "https://localhost:44307", Description = "Local" },
            new OpenApiServer { Url = "https://staging.api.sigook.ca", Description = "Staging" }
        ];
    }
}
