using Asp.Versioning;
using AutoMapper.EquivalencyExpression;
using Covenant.Api.Authorization;
using Covenant.Api.BackgroundServices;
using Covenant.Api.Configuration;
using Covenant.Api.Middlewares;
using Covenant.Common.Resources;
using Covenant.Documents;
using Covenant.Infrastructure.Context;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure logging early to capture startup errors
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddApplicationInsights();
}

var logger = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddDebug();
}).CreateLogger<Program>();

logger.LogInformation("Starting Covenant API application...");

builder.WebHost
    .UseSetting("detailedErrors", "true")
    .CaptureStartupErrors(true)
    .UseShutdownTimeout(TimeSpan.FromSeconds(30));

builder.Services
    .AddControllersWithViews()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddNewtonsoftJson();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Covenant", Version = "v1" });

        opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Ingrese un token JWT válido",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });

        var securityRequirement = new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }
            ] = Array.Empty<string>()
        };
        opt.AddSecurityRequirement(securityRequirement);

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            opt.IncludeXmlComments(xmlPath);
        }
    });

logger.LogInformation("Configuring services...");

builder.Services.AddHostedService<SigookBackgroundService>();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ServicesConfiguration).Assembly));
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddAdapters();
builder.Services.AddConfigurations(builder.Configuration);
builder.Services.AddClients(builder.Configuration);
builder.Services.AddContainers(builder);
builder.Services.AddAzureServiceBusConsumer(builder);
builder.Services.AddAutoMapper(expr => expr.AddCollectionMappers(), Assembly.GetExecutingAssembly());
builder.Services.AddLocalization();
builder.Services.AddPolices();
builder.Services.AddCors(opt => opt.AddPolicy("default", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCaching();
builder.Services.AddOptions();

builder.Services.AddApiVersioning(v =>
{
    v.ReportApiVersions = true;
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AuthenticationOptions:Authority"];
        options.RequireHttpsMetadata = false;
        options.Audience = builder.Configuration["AuthenticationOptions:ApiName"];
    });

logger.LogInformation("Configuring database connection...");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!builder.Environment.IsDevelopment())
{
    connectionString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogWarning("Database connection string is missing or empty. Application will start but health checks will report unhealthy.");
    // Register DbContext with empty connection string to prevent startup errors
    builder.Services.AddDbContext<CovenantContext>(b => b.UseNpgsql(""));
    builder.Services.AddDbContext<MyKeysContext>(b => b.UseNpgsql(""));
}
else
{
    builder.Services.AddDbContext<CovenantContext>(b => b.UseNpgsql(connectionString));
    builder.Services.AddDbContext<MyKeysContext>(b => b.UseNpgsql(connectionString))
        .AddDataProtection()
        .PersistKeysToDbContext<MyKeysContext>();
}

builder.Services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

logger.LogInformation("Configuring health checks...");
builder.Services.AddCovenantHealthChecks(builder.Configuration, builder.Environment);

TelemetryDebugWriter.IsTracingDisabled = true;

logger.LogInformation("Building application...");
var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseCovenantHealthChecks();

var supportedCultures = new[]
{
    new CultureInfo(ApiServicesConfiguration.EnUsCulture),
    new CultureInfo(ApiServicesConfiguration.EsCulture)
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(ApiServicesConfiguration.EnUsCulture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync($"{{ \"Error\" : \"{ApiResources.GeneralError}\" }}");
        });
    });
}

app.UseRouting();
app.UseCors("default");

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger(o => o.RouteTemplate = "sigook/swagger/{documentname}/swagger.json")
       .UseSwaggerUI(o =>
       {
           o.SwaggerEndpoint("/sigook/swagger/v1/swagger.json", "Sigook");
           o.RoutePrefix = string.Empty;
       });
}
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseResponseCaching();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}"
);
app.UseMiddleware<BufferingMiddleware>();
logger.LogInformation("Application configured successfully, starting web host...");
logger.LogInformation("Health checks available at /health, /healthz, /ready, and /live endpoints");

await app.RunAsync();
logger.LogInformation("Application stopped gracefully.");

public partial class Program
{
}