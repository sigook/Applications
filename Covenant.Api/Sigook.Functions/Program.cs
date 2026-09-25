using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sigook.Functions.Configuration;
using Sigook.Functions.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

var keyVaultUrl = builder.Configuration["KeyVault:Url"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    var prefix = builder.Environment.IsProduction() ? "production" : "staging";
    TokenCredential credential = builder.Environment.IsDevelopment()
        ? new ChainedTokenCredential(new AzureCliCredential(), new VisualStudioCredential())
        : new DefaultAzureCredential();
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        credential,
        new PrefixKeyVaultSecretManager($"{prefix}-func"));
}

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.Configure<ScheduleTasksOptions>(builder.Configuration.GetSection(ScheduleTasksOptions.SectionName));
builder.Services.Configure<CraTablesOptions>(builder.Configuration.GetSection(CraTablesOptions.SectionName));
builder.Services.AddHttpClient(HttpClients.Api);
builder.Services.AddHttpClient(HttpClients.Teams);
builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

builder.Build().Run();
