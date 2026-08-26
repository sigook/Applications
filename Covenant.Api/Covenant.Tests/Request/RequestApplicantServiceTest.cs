using Covenant.Api.Validators.Request;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Request;

public class RequestApplicantServiceTest
{
    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly Mock<ICandidateRepository> _candidateRepository = new();
    private readonly Mock<IWorkerRepository> _workerRepository = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<IUploadedFilesService> _uploadedFilesService = new();
    private readonly Mock<IIdentityServerService> _identityServerService = new();
    private readonly RequestApplicantService _sut;

    private readonly Guid _requestId = Guid.NewGuid();

    public RequestApplicantServiceTest()
    {
        _identityServerService.Setup(s => s.GetNickname()).Returns("tester");
        _uploadedFilesService.Setup(s => s.Validate()).Returns(Result.Ok());
        _uploadedFilesService.Setup(s => s.Upload(It.IsAny<IEnumerable<string>>())).Returns(Task.CompletedTask);
        _catalogRepository.Setup(r => r.GetIdentificationTypeCode(It.IsAny<Guid>())).ReturnsAsync(IdentificationTypeCode.None);
        var filesOptions = new Mock<IOptions<FilesConfiguration>>();
        filesOptions.Setup(o => o.Value).Returns(new FilesConfiguration { FilesPath = "https://files.test/" });
        _sut = new RequestApplicantService(
            _requestRepository.Object,
            _workerRequestRepository.Object,
            _candidateRepository.Object,
            _workerRepository.Object,
            _catalogRepository.Object,
            _uploadedFilesService.Object,
            _identityServerService.Object,
            filesOptions.Object,
            new ChangeRequestApplicantStatusModelValidator(),
            new CompleteApplicantComplianceItemModelValidator());
    }

    private RequestApplicant SetupApplicant(RequestApplicantStatus status, Guid? workerProfileId = null, Guid? candidateId = null)
    {
        var applicant = candidateId.HasValue
            ? RequestApplicant.CreateWithCandidate(_requestId, candidateId.Value, "tester", null, status).Value
            : RequestApplicant.CreateWithWorker(_requestId, workerProfileId ?? Guid.NewGuid(), "tester", null, status).Value;
        _requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync(applicant);
        return applicant;
    }

    private RequestComplianceItem SetupComplianceItem(string name, bool isMandatory, ComplianceDocumentTarget target)
    {
        var item = RequestComplianceItem.Create(_requestId, name, isMandatory, target).Value;
        _requestRepository.Setup(r => r.GetComplianceItems(_requestId)).ReturnsAsync([item]);
        return item;
    }

    private void SetupCompletions(params RequestApplicantComplianceItem[] completions) =>
        _requestRepository.Setup(r => r.GetApplicantComplianceItems(It.IsAny<Guid>())).ReturnsAsync(completions);

    [Fact]
    public async Task CreateWithWorkerStartsInProgress()
    {
        _requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync((RequestApplicant)null);
        var result = await _sut.Create(_requestId, new RequestApplicantModel { WorkerProfileId = Guid.NewGuid() });
        Assert.True(result);
        Assert.Equal(RequestApplicantStatus.InProgress, result.Value.Status);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e => e.Single().Status == RequestApplicantStatus.InProgress)), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateFailsWhenApplicantIsDuplicated()
    {
        SetupApplicant(RequestApplicantStatus.Pending);
        var result = await _sut.Create(_requestId, new RequestApplicantModel { WorkerProfileId = Guid.NewGuid() });
        Assert.False(result);
    }

    [Fact]
    public async Task CreateFailsWhenWorkerIsBooked()
    {
        _requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync((RequestApplicant)null);
        var workerProfileId = Guid.NewGuid();
        var workerRequest = WorkerRequest.AgencyBook(workerProfileId, _requestId);
        _workerRequestRepository.Setup(r => r.GetWorkerRequestByWorkerProfileId(workerProfileId, _requestId)).ReturnsAsync(workerRequest);
        var result = await _sut.Create(_requestId, new RequestApplicantModel { WorkerProfileId = workerProfileId });
        Assert.False(result);
    }

    [Fact]
    public async Task ChangeStatusToConfirmedFailsWithPendingMandatoryItems()
    {
        SetupApplicant(RequestApplicantStatus.InProgress);
        SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        SetupCompletions();
        var result = await _sut.ChangeStatus(_requestId, Guid.NewGuid(), new ChangeRequestApplicantStatusModel { Status = RequestApplicantStatus.Confirmed });
        Assert.False(result);
    }

    [Fact]
    public async Task ChangeStatusToConfirmedIgnoresOptionalItems()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress);
        SetupComplianceItem("WP", isMandatory: false, ComplianceDocumentTarget.None);
        SetupCompletions();
        var result = await _sut.ChangeStatus(_requestId, applicant.Id, new ChangeRequestApplicantStatusModel { Status = RequestApplicantStatus.Confirmed });
        Assert.True(result);
        Assert.Equal(RequestApplicantStatus.Confirmed, applicant.Status);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangeStatusToPendingIsRejected()
    {
        SetupApplicant(RequestApplicantStatus.InProgress);
        var result = await _sut.ChangeStatus(_requestId, Guid.NewGuid(), new ChangeRequestApplicantStatusModel { Status = RequestApplicantStatus.Pending });
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteItemWithoutFileCreatesCompletion()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress);
        var item = SetupComplianceItem("Banking", isMandatory: true, ComplianceDocumentTarget.None);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, item.Id)).ReturnsAsync((RequestApplicantComplianceItem)null);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e => e.Single().RequestApplicantId == applicant.Id
            && e.Single().RequestComplianceItemId == item.Id && e.Single().CompletedBy == "tester")), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CompleteItemWithoutFileFailsForWorkerWhenItemTargetsADocument()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId: Guid.NewGuid());
        var item = SetupComplianceItem("W4", isMandatory: true, ComplianceDocumentTarget.OtherDocument);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, item.Id)).ReturnsAsync((RequestApplicantComplianceItem)null);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteItemWithoutFileSucceedsForCandidateWhenItemTargetsADocument()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, candidateId: Guid.NewGuid());
        var item = SetupComplianceItem("W4", isMandatory: true, ComplianceDocumentTarget.OtherDocument);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, item.Id)).ReturnsAsync((RequestApplicantComplianceItem)null);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.True(result);
    }

    [Fact]
    public async Task CompleteOptionalItemWithoutFileSucceedsForWorkerWhenItemTargetsADocument()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId: Guid.NewGuid());
        var item = SetupComplianceItem("WP", isMandatory: false, ComplianceDocumentTarget.OtherDocument);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, item.Id)).ReturnsAsync((RequestApplicantComplianceItem)null);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.True(result);
    }

    [Fact]
    public async Task CompleteItemFailsWhenAlreadyCompleted()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress);
        var item = SetupComplianceItem("W4", isMandatory: true, ComplianceDocumentTarget.OtherDocument);
        var completion = RequestApplicantComplianceItem.Create(applicant.Id, item.Id, "tester").Value;
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, item.Id)).ReturnsAsync(completion);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteItemFailsWhenApplicantIsNotInProgress()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.Pending);
        var item = SetupComplianceItem("W4", isMandatory: true, ComplianceDocumentTarget.OtherDocument);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteItemWithFileFailsForCandidate()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, candidateId: Guid.NewGuid());
        var item = SetupComplianceItem("Resume", isMandatory: false, ComplianceDocumentTarget.Resume);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "resume.pdf" });
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteItemWithFileFailsWhenTargetIsNone()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId: Guid.NewGuid());
        var item = SetupComplianceItem("Banking", isMandatory: true, ComplianceDocumentTarget.None);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "banking.pdf" });
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteIdentificationFailsWithoutNumberOrType()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(new WorkerProfile { Id = workerProfileId });
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "id.pdf" });
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteIdentificationRoutesToIdentificationSlot()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        var identificationTypeId = Guid.NewGuid();
        var model = new CompleteApplicantComplianceItemModel
        {
            FileName = "id.pdf",
            IdentificationNumber = "123456789",
            IdentificationTypeId = identificationTypeId
        };
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, model);
        Assert.True(result);
        Assert.Equal("123456789", profile.IdentificationNumber1);
        Assert.Equal(identificationTypeId, profile.IdentificationType1Id);
        Assert.Equal("id.pdf", profile.IdentificationType1File.FileName);
        Assert.Null(profile.IdentificationType2File);
        _uploadedFilesService.Verify(s => s.Upload(It.Is<IEnumerable<string>>(f => f.Contains("id.pdf"))), Times.Once);
    }

    [Fact]
    public async Task CompleteOtherDocumentAddsToOtherDocumentsWithItemName()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("W4", isMandatory: true, ComplianceDocumentTarget.OtherDocument);
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "w4.pdf" });
        Assert.True(result);
        var document = Assert.Single(profile.OtherDocuments);
        Assert.Equal("w4.pdf", document.Document.FileName);
        Assert.Equal("W4", document.Document.Description);
    }

    [Fact]
    public async Task CompleteSocialInsuranceFailsWithoutNumber()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("SSN", isMandatory: true, ComplianceDocumentTarget.SocialInsurance);
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(new WorkerProfile { Id = workerProfileId });
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "sin.pdf" });
        Assert.False(result);
    }

    [Fact]
    public async Task CompleteOptionalIdentificationWithoutInputsKeepsProfileValues()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: false, ComplianceDocumentTarget.Identification1);
        var identificationTypeId = Guid.NewGuid();
        var profile = new WorkerProfile
        {
            Id = workerProfileId,
            IdentificationNumber1 = "123456789",
            IdentificationType1Id = identificationTypeId
        };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "id.pdf" });
        Assert.True(result);
        Assert.Equal("123456789", profile.IdentificationNumber1);
        Assert.Equal(identificationTypeId, profile.IdentificationType1Id);
        Assert.Equal("id.pdf", profile.IdentificationType1File.FileName);
    }

    [Fact]
    public async Task CompleteOptionalIdentificationWithoutFileSavesTypedValues()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: false, ComplianceDocumentTarget.Identification1);
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        var identificationTypeId = Guid.NewGuid();
        var model = new CompleteApplicantComplianceItemModel
        {
            IdentificationNumber = "123456789",
            IdentificationTypeId = identificationTypeId
        };
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, model);
        Assert.True(result);
        Assert.Equal("123456789", profile.IdentificationNumber1);
        Assert.Equal(identificationTypeId, profile.IdentificationType1Id);
        Assert.Null(profile.IdentificationType1File);
        _uploadedFilesService.Verify(s => s.Upload(It.IsAny<IEnumerable<string>>()), Times.Never);
    }

    [Fact]
    public async Task CompleteOptionalSocialInsuranceWithoutNumberOnlyAttachesFile()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("SSN", isMandatory: false, ComplianceDocumentTarget.SocialInsurance);
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel { FileName = "sin.pdf" });
        Assert.True(result);
        Assert.Equal("sin.pdf", profile.SocialInsuranceFile.FileName);
        Assert.Null(profile.SocialInsurance);
        _workerRepository.Verify(r => r.SocialInsuranceIsAlreadyTaken(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
    }

    [Fact]
    public async Task CompleteOptionalItemWithDocumentTargetWithoutFileIsAllowed()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("Resume", isMandatory: false, ComplianceDocumentTarget.Resume);
        SetupCompletions();
        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, new CompleteApplicantComplianceItemModel());
        Assert.True(result);
        _workerRepository.Verify(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()), Times.Never);
    }

    private Guid SetupSinIdentificationType()
    {
        var identificationTypeId = Guid.NewGuid();
        _catalogRepository.Setup(r => r.GetIdentificationTypeCode(identificationTypeId)).ReturnsAsync(IdentificationTypeCode.SinSsn);
        return identificationTypeId;
    }

    private static CompleteApplicantComplianceItemModel SinIdentificationModel(Guid identificationTypeId) => new()
    {
        FileName = "sin.pdf",
        IdentificationNumber = "123-456-789",
        IdentificationTypeId = identificationTypeId
    };

    [Fact]
    public async Task CompleteSinTypedIdentificationFillsSocialInsuranceAndCompletesSsnItem()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var identificationItem = RequestComplianceItem.Create(_requestId, "ID", true, ComplianceDocumentTarget.Identification1).Value;
        var ssnItem = RequestComplianceItem.Create(_requestId, "SSN", true, ComplianceDocumentTarget.SocialInsurance).Value;
        _requestRepository.Setup(r => r.GetComplianceItems(_requestId)).ReturnsAsync([identificationItem, ssnItem]);
        SetupCompletions();
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken("123-456-789", workerProfileId)).ReturnsAsync(false);
        var identificationTypeId = SetupSinIdentificationType();

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, identificationItem.Id, SinIdentificationModel(identificationTypeId));

        Assert.True(result);
        Assert.Equal("123-456-789", profile.SocialInsurance);
        Assert.Equal("sin.pdf", profile.SocialInsuranceFile.FileName);
        Assert.Equal("123-456-789", profile.IdentificationNumber1);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e =>
            e.Count() == 2
            && e.Any(c => c.RequestComplianceItemId == identificationItem.Id)
            && e.Any(c => c.RequestComplianceItemId == ssnItem.Id)
            && e.All(c => c.CompletedBy == "tester" && c.RequestApplicantId == applicant.Id))), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CompleteSinTypedIdentificationSkipsAlreadyCompletedSsnItem()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var identificationItem = RequestComplianceItem.Create(_requestId, "ID", true, ComplianceDocumentTarget.Identification1).Value;
        var ssnItem = RequestComplianceItem.Create(_requestId, "SSN", true, ComplianceDocumentTarget.SocialInsurance).Value;
        _requestRepository.Setup(r => r.GetComplianceItems(_requestId)).ReturnsAsync([identificationItem, ssnItem]);
        SetupCompletions(RequestApplicantComplianceItem.Create(applicant.Id, ssnItem.Id, "tester").Value);
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        var identificationTypeId = SetupSinIdentificationType();

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, identificationItem.Id, SinIdentificationModel(identificationTypeId));

        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e => e.Count() == 1
            && e.Single().RequestComplianceItemId == identificationItem.Id)), Times.Once);
    }

    [Fact]
    public async Task CompleteSinTypedIdentificationFailsWhenSinIsTaken()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        SetupCompletions();
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(new WorkerProfile { Id = workerProfileId });
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken("123-456-789", workerProfileId)).ReturnsAsync(true);
        var identificationTypeId = SetupSinIdentificationType();

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, SinIdentificationModel(identificationTypeId));

        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicantComplianceItem>>()), Times.Never);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CompleteSinTypedIdentificationReplacesDifferentSinAndCreatesNote()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        SetupCompletions();
        var profile = new WorkerProfile { Id = workerProfileId, SocialInsurance = "999-888-777" };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        var identificationTypeId = SetupSinIdentificationType();

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, SinIdentificationModel(identificationTypeId));

        Assert.True(result);
        Assert.Equal("123-456-789", profile.SocialInsurance);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<WorkerProfileNote>>(e =>
            e.Single().WorkerProfileId == workerProfileId
            && e.Single().CreatedBy == "tester"
            && e.Single().Note.Contains("******-777")
            && e.Single().Note.Contains("******-789"))), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CompleteSinTypedIdentificationWithSameSinDoesNotCreateNote()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        SetupCompletions();
        var profile = new WorkerProfile { Id = workerProfileId, SocialInsurance = "123-456-789" };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        var identificationTypeId = SetupSinIdentificationType();

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, SinIdentificationModel(identificationTypeId));

        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<WorkerProfileNote>>()), Times.Never);
    }

    [Fact]
    public async Task CompleteSocialInsuranceItemFailsWhenSinIsTaken()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("SSN", isMandatory: true, ComplianceDocumentTarget.SocialInsurance);
        SetupCompletions();
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(new WorkerProfile { Id = workerProfileId });
        _workerRepository.Setup(r => r.SocialInsuranceIsAlreadyTaken("123-456-789", workerProfileId)).ReturnsAsync(true);
        var model = new CompleteApplicantComplianceItemModel { FileName = "sin.pdf", SocialInsuranceNumber = "123-456-789" };

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, model);

        Assert.False(result);
    }

    [Fact]
    public async Task CompleteNonSinIdentificationDoesNotTouchSocialInsurance()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var item = SetupComplianceItem("ID", isMandatory: true, ComplianceDocumentTarget.Identification1);
        SetupCompletions();
        var profile = new WorkerProfile { Id = workerProfileId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(profile);
        _workerRepository.Setup(r => r.InfoIsAlreadyTaken(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(false);
        var model = SinIdentificationModel(Guid.NewGuid());

        var result = await _sut.CompleteComplianceItem(_requestId, applicant.Id, item.Id, model);

        Assert.True(result);
        Assert.Null(profile.SocialInsurance);
        Assert.Null(profile.SocialInsuranceFile);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e => e.Count() == 1)), Times.Once);
    }

    [Fact]
    public async Task UncompleteItemDeletesCompletion()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress);
        var itemId = Guid.NewGuid();
        var completion = RequestApplicantComplianceItem.Create(applicant.Id, itemId, "tester").Value;
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, itemId)).ReturnsAsync(completion);
        var result = await _sut.UncompleteComplianceItem(_requestId, applicant.Id, itemId);
        Assert.True(result);
        _requestRepository.Verify(r => r.Delete(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e => e.Single() == completion)), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UncompleteSsnItemAlsoUncompletesSinTypedIdentificationItem()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var identificationItem = RequestComplianceItem.Create(_requestId, "ID", true, ComplianceDocumentTarget.Identification1).Value;
        var ssnItem = RequestComplianceItem.Create(_requestId, "SSN", true, ComplianceDocumentTarget.SocialInsurance).Value;
        _requestRepository.Setup(r => r.GetComplianceItems(_requestId)).ReturnsAsync([identificationItem, ssnItem]);
        var ssnCompletion = RequestApplicantComplianceItem.Create(applicant.Id, ssnItem.Id, "tester").Value;
        var identificationCompletion = RequestApplicantComplianceItem.Create(applicant.Id, identificationItem.Id, "tester").Value;
        SetupCompletions(ssnCompletion, identificationCompletion);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, ssnItem.Id)).ReturnsAsync(ssnCompletion);
        var identificationTypeId = SetupSinIdentificationType();
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
            .ReturnsAsync(new WorkerProfile { Id = workerProfileId, IdentificationType1Id = identificationTypeId });

        var result = await _sut.UncompleteComplianceItem(_requestId, applicant.Id, ssnItem.Id);

        Assert.True(result);
        _requestRepository.Verify(r => r.Delete(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e =>
            e.Count() == 2 && e.Contains(ssnCompletion) && e.Contains(identificationCompletion))), Times.Once);
    }

    [Fact]
    public async Task UncompleteSinTypedIdentificationItemAlsoUncompletesSsnItem()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var identificationItem = RequestComplianceItem.Create(_requestId, "ID", true, ComplianceDocumentTarget.Identification1).Value;
        var ssnItem = RequestComplianceItem.Create(_requestId, "SSN", true, ComplianceDocumentTarget.SocialInsurance).Value;
        _requestRepository.Setup(r => r.GetComplianceItems(_requestId)).ReturnsAsync([identificationItem, ssnItem]);
        var ssnCompletion = RequestApplicantComplianceItem.Create(applicant.Id, ssnItem.Id, "tester").Value;
        var identificationCompletion = RequestApplicantComplianceItem.Create(applicant.Id, identificationItem.Id, "tester").Value;
        SetupCompletions(ssnCompletion, identificationCompletion);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, identificationItem.Id)).ReturnsAsync(identificationCompletion);
        var identificationTypeId = SetupSinIdentificationType();
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
            .ReturnsAsync(new WorkerProfile { Id = workerProfileId, IdentificationType1Id = identificationTypeId });

        var result = await _sut.UncompleteComplianceItem(_requestId, applicant.Id, identificationItem.Id);

        Assert.True(result);
        _requestRepository.Verify(r => r.Delete(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e =>
            e.Count() == 2 && e.Contains(ssnCompletion) && e.Contains(identificationCompletion))), Times.Once);
    }

    [Fact]
    public async Task UncompleteSsnItemAloneWhenIdentificationIsNotSinTyped()
    {
        var workerProfileId = Guid.NewGuid();
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress, workerProfileId);
        var identificationItem = RequestComplianceItem.Create(_requestId, "ID", true, ComplianceDocumentTarget.Identification1).Value;
        var ssnItem = RequestComplianceItem.Create(_requestId, "SSN", true, ComplianceDocumentTarget.SocialInsurance).Value;
        _requestRepository.Setup(r => r.GetComplianceItems(_requestId)).ReturnsAsync([identificationItem, ssnItem]);
        var ssnCompletion = RequestApplicantComplianceItem.Create(applicant.Id, ssnItem.Id, "tester").Value;
        var identificationCompletion = RequestApplicantComplianceItem.Create(applicant.Id, identificationItem.Id, "tester").Value;
        SetupCompletions(ssnCompletion, identificationCompletion);
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, ssnItem.Id)).ReturnsAsync(ssnCompletion);
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
            .ReturnsAsync(new WorkerProfile { Id = workerProfileId, IdentificationType1Id = Guid.NewGuid() });

        var result = await _sut.UncompleteComplianceItem(_requestId, applicant.Id, ssnItem.Id);

        Assert.True(result);
        _requestRepository.Verify(r => r.Delete(It.Is<IEnumerable<RequestApplicantComplianceItem>>(e =>
            e.Single() == ssnCompletion)), Times.Once);
    }

    [Fact]
    public async Task UncompleteItemFailsWhenNotCompleted()
    {
        var applicant = SetupApplicant(RequestApplicantStatus.InProgress);
        var itemId = Guid.NewGuid();
        _requestRepository.Setup(r => r.GetApplicantComplianceItem(applicant.Id, itemId)).ReturnsAsync((RequestApplicantComplianceItem)null);
        var result = await _sut.UncompleteComplianceItem(_requestId, applicant.Id, itemId);
        Assert.False(result);
    }
}
