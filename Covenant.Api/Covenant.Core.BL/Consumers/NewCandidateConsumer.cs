using Azure.Messaging.ServiceBus;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;

namespace Covenant.Core.BL.Consumers;

public class NewCandidateConsumer : IAzureServiceBusConsumer
{
    private readonly SigookBusClient client;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ServiceBusConfiguration serviceBusConfiguration;

    public NewCandidateConsumer(SigookBusClient client, IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusConfiguration> options)
    {
        this.client = client;
        this.serviceScopeFactory = serviceScopeFactory;
        serviceBusConfiguration = options.Value;
    }

    public async Task OnInit()
    {
        await client.CreateProcessorAsync(serviceBusConfiguration.ValidateCandidateQueue, ValidateCandidateAsync);
    }

    private async Task ValidateCandidateAsync(ProcessMessageEventArgs args)
    {

        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var candidateViewValidator = scope.ServiceProvider.GetService<IValidator<CandidateViewModel>>();
        var candidateRepository = scope.ServiceProvider.GetService<ICandidateRepository>();
        var workerRepository = scope.ServiceProvider.GetService<IWorkerRepository>();
        var requestRepository = scope.ServiceProvider.GetService<IRequestRepository>();
        var agencyRepository = scope.ServiceProvider.GetService<IAgencyRepository>();
        var candidateService = scope.ServiceProvider.GetService<ICandidateService>();
        var filesContainer = scope.ServiceProvider.GetService<IFilesContainer>();
        var message = args.Message.Body.ToObjectFromJson<CandidateViewModel>();
        var response = new ServiceBusMessage();
        var errorMessage = string.Empty;
        if (message is not null)
        {
            var candidateValidation = await candidateViewValidator.ValidateAsync(message);
            if (candidateValidation.IsValid)
            {
                var email = CvnEmail.Create(message.Email);
                if (email)
                {
                    var worker = await workerRepository.GetProfile(wp => wp.Worker.Email.ToLower() == email.Value.Email.ToLower());
                    var candidate = await candidateRepository.GetCandidate(c => c.Email.ToLower() == email.Value.Email.ToLower());
                    var request = await requestRepository.GetRequest(r => r.Id == message.RequestId);
                    var requestApplicant = default(Result<RequestApplicant>);
                    if (worker == null && candidate == null)
                    {
                        if (!string.IsNullOrWhiteSpace(message.FullName))
                        {
                            var agency = await agencyRepository.GetAgencyMasterByCountry(message.CountryId);
                            var agencyId = agency.Id;
                            var model = new CandidateCreateModel
                            {
                                Email = email.Value,
                                Name = message.FullName,
                                Address = message.Address,
                                HasVehicle = message.HasVehicle,
                                ResidencyStatus = message.Status,
                                Source = "Covenant|Sigook",
                                FileName = message.FileName,
                            };
                            if (!string.IsNullOrWhiteSpace(message.Phone))
                            {
                                model.PhoneNumbers = new List<PhoneNumberModel> { new PhoneNumberModel(message.Phone) };
                            }
                            if (request != null)
                            {
                                if (message.Skills == null)
                                {
                                    message.Skills = new List<string> { request.JobTitle };
                                }
                                else if (!message.Skills.Any(s => s.Equals(request.JobTitle, StringComparison.OrdinalIgnoreCase)))
                                {
                                    message.Skills.Add(request.JobTitle);
                                }
                                model.Skills = message.Skills.Select(s => new SkillModel(s));
                            }
                            else if (message.Skills?.Any() == true)
                            {
                                model.Skills = message.Skills.Select(s => new SkillModel(s));
                            }
                            var candidateId = await candidateService.CreateCandidate(model, agencyId, false);
                            if (candidateId && request != null)
                            {
                                if (request.JobLocation.City.Province.CountryId == message.CountryId)
                                {
                                    requestApplicant = RequestApplicant.CreateWithCandidate(message.RequestId.Value, candidateId.Value, "Sigook", string.Empty);
                                    response.Body = new BinaryData(requestApplicant.Value);
                                    response.ApplicationProperties[ServiceBusSqlConstants.RequestApplication] = true;
                                }
                                else
                                {
                                    errorMessage = $"The candidate with email {email.Value.Email} attempted to apply for order {request.NumberId}, but the country selected during registration does not match the one in the order.";
                                }
                            }
                            else
                            {
                                response.Body = new BinaryData(model);
                                response.ApplicationProperties[ServiceBusSqlConstants.RequestApplication] = false;
                            }
                        }
                        else
                        {
                            errorMessage = $"Candidate with email: {email.Value.Email} is not registered";
                        }
                    }
                    else if (request != null)
                    {
                        if (worker != null)
                        {
                            requestApplicant = RequestApplicant.CreateWithWorker(message.RequestId.Value, worker.Id, "Sigook", string.Empty);
                        }
                        else if (candidate != null)
                        {
                            requestApplicant = RequestApplicant.CreateWithCandidate(message.RequestId.Value, candidate.Id, "Sigook", string.Empty);
                        }
                        response.Body = new BinaryData(requestApplicant.Value);
                        response.ApplicationProperties[ServiceBusSqlConstants.RequestApplication] = true;
                    }
                    else
                    {
                        errorMessage = $"Candidate with email: {email.Value.Email} is already registered";
                    }
                }
            }
            else
            {
                var builder = new StringBuilder();
                builder.AppendLine($"Candidate Details: {message.ToString()}");
                builder.AppendLine($"Validation Errors: {string.Join(", ", candidateValidation.Errors.Select(e => e.ErrorMessage))}");
                errorMessage = builder.ToString();
            }
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                await filesContainer.DeleteFileIfExists(message.FileName);
                response.ApplicationProperties[ServiceBusSqlConstants.ErrorCandidate] = errorMessage;
            }
            await client.SendMessageAsync(response, serviceBusConfiguration.CreateApplicantTopic);
        }
        await args.CompleteMessageAsync(args.Message);
    }
}
