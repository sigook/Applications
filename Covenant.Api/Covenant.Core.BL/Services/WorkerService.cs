using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Notification;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
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
        private readonly IFilesContainer filesContainer;
        private readonly IDocumentService documentService;

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
            IHttpContextAccessor httpContextAccessor,
            IFilesContainer filesContainer,
            IDocumentService documentService)
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
            this.filesContainer = filesContainer;
            this.documentService = documentService;
        }

        public async Task<Result<Guid>> CreateWorker(int? orderId)
        {
            var dataField = httpContextAccessor.HttpContext.Request.Form["data"];
            var model = JsonConvert.DeserializeObject<WorkerProfileCreateModel>(dataField);
            var validationResult = await workerProfileValidator.ValidateAsync(model);
            if (!validationResult.IsValid) return Result.Fail<Guid>(validationResult.Errors.Select(e => new ResultError(e.PropertyName, e.ErrorMessage)));

            var workerProfile = await workerAdapter.MapToWorkerProfile(model);
            if (!workerProfile) return Result.Fail<Guid>(workerProfile.Errors);
            var entity = workerProfile.Value;

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

        public async Task<Result> UpdateProfileImage(Guid profileId)
        {
            var request = httpContextAccessor.HttpContext.Request;
            var profileImageFile = request.Form.Files[0];
            if (profileImageFile is null)
            {
                return Result.Fail("Profile image file is required");
            }
            var entity = await workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null)
            {
                return Result.Fail("Worker profile not found");
            }
            var oldProfileImageId = entity.ProfileImage?.Id;
            var fileModel = new CovenantFileModel(profileImageFile.FileName);
            var result = entity.PatchProfileImage(fileModel);
            if (!result) return result;
            await workerRepository.Create(entity.ProfileImage);
            await workerRepository.UpdateProfile(entity);
            await workerRepository.SaveChangesAsync();
            await filesContainer.UploadAsync(profileImageFile.OpenReadStream(), profileImageFile.FileName);
            if (oldProfileImageId.HasValue)
            {
                await documentService.DeleteFile(oldProfileImageId.Value);
            }
            return Result.Ok();
        }

        public async Task<Result> UpdateDocumentSection(Guid profileId, WorkerDocumentType documentType)
        {
            var form = httpContextAccessor.HttpContext.Request.Form;
            var entity = await workerRepository.GetProfile(p => p.Id == profileId);
            if (entity is null) return Result.Fail("Worker not found");

            entity.OnNewDocumentAdded += async (_, file) => await workerRepository.Create(file);

            var handlerResult = documentType switch
            {
                WorkerDocumentType.Identification  => await HandleIdentification(entity, form, profileId),
                WorkerDocumentType.Licenses        => HandleLicenses(entity, form),
                WorkerDocumentType.Certificates    => HandleCertificates(entity, form),
                WorkerDocumentType.Resume          => HandleResume(entity, form),
                WorkerDocumentType.OtherDocument   => HandleOtherDocument(entity, form),
                WorkerDocumentType.SocialInsurance => HandleSocialInsurance(entity, form),
                _ => throw new ArgumentOutOfRangeException(nameof(documentType))
            };

            if (!handlerResult) return Result.Fail(handlerResult.Errors);

            await UploadFormFiles(form.Files, handlerResult.Value);
            await workerRepository.UpdateProfile(entity);
            await workerRepository.SaveChangesAsync();
            return Result.Ok();
        }

        private async Task UploadFormFiles(IFormFileCollection files, IEnumerable<string> fileNames)
        {
            foreach (var fileName in fileNames.Where(f => !string.IsNullOrEmpty(f)))
            {
                var file = files[fileName];
                if (file != null)
                    await filesContainer.UploadAsync(file.OpenReadStream(), fileName);
            }
        }

        private async Task<Result<IEnumerable<string>>> HandleIdentification(WorkerProfile entity, IFormCollection form, Guid profileId)
        {
            var model = JsonConvert.DeserializeObject<DocumentsInformationModel>(form["data"]);
            foreach (var number in new[] { model?.IdentificationNumber1, model?.IdentificationNumber2 })
            {
                if (!string.IsNullOrEmpty(number) && await workerRepository.InfoIsAlreadyTaken(x => x.Id != profileId && (x.IdentificationNumber1 == number || x.IdentificationNumber2 == number)))
                    return Result.Fail<IEnumerable<string>>(string.Format(ApiResources.IdentificationNumberAlreadyTaken, number));
            }
            var result = entity.PatchDocuments(model);
            if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
            return Result.Ok<IEnumerable<string>>(new[]
            {
                model.IdentificationType1File?.FileName,
                model.IdentificationType2File?.FileName
            });
        }

        private static Result<IEnumerable<string>> HandleLicenses(WorkerProfile entity, IFormCollection form)
        {
            var model = JsonConvert.DeserializeObject<List<WorkerProfileLicenseModel>>(form["data"]);
            var result = entity.PatchLicenses(model);
            if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
            return Result.Ok<IEnumerable<string>>(model.Select(l => l.License?.FileName));
        }

        private static Result<IEnumerable<string>> HandleCertificates(WorkerProfile entity, IFormCollection form)
        {
            var model = JsonConvert.DeserializeObject<List<CovenantFileModel>>(form["data"]);
            var result = entity.PatchCertificates(model);
            if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
            return Result.Ok<IEnumerable<string>>(model.Select(c => c.FileName));
        }

        private static Result<IEnumerable<string>> HandleResume(WorkerProfile entity, IFormCollection form)
        {
            var model = JsonConvert.DeserializeObject<CovenantFileModel>(form["data"]);
            var result = entity.PatchResume(model);
            if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
            return Result.Ok<IEnumerable<string>>(new[] { model?.FileName });
        }

        private static Result<IEnumerable<string>> HandleOtherDocument(WorkerProfile entity, IFormCollection form)
        {
            var model = JsonConvert.DeserializeObject<CovenantFileModel>(form["data"]);
            var fileResult = CovenantFile.Create(model);
            if (!fileResult) return Result.Fail<IEnumerable<string>>(fileResult.Errors);
            var docResult = WorkerProfileOtherDocument.Create(entity.Id, fileResult.Value);
            if (!docResult) return Result.Fail<IEnumerable<string>>(docResult.Errors);
            entity.OtherDocuments.Add(docResult.Value);
            return Result.Ok<IEnumerable<string>>(new[] { model?.FileName });
        }

        private static Result<IEnumerable<string>> HandleSocialInsurance(WorkerProfile entity, IFormCollection form)
        {
            var model = JsonConvert.DeserializeObject<SinInformationModel>(form["data"]);
            var result = entity.PatchSinInformation(model);
            if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
            return Result.Ok<IEnumerable<string>>(new[] { model.SocialInsuranceFile?.FileName });
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
