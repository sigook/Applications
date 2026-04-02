using Covenant.Common.Models.Agency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Sigook.Functions.Models;

namespace Sigook.Functions.Services;

public class SendGridService : IEmailService
{
    private readonly EmailAddress _replyToEmailAddress;
    private readonly EmailAddress _fromEmailAddress;
    private readonly EmailAddress _testingEmailAddress;
    private readonly string _templateId;
    private readonly SendGridClient _sendGridClient;
    private readonly bool _sandBoxMode;
    private readonly bool _isProduction;
    private readonly string _unsubscribeUrl;
    private readonly string _applyOnlineUrl;
    private readonly IConfiguration _configuration;

    public SendGridService(IConfiguration configuration)
    {
        _configuration = configuration;
        _replyToEmailAddress = new EmailAddress(configuration["ReplyToEmailAddress"] ?? "it@covenantgroupl.com", "Sigook");
        _fromEmailAddress = new EmailAddress(configuration["FromEmailAddress"] ?? "it@covenantgroupl.com", "Sigook");
        _testingEmailAddress = new EmailAddress(configuration["TestingEmailAddress"] ?? "it@covenantgroupl.com");
        var sendGridApiKey = configuration["SendGridApiKey"] ?? "";
        _templateId = configuration["NewJobTemplateId"] ?? "d-4bf3a224275f4d6fb24f4f2a7e326daa";
        _sendGridClient = new SendGridClient(sendGridApiKey);
        _sandBoxMode = (configuration["SendGridSandBoxMode"] ?? "true") != "false";
        _isProduction = configuration["ENVIRONMENT"] == "Production";
        _unsubscribeUrl = configuration["UnsubscribeUrl"]
                          ?? "https://staging.web.sigook.ca/email-preferences?u=w&id={{workerId}}&t=10";
        _applyOnlineUrl = configuration["ApplyOnlineUrl"]
                          ?? "https://staging.web.sigook.ca/worker-apply?r={{requestId}}&w={{workerId}}";
    }

    public async Task SendEmail(InvitationToApplyModel request, List<WorkerContactInfoModel> workers, ILogger logger)
    {
        var tos = new List<EmailAddress>(workers.Count);
        var objects = new List<object>(workers.Count);
        foreach (WorkerContactInfoModel worker in workers)
        {
            if (!worker.IsModelValid()) continue;
            tos.Add(_isProduction ? new EmailAddress(worker.Email, worker.FullName) : _testingEmailAddress);
            string uUrl = _unsubscribeUrl.Replace("{{workerId}}", worker.WorkerId);
            string applyUrl = _applyOnlineUrl.Replace("{{requestId}}", request.RequestId.ToString())
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

        SendGridMessage msg = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(_fromEmailAddress, tos, _templateId, objects);
        msg.SetSandBoxMode(_sandBoxMode);
        msg.SetReplyTo(_replyToEmailAddress);
        Response emailResponse = await _sendGridClient.SendEmailAsync(msg);
        if (!emailResponse.IsSuccessStatusCode)
        {
            logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
        }
    }

    public async Task SendEmail(EmailModel model, ILogger logger)
    {
        SendGridMessage msg = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(_fromEmailAddress,
            model.Tos.Select(s => _isProduction ? new EmailAddress(s) : _testingEmailAddress).ToList(), GetTemplateId(model.Type), model.Data);
        msg.SetSandBoxMode(_sandBoxMode);
        if (!string.IsNullOrEmpty(model.ReplyTo)) msg.SetReplyTo(new EmailAddress(model.ReplyTo));
        string bccEmailAddress = _configuration["BCCEMailAddress"];
        if (!string.IsNullOrEmpty(bccEmailAddress)) msg.AddBcc(new EmailAddress(bccEmailAddress));
        Response emailResponse = await _sendGridClient.SendEmailAsync(msg);
        if (!emailResponse.IsSuccessStatusCode)
        {
            logger?.LogError("Error sending emails error: {Error}", await emailResponse.Body.ReadAsStringAsync());
        }
    }

    /// <summary>
    /// Returns the id of the template created on sendgrid
    /// </summary>
    private static string GetTemplateId(string type)
    {
        return type switch
        {
            "NEW_APPLICANT" => "d-0ea177072a3146a2985cbd184e1f0b51",
            _ => "d-0ea177072a3146a2985cbd184e1f0b51"
        };
    }
}
