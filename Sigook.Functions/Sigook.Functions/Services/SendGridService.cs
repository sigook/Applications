using Covenant.Common.Models.Agency;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Sigook.Functions.Models;

namespace Sigook.Functions.Services;

public class SendGridService : IEmailService
{
    private readonly SendGridSettings sendGridSettings;
    private readonly SendGridClient _sendGridClient;
    private readonly IHostEnvironment hostEnvironment;

    public SendGridService(IOptions<SendGridSettings> options, IHostEnvironment hostEnvironment)
    {
        sendGridSettings = options.Value;
        _sendGridClient = new SendGridClient(sendGridSettings.SendGridApiKey);
        this.hostEnvironment = hostEnvironment;
    }

    public async Task SendEmail(InvitationToApplyModel request, List<WorkerContactInfoModel> workers, ILogger logger)
    {
        var tos = new List<EmailAddress>(workers.Count);
        var objects = new List<object>(workers.Count);
        foreach (var worker in workers)
        {
            if (!worker.IsModelValid()) continue;
            tos.Add(new EmailAddress(worker.Email, worker.FullName));
            var unsubscribeUrl = sendGridSettings.UnsubscribeUrl.Replace("{{workerId}}", worker.WorkerId);
            var applyUrl = sendGridSettings.ApplyOnlineUrl
                .Replace("{{requestId}}", request.RequestId.ToString())
                .Replace("{{workerId}}", worker.WorkerId);
            objects.Add(new
            {
                worker_name = worker.FullName,
                unsubscribe = unsubscribeUrl,
                unsubscribe_preferences = unsubscribeUrl,
                job_title = request.JobTitle,
                description = request.Description,
                requirements = request.Requirements,
                rate = request.Rate,
                city = request.City,
                apply = applyUrl
            });
        }
        if (hostEnvironment.IsProduction())
        {
            tos = [new EmailAddress(sendGridSettings.TestingEmailAddress, "Testing Recipient")];
            objects = objects[..1];
        }
        var fromEmail = new EmailAddress(sendGridSettings.FromEmailAddress);
        var msg = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(fromEmail, tos, sendGridSettings.NewJobTemplateId, objects);
        msg.SetSandBoxMode(!hostEnvironment.IsProduction());
        var emailResponse = await _sendGridClient.SendEmailAsync(msg);
        if (!emailResponse.IsSuccessStatusCode)
        {
            logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
        }
    }
}
