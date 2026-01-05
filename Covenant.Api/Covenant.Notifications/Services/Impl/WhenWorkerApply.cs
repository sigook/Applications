using Covenant.Common.Args;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Notification;
using Covenant.Notifications.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Covenant.Notifications.Services.Impl
{
    public class WhenWorkerApply : IWhenWorkerApply
    {
        private readonly ILogger<WhenWorkerApply> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WhenWorkerApply(
            ILogger<WhenWorkerApply> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task NotifyAgency(object sender, WorkerChangeStateEventArgs args)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var renderer = serviceProvider.GetRequiredService<IRazorViewToStringRenderer>();
                var repository = serviceProvider.GetRequiredService<INotificationDataRepository>();
                var emailService = serviceProvider.GetRequiredService<IEmailService>();

                var data = await repository.GetAgencyData(args.RequestId, args.WorkerId, NotificationType.WorkerApplyNotifyAgency.Id);
                if (data is null || !data.EmailNotification) return;
                string agencyTemplate = await renderer.RenderViewToStringAsync("/Views/Notifications/OnWorkerApply/AgencyTemplate.cshtml",
                    new AgencyTemplateViewModel { JobTitle = data.JobTitle, WorkerFullName = data.WorkerFullName, CompanyFullName = data.CompanyFullName });
                await emailService.SendEmail(new EmailParams(data.AgencyEmail, $"Worker Applied Request {data.JobTitle}", agencyTemplate));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        public async Task NotifyCompany(object sender, WorkerChangeStateEventArgs args)
        {
            try
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                IServiceProvider serviceProvider = scope.ServiceProvider;
                var renderer = serviceProvider.GetRequiredService<IRazorViewToStringRenderer>();
                var repository = serviceProvider.GetRequiredService<INotificationDataRepository>();
                var emailService = serviceProvider.GetRequiredService<IEmailService>();

                var data = await repository.GetCompanyData(args.RequestId, args.WorkerId, NotificationType.WorkerApplyNotifyCompany.Id);
                if (data is null || !data.EmailNotification) return;
                string companyTemplate = await renderer.RenderViewToStringAsync("/Views/Notifications/OnWorkerApply/CompanyTemplate.cshtml",
                    new CompanyTemplateViewModel { JobTitle = data.JobTitle, WorkerFullName = data.WorkerFullName, AgencyFullName = data.AgencyFullName });
                await emailService.SendEmail(new EmailParams(data.CompanyEmail, $"Worker Applied Request {data.JobTitle}", companyTemplate));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}