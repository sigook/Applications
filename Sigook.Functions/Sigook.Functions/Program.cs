using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sigook.Functions.Configuration;
using Sigook.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var builtConfig = config.Build();
        var keyVaultUrl = builtConfig["KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            var env = builtConfig["ENVIRONMENT"] == "Production" ? "production" : "staging";
            config.AddAzureKeyVault(
                new Uri(keyVaultUrl),
                new DefaultAzureCredential(),
                new PrefixKeyVaultSecretManager($"{env}-func"));
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient("Api");
        services.AddHttpClient("Teams");
        services.AddSingleton<ISigookApi, SigookApi>();
        services.AddSingleton<IEmailService, SendGridService>();
    })
    .Build();

host.Run();
