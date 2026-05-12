using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Covenant.Infrastructure.Services;

public class SendGridInvitationEmailService : IInvitationEmailService
{
    private const int SendGridRecipientsPerRequest = 1000;

    private readonly SendGridConfiguration configuration;
    private readonly ILogger<SendGridInvitationEmailService> logger;

    public SendGridInvitationEmailService(
        IOptions<SendGridConfiguration> options,
        ILogger<SendGridInvitationEmailService> logger)
    {
        configuration = options.Value;
        this.logger = logger;
    }

    public async Task SendInvitations(InvitationToApplyModel invitation, IReadOnlyCollection<WorkerContactInfoModel> workers)
    {
        var client = new SendGridClient(configuration.ApiKey);
        var fromEmail = new EmailAddress(configuration.From);
        var redirectToTesting = configuration.RedirectInvitationsToTestingEmail;

        var batches = redirectToTesting
            ? new[] { new[] { workers.First() } }
            : workers.Chunk(SendGridRecipientsPerRequest);

        foreach (var batch in batches)
        {
            var personalizations = BuildPersonalizations(invitation, batch);
            var tos = redirectToTesting
                ? [new EmailAddress(configuration.TestingEmailAddress, "Testing Recipient")]
                : batch.Select(w => new EmailAddress(w.Email, w.FullName)).ToList();

            var message = MailHelper.CreateMultipleTemplateEmailsToMultipleRecipients(
                fromEmail, tos, configuration.NewJobTemplateId, personalizations);
            message.SetSandBoxMode(configuration.SandBox);

            var response = await client.SendEmailAsync(message);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Body.ReadAsStringAsync();
                logger.LogError("Error sending invitation emails. RequestId={RequestId} Error={Error}", invitation.RequestId, error);
            }
        }
    }

    private List<object> BuildPersonalizations(InvitationToApplyModel invitation, IReadOnlyList<WorkerContactInfoModel> batch)
    {
        var personalizations = new List<object>(batch.Count);
        foreach (var worker in batch)
        {
            var workerId = worker.WorkerId.ToString();
            var unsubscribeUrl = configuration.UnsubscribeUrl.Replace("{{workerId}}", workerId);
            var applyUrl = configuration.ApplyOnlineUrl
                .Replace("{{requestId}}", invitation.RequestId.ToString())
                .Replace("{{workerId}}", workerId);

            personalizations.Add(new
            {
                worker_name = worker.FullName,
                unsubscribe = unsubscribeUrl,
                unsubscribe_preferences = unsubscribeUrl,
                job_title = invitation.JobTitle,
                description = invitation.Description,
                requirements = invitation.Requirements,
                rate = invitation.Rate,
                city = invitation.City,
                apply = applyUrl,
            });
        }
        return personalizations;
    }
}
