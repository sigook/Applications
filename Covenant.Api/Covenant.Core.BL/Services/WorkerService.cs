using Covenant.Common.Configuration;
using Covenant.Core.BL.Extensions;
using Covenant.Common.Constants;
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
using Covenant.Common.Models.Security;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Covenant.Core.BL.Services;

public class WorkerService : IWorkerService
{
    private readonly IWorkerRepository workerRepository;
    private readonly IAgencyRepository agencyRepository;
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
    private readonly ICandidateService candidateService;
    private readonly IUploadedFilesService uploadedFilesService;
    private readonly ICompanyRepository companyRepository;
    private readonly ICatalogRepository catalogRepository;

    public WorkerService(
        IWorkerRepository workerRepository,
        IAgencyRepository agencyRepository,
        ICompanyRepository companyRepository,
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
        IDocumentService documentService,
        ICandidateService candidateService,
        IUploadedFilesService uploadedFilesService,
        ICatalogRepository catalogRepository)
    {
        this.workerRepository = workerRepository;
        this.agencyRepository = agencyRepository;
        this.companyRepository = companyRepository;
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
        this.candidateService = candidateService;
        this.uploadedFilesService = uploadedFilesService;
        this.catalogRepository = catalogRepository;
    }

    public async Task<Result<Guid>> CreateWorker(int? requestId)
    {
        var form = httpContextAccessor.HttpContext.Request.Form;
        var model = form.DeserializeData<WorkerProfileCreateModel>();
        var filesValidation = uploadedFilesService.Validate();
        if (!filesValidation) return Result.Fail<Guid>(filesValidation.Errors);
        var validationResult = await workerProfileValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return Result.Fail<Guid>(validationResult.Errors.Select(e => new ResultError(e.PropertyName, e.ErrorMessage)));

        var agency = await agencyRepository.GetAgencyMasterByLocation(model.Location.City);
        if (agency is null) return Result.Fail<Guid>(ApiResources.AgencyNotFound);

        var user = await identityServerService.CreateUser(new CreateUserModel
        {
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword,
            UserType = UserType.Worker,
            Role = CovenantConstants.Role.Worker
        });
        if (!user) return Result.Fail<Guid>(user.Errors);

        var entity = workerAdapter.MapToWorkerProfile(model, agency, user.Value);

        var socialInsuranceFill = await ApplySocialInsuranceFromIdentification(entity, model);
        if (!socialInsuranceFill) return Result.Fail<Guid>(socialInsuranceFill.Errors);

        await workerRepository.Create(entity);
        await workerRepository.SaveChangesAsync();

        await candidateService.DeleteCandidateByEmail(model.Email);

        await UploadWorkerFiles(entity);

        var notification = TeamsNotificationModel.CreateSuccess($"SIGOOK.COM|{model.WorkerFullName}|{model.Email}", $"Worker created on Sigook");
        notification.PotentialAction =
        [
            new PotentialAction
            {
                Targets = [new Target()]
            }
        ];
        await teamsNotification.SendNotification(teamsWebhookConfiguration.CandidateAndWorker, notification);
        await NotifyAgencyAndSubscribe(entity.Agency, entity);
        if (requestId.HasValue)
        {
            var request = await requestRepository.GetRequest(r => r.NumberId == requestId.Value);
            if (request != null)
            {
                await NotifyApplicant(request, entity, string.Empty);
            }
        }
        return Result.Ok(entity.Id);
    }

    private async Task UploadWorkerFiles(WorkerProfile entity)
    {
        var fileNames = new List<string>
        {
            entity.ProfileImage?.FileName,
            entity.IdentificationType1File?.FileName,
            entity.IdentificationType2File?.FileName,
            entity.PoliceCheckBackGround?.FileName,
            entity.Resume?.FileName
        };
        fileNames.AddRange(entity.Licenses.Select(l => l.License?.FileName));
        fileNames.AddRange(entity.Certificates.Select(c => c.Certificate?.FileName));
        fileNames.AddRange(entity.OtherDocuments.Select(d => d.Document?.FileName));

        await uploadedFilesService.Upload(fileNames);
    }

    public Task<Result> DeleteWorker(Guid workerProfileId)
    {
        throw new NotImplementedException();
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
        if (await workerRequestRepository.WorkerRequestExists(worker.Id, requestId)) return Result.Fail<RequestApplicantDetailModel>("You already apply to this request");
        var requestCandidate = await requestRepository.GetRequestApplicant(ra => ra.RequestId == requestId && ra.WorkerProfileId == worker.Id);
        if (requestCandidate != null) return Result.Fail<RequestApplicantDetailModel>("You already apply to this request");
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
        var filesValidation = uploadedFilesService.Validate();
        if (!filesValidation) return filesValidation;
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
        var filesValidation = uploadedFilesService.Validate();
        if (!filesValidation) return filesValidation;
        var entity = await workerRepository.GetProfile(p => p.Id == profileId);
        if (entity is null) return Result.Fail("Worker not found");

        var handlerResult = documentType switch
        {
            WorkerDocumentType.Identification  => await HandleIdentification(entity, form, profileId),
            WorkerDocumentType.Licenses        => HandleLicenses(entity, form),
            WorkerDocumentType.Certificates    => HandleCertificates(entity, form),
            WorkerDocumentType.Resume          => HandleResume(entity, form),
            WorkerDocumentType.OtherDocument   => HandleOtherDocument(entity, form),
            WorkerDocumentType.SocialInsurance => await HandleSocialInsurance(entity, form),
            _ => throw new ArgumentOutOfRangeException(nameof(documentType))
        };

        if (!handlerResult) return Result.Fail(handlerResult.Errors);

        await uploadedFilesService.Upload(handlerResult.Value);
        await workerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<PaginatedList<WorkerCommentModel>>> GetComments(Guid workerId, Pagination pagination)
    {
        var condition = await CommentsScope(workerId);
        if (condition is null) return Result.Fail<PaginatedList<WorkerCommentModel>>("Not allowed to read these comments");
        return Result.Ok(await workerRepository.GetComments(condition, pagination));
    }

    private async Task<Expression<Func<WorkerComment, bool>>> CommentsScope(Guid workerId)
    {
        if (identityServerService.IsAgencyStaff())
        {
            var agencyId = identityServerService.GetAgencyId();
            return c => c.WorkerProfile.WorkerId == workerId && c.WorkerProfile.AgencyId == agencyId;
        }
        if (identityServerService.GetUserId() == workerId)
            return c => c.WorkerProfile.WorkerId == workerId;
        var companyId = identityServerService.GetCompanyId();
        var companyProfile = await companyRepository.GetCompanyProfileId(p => p.CompanyId == companyId);
        if (companyProfile is null) return null;
        var companyProfileId = companyProfile.Id;
        return c => c.WorkerProfile.WorkerId == workerId && c.CompanyProfileId == companyProfileId;
    }

    public Task<Result> AddAgencyComment(Guid workerProfileId, string comment, decimal rate) =>
        CreateComment(WorkerComment.CommentPostByAgency(workerProfileId, comment, rate));

    public async Task<Result> AddCompanyComment(Guid workerProfileId, string comment, decimal rate)
    {
        var companyId = identityServerService.GetCompanyId();
        var companyProfile = await companyRepository.GetCompanyProfileId(p => p.CompanyId == companyId);
        if (companyProfile is null) return Result.Fail("Company profile not found");
        return await CreateComment(WorkerComment.CommentPostByCompany(workerProfileId, companyProfile.Id, comment, rate));
    }

    private async Task<Result> CreateComment(WorkerComment entity)
    {
        await workerRepository.Create(entity);
        await workerRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<Result<IEnumerable<string>>> HandleIdentification(WorkerProfile entity, IFormCollection form, Guid profileId)
    {
        var model = form.DeserializeData<DocumentsInformationModel>();
        foreach (var number in new[] { model?.IdentificationNumber1, model?.IdentificationNumber2 })
        {
            if (!string.IsNullOrEmpty(number) && await workerRepository.InfoIsAlreadyTaken(x => x.Id != profileId && (x.IdentificationNumber1 == number || x.IdentificationNumber2 == number)))
                return Result.Fail<IEnumerable<string>>(string.Format(ApiResources.IdentificationNumberAlreadyTaken, number));
        }
        var socialInsuranceValidation = await ValidateSocialInsuranceFromIdentification(model, profileId);
        if (!socialInsuranceValidation) return Result.Fail<IEnumerable<string>>(socialInsuranceValidation.Errors);
        var previousFiles = ProfileFiles(entity);
        var result = entity.PatchDocuments(model);
        if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
        var socialInsuranceFill = await ApplySocialInsuranceFromIdentification(entity, model);
        if (!socialInsuranceFill) return Result.Fail<IEnumerable<string>>(socialInsuranceFill.Errors);
        await TrackFilesCreatedByPatch(entity, previousFiles);
        return Result.Ok<IEnumerable<string>>(
        [
            model.IdentificationType1File?.FileName,
            model.IdentificationType2File?.FileName
        ]);
    }

    private async Task<IdentificationTypeCode> GetIdentificationTypeCode(Guid? identificationTypeId) =>
        identificationTypeId.HasValue && identificationTypeId != Guid.Empty
            ? await catalogRepository.GetIdentificationTypeCode(identificationTypeId.Value)
            : IdentificationTypeCode.None;

    private async Task<Result> ValidateSocialInsuranceFromIdentification(IWorkerDocumentsInformation<BaseModel<Guid>, CovenantFileModel> documentsInformation, Guid? excludeProfileId)
    {
        var isSin1 = await GetIdentificationTypeCode(documentsInformation.IdentificationType1?.Id) == IdentificationTypeCode.SinSsn;
        var isSin2 = await GetIdentificationTypeCode(documentsInformation.IdentificationType2?.Id) == IdentificationTypeCode.SinSsn;
        if (isSin1 && isSin2 && documentsInformation.IdentificationNumber1 != documentsInformation.IdentificationNumber2)
            return Result.Fail(ApiResources.SocialInsuranceConflict);
        if (isSin1 && await workerRepository.SocialInsuranceIsAlreadyTaken(documentsInformation.IdentificationNumber1, excludeProfileId))
            return Result.Fail(ApiResources.SocialInsuranceAlreadyTaken);
        if (isSin2 && await workerRepository.SocialInsuranceIsAlreadyTaken(documentsInformation.IdentificationNumber2, excludeProfileId))
            return Result.Fail(ApiResources.SocialInsuranceAlreadyTaken);
        return Result.Ok();
    }

    private async Task<Result> ApplySocialInsuranceFromIdentification(WorkerProfile entity, IWorkerDocumentsInformation<BaseModel<Guid>, CovenantFileModel> documentsInformation)
    {
        var isSin1 = await GetIdentificationTypeCode(documentsInformation.IdentificationType1?.Id) == IdentificationTypeCode.SinSsn;
        var isSin2 = await GetIdentificationTypeCode(documentsInformation.IdentificationType2?.Id) == IdentificationTypeCode.SinSsn;
        if (!isSin1 && !isSin2) return Result.Ok();
        var number = isSin1 ? documentsInformation.IdentificationNumber1 : documentsInformation.IdentificationNumber2;
        var file = isSin1 ? documentsInformation.IdentificationType1File : documentsInformation.IdentificationType2File;
        var previousMaskedSocialInsurance = entity.MaskedSocialInsurance;
        var fill = entity.PatchSocialInsuranceFromIdentification(number, file);
        if (!fill) return Result.Fail(fill.Errors);
        if (fill.Value)
        {
            var note = WorkerProfileNote.Create(entity.Id,
                string.Format(ApiResources.SocialInsuranceReplacedNote, previousMaskedSocialInsurance, number.MaskSIN()),
                identityServerService.GetNickname());
            if (!note) return Result.Fail(note.Errors);
            await workerRepository.Create(note.Value);
        }
        return Result.Ok();
    }

    private static Result<IEnumerable<string>> HandleLicenses(WorkerProfile entity, IFormCollection form)
    {
        var model = form.DeserializeData<List<WorkerProfileLicenseModel>>();
        var result = entity.PatchLicenses(model);
        if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
        return Result.Ok(model.Select(l => l.License?.FileName));
    }

    private static Result<IEnumerable<string>> HandleCertificates(WorkerProfile entity, IFormCollection form)
    {
        var model = form.DeserializeData<List<CovenantFileModel>>();
        var result = entity.PatchCertificates(model);
        if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
        return Result.Ok(model.Select(c => c.FileName));
    }

    private static Result<IEnumerable<string>> HandleResume(WorkerProfile entity, IFormCollection form)
    {
        var model = form.DeserializeData<CovenantFileModel>();
        var result = entity.PatchResume(model);
        if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
        return Result.Ok<IEnumerable<string>>([model?.FileName]);
    }

    private static Result<IEnumerable<string>> HandleOtherDocument(WorkerProfile entity, IFormCollection form)
    {
        var model = form.DeserializeData<CovenantFileModel>();
        var fileResult = CovenantFile.Create(model);
        if (!fileResult) return Result.Fail<IEnumerable<string>>(fileResult.Errors);
        var docResult = WorkerProfileOtherDocument.Create(entity.Id, fileResult.Value);
        if (!docResult) return Result.Fail<IEnumerable<string>>(docResult.Errors);
        entity.OtherDocuments.Add(docResult.Value);
        return Result.Ok<IEnumerable<string>>([model?.FileName]);
    }

    private async Task<Result<IEnumerable<string>>> HandleSocialInsurance(WorkerProfile entity, IFormCollection form)
    {
        var model = form.DeserializeData<SinInformationModel>();
        if (!string.IsNullOrEmpty(model?.SocialInsurance) && await workerRepository.SocialInsuranceIsAlreadyTaken(model.SocialInsurance, entity.Id))
            return Result.Fail<IEnumerable<string>>(ApiResources.SocialInsuranceAlreadyTaken);
        var previousFiles = ProfileFiles(entity);
        var result = entity.PatchSinInformation(model);
        if (!result) return Result.Fail<IEnumerable<string>>(result.Errors);
        await TrackFilesCreatedByPatch(entity, previousFiles);
        return Result.Ok<IEnumerable<string>>([model.SocialInsuranceFile?.FileName]);
    }

    private static List<CovenantFile> ProfileFiles(WorkerProfile entity) =>
    [
        entity.IdentificationType1File,
        entity.IdentificationType2File,
        entity.SocialInsuranceFile,
        entity.PoliceCheckBackGround,
        entity.Resume
    ];

    private async Task TrackFilesCreatedByPatch(WorkerProfile entity, List<CovenantFile> previousFiles)
    {
        var createdFiles = ProfileFiles(entity).Where(f => f is not null && !previousFiles.Contains(f));
        foreach (var file in createdFiles) await workerRepository.Create(file);
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
        var result = RequestApplicant.CreateWithWorker(request.Id, workerProfile.Id, "Sigook", comments, RequestApplicantStatus.Pending);
        await requestRepository.Create([result.Value]);
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
