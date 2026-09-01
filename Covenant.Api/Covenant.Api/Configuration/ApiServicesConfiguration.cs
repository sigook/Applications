using Covenant.Api.Authorization;
using Covenant.Api.HealthChecks;
using Covenant.Api.Utils;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Accounting;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Adapters;
using Covenant.Core.BL.Consumers;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Core.BL.Services.Accounting;
using Covenant.Core.BL.Services.Accounting.Invoices;
using Covenant.Core.BL.Services.Accounting.Shared;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Accounting;
using Covenant.Infrastructure.Repositories.Agency;
using Covenant.Infrastructure.Repositories.Candidate;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Repositories.Notification;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Infrastructure.Services.Handlers;
using Covenant.Infrastructure.Services.Storage;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Globalization;

namespace Covenant.Api.Configuration;

public static class ApiServicesConfiguration
{
    public const string EnUsCulture = "en-US";
    public const string EsCulture = "es";
    public const string FrCulture = "fr";

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IDeductionsRepository, DeductionsRepository>();
        services.AddScoped<ISkipPayrollNumberRepository, SkipPayrollNumberRepository>();
        services.AddScoped<ISubcontractorRepository, SubcontractorRepository>();
        services.AddScoped<IPayStubRepository, PayStubRepository>();
        services.AddScoped<IAgencyRepository, AgencyRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<IRunnerRepository, RunnerRepository>();
        services.AddScoped<ITimesheetRepository, TimesheetRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkerRequestRepository, WorkerRequestRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<IShiftRepository, ShiftRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<INotificationDataRepository, NotificationDataRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IAgencyService, AgencyService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IRequestService, RequestService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IRunnerService, RunnerService>();
        services.AddScoped<IRequestApplicantService, RequestApplicantService>();
        services.AddScoped<IWeeklyBoardService, WeeklyBoardService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ICandidateService, CandidateService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ITimesheetService, TimesheetService>();
        services.AddScoped<IPayStubService, PayStubService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IUploadedFilesService, UploadedFilesService>();
        services.AddScoped<IGeocodeService, GeocodeService>();
        services.AddScoped<IIdentityServerService, IdentityServerService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISendGridService, SendGridService>();
        services.AddScoped<IPushNotifications, PushNotifications>();
        services.AddScoped<ITeamsService, TeamsService>();
        services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
        services.AddScoped<IPdfGeneratorService, PdfGeneratorService>();
        services.AddScoped<IAccountingService, AccountingService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<CompanyIdFilter>();
        services.AddScoped<AgencyIdFilter>();
        services.AddScoped<AgencyPersonnelIdFilter>();
        services.AddScoped<CaptchaFilter>();

        // Shared calculation services
        services.AddScoped<ITimesheetCalculatorService, TimesheetCalculatorService>();

        // Invoice services with Strategy pattern
        services.AddScoped<UsaInvoiceService>();
        services.AddScoped<CanadaInvoiceService>();
        services.AddScoped<InvoiceServiceFactory>();

        // CRA deduction tables
        services.AddScoped<ICraPdfParser, CraPdfParser>();
        services.AddScoped<IDeductionImportService, DeductionImportService>();

        //TODO: To Refactor
        services.AddScoped<IDefaultLogoProvider, DefaultLogoProvider>();
        return services;
    }

    public static IServiceCollection AddAdapters(this IServiceCollection services)
    {
        services.AddScoped<ICandidateAdapter, CandidateAdapter>();
        services.AddScoped<ICompanyAdapter, CompanyAdapter>();
        services.AddScoped<IWorkerAdapter, WorkerAdapter>();
        services.AddScoped<IRequestAdapter, RequestAdapter>();
        services.AddScoped<IInvoiceDocumentAdapter, InvoiceDocumentAdapter>();
        services.AddScoped<IPayrollDocumentAdapter, PayrollDocumentAdapter>();
        return services;
    }

    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var rates = new Rates();
        configuration.Bind("Rates", rates);
        services.AddSingleton(rates);

        var timeLimits = new TimeLimits();
        configuration.Bind("TimeLimits", timeLimits);
        services.AddSingleton(timeLimits);

        services.Configure<FilesConfiguration>(configuration.GetSection(nameof(FilesConfiguration)));
        services.Configure<List<EmailSettings>>(configuration.GetSection(nameof(EmailSettings)));
        services.Configure<ServiceBusConfiguration>(configuration.GetSection(nameof(ServiceBusConfiguration)));
        services.Configure<TeamsWebhookConfiguration>(configuration.GetSection(nameof(TeamsWebhookConfiguration)));
        services.Configure<GeocodeGoogleConfiguration>(configuration.GetSection(nameof(GeocodeGoogleConfiguration)));
        services.Configure<PushNotificationConfiguration>(configuration.GetSection(nameof(PushNotificationConfiguration)));
        services.Configure<SendGridConfiguration>(configuration.GetSection(nameof(SendGridConfiguration)));
        services.Configure<Microsoft365Configuration>(configuration.GetSection(nameof(Microsoft365Configuration)));
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { new CultureInfo(EnUsCulture), new CultureInfo(EsCulture) };
            options.DefaultRequestCulture = new RequestCulture(EnUsCulture, EnUsCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
                await Task.FromResult(new ProviderCultureResult("en"))));
        });
        services.Configure<ApiBehaviorOptions>(opt =>
        {
            opt.SuppressConsumesConstraintForFormFileParameters = true;
            opt.SuppressInferBindingSourcesForParameters = true;
            opt.SuppressModelStateInvalidFilter = true;
        });
        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Microsoft365TokenHandler>();
        services.AddHttpClient(IdentityServerService.IdentityClient, c => c.BaseAddress = new Uri($"{configuration["AuthenticationOptions:Authority"]}/UserAdministration/"))
            .AddHttpMessageHandler<Microsoft365TokenHandler>();
        services.AddHttpClient(PdfGeneratorService.PdfGeneratorClient, c => c.BaseAddress = new Uri(configuration["PdfGeneratorUrl"]));
        return services;
    }

    public static IServiceCollection AddContainers(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var accountingStorageConnection = builder.Configuration.GetConnectionString("AccountingStorageConnection");
        var fileStorageConnection = builder.Configuration.GetConnectionString("FileStorageConnection");

        var accountingConnectionSafe = accountingStorageConnection ?? string.Empty;
        var fileConnectionSafe = fileStorageConnection ?? string.Empty;

        services.AddSingleton(new AzureStorageConfiguration(new[]
        {
            new AccessKey(InvoicesContainer.ContainerName, accountingConnectionSafe),
            new AccessKey(PayStubsContainer.ContainerName, accountingConnectionSafe),
            new AccessKey(FilesContainer.ContainerName, fileConnectionSafe),
            new AccessKey(CraTablesContainer.ContainerName, fileConnectionSafe)
        }, new AccessKey("default", accountingConnectionSafe)));
        services.AddScoped<IInvoicesContainer, InvoicesContainer>();
        services.AddScoped<IPayStubsContainer, PayStubsContainer>();
        services.AddScoped<IFilesContainer, FilesContainer>();
        services.AddScoped<ICraTablesContainer, CraTablesContainer>();
        return services;
    }

    public static IServiceCollection AddAzureServiceBusConsumer(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var serviceBusConnection = builder.Configuration.GetConnectionString("ServiceBusConnection");

        if (!string.IsNullOrEmpty(serviceBusConnection))
        {
            services.AddSingleton(sp => new SigookBusAdministrationClient(serviceBusConnection));
            services.AddSingleton(sp => new SigookBusClient(serviceBusConnection, sp.GetRequiredService<SigookBusAdministrationClient>(), sp.GetRequiredService<ILogger<SigookBusClient>>()));
            services.AddSingleton<ISigookBusClient>(sp => sp.GetRequiredService<SigookBusClient>());
            services.AddSingleton<IAzureServiceBusConsumer, NewCandidateConsumer>();
            services.AddSingleton<IAzureServiceBusConsumer, TeamsConsumer>();
            services.AddSingleton<IAzureServiceBusConsumer, RequestApplicantConsumer>();
            services.AddSingleton<IAzureServiceBusConsumer, BulkPayStubEmailConsumer>();
            services.AddSingleton<IAzureServiceBusConsumer, InvitationConsumer>();
        }

        return services;
    }

    public static IServiceCollection AddCovenantHealthChecks(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Get all connection strings
        var databaseConnection = configuration.GetConnectionString("DefaultConnection");
        var accountingStorageConnection = configuration.GetConnectionString("AccountingStorageConnection");
        var fileStorageConnection = configuration.GetConnectionString("FileStorageConnection");
        var serviceBusConnection = configuration.GetConnectionString("ServiceBusConnection");

        var healthChecksBuilder = services.AddHealthChecks();

        healthChecksBuilder.AddTypeActivatedCheck<DatabaseConfigurationHealthCheck>(
            "config-database",
            HealthStatus.Unhealthy,
            tags: ["config", "ready", "live"],
            args: [databaseConnection!]);

        healthChecksBuilder.AddTypeActivatedCheck<AzureStorageConfigurationHealthCheck>(
            "config-azure-storage",
            HealthStatus.Unhealthy,
            tags: ["config", "ready"],
            args: [accountingStorageConnection!, fileStorageConnection!]);

        healthChecksBuilder.AddTypeActivatedCheck<ServiceBusConfigurationHealthCheck>(
            "config-service-bus",
            HealthStatus.Unhealthy,
            tags: ["config", "ready"],
            args: [serviceBusConnection!]);

        if (!string.IsNullOrEmpty(databaseConnection))
        {
            healthChecksBuilder.AddDbContextCheck<CovenantContext>(
                "database-connectivity",
                HealthStatus.Unhealthy,
                tags: ["connectivity", "ready", "live"]);
        }

        healthChecksBuilder.AddCheck("azure-storage-accounting-connectivity",
            new AzureStorageHealthCheck(accountingStorageConnection, "Accounting"),
            HealthStatus.Unhealthy,
            tags: ["connectivity", "ready"]);

        healthChecksBuilder.AddCheck("azure-storage-files-connectivity",
            new AzureStorageHealthCheck(fileStorageConnection, "Files"),
            HealthStatus.Unhealthy,
            tags: ["connectivity", "ready"]);

        healthChecksBuilder.AddCheck("azure-service-bus-connectivity",
            new AzureServiceBusHealthCheck(serviceBusConnection),
            HealthStatus.Unhealthy,
            tags: ["connectivity", "ready"]);

        return services;
    }
}