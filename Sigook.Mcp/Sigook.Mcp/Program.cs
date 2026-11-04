using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using Sigook.Mcp.Api;
using Sigook.Mcp.Auth;
using Sigook.Mcp.Configuration;
using Sigook.Mcp.Instructions;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

var environment = SigookEnvironment.FromProcessEnvironment();
builder.Services.AddSingleton(environment);
builder.Services.AddSingleton<TokenStore>();
builder.Services.AddSingleton<SigookSession>();
builder.Services.AddSingleton<ExportCache>();
builder.Services.AddSingleton<CatalogCache>();
builder.Services.AddSingleton<ImportInstructions>();
builder.Services.AddTransient<BearerHandler>();
builder.Services.AddHttpClient<PkceLoginFlow>()
    .ConfigurePrimaryHttpMessageHandler(environment.CreateHandler);
builder.Services.AddRefitGeneratedClient<ISigookApi>()
    .ConfigureHttpClient(http => http.BaseAddress = environment.ApiUrl)
    .ConfigurePrimaryHttpMessageHandler(environment.CreateHandler)
    .AddHttpMessageHandler<BearerHandler>();
builder.Services.AddRefitGeneratedClient<ISigookIdentityApi>()
    .ConfigureHttpClient(http => http.BaseAddress = environment.Authority)
    .ConfigurePrimaryHttpMessageHandler(environment.CreateHandler)
    .AddHttpMessageHandler<BearerHandler>();

builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new() { Name = "sigook", Version = "1.0.0" };
        options.ServerInstructions = "Sigook staffing platform. Call sigook_whoami first to confirm the target environment and session. To import candidates from a job-board export, call get_import_instructions before anything else: it returns the workflow and the column mapping of the source.";
    })
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();

await builder.Build().RunAsync();
