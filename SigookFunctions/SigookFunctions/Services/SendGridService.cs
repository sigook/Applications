using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SigookFunctions.Models;
using SigookFunctions.Options;

namespace SigookFunctions.Services
{
    public class SendGridService : IEmailService
    {
        private readonly SigookFunctionsOptions _options;
        private readonly SendGridClient _sendGridClient;

        public SendGridService(IOptions<SigookFunctionsOptions> options)
        {
            _options = options.Value;
            var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey")
                         ?? _options.SendGrid.ApiKey;
            _sendGridClient = new SendGridClient(apiKey);
        }

        private EmailAddress ReplyToEmailAddress => new(_options.SendGrid.ReplyToEmail, "Sigook");
        private EmailAddress FromEmailAddress => new(_options.SendGrid.FromEmail, "Sigook");
        private EmailAddress TestingEmailAddress => new(_options.SendGrid.TestingEmail);
        private bool IsProduction => _options.IsProduction;

        public async Task SendEmail(InvitationToApplyModel request, List<WorkerContactInfoModel> workers, ILogger logger)
        {
            var tos = new List<EmailAddress>(workers.Count);
            var objects = new List<object>(workers.Count);

            foreach (WorkerContactInfoModel worker in workers)
            {
                if (!worker.IsModelValid()) continue;

                tos.Add(IsProduction ? new EmailAddress(worker.Email, worker.FullName) : TestingEmailAddress);

                string uUrl = _options.Urls.UnsubscribeUrl.Replace("{{workerId}}", worker.WorkerId);
                string applyUrl = _options.Urls.ApplyOnlineUrl
                    .Replace("{{requestId}}", request.RequestId.ToString())
                    .Replace("{{workerId}}", worker.WorkerId);

                objects.Add(new
                {
                    worker_name = worker.FullName,
                    unsubscribe = uUrl,
                    unsubscribe_preferences = uUrl,
                    job_title = request.JobTitle,
                    description = request.Description,
                    requirements = request.Requirements,
                    rate = request.Rate,
                    city = request.City,
                    apply = applyUrl
                });
            }

            SendGridMessage msg = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(
                FromEmailAddress, tos, _options.SendGrid.NewJobTemplateId, objects);

            msg.SetSandBoxMode(_options.SendGrid.SandBoxMode);
            msg.SetReplyTo(ReplyToEmailAddress);

            Response emailResponse = await _sendGridClient.SendEmailAsync(msg);

            if (!emailResponse.IsSuccessStatusCode)
            {
                logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
            }
        }

        public async Task SendEmail(EmailModel model, ILogger logger)
        {
            SendGridMessage msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(
                FromEmailAddress,
                model.Tos.Select(s => IsProduction ? new EmailAddress(s) : TestingEmailAddress).ToList(),
                GetTemplateId(model.Type),
                model.Data);

            msg.SetSandBoxMode(_options.SendGrid.SandBoxMode);

            if (!string.IsNullOrEmpty(model.ReplyTo))
                msg.SetReplyTo(new EmailAddress(model.ReplyTo));

            var bccEmail = Environment.GetEnvironmentVariable("BCCEMailAddress")
                           ?? _options.SendGrid.BCCEmail;

            if (!string.IsNullOrEmpty(bccEmail))
                msg.AddBcc(new EmailAddress(bccEmail));

            Response emailResponse = await _sendGridClient.SendEmailAsync(msg);

            if (!emailResponse.IsSuccessStatusCode)
            {
                logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
            }
        }

        private static string GetTemplateId(string type)
        {
            return type switch
            {
                "NEW_APPLICANT" => "d-0ea177072a3146a2985cbd184e1f0b51",
                _ => "d-0ea177072a3146a2985cbd184e1f0b51"
            };
        }
    }
}
