using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Covenant.Core.BL.Services;

public class RequestApplicantService(
    IRequestRepository requestRepository,
    IWorkerRequestRepository workerRequestRepository,
    ICandidateRepository candidateRepository,
    IWorkerRepository workerRepository,
    ICatalogRepository catalogRepository,
    IUploadedFilesService uploadedFilesService,
    IIdentityServerService identityServerService,
    IOptions<FilesConfiguration> filesOptions,
    IValidator<ChangeRequestApplicantStatusModel> changeStatusValidator,
    IValidator<CompleteApplicantComplianceItemModel> completeComplianceItemValidator) : IRequestApplicantService
{
    private const string ApplicantNotFound = "Applicant not found";
    private readonly FilesConfiguration filesConfiguration = filesOptions.Value;

    public async Task<Result<RequestApplicantDetailModel>> Create(Guid requestId, RequestApplicantModel model)
    {
        var existing = await requestRepository.GetRequestApplicant(ra => ra.RequestId == requestId && ra.WorkerProfileId == model.WorkerProfileId && ra.CandidateId == model.CandidateId);
        if (existing != null) return Result.Fail<RequestApplicantDetailModel>("The candidate is already in the request as an applicant");
        var createdBy = identityServerService.GetNickname();
        RequestApplicant entity;
        if (model.CandidateId.HasValue)
        {
            var request = await requestRepository.GetRequest(r => r.Id == requestId);
            var candidate = await candidateRepository.GetCandidate(c => c.Id == model.CandidateId.Value);
            if (!candidate.Skills.Any(s => s.Equals(request.JobTitle)))
            {
                candidate.AddSkill(request.JobTitle);
            }
            var result = RequestApplicant.CreateWithCandidate(requestId, model.CandidateId.Value, createdBy, model.Comments, RequestApplicantStatus.InProgress);
            if (!result) return Result.Fail<RequestApplicantDetailModel>(result.Errors);
            entity = result.Value;
        }
        else if (model.WorkerProfileId.HasValue)
        {
            var workerRequest = await workerRequestRepository.GetWorkerRequestByWorkerProfileId(model.WorkerProfileId.Value, requestId);
            if (workerRequest != null && workerRequest.IsBooked) return Result.Fail<RequestApplicantDetailModel>("The worker is already in the request as a worker");
            var result = RequestApplicant.CreateWithWorker(requestId, model.WorkerProfileId.Value, createdBy, model.Comments, RequestApplicantStatus.InProgress);
            if (!result) return Result.Fail<RequestApplicantDetailModel>(result.Errors);
            entity = result.Value;
        }
        else return Result.Fail<RequestApplicantDetailModel>("A candidate or a worker profile is required");
        await requestRepository.Create([entity]);
        await requestRepository.SaveChangesAsync();
        return Result.Ok(new RequestApplicantDetailModel
        {
            Id = entity.Id,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status
        });
    }

    public async Task<Result> UpdateComments(Guid applicantId, string comments)
    {
        var entity = await requestRepository.GetRequestApplicant(ra => ra.Id == applicantId);
        if (entity is null) return Result.Fail(ApplicantNotFound);
        var result = entity.UpdateComments(comments);
        if (!result) return result;
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public Task<PaginatedList<RequestApplicantDetailModel>> GetApplicants(Guid requestId, GetRequestApplicantFilter filter) =>
        requestRepository.GetRequestApplicants(requestId, filter);

    public Task<List<ApplicantSearchResultModel>> Search(Guid requestId, string searchTerm) =>
        requestRepository.SearchApplicants(identityServerService.GetAgencyId(), requestId, searchTerm);

    public async Task<Result> Delete(Guid applicantId)
    {
        var entity = await requestRepository.GetRequestApplicant(ra => ra.Id == applicantId);
        if (entity is null) return Result.Fail(ApplicantNotFound);
        requestRepository.Delete([entity]);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> ChangeStatus(Guid requestId, Guid applicantId, ChangeRequestApplicantStatusModel model)
    {
        var validationResult = await changeStatusValidator.ValidateAsync(model);
        if (!validationResult.IsValid) return validationResult.ToResultFailure();
        var entity = await requestRepository.GetRequestApplicant(ra => ra.Id == applicantId && ra.RequestId == requestId);
        if (entity is null) return Result.Fail(ApplicantNotFound);
        Result result;
        switch (model.Status)
        {
            case RequestApplicantStatus.InProgress:
                result = entity.MoveToInProgress();
                break;
            case RequestApplicantStatus.Cancelled:
                result = entity.Cancel();
                break;
            case RequestApplicantStatus.Confirmed:
                var mandatoryCompleted = await MandatoryItemsCompleted(requestId, applicantId);
                if (!mandatoryCompleted) return mandatoryCompleted;
                result = entity.Confirm();
                break;
            default:
                return Result.Fail("Invalid target status");
        }
        if (!result) return result;
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<List<ApplicantComplianceItemModel>>> GetComplianceItems(Guid requestId, Guid applicantId)
    {
        var entity = await requestRepository.GetRequestApplicant(ra => ra.Id == applicantId && ra.RequestId == requestId);
        if (entity is null) return Result.Fail<List<ApplicantComplianceItemModel>>(ApplicantNotFound);
        var items = await requestRepository.GetComplianceItems(requestId);
        var completions = (await requestRepository.GetApplicantComplianceItems(applicantId)).ToList();
        var profile = entity.WorkerProfileId.HasValue
            ? await workerRepository.GetProfile(p => p.Id == entity.WorkerProfileId.Value)
            : null;
        var models = new List<ApplicantComplianceItemModel>();
        foreach (var item in items)
        {
            var completion = completions.FirstOrDefault(c => c.RequestComplianceItemId == item.Id);
            models.Add(new ApplicantComplianceItemModel
            {
                Id = item.Id,
                Name = item.Name,
                IsMandatory = item.IsMandatory,
                DocumentTarget = item.DocumentTarget,
                IsCompleted = completion != null,
                CompletedAt = completion?.CompletedAt,
                CompletedBy = completion?.CompletedBy,
                CanUpload = entity.WorkerProfileId.HasValue && item.DocumentTarget != ComplianceDocumentTarget.None,
                ExistingFileUrl = GetExistingFileUrl(profile, item.DocumentTarget)
            });
        }
        return Result.Ok(models);
    }

    public async Task<Result> CompleteComplianceItem(Guid requestId, Guid applicantId, Guid itemId, CompleteApplicantComplianceItemModel model)
    {
        if (model != null)
        {
            var validationResult = await completeComplianceItemValidator.ValidateAsync(model);
            if (!validationResult.IsValid) return validationResult.ToResultFailure();
        }
        var entity = await requestRepository.GetRequestApplicant(ra => ra.Id == applicantId && ra.RequestId == requestId);
        if (entity is null) return Result.Fail(ApplicantNotFound);
        if (entity.Status != RequestApplicantStatus.InProgress) return Result.Fail("Compliance items can only be completed while the applicant is in progress");
        var items = (await requestRepository.GetComplianceItems(requestId)).ToList();
        var item = items.FirstOrDefault(i => i.Id == itemId);
        if (item is null) return Result.Fail("Compliance item not found");
        var completion = await requestRepository.GetApplicantComplianceItem(applicantId, itemId);
        if (completion != null) return Result.Fail("The compliance item is already completed");

        var hasFile = !string.IsNullOrEmpty(model?.FileName);
        if (!hasFile && entity.WorkerProfileId.HasValue && item.IsMandatory && item.DocumentTarget != ComplianceDocumentTarget.None)
            return Result.Fail("A document is required to complete this item");
        var socialInsurancePopulated = false;
        if (hasFile)
        {
            var upload = await UploadDocument(entity, item, model);
            if (!upload) return Result.Fail(upload.Errors);
            socialInsurancePopulated = upload.Value;
        }

        var completedBy = identityServerService.GetNickname();
        var completions = new List<RequestApplicantComplianceItem>();
        var completionResult = RequestApplicantComplianceItem.Create(applicantId, itemId, completedBy);
        if (!completionResult) return completionResult;
        completions.Add(completionResult.Value);
        if (socialInsurancePopulated)
        {
            var completedItemIds = (await requestRepository.GetApplicantComplianceItems(applicantId)).Select(c => c.RequestComplianceItemId).ToList();
            foreach (var socialInsuranceItem in items.Where(i => i.DocumentTarget == ComplianceDocumentTarget.SocialInsurance && i.Id != itemId && !completedItemIds.Contains(i.Id)))
            {
                var socialInsuranceCompletion = RequestApplicantComplianceItem.Create(applicantId, socialInsuranceItem.Id, completedBy);
                if (!socialInsuranceCompletion) return socialInsuranceCompletion;
                completions.Add(socialInsuranceCompletion.Value);
            }
        }
        await requestRepository.Create(completions);
        await requestRepository.SaveChangesAsync();
        if (hasFile) await uploadedFilesService.Upload([model.FileName]);
        return Result.Ok();
    }

    public async Task<Result> UncompleteComplianceItem(Guid requestId, Guid applicantId, Guid itemId)
    {
        var entity = await requestRepository.GetRequestApplicant(ra => ra.Id == applicantId && ra.RequestId == requestId);
        if (entity is null) return Result.Fail(ApplicantNotFound);
        if (entity.Status != RequestApplicantStatus.InProgress) return Result.Fail("Compliance items can only be unchecked while the applicant is in progress");
        var completion = await requestRepository.GetApplicantComplianceItem(applicantId, itemId);
        if (completion is null) return Result.Fail("The compliance item is not completed");
        var completionsToDelete = new List<RequestApplicantComplianceItem> { completion };
        var items = (await requestRepository.GetComplianceItems(requestId)).ToList();
        var item = items.FirstOrDefault(i => i.Id == itemId);
        var linkedTargets = item is null
            ? new List<ComplianceDocumentTarget>()
            : await GetSinLinkedTargets(entity, item.DocumentTarget);
        if (linkedTargets.Count > 0)
        {
            var completions = (await requestRepository.GetApplicantComplianceItems(applicantId)).ToList();
            var linkedItemIds = items.Where(i => linkedTargets.Contains(i.DocumentTarget)).Select(i => i.Id).ToList();
            completionsToDelete.AddRange(completions.Where(c => c.RequestComplianceItemId != itemId && linkedItemIds.Contains(c.RequestComplianceItemId)));
        }
        requestRepository.Delete(completionsToDelete);
        await requestRepository.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task<List<ComplianceDocumentTarget>> GetSinLinkedTargets(RequestApplicant applicant, ComplianceDocumentTarget target)
    {
        if (!applicant.WorkerProfileId.HasValue) return [];
        if (target is not (ComplianceDocumentTarget.SocialInsurance or ComplianceDocumentTarget.Identification1 or ComplianceDocumentTarget.Identification2)) return [];
        var profile = await workerRepository.GetProfile(p => p.Id == applicant.WorkerProfileId.Value);
        if (profile is null) return [];
        var isSin1 = await GetIdentificationTypeCode(profile.IdentificationType1Id) == IdentificationTypeCode.SinSsn;
        var isSin2 = await GetIdentificationTypeCode(profile.IdentificationType2Id) == IdentificationTypeCode.SinSsn;
        switch (target)
        {
            case ComplianceDocumentTarget.SocialInsurance:
                var linked = new List<ComplianceDocumentTarget>();
                if (isSin1) linked.Add(ComplianceDocumentTarget.Identification1);
                if (isSin2) linked.Add(ComplianceDocumentTarget.Identification2);
                return linked;
            case ComplianceDocumentTarget.Identification1 when isSin1:
            case ComplianceDocumentTarget.Identification2 when isSin2:
                return [ComplianceDocumentTarget.SocialInsurance];
            default:
                return [];
        }
    }

    private async Task<IdentificationTypeCode> GetIdentificationTypeCode(Guid? identificationTypeId) =>
        identificationTypeId.HasValue && identificationTypeId != Guid.Empty
            ? await catalogRepository.GetIdentificationTypeCode(identificationTypeId.Value)
            : IdentificationTypeCode.None;

    private async Task<Result> MandatoryItemsCompleted(Guid requestId, Guid applicantId)
    {
        var items = await requestRepository.GetComplianceItems(requestId);
        var completedItemIds = (await requestRepository.GetApplicantComplianceItems(applicantId)).Select(c => c.RequestComplianceItemId).ToList();
        var pending = items.Where(i => i.IsMandatory && !completedItemIds.Contains(i.Id)).Select(i => i.Name).ToList();
        return pending.Count == 0
            ? Result.Ok()
            : Result.Fail($"All mandatory compliance items must be completed before confirming. Pending: {string.Join(", ", pending)}");
    }

    private async Task<Result<bool>> UploadDocument(RequestApplicant applicant, RequestComplianceItem item, CompleteApplicantComplianceItemModel model)
    {
        if (!applicant.WorkerProfileId.HasValue) return Result.Fail<bool>("Only workers can upload documents, convert the candidate to a worker first");
        if (item.DocumentTarget == ComplianceDocumentTarget.None) return Result.Fail<bool>("The compliance item does not accept documents");
        var filesValidation = uploadedFilesService.Validate();
        if (!filesValidation) return Result.Fail<bool>(filesValidation.Errors);
        var profile = await workerRepository.GetProfile(p => p.Id == applicant.WorkerProfileId.Value);
        if (profile is null) return Result.Fail<bool>("Worker profile not found");
        var file = new CovenantFileModel(model.FileName, item.Name);
        switch (item.DocumentTarget)
        {
            case ComplianceDocumentTarget.Identification1:
            case ComplianceDocumentTarget.Identification2:
                var identificationValidation = await ValidateIdentification(profile.Id, item.DocumentTarget, model);
                if (!identificationValidation) return Result.Fail<bool>(identificationValidation.Errors);
                var isSocialInsuranceType = await catalogRepository.GetIdentificationTypeCode(model.IdentificationTypeId.Value) == IdentificationTypeCode.SinSsn;
                if (isSocialInsuranceType && await workerRepository.SocialInsuranceIsAlreadyTaken(model.IdentificationNumber, profile.Id))
                    return Result.Fail<bool>(ApiResources.SocialInsuranceAlreadyTaken);
                var existingIdentificationFile = item.DocumentTarget == ComplianceDocumentTarget.Identification1
                    ? profile.IdentificationType1File
                    : profile.IdentificationType2File;
                var existingSocialInsuranceFile = profile.SocialInsuranceFile;
                var previousMaskedSocialInsurance = profile.MaskedSocialInsurance;
                var identificationPatch = item.DocumentTarget == ComplianceDocumentTarget.Identification1
                    ? profile.PatchIdentification1(model.IdentificationNumber, model.IdentificationTypeId, file)
                    : profile.PatchIdentification2(model.IdentificationNumber, model.IdentificationTypeId, file);
                if (!identificationPatch) return Result.Fail<bool>(identificationPatch.Errors);
                if (isSocialInsuranceType)
                {
                    var socialInsuranceFill = profile.PatchSocialInsuranceFromIdentification(model.IdentificationNumber, file);
                    if (!socialInsuranceFill) return Result.Fail<bool>(socialInsuranceFill.Errors);
                    if (socialInsuranceFill.Value)
                    {
                        var note = WorkerProfileNote.Create(profile.Id,
                            string.Format(ApiResources.SocialInsuranceReplacedNote, previousMaskedSocialInsurance, model.IdentificationNumber.MaskSIN()),
                            identityServerService.GetNickname());
                        if (!note) return Result.Fail<bool>(note.Errors);
                        await requestRepository.Create([note.Value]);
                    }
                }
                var currentIdentificationFile = item.DocumentTarget == ComplianceDocumentTarget.Identification1
                    ? profile.IdentificationType1File
                    : profile.IdentificationType2File;
                await CreateNewFiles((existingIdentificationFile, currentIdentificationFile), (existingSocialInsuranceFile, profile.SocialInsuranceFile));
                return Result.Ok(isSocialInsuranceType);
            case ComplianceDocumentTarget.SocialInsurance:
                if (string.IsNullOrEmpty(model.SocialInsuranceNumber)) return Result.Fail<bool>(ValidationMessages.RequiredMsg(ApiResources.SocialInsurance));
                if (await workerRepository.SocialInsuranceIsAlreadyTaken(model.SocialInsuranceNumber, profile.Id))
                    return Result.Fail<bool>(ApiResources.SocialInsuranceAlreadyTaken);
                var previousSocialInsuranceFile = profile.SocialInsuranceFile;
                var socialInsurancePatch = profile.PatchSocialInsuranceDocument(model.SocialInsuranceNumber, file);
                if (!socialInsurancePatch) return Result.Fail<bool>(socialInsurancePatch.Errors);
                await CreateNewFiles((previousSocialInsuranceFile, profile.SocialInsuranceFile));
                return Result.Ok(false);
            case ComplianceDocumentTarget.Resume:
                var previousResume = profile.Resume;
                var resumePatch = profile.PatchResume(file);
                if (!resumePatch) return Result.Fail<bool>(resumePatch.Errors);
                await CreateNewFiles((previousResume, profile.Resume));
                return Result.Ok(false);
            case ComplianceDocumentTarget.PoliceCheck:
                var previousPoliceCheck = profile.PoliceCheckBackGround;
                var policeCheckPatch = profile.PatchPoliceCheck(file);
                if (!policeCheckPatch) return Result.Fail<bool>(policeCheckPatch.Errors);
                await CreateNewFiles((previousPoliceCheck, profile.PoliceCheckBackGround));
                return Result.Ok(false);
            case ComplianceDocumentTarget.OtherDocument:
                var fileResult = CovenantFile.Create(file);
                if (!fileResult) return Result.Fail<bool>(fileResult.Errors);
                var documentResult = WorkerProfileOtherDocument.Create(profile.Id, fileResult.Value);
                if (!documentResult) return Result.Fail<bool>(documentResult.Errors);
                profile.OtherDocuments.Add(documentResult.Value);
                return Result.Ok(false);
            default:
                return Result.Fail<bool>("The compliance item does not accept documents");
        }
    }

    private string GetExistingFileUrl(WorkerProfile profile, ComplianceDocumentTarget target)
    {
        var fileName = target switch
        {
            ComplianceDocumentTarget.Identification1 => profile?.IdentificationType1File?.FileName,
            ComplianceDocumentTarget.Identification2 => profile?.IdentificationType2File?.FileName,
            ComplianceDocumentTarget.SocialInsurance => profile?.SocialInsuranceFile?.FileName,
            ComplianceDocumentTarget.Resume => profile?.Resume?.FileName,
            ComplianceDocumentTarget.PoliceCheck => profile?.PoliceCheckBackGround?.FileName,
            _ => null
        };
        return string.IsNullOrEmpty(fileName) ? null : string.Concat(filesConfiguration.FilesPath, fileName);
    }

    private async Task CreateNewFiles(params (CovenantFile Previous, CovenantFile Current)[] files)
    {
        var newFiles = files
            .Where(f => f.Previous is null && f.Current is not null)
            .Select(f => f.Current)
            .ToList();
        if (newFiles.Count > 0) await requestRepository.Create(newFiles);
    }

    private async Task<Result> ValidateIdentification(Guid profileId, ComplianceDocumentTarget target, CompleteApplicantComplianceItemModel model)
    {
        var isFirstSlot = target == ComplianceDocumentTarget.Identification1;
        if (string.IsNullOrEmpty(model.IdentificationNumber))
            return Result.Fail(ValidationMessages.RequiredMsg(isFirstSlot ? ApiResources.IdentificationNumber1 : ApiResources.IdentificationNumber2));
        if (model.IdentificationTypeId is null || model.IdentificationTypeId == Guid.Empty)
            return Result.Fail(ValidationMessages.RequiredMsg(isFirstSlot ? ApiResources.IdentificationType1 : ApiResources.IdentificationType2));
        var number = model.IdentificationNumber;
        if (await workerRepository.InfoIsAlreadyTaken(x => x.Id != profileId && (x.IdentificationNumber1 == number || x.IdentificationNumber2 == number)))
            return Result.Fail(string.Format(ApiResources.IdentificationNumberAlreadyTaken, number));
        return Result.Ok();
    }
}
