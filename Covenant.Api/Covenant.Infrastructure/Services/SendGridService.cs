using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Covenant.Infrastructure.Services;

public class SendGridService : ISendGridService
{
    private const int SendGridRecipientsPerRequest = 1000;

    private readonly SendGridConfiguration configuration;
    private readonly SendGridClient sendGridClient;
    private readonly ILogger<SendGridService> logger;

    public SendGridService(IOptions<SendGridConfiguration> options, ILogger<SendGridService> logger)
    {
        this.logger = logger;
        configuration = options.Value;
        sendGridClient = new SendGridClient(configuration.ApiKey);
    }

    public async Task<Result> SendEmail(SendGridModel model)
    {
        var templateId = configuration.NewApplicantTemplateId;
        var message = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(
            new EmailAddress(configuration.TestingEmailAddress),
            [.. model.Tos.Select(t => new EmailAddress(t))],
            templateId,
            model.Data);
        var response = await sendGridClient.SendEmailAsync(message);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Error sending emails error: {Error}", await response.Body.ReadAsStringAsync());
            return Result.Fail();
        }
        return Result.Ok();
    }

    public async Task<Result> SendTemplateBatch(string templateId, IReadOnlyCollection<TemplateRecipient> recipients)
    {
        var fromEmail = new EmailAddress(configuration.TestingEmailAddress);
        var redirectToTesting = configuration.RedirectInvitationsToTestingEmail;

        var batches = redirectToTesting
            ? [[recipients.First()]]
            : recipients.Chunk(SendGridRecipientsPerRequest);

        foreach (var batch in batches)
        {
            var personalizations = batch.Select(r => r.Data).ToList();
            var tos = redirectToTesting
                ? [new(configuration.TestingEmailAddress, "Testing Recipient")]
                : batch.Select(r => new EmailAddress(r.Email, r.FullName)).ToList();

            var message = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(fromEmail, tos, templateId, personalizations);
            var response = await sendGridClient.SendEmailAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Body.ReadAsStringAsync();
                logger.LogError("Error sending template batch. TemplateId={TemplateId} Error={Error}", templateId, error);
            }
        }
        return Result.Ok();
    }
}
