using Asp.Versioning;
using Covenant.Api.Authorization;
using Covenant.Api.Configuration;
using Covenant.Api.Validators.Candidate;
using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Documents;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace Covenant.Integration.Tests.Configuration
{
    public static class TestStartupExtensions
    {
        public static void AddDefaultTestConfiguration(this IServiceCollection services)
        {
            services.AddRepositories().AddServices().AddAdapters();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressConsumesConstraintForFormFileParameters = true;
                opt.SuppressInferBindingSourcesForParameters = true;
                opt.SuppressModelStateInvalidFilter = true;
            });
            services.AddPolices();
            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddValidatorsFromAssemblyContaining<CandidateCsvModelValidator>();
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddSingleton(Mock.Of<IDocumentService>());
            services.AddSingleton(Mock.Of<IPushNotifications>());
            services.AddSingleton(Mock.Of<AzureStorageConfiguration>());
            services.AddSingleton(Mock.Of<Rates>());
            services.AddSingleton(Mock.Of<TimeLimits>());
            services.AddSingleton(Mock.Of<IInvoicesContainer>());
            services.AddSingleton(Mock.Of<IFilesContainer>());
            var mockEmailService = new Mock<IEmailService>();
            mockEmailService.Setup(es => es.SendEmail(It.IsAny<EmailParams>())).ReturnsAsync(true);
            services.AddSingleton(mockEmailService.Object);
            var mockSendGridService = new Mock<ISendGridService>();
            mockSendGridService.Setup(ss => ss.SendEmail(It.IsAny<SendGridModel>())).ReturnsAsync(Result.Ok());
            services.AddSingleton(mockSendGridService.Object);
            var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
            mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration
            {
                TemplatesReportsPath = "../../../../Covenant.Api/Report/Templates/",
                FilesUrl = Environment.CurrentDirectory,
                MaximumFileSize = 1000000
            });
            services.AddSingleton(mockFilesConfiguration.Object);
            var mockMicrosoft365Configuration = new Mock<IOptions<Microsoft365Configuration>>();
            mockMicrosoft365Configuration.Setup(m => m.Value).Returns(new Microsoft365Configuration
            {
                ClientId = "Test",
                ClientSecret = "Test",
                TenantId = "Test",
                Scope = "Test"
            });
            services.AddSingleton(mockMicrosoft365Configuration.Object);
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ServicesConfiguration).Assembly));
        }
    }
}