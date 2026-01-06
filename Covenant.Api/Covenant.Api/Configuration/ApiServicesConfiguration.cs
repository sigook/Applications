using Covenant.Api.AccountingModule.InvoiceDocument.Controllers;
using Covenant.Api.AccountingModule.PayStubDocument.Controllers;
using Covenant.Api.Authorization;
using Covenant.Api.HealthChecks;
using Covenant.Api.Utils;
using Covenant.Billing.Services;
using Covenant.Billing.Services.Impl;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
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
using Covenant.Core.BL.Services.Invoices;
using Covenant.Deductions.Repositories;
using Covenant.Deductions.Services;
using Covenant.Infrastructure.Deductions;
using Covenant.Infrastructure.Deductions.Repositories;
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
using Covenant.PayStubs.Services;
using Covenant.PayStubs.Services.Impl;
using Covenant.Subcontractor.Services;
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
        services.AddScoped<IWorkerCommentsRepository, WorkerCommentsRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IRequestRepository, RequestRepository>();
        services.AddScoped<ITimeSheetRepository, TimeSheetRepository>();
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
        services.AddScoped<ICandidateService, CandidateService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ITimesheetService, TimesheetService>();
        services.AddScoped<IPayStubService, PayStubService>();
        services.AddScoped<IDocumentService, DocumentService>();
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
        services.AddScoped<CaptchaFilter>();

        // Invoice services with Strategy pattern
        services.AddScoped<UsaInvoiceService>();
        services.AddScoped<CanadaInvoiceService>();

        //TODO: To Refactor
        services.AddScoped<IDefaultLogoProvider, DefaultLogoProvider>();
        services.AddScoped<IPayrollDeductionsAndContributionsCalculator, PayrollDeductionsAndContributionsCalculator>();
        services.AddScoped<ICreateInvoiceUsingTimeSheet, CreateInvoiceUsingTimeSheet>();
        services.AddScoped<ICreateInvoiceWithoutTimeSheet, CreateInvoiceWithoutTimeSheet>();
        services.AddScoped<CreateInvoiceUSA>();
        services.AddScoped<IPayStubPublicHolidays, PayStubPublicHolidays>();
        services.AddScoped<ISubContractorPublicHolidays, SubContractorPublicHolidays>();
        services.AddScoped<CreatePayStubWithOutTimeSheet>();
        services.AddScoped<CreatePayStubUsingTimeSheet>();
        services.AddScoped<CreateReportSubcontractorUsingTimeSheet>();
        services.AddScoped<AccountingCreateInvoiceAndReportsSubcontractor>();
        services.AddScoped<ICppTablesLoader, CppTablesLoader>();
        services.AddScoped<FederalTaxTablesLoader>();
        services.AddScoped<ProvincialTaxTablesLoader>();
        services.AddScoped<PayStubPdf>();
        services.AddScoped<InvoicePdf>();
        return services;
    }

    public static IServiceCollection AddAdapters(this IServiceCollection services)
    {
        services.AddScoped<ICandidateAdapter, CandidateAdapter>();
        services.AddScoped<ICompanyAdapter, CompanyAdapter>();
        services.AddScoped<ICompanyAdapter, CompanyAdapter>();
        services.AddScoped<IWorkerAdapter, WorkerAdapter>();
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
        if (!builder.Environment.IsDevelopment())
        {
            accountingStorageConnection = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_AccountingStorageConnection");
            fileStorageConnection = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_FileStorageConnection");
        }
        services.AddSingleton(new AzureStorageConfiguration(new[]
        {
            new AccessKey(InvoicesContainer.ContainerName, accountingStorageConnection),
            new AccessKey(PayStubsContainer.ContainerName, accountingStorageConnection),
            new AccessKey(FilesContainer.ContainerName, fileStorageConnection)
        }, new AccessKey("default", accountingStorageConnection)));
        services.AddScoped<IInvoicesContainer, InvoicesContainer>();
        services.AddScoped<IPayStubsContainer, PayStubsContainer>();
        services.AddScoped<IFilesContainer, FilesContainer>();
        return services;
    }

    public static IServiceCollection AddAzureServiceBusConsumer(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var serviceBusConnection = builder.Configuration.GetConnectionString("ServiceBusConnection");
        if (!builder.Environment.IsDevelopment())
        {
            serviceBusConnection = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_ServiceBusConnection");
        }
        services.AddSingleton(sp => new SigookBusAdministrationClient(serviceBusConnection));
        services.AddSingleton(sp => new SigookBusClient(serviceBusConnection, sp.GetRequiredService<SigookBusAdministrationClient>(), sp.GetRequiredService<ILogger<SigookBusClient>>()));
        services.AddSingleton<IAzureServiceBusConsumer, NewCandidateConsumer>();
        services.AddSingleton<IAzureServiceBusConsumer, TeamsConsumer>();
        services.AddSingleton<IAzureServiceBusConsumer, RequestApplicantConsumer>();
        return services;
    }

    public static void AddCovenantHealthChecks(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var healthChecksBuilder = services.AddHealthChecks()
            .AddDbContextCheck<Infrastructure.Context.CovenantContext>("database", HealthStatus.Unhealthy, tags: new[] { "ready", "live" });

        var accountingStorageConnection = configuration.GetConnectionString("AccountingStorageConnection");
        var fileStorageConnection = configuration.GetConnectionString("FileStorageConnection");
        var serviceBusConnection = configuration.GetConnectionString("ServiceBusConnection");

        if (!environment.IsDevelopment())
        {
            accountingStorageConnection = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_AccountingStorageConnection");
            fileStorageConnection = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_FileStorageConnection");
            serviceBusConnection = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_ServiceBusConnection");
        }

        if (!string.IsNullOrEmpty(accountingStorageConnection))
        {
            healthChecksBuilder.AddCheck("azure-storage-accounting",
                new AzureStorageHealthCheck(accountingStorageConnection, "Accounting"),
                HealthStatus.Degraded, tags: new[] { "ready" });
        }

        if (!string.IsNullOrEmpty(fileStorageConnection))
        {
            healthChecksBuilder.AddCheck("azure-storage-files",
                new AzureStorageHealthCheck(fileStorageConnection, "Files"),
                HealthStatus.Degraded, tags: new[] { "ready" });
        }

        if (!string.IsNullOrEmpty(serviceBusConnection))
        {
            healthChecksBuilder.AddCheck("azure-service-bus",
                new AzureServiceBusHealthCheck(serviceBusConnection),
                HealthStatus.Degraded, tags: new[] { "ready" });
        }
    }
}