using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Services.Requests;

public class RequestInvitationService(
    IRequestRepository requestRepository,
    IWorkerRepository workerRepository,
    ICandidateRepository candidateRepository,
    TimeProvider timeProvider,
    ISendGridService sendGridService,
    ITeamsService teamsService,
    IOptions<SendGridConfiguration> sendGridOptions,
    IOptions<TeamsWebhookConfiguration> teamsWebhookOptions,
    ILogger<RequestInvitationService> logger) : IRequestInvitationService
{
    private readonly SendGridConfiguration sendGridConfiguration = sendGridOptions.Value;
    private readonly string webhook = teamsWebhookOptions.Value.CandidateAndWorker;

    public async Task Send(SendInvitationJob job)
    {
        try
        {
            var result = await SendInvitationEmails(job.RequestId);
            var notification = result
                ? TeamsNotificationModel.CreateSuccess(
                    $"Invitations sent by {job.Nickname}",
                    $"Sent {result.Value.WorkersSentCount} worker and {result.Value.CandidatesSentCount} candidate invitations for request #{result.Value.NumberId} ({result.Value.JobTitle})")
                : TeamsNotificationModel.CreateWarning(
                    $"Invitations sent by {job.Nickname}",
                    string.Join(" - ", result.Errors));
            await teamsService.SendNotification(webhook, notification);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send invitations for request {RequestId}", job.RequestId);
            var notification = TeamsNotificationModel.CreateError($"Invitations sent by {job.Nickname}", e.Message);
            await teamsService.SendNotification(webhook, notification);
        }
    }

    private async Task<Result<InvitationSentResult>> SendInvitationEmails(Guid requestId)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null || !request.CanBeUpdated) return Result.Fail<InvitationSentResult>(ApiResources.RequestNotAvailable);

        var now = timeProvider.GetLocalNow().DateTime;
        var canBeSent = request.CanInvitationBeSendIt(now);
        if (!canBeSent) return Result.Fail<InvitationSentResult>(canBeSent.Errors);

        var jobTitle = request.JobTitle;
        var description = request.Description;
        var requirements = request.Requirements;
        var city = request.JobLocation?.City?.Value;
        var rateValue = request.WorkerRate ?? request.WorkerSalary.Value;
        var rate = request.JobLocation.IsUSA ? rateValue.ToUsMoney() : rateValue.ToCaMoney();
        var agencyId = request.CompanyProfile.AgencyId;
        var cityId = request.JobLocation.CityId;

        var workers = await workerRepository.GetWorkersAvailableToInvite(agencyId, cityId);
        var recipients = new List<TemplateRecipient>(workers.Count);
        var invitedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var worker in workers)
        {
            if (!invitedEmails.Add(worker.Email)) continue;
            var unsubscribeUrl = BuildUnsubscribeUrl(sendGridConfiguration.UnsubscribeUrl, worker.Email);
            object data = new
            {
                worker_name = worker.FullName,
                unsubscribe = unsubscribeUrl,
                unsubscribe_preferences = unsubscribeUrl,
                job_title = jobTitle,
                description,
                requirements,
                rate,
                city,
                apply = BuildApplyUrl(sendGridConfiguration.ApplyOnlineUrl, request.NumberId, worker.Email),
            };
            recipients.Add(new TemplateRecipient(worker.Email, worker.FullName, data));
        }
        var workersSentCount = recipients.Count;

        var candidatesSentCount = 0;
        var candidates = await candidateRepository.GetCandidatesAvailableToInvite(agencyId, requestId);
        foreach (var candidate in candidates)
        {
            var sameCity = candidate.CityId.HasValue
                ? candidate.CityId == cityId
                : candidate.Address.ContainsNormalized(city);
            if (!sameCity) continue;
            if (!invitedEmails.Add(candidate.Email)) continue;
            var candidateUnsubscribeUrl = BuildUnsubscribeUrl(sendGridConfiguration.UnsubscribeUrl, candidate.Email);
            object data = new
            {
                worker_name = candidate.Name,
                unsubscribe = candidateUnsubscribeUrl,
                unsubscribe_preferences = candidateUnsubscribeUrl,
                job_title = jobTitle,
                description,
                requirements,
                rate,
                city,
                apply = BuildApplyUrl(sendGridConfiguration.ApplyOnlineUrl, request.NumberId, candidate.Email),
            };
            recipients.Add(new TemplateRecipient(candidate.Email, candidate.Name, data));
            candidatesSentCount++;
        }

        if (recipients.Count > 0)
        {
            await sendGridService.SendTemplateBatch(request.CompanyProfile.Agency.RecruitmentEmail, recipients);
            request.MarkInvitationSent(now);
            requestRepository.Update(request);
            await requestRepository.SaveChangesAsync();
        }
        return Result.Ok(new InvitationSentResult(workersSentCount, candidatesSentCount, request.NumberId, request.JobTitle));
    }

    private static string BuildUnsubscribeUrl(string template, string email) =>
        template.Replace("{{email}}", Uri.EscapeDataString(email));

    private static string BuildApplyUrl(string template, int numberId, string email) =>
        template
            .Replace("{{numberId}}", numberId.ToString())
            .Replace("{{email}}", Uri.EscapeDataString(email));
}
