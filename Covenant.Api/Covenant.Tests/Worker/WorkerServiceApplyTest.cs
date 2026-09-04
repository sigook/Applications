using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.WebSite;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Tests.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using Xunit;
using RequestEntity = Covenant.Common.Entities.Request.Request;

namespace Covenant.Tests.Worker;

public class WorkerServiceApplyTest
{
    private readonly Mock<IWorkerRepository> _workerRepository = new();
    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<IWorkerRequestRepository> _workerRequestRepository = new();
    private readonly Mock<ICandidateRepository> _candidateRepository = new();
    private readonly Mock<IIdentityServerService> _identityServerService = new();
    private readonly Mock<IRequestApplicantNotificationService> _applicantNotificationService = new();
    private readonly WorkerService _sut;
    private readonly Guid _agencyId = Guid.NewGuid();

    public WorkerServiceApplyTest()
    {
        _sut = new WorkerService(
            _workerRepository.Object,
            Mock.Of<IAgencyRepository>(),
            Mock.Of<ICompanyRepository>(),
            Mock.Of<INotificationRepository>(),
            _requestRepository.Object,
            _workerRequestRepository.Object,
            _identityServerService.Object,
            Mock.Of<ITeamsService>(),
            Mock.Of<IEmailService>(),
            Mock.Of<IRazorViewToStringRenderer>(),
            Options.Create(new TeamsWebhookConfiguration()),
            Mock.Of<ILogger<WorkerService>>(),
            Mock.Of<IWorkerAdapter>(),
            Mock.Of<IValidator<WorkerProfileCreateModel>>(),
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IFilesContainer>(),
            Mock.Of<IDocumentService>(),
            Mock.Of<ICandidateService>(),
            _candidateRepository.Object,
            Mock.Of<IUploadedFilesService>(),
            Mock.Of<ICatalogRepository>(),
            _applicantNotificationService.Object);
    }

    private RequestEntity SetupRequest(string city = "Toronto", int numberId = 4242)
    {
        var request = FakeData.FakeRequest(location: new Location
        {
            City = new City { Value = city, Province = new Province { Country = new Country() } }
        });
        request.NumberId = numberId;
        request.CompanyProfile = new CompanyProfile { AgencyId = _agencyId };
        _requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<RequestEntity, bool>>>()))
            .ReturnsAsync((Expression<Func<RequestEntity, bool>> e) => new[] { request }.AsQueryable().FirstOrDefault(e));
        return request;
    }

    private WorkerProfile SetupWorker(string email)
    {
        var profile = new WorkerProfile(new User(CvnEmail.Create(email).Value)) { AgencyId = _agencyId };
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
            .ReturnsAsync((Expression<Func<WorkerProfile, bool>> e) => new[] { profile }.AsQueryable().FirstOrDefault(e));
        return profile;
    }

    private Candidate SetupCandidate(string email, string address, bool dnu = false)
    {
        var candidate = new Candidate(_agencyId, "Jane Candidate", CvnEmail.Create(email).Value) { Address = address, Dnu = dnu };
        _candidateRepository.Setup(r => r.GetCandidate(It.IsAny<Expression<Func<Candidate, bool>>>()))
            .ReturnsAsync((Expression<Func<Candidate, bool>> e) => new[] { candidate }.AsQueryable().FirstOrDefault(e));
        return candidate;
    }

    private Task<Result<RequestApplicantDetailModel>> Apply(int numberId, string email) =>
        _sut.Apply(new WorkerRequestApplyModel { NumberId = numberId, Email = email });

    private Task<Result<RequestApplicantDetailModel>> ApplyAsSelf(Guid requestId, string comments = null) =>
        _sut.Apply(new WorkerRequestApplyModel { Comments = comments }, requestId);

    [Fact]
    public async Task FailsWhenRequestIsUnknown()
    {
        var result = await Apply(4242, "worker@mail.com");
        Assert.False(result);
    }

    [Fact]
    public async Task FailsWhenRequestIsNotOpen()
    {
        var request = SetupRequest();
        request.Cancel(DateTime.Now);
        SetupWorker("worker@mail.com");
        var result = await Apply(request.NumberId, "worker@mail.com");
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task FailsWhenEmailIsInvalid()
    {
        SetupRequest();
        var result = await Apply(4242, "not-an-email");
        Assert.False(result);
        _requestRepository.Verify(r => r.GetRequest(It.IsAny<Expression<Func<RequestEntity, bool>>>()), Times.Never);
    }

    [Fact]
    public async Task FailsWhenEmailIsUnknown()
    {
        SetupRequest();
        var result = await Apply(4242, "unknown@mail.com");
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task WorkerEmailMatchCreatesWorkerApplicant()
    {
        var request = SetupRequest();
        var profile = SetupWorker("worker@mail.com");
        var result = await Apply(4242, "worker@mail.com");
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().WorkerProfileId == profile.Id && e.Single().CandidateId == null)), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _applicantNotificationService.Verify(s => s.Notify(request, profile), Times.Once);
        _applicantNotificationService.Verify(s => s.Notify(It.IsAny<RequestEntity>(), It.IsAny<Candidate>()), Times.Never);
    }

    [Fact]
    public async Task WorkerEmailMatchIsCaseAndWhitespaceInsensitive()
    {
        SetupRequest();
        var profile = SetupWorker("worker@mail.com");
        var result = await Apply(4242, "  WORKER@MAIL.COM  ");
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().WorkerProfileId == profile.Id)), Times.Once);
    }

    [Fact]
    public async Task WorkerAppliesEvenWhenRequestHasNoCity()
    {
        SetupRequest(city: null);
        var profile = SetupWorker("worker@mail.com");
        var result = await Apply(4242, "worker@mail.com");
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().WorkerProfileId == profile.Id)), Times.Once);
    }

    [Fact]
    public async Task CandidateEmailMatchCreatesPendingApplicant()
    {
        var request = SetupRequest();
        var candidate = SetupCandidate("jane@mail.com", "25 Bay St, Toronto ON");
        var result = await Apply(4242, "jane@mail.com");
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().CandidateId == candidate.Id
            && e.Single().WorkerProfileId == null
            && e.Single().CreatedBy == "Sigook"
            && e.Single().Status == RequestApplicantStatus.Pending)), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _applicantNotificationService.Verify(s => s.Notify(request, candidate), Times.Once);
        _applicantNotificationService.Verify(s => s.Notify(It.IsAny<RequestEntity>(), It.IsAny<WorkerProfile>()), Times.Never);
    }

    [Fact]
    public async Task CandidateCityMatchIgnoresAccentsAndCasing()
    {
        SetupRequest(city: "Montreal");
        var candidate = SetupCandidate("jane@mail.com", "25 Bay St, MONTRÉAL QC");
        var result = await Apply(4242, "jane@mail.com");
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().CandidateId == candidate.Id)), Times.Once);
    }

    [Fact]
    public async Task CandidateFailsWhenCityDoesNotMatch()
    {
        SetupRequest();
        SetupCandidate("jane@mail.com", "25 Laurier Ave, Ottawa ON");
        var result = await Apply(4242, "jane@mail.com");
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task CandidateFailsWhenRequestHasNoCity()
    {
        SetupRequest(city: null);
        SetupCandidate("jane@mail.com", "25 Bay St, Toronto ON");
        var result = await Apply(4242, "jane@mail.com");
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task CandidateFailsWhenMarkedAsDnu()
    {
        SetupRequest();
        SetupCandidate("jane@mail.com", "25 Bay St, Toronto ON", dnu: true);
        var result = await Apply(4242, "jane@mail.com");
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task CandidateFailsWhenAlreadyApplied()
    {
        var request = SetupRequest();
        var candidate = SetupCandidate("jane@mail.com", "25 Bay St, Toronto ON");
        var applicant = RequestApplicant.CreateWithCandidate(request.Id, candidate.Id, "Sigook", null, RequestApplicantStatus.Pending).Value;
        _requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync(applicant);
        var result = await Apply(4242, "jane@mail.com");
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task CandidateEmailMatchIsCaseAndWhitespaceInsensitive()
    {
        SetupRequest();
        var candidate = SetupCandidate("jane@mail.com", "25 Bay St, Toronto ON");
        var result = await Apply(4242, "  JANE@Mail.com ");
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().CandidateId == candidate.Id)), Times.Once);
    }

    [Fact]
    public async Task SelfApplyResolvesTheWorkerFromTheToken()
    {
        var request = SetupRequest();
        var profile = SetupWorker("worker@mail.com");
        _identityServerService.Setup(s => s.GetUserId()).Returns(profile.WorkerId);
        var result = await ApplyAsSelf(request.Id, "Hard Worker");
        Assert.True(result);
        Assert.Equal(profile.Id, result.Value.WorkerProfileId);
        Assert.Equal(profile.WorkerId, result.Value.WorkerId);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().WorkerProfileId == profile.Id
            && e.Single().CandidateId == null
            && e.Single().Comments == "Hard Worker")), Times.Once);
    }

    [Fact]
    public async Task SelfApplyIgnoresTheEmailInTheBody()
    {
        var request = SetupRequest();
        var profile = SetupWorker("worker@mail.com");
        _identityServerService.Setup(s => s.GetUserId()).Returns(profile.WorkerId);
        var result = await _sut.Apply(new WorkerRequestApplyModel { Email = "someone.else@mail.com" }, request.Id);
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().WorkerProfileId == profile.Id)), Times.Once);
    }

    [Fact]
    public async Task SelfApplyFailsWhenTheWorkerHasNoProfile()
    {
        var request = SetupRequest();
        var result = await ApplyAsSelf(request.Id);
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task SelfApplyFailsWhenTheWorkerIsAlreadyBooked()
    {
        var request = SetupRequest();
        var profile = SetupWorker("worker@mail.com");
        _identityServerService.Setup(s => s.GetUserId()).Returns(profile.WorkerId);
        _workerRequestRepository.Setup(r => r.WorkerRequestExists(profile.Id, request.Id)).ReturnsAsync(true);
        var result = await ApplyAsSelf(request.Id);
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Fact]
    public async Task SelfApplyFailsWhenRequestIsNotOpen()
    {
        var request = SetupRequest();
        request.Cancel(DateTime.Now);
        var profile = SetupWorker("worker@mail.com");
        _identityServerService.Setup(s => s.GetUserId()).Returns(profile.WorkerId);
        var result = await ApplyAsSelf(request.Id);
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }
}
