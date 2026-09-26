using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Covenant.Api.Configuration.OpenApi;

public sealed class BearerSecurityDocumentTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Title = "Covenant/Sigook API";
        document.Info.Version = "v1";
        document.Info.Description = "Staffing and recruitment platform API for the Canadian market. "
            + "Routes for the Agency, Company, Worker and Accounting modules.";
        document.Info.Contact = new OpenApiContact { Name = "Covenant/Sigook" };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter a valid JWT access token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, document)] = []
        });
        return Task.CompletedTask;
    }
}
