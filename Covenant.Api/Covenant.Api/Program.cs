using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Asp.Versioning;
using AutoMapper.EquivalencyExpression;
using Covenant.Api.Authorization;
using Covenant.Api.BackgroundServices;
using Covenant.Api.Configuration;
using Covenant.Api.HealthChecks;
using Covenant.Api.Middlewares;
using Covenant.Common.Interfaces;
using Covenant.Common.Resources;
using Covenant.Documents;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Services;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

try
{
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
        logger.LogError("Database connection string is missing or empty");
        throw new InvalidOperationException("Database connection string is required");
    }

    builder.Services.AddDbContext<CovenantContext>(b => b.UseNpgsql(connectionString));
    builder.Services.AddDbContext<MyKeysContext>(b => b.UseNpgsql(connectionString))
        .AddDataProtection()
        .PersistKeysToDbContext<MyKeysContext>();

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
    await app.RunAsync();
    logger.LogInformation("Application stopped gracefully.");
}
catch (Exception ex)
{
    await StartDegradedMode(args, ex);
}

static async Task StartDegradedMode(string[] args, Exception startupException)
{
    var logger = CreateBootstrapLogger();

    logger.LogCritical(startupException, "Application failed to start normally. Starting in degraded mode.");
    Console.WriteLine($"CRITICAL ERROR: Application failed to start - {startupException.Message}");
    Console.WriteLine("Starting in degraded mode with health checks only...");

    try
    {
        var degradedApp = CreateDegradedApplication(args, startupException, logger);

        logger.LogWarning("Application started in degraded mode. Health checks available at /healthz");

        await degradedApp.RunAsync();
    }
    catch (Exception degradedEx)
    {
        logger.LogCritical(degradedEx, "Failed to start even in degraded mode");
        Console.WriteLine($"CRITICAL: Failed to start degraded mode - {degradedEx.Message}");
        Environment.Exit(1);
    }
}

static WebApplication CreateDegradedApplication(string[] args, Exception startupException, ILogger logger)
{
    var degradedBuilder = WebApplication.CreateBuilder(args);

    // Minimal logging configuration
    degradedBuilder.Logging.ClearProviders();
    degradedBuilder.Logging.AddConsole();
    degradedBuilder.Logging.AddDebug();

    // Add minimal services for degraded mode
    degradedBuilder.Services.AddSingleton<IStartupFailureService, StartupFailureService>();
    degradedBuilder.Services.AddHealthChecks()
        .AddCheck<StartupFailureHealthCheck>("startup-failure", HealthStatus.Unhealthy, tags: new[] { "ready", "live" });

    var degradedApp = degradedBuilder.Build();

    // Record the startup failure
    var startupFailureService = degradedApp.Services.GetRequiredService<IStartupFailureService>();
    startupFailureService.RecordStartupFailure(startupException, "Application startup failed");

    // Configure minimal middleware
    degradedApp.UseRouting();

    // Add health check endpoints
    degradedApp.UseHealthChecks("/health");
    degradedApp.UseHealthChecks("/healthz", new HealthCheckOptions
    {
        ResponseWriter = (context, report) => WriteHealthCheckResponse(context, report, startupException)
    });
    degradedApp.UseHealthChecks("/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = (context, report) => WriteHealthCheckResponse(context, report, startupException)
    });
    degradedApp.UseHealthChecks("/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = (context, report) => WriteHealthCheckResponse(context, report, startupException)
    });

    // Add a basic error endpoint
    degradedApp.MapGet("/", () => Results.Json(new
    {
        status = "degraded",
        message = "Application is running in degraded mode due to startup failure",
        error = startupException.Message,
        healthCheck = "/healthz",
        timestamp = DateTime.UtcNow
    }, GetJsonSerializerOptions()));

    return degradedApp;
}

static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report, Exception startupException)
{
    context.Response.ContentType = "application/json";

    var response = new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.TotalMilliseconds,
        startupError = startupException.Message,
        timestamp = DateTime.UtcNow,
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

    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, GetJsonSerializerOptions()));
}

static ILogger<Program> CreateBootstrapLogger()
{
    return LoggerFactory.Create(config =>
    {
        config.AddConsole();
        config.AddDebug();
        config.SetMinimumLevel(LogLevel.Information);
    }).CreateLogger<Program>();
}

static JsonSerializerOptions GetJsonSerializerOptions()
{
    return new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}

public partial class Program
{
}