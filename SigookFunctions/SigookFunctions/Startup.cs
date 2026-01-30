using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SigookFunctions;
using SigookFunctions.Options;
using SigookFunctions.Services;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SigookFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var context = builder.GetContext();

            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.AddOptions<SigookFunctionsOptions>()
                .Bind(configuration.GetSection(SigookFunctionsOptions.SectionName));

            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddSingleton<ISigookApi, SigookApi>();
            builder.Services.AddSingleton<IEmailService, SendGridService>();
        }
    }
}
