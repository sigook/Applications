using Asp.Versioning;
using Azure.Identity;
using Covenant.Api.Authorization;
using Covenant.Api.BackgroundServices;
using Covenant.Api.Configuration;
using Covenant.Api.Configuration.OpenApi;
using Covenant.Api.Extensions;
using Covenant.Common.Resources;
using Covenant.Documents;
using Covenant.Infrastructure.Contexts;
using FluentValidation;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OpenIddict.Validation.AspNetCore;
using Scalar.AspNetCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Azure Key Vault configuration
var keyVaultUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    var env = builder.Environment.IsProduction() ? "production" : "staging";
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential(),
        new PrefixKeyVaultSecretManager($"{env}-api"));
}

// Configure logging early to capture startup errors
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

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
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddOpenApi("v1", options =>
{
    options.CreateSchemaReferenceId = type => type.Type.FullName?.Replace("+", ".");
    options.AddDocumentTransformer<BearerSecurityDocumentTransformer>();
    options.AddDocumentTransformer<ServersDocumentTransformer>();
    options.AddOperationTransformer<DefaultResponsesOperationTransformer>();
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

var authentication = builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
authentication.AddIdentityCookies();
authentication.AddMicrosoftAuthentication365(builder.Configuration);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Home/InvalidUser";
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});
builder.Services.AddCovenantRateLimiting();

logger.LogInformation("Configuring database connection...");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogWarning("Database connection string is missing or empty. Application will start but health checks will report unhealthy.");
    // Register DbContext with empty connection string to prevent startup errors
    builder.Services.AddDbContext<CovenantContext>(b => b.UseNpgsql("").ConfigureWarnings(IgnorePendingModelChanges));
    builder.Services.AddDbContext<MyKeysContext>(b => b.UseNpgsql("").ConfigureWarnings(IgnorePendingModelChanges));
}
else
{
    builder.Services.AddDbContext<CovenantContext>(b => b.UseNpgsql(connectionString).ConfigureWarnings(IgnorePendingModelChanges));
    builder.Services.AddDbContext<MyKeysContext>(b => b.UseNpgsql(connectionString).ConfigureWarnings(IgnorePendingModelChanges))
        .AddDataProtection()
        .PersistKeysToDbContext<MyKeysContext>();
}

static void IgnorePendingModelChanges(WarningsConfigurationBuilder warnings) =>
    warnings.Ignore(RelationalEventId.PendingModelChangesWarning);

builder.Services.AddCovenantIdentity(builder.Configuration);
builder.Services.AddCovenantOpenIddict(builder.Configuration, builder.Environment);

logger.LogInformation("Configuring health checks...");
builder.Services.AddCovenantHealthChecks(builder.Configuration, builder.Environment);

TelemetryDebugWriter.IsTracingDisabled = true;

logger.LogInformation("Building application...");
var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
}

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
app.UseRateLimiter();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("Covenant/Sigook API")
        .AddPreferredSecuritySchemes(BearerSecurityDocumentTransformer.SchemeName)
        .EnablePersistentAuthentication());
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
logger.LogInformation("Application configured successfully, starting web host...");
logger.LogInformation("Health checks available at /health, /healthz, /ready, and /live endpoints");

await app.RunAsync();
logger.LogInformation("Application stopped gracefully.");

public partial class Program
{
}