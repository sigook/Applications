using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Website;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Candidates;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Text;

namespace Covenant.Core.BL.Services.Candidates;

public class CandidateApplicationService(
    IValidator<CandidateViewModel> candidateValidator,
    ICandidateRepository candidateRepository,
    IWorkerRepository workerRepository,
    IRequestRepository requestRepository,
    IAgencyRepository agencyRepository,
    ICatalogRepository catalogRepository,
    ICandidateService candidateService,
    IFilesContainer filesContainer,
    IMessagePublisher messagePublisher,
    ITeamsService teamsService,
    IOptions<ServiceBusConfiguration> serviceBusOptions,
    IOptions<TeamsWebhookConfiguration> teamsWebhookOptions) : ICandidateApplicationService
{
    private const string Source = "Sigook";
    private readonly ServiceBusConfiguration serviceBusConfiguration = serviceBusOptions.Value;
    private readonly TeamsWebhookConfiguration teamsWebhookConfiguration = teamsWebhookOptions.Value;

    public async Task ProcessApplication(CandidateViewModel message)
    {
        var outcome = await EvaluateApplication(message);
        var properties = new Dictionary<string, object>();
        if (outcome.RequestApplication.HasValue)
        {
            properties[ServiceBusSqlConstants.RequestApplication] = outcome.RequestApplication.Value;
        }
        if (!string.IsNullOrWhiteSpace(outcome.Error))
        {
            await filesContainer.DeleteFileIfExists(message.FileName);
            properties[ServiceBusSqlConstants.ErrorCandidate] = outcome.Error;
        }
        await messagePublisher.SendMessageAsync(outcome.Body, serviceBusConfiguration.CreateApplicantTopic, properties);
    }

    public async Task RegisterApplicant(RequestApplicant applicant)
    {
        var existing = await requestRepository.GetRequestApplicant(ra => ra.RequestId == applicant.RequestId && ra.WorkerProfileId == applicant.WorkerProfileId && ra.CandidateId == applicant.CandidateId);
        if (existing is not null) return;
        await requestRepository.Create(applicant);
        await requestRepository.SaveChangesAsync();
    }

    public Task NotifyFailure(string error) =>
        NotifyTeams(TeamsNotificationModel.CreateError("Candidate application", error));

    public async Task NotifyApplication(RequestApplicant applicant)
    {
        var candidate = await candidateRepository.GetCandidate(c => c.Id == applicant.CandidateId.Value);
        var request = await requestRepository.GetRequest(r => r.Id == applicant.RequestId);
        await NotifyTeams(TeamsNotificationModel.CreateSuccess("Candidate application", $"Candidate {candidate.Name} applied to request number {request.NumberId}"));
    }

    public Task NotifyCreation(CandidateCreateModel model) =>
        NotifyTeams(TeamsNotificationModel.CreateSuccess("Candidate creation", $"Candidate {model.Name} has been created"));

    private Task NotifyTeams(TeamsNotificationModel notification) =>
        teamsService.SendNotification(teamsWebhookConfiguration.CandidateAndWorker, notification);

    private async Task<ApplicationOutcome> EvaluateApplication(CandidateViewModel message)
    {
        var validation = await candidateValidator.ValidateAsync(message);
        if (!validation.IsValid)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Candidate Details: {message}");
            builder.AppendLine($"Validation Errors: {string.Join(", ", validation.Errors.Select(e => e.ErrorMessage))}");
            return ApplicationOutcome.Failed(builder.ToString());
        }

        var email = CvnEmail.Create(message.Email);
        if (!email) return ApplicationOutcome.Failed(string.Join(", ", email.Errors.Select(e => e.Message)));

        var normalizedEmail = email.Value.Email.ToLower();
        var worker = await workerRepository.GetProfile(wp => wp.Worker.Email.ToLower() == normalizedEmail);
        var candidate = await candidateRepository.GetCandidate(c => c.Email.ToLower() == normalizedEmail);
        var request = await requestRepository.GetRequest(r => r.Id == message.RequestId);

        if (worker is null && candidate is null) return await CreateCandidate(message, email.Value, request);
        if (request is null) return ApplicationOutcome.Failed($"Candidate with email: {email.Value.Email} is already registered");

        var applicant = worker is not null
            ? RequestApplicant.CreateWithWorker(message.RequestId.Value, worker.Id, Source, string.Empty, RequestApplicantStatus.Pending)
            : RequestApplicant.CreateWithCandidate(message.RequestId.Value, candidate.Id, Source, string.Empty, RequestApplicantStatus.Pending);
        return ApplicationOutcome.Application(applicant.Value);
    }

    private async Task<ApplicationOutcome> CreateCandidate(CandidateViewModel message, CvnEmail email, Request request)
    {
        if (string.IsNullOrWhiteSpace(message.FullName)) return ApplicationOutcome.Failed($"Candidate with email: {email.Email} is not registered");

        var city = await catalogRepository.GetCity(message.CityId.Value);
        var agency = city is null ? null : await agencyRepository.GetAgencyMasterByCountry(city.Province.Country.Id);
        if (agency is null) return ApplicationOutcome.Failed($"Candidate with email: {email.Email} selected a city ({message.CityId}) without a master agency");

        var model = new CandidateCreateModel
        {
            Email = email,
            Name = message.FullName,
            Address = message.Address,
            City = city,
            HasVehicle = message.HasVehicle,
            ResidencyStatus = message.Status,
            SourceId = message.SourceId,
            FileName = message.FileName,
        };
        if (!string.IsNullOrWhiteSpace(message.Phone))
        {
            model.PhoneNumbers = [new PhoneNumberModel(message.Phone)];
        }
        var skills = message.Skills?.ToList() ?? [];
        if (request is not null && !skills.Any(s => s.Equals(request.JobTitle, StringComparison.OrdinalIgnoreCase)))
        {
            skills.Add(request.JobTitle);
        }
        if (skills.Count > 0)
        {
            model.Skills = skills.Select(s => new SkillModel(s));
        }

        var candidateId = await candidateService.CreateCandidate(model, agency.Id, false);
        if (!candidateId || request is null) return ApplicationOutcome.Creation(model);
        if (request.JobLocation.City.Province.CountryId != city.Province.Country.Id)
        {
            return ApplicationOutcome.Failed($"The candidate with email {email.Email} attempted to apply for request {request.NumberId}, but the country selected during registration does not match the one in the request.");
        }
        var applicant = RequestApplicant.CreateWithCandidate(message.RequestId.Value, candidateId.Value, Source, string.Empty, RequestApplicantStatus.Pending);
        return ApplicationOutcome.Application(applicant.Value);
    }

    private sealed record ApplicationOutcome(object Body, bool? RequestApplication, string Error)
    {
        public static ApplicationOutcome Failed(string error) => new(null, null, error);
        public static ApplicationOutcome Application(RequestApplicant applicant) => new(applicant, true, null);
        public static ApplicationOutcome Creation(CandidateCreateModel model) => new(model, false, null);
    }
}
