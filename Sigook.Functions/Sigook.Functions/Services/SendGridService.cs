using Covenant.Common.Models.Agency;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Sigook.Functions.Models;

namespace Sigook.Functions.Services;

public class SendGridService : IEmailService
{
    private static readonly EmailAddress ReplyToEmailAddress = new EmailAddress(Environment.GetEnvironmentVariable("ReplyToEmailAddress") ?? "it@covenantgroupl.com", "Sigook");
    private static readonly EmailAddress FromEmailAddress = new EmailAddress(Environment.GetEnvironmentVariable("FromEmailAddress") ?? "it@covenantgroupl.com", "Sigook");
    private static readonly EmailAddress TestingEmailAddress = new EmailAddress(Environment.GetEnvironmentVariable("TestingEmailAddress") ?? "it@covenantgroupl.com");
    private static readonly string SendGridApiKey = Environment.GetEnvironmentVariable("SendGridApiKey") ?? "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";
    private static readonly string TemplateId = Environment.GetEnvironmentVariable("NewJobTemplateId") ?? "d-4bf3a224275f4d6fb24f4f2a7e326daa";
    private static readonly SendGridClient SendGridClient = new SendGridClient(Environment.GetEnvironmentVariable("SendGridApiKey") ?? SendGridApiKey);
    private static readonly string SendGridSandBoxMode = Environment.GetEnvironmentVariable("SendGridSandBoxMode") ?? "true";
    private static readonly bool IsProduction = Environment.GetEnvironmentVariable("ENVIRONMENT") == "Production";

    private static readonly string UnsubscribeUrl = Environment.GetEnvironmentVariable("UnsubscribeUrl")
                                                    ?? "https://staging.web.sigook.ca/email-preferences?u=w&id={{workerId}}&t=10";

    private static readonly string ApplyOnlineUrl = Environment.GetEnvironmentVariable("ApplyOnlineUrl")
                                                    ?? "https://staging.web.sigook.ca/worker-apply?r={{requestId}}&w={{workerId}}";

    public async Task SendEmail(InvitationToApplyModel request, List<WorkerContactInfoModel> workers, ILogger logger)
    {
        var tos = new List<EmailAddress>(workers.Count);
        var objects = new List<object>(workers.Count);
        foreach (WorkerContactInfoModel worker in workers)
        {
            if (!worker.IsModelValid()) continue;
            tos.Add(IsProduction ? new EmailAddress(worker.Email, worker.FullName) : TestingEmailAddress);
            string uUrl = UnsubscribeUrl.Replace("{{workerId}}", worker.WorkerId);
            string applyUrl = ApplyOnlineUrl.Replace("{{requestId}}", request.RequestId.ToString())
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

        SendGridMessage msg = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(FromEmailAddress, tos, TemplateId, objects);
        msg.SetSandBoxMode(!SendGridSandBoxMode.Equals("false"));
        msg.SetReplyTo(ReplyToEmailAddress);
        Response emailResponse = await SendGridClient.SendEmailAsync(msg);
        if (!emailResponse.IsSuccessStatusCode)
        {
            logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
        }
    }

    public async Task SendEmail(EmailModel model, ILogger logger)
    {
        SendGridMessage msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(FromEmailAddress,
            model.Tos.Select(s => IsProduction ? new EmailAddress(s) : TestingEmailAddress).ToList(), GetTemplateId(model.Type), model.Data);
        msg.SetSandBoxMode(!SendGridSandBoxMode.Equals("false"));
        if (!string.IsNullOrEmpty(model.ReplyTo)) msg.SetReplyTo(new EmailAddress(model.ReplyTo));
        string bccEmailAddress = Environment.GetEnvironmentVariable("BCCEMailAddress");
        if (!string.IsNullOrEmpty(bccEmailAddress)) msg.AddBcc(new EmailAddress(bccEmailAddress));
        Response emailResponse = await SendGridClient.SendEmailAsync(msg);
        if (!emailResponse.IsSuccessStatusCode)
        {
            logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
        }
    }

    /// <summary>
    /// Returns the id of the template created on sendgrid
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static string GetTemplateId(string type)
    {
        return type switch
        {
            "NEW_APPLICANT" => "d-0ea177072a3146a2985cbd184e1f0b51",
            _ => "d-0ea177072a3146a2985cbd184e1f0b51"
        };
    }
}