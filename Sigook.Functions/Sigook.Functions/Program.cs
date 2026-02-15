using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sigook.Functions.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
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
