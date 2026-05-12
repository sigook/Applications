using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sigook.Functions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var environment = context.HostingEnvironment;

        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
              .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: false);

        var builtConfig = config.Build();
        var keyVaultUrl = builtConfig["KeyVault:Url"];
        if (!string.IsNullOrEmpty(keyVaultUrl))
        {
            var prefix = environment.IsProduction() ? "production" : "staging";
            config.AddAzureKeyVault(
                new Uri(keyVaultUrl),
                new DefaultAzureCredential(),
                new PrefixKeyVaultSecretManager($"{prefix}-func"));
        }
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient("Api");
        services.AddHttpClient("Teams");
    })
    .Build();

host.Run();
