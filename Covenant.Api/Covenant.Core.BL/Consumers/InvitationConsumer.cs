using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Notification;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Consumers;

public class InvitationConsumer : IAzureServiceBusConsumer
{
    private readonly ISigookBusClient client;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ServiceBusConfiguration serviceBusConfiguration;
    private readonly ILogger<InvitationConsumer> logger;

    public InvitationConsumer(
        ISigookBusClient client,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<ServiceBusConfiguration> options,
        ILogger<InvitationConsumer> logger)
    {
        this.client = client;
        this.serviceScopeFactory = serviceScopeFactory;
        serviceBusConfiguration = options.Value;
        this.logger = logger;
    }

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.InvitationQueue, ProcessAsync);
    }

    private async Task ProcessAsync(ProcessMessageEventArgs args)
    {
        var job = args.Message.Body.ToObjectFromJson<SendInvitationJob>();
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var teamsService = scope.ServiceProvider.GetRequiredService<ITeamsService>();
        var webhook = scope.ServiceProvider.GetRequiredService<IOptions<TeamsWebhookConfiguration>>().Value.CandidateAndWorker;
        try
        {
            var result = await SendInvitationEmails(scope.ServiceProvider, job.RequestId);
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
        await args.CompleteMessageAsync(args.Message);
    }

    private static async Task<Result<InvitationSentResult>> SendInvitationEmails(IServiceProvider serviceProvider, Guid requestId)
    {
        var requestRepository = serviceProvider.GetRequiredService<IRequestRepository>();
        var workerRepository = serviceProvider.GetRequiredService<IWorkerRepository>();
        var candidateRepository = serviceProvider.GetRequiredService<ICandidateRepository>();
        var timeService = serviceProvider.GetRequiredService<ITimeService>();
        var sendGridService = serviceProvider.GetRequiredService<ISendGridService>();
        var sendGridConfiguration = serviceProvider.GetRequiredService<IOptions<SendGridConfiguration>>().Value;

        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null || !request.CanBeUpdated) return Result.Fail<InvitationSentResult>(ApiResources.RequestNotAvailable);

        var now = timeService.GetCurrentDateTime();
        var canBeSent = request.CanInvitationBeSendIt(now);
        if (!canBeSent) return Result.Fail<InvitationSentResult>(canBeSent.Errors);

        var jobTitle = request.JobTitle;
        var description = request.Description;
        var requirements = request.Requirements;
        var city = request.JobLocation?.City?.Value;
        var rateValue = request.WorkerRate ?? request.WorkerSalary.Value;
        var rate = request.JobLocation.IsUSA ? rateValue.ToUsMoney() : rateValue.ToCaMoney();
        var agencyId = request.CompanyProfile.AgencyId;
        var provinceId = request.JobLocation.City.ProvinceId;

        var workers = await workerRepository.GetWorkersAvailableToInvite(agencyId, provinceId);
        var recipients = new List<TemplateRecipient>(workers.Count);
        var invitedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var worker in workers)
        {
            if (!invitedEmails.Add(worker.Email)) continue;
            var unsubscribeUrl = sendGridConfiguration.UnsubscribeUrl.Replace("{{workerId}}", worker.WorkerId.ToString());
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
            if (!candidate.Address.ContainsNormalized(city)) continue;
            if (!invitedEmails.Add(candidate.Email)) continue;
            var candidateUnsubscribeUrl = sendGridConfiguration.CandidateUnsubscribeUrl.Replace("{{candidateId}}", candidate.Id.ToString());
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
            request.InvitationSentItAt = now;
            await requestRepository.Update(request);
            await requestRepository.SaveChangesAsync();
        }
        return Result.Ok(new InvitationSentResult(workersSentCount, candidatesSentCount, request.NumberId, request.JobTitle));
    }

    private static string BuildApplyUrl(string template, int numberId, string email) =>
        template
            .Replace("{{numberId}}", numberId.ToString())
            .Replace("{{email}}", Uri.EscapeDataString(email));
}
