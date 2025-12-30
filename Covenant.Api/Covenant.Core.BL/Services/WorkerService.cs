using Covenant.Common.Configuration;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models;
using Covenant.Common.Models.Notification;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Covenant.Core.BL.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IAgencyRepository agencyRepository;
        private readonly IWorkerRepository workerRepository;
        private readonly INotificationRepository notificationRepository;
        private readonly IRequestRepository requestRepository;
        private readonly IWorkerRequestRepository workerRequestRepository;
        private readonly IIdentityServerService identityServerService;
        private readonly ITeamsService teamsNotification;
        private readonly IEmailService emailService;
        private readonly ISendGridService sendGridService;
        private readonly IRazorViewToStringRenderer razorViewToStringRenderer;
        private readonly ILogger<WorkerService> logger;
        private readonly TeamsWebhookConfiguration teamsWebhookConfiguration;
        private readonly IWorkerAdapter workerAdapter;
        private readonly IValidator<WorkerProfileCreateModel> workerProfileValidator;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WorkerService(
            IAgencyRepository agencyRepository,
            IWorkerRepository workerRepository,
            IUserRepository userRepository,
            INotificationRepository notificationRepository,
            IRequestRepository requestRepository,
            IWorkerRequestRepository workerRequestRepository,
            IIdentityServerService identityServerService,
            ITeamsService teamsNotification,
            IEmailService emailService,
            ISendGridService sendGridService,
            ITimeService timeService,
            IRazorViewToStringRenderer razorViewToStringRenderer,
            IOptions<TeamsWebhookConfiguration> options,
            ILogger<WorkerService> logger,
            IWorkerAdapter workerAdapter,
            IValidator<WorkerProfileCreateModel> workerProfileValidator,
            IHttpContextAccessor httpContextAccessor)
        {
            this.agencyRepository = agencyRepository;
            this.workerRepository = workerRepository;
            this.notificationRepository = notificationRepository;
            this.requestRepository = requestRepository;
            this.workerRequestRepository = workerRequestRepository;
            this.identityServerService = identityServerService;
            this.teamsNotification = teamsNotification;
            this.emailService = emailService;
            this.sendGridService = sendGridService;
            this.razorViewToStringRenderer = razorViewToStringRenderer;
            teamsWebhookConfiguration = options.Value;
            this.logger = logger;
            this.workerAdapter = workerAdapter;
            this.workerProfileValidator = workerProfileValidator;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Guid>> CreateWorker(int? orderId)
        {
            var dataField = httpContextAccessor.HttpContext.Request.Form["data"];
            var model = JsonConvert.DeserializeObject<WorkerProfileCreateModel>(dataField);
            var validationResult = await workerProfileValidator.ValidateAsync(model);
            if (!validationResult.IsValid) return Result.Fail<Guid>(validationResult.Errors.Select(e => new ResultError(e.PropertyName, e.ErrorMessage)));

            var wp = await workerAdapter.MapToWorkerProfile(model);
            if (!wp) return Result.Fail<Guid>(wp.Errors);
            var entity = wp.Value;

            await workerRepository.Create(entity);
            await workerRepository.SaveChangesAsync();
            var notification = TeamsNotificationModel.CreateSuccess($"SIGOOK.COM|{model.WorkerFullName}|{model.Email}", $"Worker created on Sigook");
            notification.PotentialAction = new[]
            {
                new PotentialAction
                {
                    Targets = new []
                    {
                        new Target()
                    }
                }
            };
            await teamsNotification.SendNotification(teamsWebhookConfiguration.CandidateAndWorker, notification);
            await NotifyAgencyAndSubscribe(entity.Agency, entity);
            if (orderId.HasValue)
            {
                var request = await requestRepository.GetRequest(r => r.NumberId == orderId.Value);
                if (request != null)
                {
                    await NotifyApplicant(request, entity, string.Empty);
                }
            }
            return Result.Ok(entity.Id);
        }

        public Task<Result> DeleteWorker(Guid workerProfileId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdateWorkerPunchCardId(Guid profileId, Guid agencyId, string punchCardId)
        {
            var entity = await workerRepository.GetProfile(p => p.Id == profileId && p.AgencyId == agencyId);
            if (entity is null) return Result.Fail();
            if (!string.IsNullOrEmpty(punchCardId))
            {
                WorkerProfilePunchCardIdModel punchCardModel = await workerRepository.GetWorkerProfilePunchCarId(punchCardId);
                if (punchCardModel != null && entity.Id != punchCardModel.Id)
                    return Result.Fail($"The worker {punchCardModel.WorkerFullName} is using this card please try another one");
            }
            entity.PunchCardId = punchCardId;
            await workerRepository.UpdateProfile(entity);
            await workerRepository.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result<RequestApplicantDetailModel>> Apply(Guid requestId, WorkerRequestApplyModel model, Guid? workerId = null)
        {
            if (!workerId.HasValue)
            {
                workerId = identityServerService.GetUserId();
            }
            var request = await requestRepository.GetRequest(r => r.Id == requestId);
            if (request is null || !request.IsAvailableToApply) return Result.Fail<RequestApplicantDetailModel>(ApiResources.RequestNotAvailable);
            var worker = await workerRepository.GetProfile(p => p.WorkerId == workerId);
            if (worker is null) return Result.Fail<RequestApplicantDetailModel>(ApiResources.WorkerNotFound);
            if (await workerRequestRepository.WorkerRequestExists(worker.WorkerId, requestId)) return Result.Fail<RequestApplicantDetailModel>("You already apply to this order");
            var requestCandidate = await requestRepository.GetRequestApplicant(ra => ra.RequestId == requestId && ra.WorkerProfileId == worker.Id);
            if (requestCandidate != null) return Result.Fail<RequestApplicantDetailModel>("You already apply to this order");
            var result = await NotifyApplicant(request, worker, model.Comments);
            return Result.Ok(new RequestApplicantDetailModel
            {
                Id = result.Id,
                WorkerId = worker.WorkerId,
                WorkerProfileId = worker.Id
            });
        }

        private async Task NotifyAgencyAndSubscribe(Common.Entities.Agency.Agency agency, WorkerProfile workerProfile)
        {
            try
            {
                var message = await razorViewToStringRenderer.RenderViewToStringAsync(
                    "/Views/Notifications/OnNewWorker/AgencyTemplate.cshtml",
                    new OnNewWorkerAgencyTemplateViewModel
                    {
                        ApprovedToWork = workerProfile.ApprovedToWork,
                        WorkerFullName = workerProfile.FullName
                    });
                await emailService.SendEmail(new EmailParams(agency.RecruitmentEmail, "New Worker", message));
                await notificationRepository.CreateUpdate(workerProfile.WorkerId, new UserNotificationUpdateModel
                {
                    EmailNotification = true,
                    Id = NotificationType.NewRequestNotifyWorker.Id
                });
                await notificationRepository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error notifying agency {Error}", e.Message);
            }
        }

        private async Task<RequestApplicant> NotifyApplicant(Request request, WorkerProfile workerProfile, string comments)
        {
            var result = RequestApplicant.CreateWithWorker(request.Id, workerProfile.Id, "Sigook", comments);
            await requestRepository.Create(result.Value);
            await requestRepository.SaveChangesAsync();
            if (request.Recruiters.Any())
            {
                var sendGridModel = new SendGridModel
                {
                    Tos = request.Recruiters.Select(r => r.Recruiter.User.Email),
                    Template = "NEW_APPLICANT",
                    Data = new
                    {
                        RequestNumberId = request.NumberId,
                        request.JobTitle,
                        WorkerNumberId = workerProfile.NumberId,
                        Nmae = workerProfile.FullName,
                        workerProfile.Worker?.Email,
                        Phone = $"{workerProfile.Phone}",
                        workerProfile.MobileNumber,
                        workerProfile.Location?.FormattedAddress,
                        Skills = string.Join(",", workerProfile.Skills?.Select(s => s.Skill)),
                        Sin = workerProfile.MaskedSocialInsurance,
                        SinExpire = workerProfile.DueDate?.ToString("D")
                    }
                };
                await sendGridService.SendEmail(sendGridModel);
            }
            return result.Value;
        }
    }
}
