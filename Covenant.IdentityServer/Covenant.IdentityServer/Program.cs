using Covenant.Common.Entities;
using Covenant.IdentityServer.BackgroundServices;
using Covenant.IdentityServer.Configuration;
using Covenant.IdentityServer.Data;
using Covenant.IdentityServer.Entities;
using Covenant.IdentityServer.Services;
using Covenant.IdentityServer.Services.Impl;
using Covenant.IdentityServer.Services.Models;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost
    .UseSetting("detailedErrors", "true")
    .CaptureStartupErrors(true);

builder.Services.AddControllersWithViews();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.Configure<SecurityStampValidatorOptions>(o => o.ValidationInterval = TimeSpan.FromDays(1));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddCors(options => options.AddPolicy("default", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!builder.Environment.IsDevelopment())
{
    connectionString = Environment.GetEnvironmentVariable("POSTGRESQLCONNSTR_DefaultConnection");
}
builder.Services.AddDbContext<CovenantContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddIdentity<CovenantUser, CovenantRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<CovenantContext>().AddDefaultTokenProviders();

var issuerUri = Environment.GetEnvironmentVariable("IssuerUri") ?? builder.Configuration.GetValue<string>("IssuerUri") ?? "https://accounts.com";
var identityBuilder = builder.Services.AddIdentityServer(options => options.IssuerUri = issuerUri)
    .AddDeveloperSigningCredential()
    .AddConfigurationStore(options => options.ConfigureDbContext = b => b.UseNpgsql(connectionString))
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = b => b.UseNpgsql(connectionString);
        options.EnableTokenCleanup = true;
        options.TokenCleanupInterval = 30;
    })
    .AddAspNetIdentity<CovenantUser>()
    .AddProfileService<CustomProfileService<CovenantUser>>();

builder.Services
    .AddAuthentication()
    .AddMicrosoftAuthentication365(builder.Configuration);

builder.Services.AddHttpClient();
if (!builder.Environment.IsProduction())
{
    IdentityModelEventSource.ShowPII = true;
}
builder.Services.AddScoped<ITeamsNotification, TeamsNotification>();
builder.Services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
builder.Services.AddScoped<IClientService, ClientService>();
if (builder.Environment.IsProduction())
{
    builder.Services.AddScoped<IEmailService, EmailService>();
}
else
{
    builder.Services.AddScoped<IEmailService>(s =>
    {
        var emailService = new EmailService(s.GetService<IOptions<EmailSettings>>(), s.GetService<ILogger<EmailService>>());
        var teamsNotification = s.GetService<ITeamsNotification>();
        return new EmailServiceDevelopment(emailService, teamsNotification);
    });
}
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<CovenantContext>()
    .AddDbContextCheck<PersistedGrantDbContext>()
    .AddDbContextCheck<ConfigurationDbContext>()
    .AddDbContextCheck<MyKeysContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddHostedService<SigookIdentityBackgroundService>();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Use((ctx, next) =>
{
    ctx.Request.Scheme = "https";
    return next();
});
app.UseForwardedHeaders();
app.UseHealthChecks("/health");
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}
app.UseRouting();
app.UseCors("default");
app.UseIdentityServer();
app.UseAuthorization();
app.UseStaticFiles();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
await app.RunAsync();

public partial class Program { }
