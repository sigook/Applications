using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Notification;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace Covenant.Core.BL.Services;

public class RequestService : IRequestService
{
    private readonly ICompanyRepository companyRepository;
    private readonly ILocationRepository locationRepository;
    private readonly ITimeService timeService;
    private readonly IRequestRepository requestRepository;
    private readonly INotificationDataRepository notificationDataRepository;
    private readonly IPushNotifications pushNotifications;
    private readonly IIdentityServerService identityServerService;
    private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
    private readonly IEmailService emailService;
    private readonly ISigookBusClient busClient;
    private readonly ServiceBusConfiguration serviceBusConfiguration;
    private readonly ILogger<RequestService> logger;

    public RequestService(
        ICompanyRepository companyRepository,
        ILocationRepository locationRepository,
        ITimeService timeService,
        IRequestRepository requestRepository,
        INotificationDataRepository notificationDataRepository,
        IPushNotifications pushNotifications,
        IIdentityServerService identityServerService,
        IRazorViewToStringRenderer razorViewToStringRenderer,
        IEmailService emailService,
        ISigookBusClient busClient,
        IOptions<ServiceBusConfiguration> serviceBusOptions,
        ILogger<RequestService> logger)
    {
        this.companyRepository = companyRepository;
        this.locationRepository = locationRepository;
        this.timeService = timeService;
        this.requestRepository = requestRepository;
        this.notificationDataRepository = notificationDataRepository;
        this.pushNotifications = pushNotifications;
        this.identityServerService = identityServerService;
        this.razorViewToStringRenderer = razorViewToStringRenderer;
        this.emailService = emailService;
        this.busClient = busClient;
        serviceBusConfiguration = serviceBusOptions.Value;
        this.logger = logger;
    }

    public async Task<Result<Guid>> CreateRequest(RequestCreateModel model, Guid? companyId = null)
    {
        if (!companyId.HasValue)
            companyId = await companyRepository.GetCompanyId(model.CompanyProfileId);
        if (model.AgencyId == Guid.Empty)
            model.AgencyId = identityServerService.GetAgencyId();
        if (identityServerService.IsSales())
            model.SalesRepresentativeId = identityServerService.GetAgencyPersonnelId();
        var rRequest = await MapRequest(model, companyId.Value);
        if (!rRequest) return Result.Fail<Guid>(rRequest.Errors);
        var request = rRequest.Value;
        await requestRepository.Create(request);
        if (model.SalesRepresentativeId.HasValue)
        {
            var requestComission = new RequestComission
            {
                AgencyPersonnelId = model.SalesRepresentativeId.Value,
                RequestId = request.Id,
            };
            await requestRepository.Create(requestComission);
        }
        if (model.CompanyUserIds != null && model.CompanyUserIds.Any())
        {
            foreach (var companyUserId in model.CompanyUserIds)
            {
                var requestCompanyUser = new RequestCompanyUser
                {
                    CompanyUserId = companyUserId,
                    RequestId = request.Id
                };
                await requestRepository.Create(requestCompanyUser);
            }
        }
        await requestRepository.SaveChangesAsync();
        var location = await locationRepository.GetLocationById(model.LocationId.Value);
        var currency = location.IsUSA ? "USD" : "CAD";
        var salary = request.WorkerRate.HasValue ? $"{request.WorkerRate}/h" : $"{request.WorkerSalary} anually";
        var notificationModel = NotificationModel.NewRequestNotification("Job Alert", $"{request.JobTitle} {currency} ${salary}", request.Id);
        await pushNotifications.SendNotification(notificationModel);
        return Result.Ok(request.Id);
    }

    public async Task<Result<Guid>> CompanyCreateRequest(RequestCreateModel model)
    {
        var companyId = identityServerService.GetCompanyId();
        var companyProfile = await companyRepository.GetCompanyProfile(cp => cp.CompanyId == companyId);
        if (model.AnotherLocation?.City != null)
        {
            var location = Location.Create(model.AnotherLocation.City.Id, model.AnotherLocation.Address, model.AnotherLocation.PostalCode, model.AnotherLocation.Entrance, model.AnotherLocation.MainIntersection);
            if (!location) return Result.Fail<Guid>(location.Errors);
            var entity = new CompanyProfileLocation(companyProfile.Id, location.Value);
            await companyRepository.Create(entity);
            model.LocationId = location.Value.Id;
        }
        model.AgencyId = companyProfile.AgencyId;
        var requestId = await CreateRequest(model, companyId);
        var data = await notificationDataRepository.GetAgencyData(requestId.Value, NotificationType.NewRequest.Id);
        if (data != null && data.EmailNotification)
        {
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnNewRequest/AgencyTemplate.cshtml", data);
            await emailService.SendEmail(new EmailParams(data.AgencyEmail, $"New Request {data.JobTitle}", message));
        }
        return requestId;
    }

    public async Task<Result> OpenRequest(Guid requestId, string finalizedBy)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        var now = timeService.GetCurrentDateTime();
        Result result = request.Open(now);
        if (!result) return result;

        var note = CovenantNote.Create("Request open", CovenantNote.RedColor, finalizedBy).Value;
        await requestRepository.Create(new RequestNote(requestId, note));
        var requestFinalizationDetail = await requestRepository.GetRequestFinalizationDetail(requestId);
        if (requestFinalizationDetail != null)
        {
            requestRepository.Delete(requestFinalizationDetail);
        }
        var requestCancellationDetail = await requestRepository.GetRequestCancellationDetail(requestId);
        if (requestCancellationDetail != null)
        {
            requestRepository.Delete(requestCancellationDetail);
        }
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateRequest(Guid requestId, RequestCreateModel model)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null)
        {
            return Result.Fail("Request not found");
        }
        var description = RequestDescriptionModel.Create(model.Description);
        if (!description) return description;
        var requirements = RequestRequirementsModel.Create(model.Requirements);
        if (!requirements) return requirements;
        if (model.WorkerSalary.HasValue)
        {
            request.WorkerSalary = model.WorkerSalary.Value;
            request.AgencyRate = null;
            request.WorkerRate = null;
            request.JobPositionRateId = null;
        }
        else
        {
            request.WorkerSalary = null;
            var positionRate = await companyRepository.GetJobPosition(model.JobPositionRateId.Value);
            request.AgencyRate = positionRate.Rate;
            request.WorkerRate = positionRate.WorkerRate;
            request.JobPositionRateId = positionRate.Id;
        }
        request.UpdateJobTitle(model.JobTitle);
        request.UpdateBillingTitle(model.BillingTitle);
        var location = await locationRepository.GetLocationById(model.LocationId.Value);
        request.UpdateJobLocation(location, false);
        request.UpdateDescription(description.Value.Value);
        request.UpdateRequirements(requirements.Value.Value);
        request.InternalRequirements = model.InternalRequirements;
        request.Responsibilities = model.Responsibilities;
        request.UpdateIncentive(model.Incentive, model.IncentiveDescription);
        request.UpdateDurationBreak(model.DurationBreak);
        request.BreakIsPaid = model.BreakIsPaid;
        request.WorkersQuantity = model.WorkersQuantity;
        request.UpdatePunchCardVisibilityStatusInApp(model.PunchCardOptionEnabled);
        request.DurationTerm = model.DurationTerm;
        request.EmploymentType = model.EmploymentType;
        request.StartAt = model.StartAt;
        request.FinishAt = model.FinishAt;
        if (model.SalesRepresentativeId.HasValue)
        {
            var requestComission = await requestRepository.GetRequestComission(request.Id);
            if (requestComission == null)
            {
                requestComission = new RequestComission
                {
                    RequestId = request.Id,
                    AgencyPersonnelId = model.SalesRepresentativeId.Value
                };
                await requestRepository.Create(requestComission);
            }
            else if (requestComission.AgencyPersonnelId != model.SalesRepresentativeId.Value)
            {
                requestComission.AgencyPersonnelId = model.SalesRepresentativeId.Value;
            }
        }
        var requestCompanyUsers = await requestRepository.GetRequestCompanyUsers(request.Id);
        foreach (var user in requestCompanyUsers)
        {
            requestRepository.Delete(user);
        }
        if (model.CompanyUserIds != null && model.CompanyUserIds.Any())
        {
            foreach (var companyUserId in model.CompanyUserIds)
            {
                var requestCompanyUser = new RequestCompanyUser
                {
                    CompanyUserId = companyUserId,
                    RequestId = request.Id
                };
                await requestRepository.Create(requestCompanyUser);
            }
        }
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateRequirements(Guid id, RequestUpdateRequirementsModel model)
    {
        var requirements = RequestRequirementsModel.Create(model.Requirements);
        if (!requirements) return requirements;
        var request = await requestRepository.GetRequest(r => r.Id == id);
        var update = request.UpdateRequirements(requirements.Value.Value);
        if (!update) return update;
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> UpdateIsAsap(Guid id)
    {
        var request = await requestRepository.GetRequest(r => r.Id == id);
        var update = request.UpdateIsAsap();
        if (!update) return update;
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> PunchCardUpdateVisibilityStatusInApp(Guid requestId)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        var update = request.UpdatePunchCardVisibilityStatusInApp();
        if (!update) return update;
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> CancelRequest(Guid requestId, RequestCancellationDetailModel reason)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        var rCancel = await ApplyCancellation(request, reason.CancellationReasonId, reason.OtherCancellationReason);
        if (!rCancel) return rCancel;
        await requestRepository.SaveChangesAsync();
        await SendCancellationNotification(requestId);
        return Result.Ok();
    }

    public async Task<Result<BulkRequestCancellationResult>> BulkCancelRequests(BulkRequestCancellation model)
    {
        if (model?.Ids == null || !model.Ids.Any())
        {
            return Result.Ok(new BulkRequestCancellationResult { Cancelled = 0, Skipped = 0 });
        }
        var requests = await requestRepository.GetRequests(model.Ids);
        var notificationsToSend = new List<Guid>();
        var cancelled = 0;
        var skipped = 0;
        foreach (var request in requests)
        {
            var rCancel = await ApplyCancellation(request, model.CancellationReasonId, model.OtherCancellationReason);
            if (!rCancel)
            {
                skipped++;
                continue;
            }
            notificationsToSend.Add(request.Id);
            cancelled++;
        }
        skipped += model.Ids.Count() - requests.Count();
        await requestRepository.SaveChangesAsync();
        foreach (var requestId in notificationsToSend)
        {
            await SendCancellationNotification(requestId);
        }
        return Result.Ok(new BulkRequestCancellationResult { Cancelled = cancelled, Skipped = skipped });
    }

    private async Task<Result> ApplyCancellation(Request request, Guid cancellationReasonId, string otherCancellationReason)
    {
        var rCancel = request.Cancel(timeService.GetCurrentDateTime());
        if (!rCancel) return rCancel;
        var cancelBy = identityServerService.GetNickname();
        var entity = new RequestCancellationDetail
        {
            RequestId = request.Id,
            ReasonCancellationRequestId = cancellationReasonId,
            OtherReasonCancellationRequest = otherCancellationReason,
            CancelBy = cancelBy,
            CancelAt = timeService.GetCurrentDateTime()
        };
        var noteBuilder = new StringBuilder();
        noteBuilder.Append("Request canceled");
        if (!string.IsNullOrEmpty(entity.OtherReasonCancellationRequest))
        {
            noteBuilder.Append($" - {entity.OtherReasonCancellationRequest}");
        }
        var noteValue = noteBuilder.ToString();
        var note = new RequestNote(request.Id, CovenantNote.Create(noteValue, CovenantNote.RedColor, entity.CancelBy).Value);
        var existing = await requestRepository.GetRequestCancellationDetail(request.Id);
        if (existing != null)
        {
            requestRepository.Delete(existing);
        }
        await requestRepository.Create(note);
        await requestRepository.Create(entity);
        await requestRepository.Update(request);
        return Result.Ok();
    }

    private async Task SendCancellationNotification(Guid requestId)
    {
        var data = await notificationDataRepository.GetAgencyData(requestId, NotificationType.RequestHasBeenCanceledNotifyAgency.Id);
        if (data != null && data.EmailNotification)
        {
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnRequestCancel/AgencyTemplate.cshtml", data);
            await emailService.SendEmail(new EmailParams(data.AgencyEmail, $"Request {data.JobTitle} was canceled", message));
        }
    }

    public async Task<Result> ReduceWorkerQuantityByOne(Guid requestId)
    {
        Request request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        Result result = request.DecreaseWorkersQuantityByOne();
        if (!result) return result;
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<Result<Request>> MapRequest(RequestCreateModel model, Guid companyId)
    {
        if (model.AgencyId == Guid.Empty)
            return Result.Fail<Request>(ValidationMessages.RequiredMsg(ApiResources.Agency));
        if (!model.WorkerSalary.HasValue && !model.JobPositionRateId.HasValue)
            return Result.Fail<Request>(ValidationMessages.RequiredMsg(ApiResources.JobPosition));
        var rRequest = Request.AgencyCreateRequest(
            model.AgencyId,
            companyId,
            model.LocationId.Value,
            model.StartAt,
            model.JobPositionRateId,
            finishAt: model.FinishAt,
            jobIsOnBranchOffice: model.JobIsOnBranchOffice,
            workersQuantity: model.WorkersQuantity,
            breakIsPaid: model.BreakIsPaid,
            durationTerm: model.DurationTerm,
            employmentType: model.EmploymentType,
            isPunchCardOptionEnabled: model.PunchCardOptionEnabled);
        if (!rRequest) return Result.Fail<Request>(rRequest.Errors);
        var entity = rRequest.Value;
        entity.WorkerSalary = model.WorkerSalary;
        entity.InternalRequirements = model.InternalRequirements;
        entity.Responsibilities = model.Responsibilities;
        if (entity.JobPositionRateId.HasValue)
        {
            var positionRate = await companyRepository.GetJobPosition(model.JobPositionRateId.Value);
            if (positionRate is null) return Result.Fail<Request>(ApiResources.InvalidJobPosition);
            entity.AgencyRate = positionRate.Rate;
            entity.WorkerRate = positionRate.WorkerRate;
        }
        var rJobTitle = entity.UpdateJobTitle(model.JobTitle);
        if (!rJobTitle) return Result.Fail<Request>(rJobTitle.Errors);
        var rBillingTitle = entity.UpdateBillingTitle(model.BillingTitle);
        if (!rBillingTitle) return Result.Fail<Request>(rBillingTitle.Errors);
        var rRequirements = RequestRequirementsModel.Create(model.Requirements);
        if (!rRequirements) return Result.Fail<Request>(rRequirements.Errors);
        entity.UpdateRequirements(rRequirements.Value.Value);
        var rDescription = RequestDescriptionModel.Create(model.Description);
        if (!rDescription) return Result.Fail<Request>(rDescription.Errors);
        entity.UpdateDescription(rDescription.Value.Value);
        Result rDurationBreak = entity.UpdateDurationBreak(model.DurationBreak);
        if (!rDurationBreak) return Result.Fail<Request>(rDurationBreak.Errors);
        Result rIncentive = entity.UpdateIncentive(model.Incentive, model.IncentiveDescription);
        if (!rIncentive) return Result.Fail<Request>(rIncentive.Errors);
        if (model.Shift != null)
        {
            var rShift = entity.UpdateShift(model.Shift.ToShift());
            if (!rShift) return Result.Fail<Request>(rShift.Errors);
        }
        var rIsAsap = entity.UpdateIsAsap(model.IsAsap);
        if (!rIsAsap) return Result.Fail<Request>(rIsAsap.Errors);
        entity.CreatedBy = identityServerService.GetNickname();
        return Result.Ok(entity);
    }

    public async Task<Result> SendInvitation(Guid requestId)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null || !request.CanBeUpdated) return Result.Fail(ApiResources.RequestNotAvailable);

        var now = timeService.GetCurrentDateTime();
        var canBeSent = request.CanInvitationBeSendIt(now);
        if (!canBeSent) return canBeSent;

        var job = new SendInvitationJob(requestId, identityServerService.GetNickname());
        await busClient.SendMessageAsync(job, serviceBusConfiguration.InvitationQueue);
        return Result.Ok();
    }

    public async Task<Result> UpdateIsAsapRequests(RequestsQuickUpdate requestsQuickUpdate)
    {
        var requests = await requestRepository.GetRequests(requestsQuickUpdate.Ids);
        foreach (var request in requests)
        {
            request.IsAsap = requestsQuickUpdate.IsAsap;
        }
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> RejectWorker(Guid requestId, Guid workerId, CommentsModel model)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        var rejectedBy = identityServerService.GetNickname();
        var result = request.RejectWorker(workerId, model.Comments, rejectedBy);
        if (!result) return result;
        await requestRepository.Update(request);
        await requestRepository.SaveChangesAsync();
        var data = await notificationDataRepository.GetWorkerData(requestId, workerId, NotificationType.WorkerHasBeenRejected.Id);
        if (data != null && data.EmailNotification)
        {
            var message = await razorViewToStringRenderer.RenderViewToStringAsync("/Views/Notifications/OnWorkerReject/WorkerTemplate.cshtml", data.JobTitle);
            await emailService.SendEmail(new EmailParams(data.WorkerEmail, $"You have been rejected in request {data.JobTitle}", message));
        }
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<RequestSourceDetailModel>>> GetRequestSources(Guid requestId)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail<IEnumerable<RequestSourceDetailModel>>(ApiResources.RequestNotAvailable);
        var sources = await requestRepository.GetRequestSources(requestId);
        return Result.Ok(sources.Select(s => new RequestSourceDetailModel
        {
            SourceId = s.SourceId,
            Value = s.Source?.Value,
            PublishedAt = s.PublishedAt,
            ExternalUrl = s.ExternalUrl
        }));
    }

    public async Task<AgencyRequestsPagedResponse> GetRequestsForAgency(Guid agencyId, GetRequestForAgencyFilter filter)
    {
        var paged = await requestRepository.GetRequestsForAgency(agencyId, filter);
        var summary = await requestRepository.GetRequestSourcesSummaryForAgency(agencyId, filter);
        return new AgencyRequestsPagedResponse
        {
            PageIndex = paged.PageIndex,
            TotalPages = paged.TotalPages,
            TotalItems = paged.TotalItems,
            Items = paged.Items,
            JobBoardsSummary = summary
        };
    }

    public async Task<Result> SetRequestSources(Guid requestId, IEnumerable<CreateRequestSourceModel> sources)
    {
        var request = await requestRepository.GetRequest(r => r.Id == requestId);
        if (request is null) return Result.Fail(ApiResources.RequestNotAvailable);
        var distinct = (sources ?? []).GroupBy(s => s.SourceId).Select(g => g.First()).ToList();
        await requestRepository.ReplaceRequestSources(requestId, distinct);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

}
