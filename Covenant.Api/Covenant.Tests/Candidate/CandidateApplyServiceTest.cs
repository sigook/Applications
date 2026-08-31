using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;
using CandidateEntity = Covenant.Common.Entities.Candidate.Candidate;
using RequestEntity = Covenant.Common.Entities.Request.Request;

namespace Covenant.Tests.Candidate;

public class CandidateApplyServiceTest
{
    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<ICandidateRepository> _candidateRepository = new();
    private readonly Mock<ISendGridService> _sendGridService = new();
    private readonly CandidateService _sut;

    private readonly Guid _requestId = Guid.NewGuid();
    private readonly Guid _candidateId = Guid.NewGuid();
    private readonly Guid _agencyId = Guid.NewGuid();

    public CandidateApplyServiceTest()
    {
        _sut = new CandidateService(
            Mock.Of<IUserRepository>(),
            _candidateRepository.Object,
            _requestRepository.Object,
            Mock.Of<IAgencyRepository>(),
            Mock.Of<IWorkerRepository>(),
            Mock.Of<ICandidateAdapter>(),
            Mock.Of<IIdentityServerService>(),
            Mock.Of<IDocumentService>(),
            Mock.Of<IValidator<CandidateCsvModel>>(),
            Mock.Of<IUploadedFilesService>(),
            _sendGridService.Object,
            Mock.Of<ILogger<CandidateService>>());
    }

    private RequestEntity SetupRequest(string city = "Toronto")
    {
        var request = RequestEntity.AgencyCreateRequest(
            Guid.NewGuid(),
            new Location { City = new City { Value = city } },
            default,
            jobTitle: "Forklift Operator").Value;
        request.CompanyProfile = new CompanyProfile { AgencyId = _agencyId };
        _requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<RequestEntity, bool>>>())).ReturnsAsync(request);
        return request;
    }

    private CandidateEntity SetupCandidate(
        string address = "123 Main St, Toronto, ON",
        bool dnu = false,
        Guid? agencyId = null)
    {
        var candidate = new CandidateEntity(agencyId ?? _agencyId, "Test Candidate")
        {
            Id = _candidateId,
            Email = "candidate@test.com",
            Address = address,
            Dnu = dnu
        };
        _candidateRepository.Setup(r => r.GetCandidate(It.IsAny<Expression<Func<CandidateEntity, bool>>>())).ReturnsAsync(candidate);
        return candidate;
    }

    [Fact]
    public async Task ApplyFailsWhenRequestDoesNotExist()
    {
        _requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<RequestEntity, bool>>>())).ReturnsAsync((RequestEntity)null);
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyFailsWhenRequestIsNotOpen()
    {
        var request = SetupRequest();
        request.AddWorker(Guid.NewGuid(), new DateTime(2019, 01, 01));
        SetupCandidate();
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyFailsWhenCandidateDoesNotExist()
    {
        SetupRequest();
        _candidateRepository.Setup(r => r.GetCandidate(It.IsAny<Expression<Func<CandidateEntity, bool>>>())).ReturnsAsync((CandidateEntity)null);
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyFailsWhenCandidateIsDnu()
    {
        SetupRequest();
        SetupCandidate(dnu: true);
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyFailsWhenCandidateBelongsToAnotherAgency()
    {
        SetupRequest();
        SetupCandidate(agencyId: Guid.NewGuid());
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyFailsWhenAddressDoesNotContainTheRequestCity()
    {
        SetupRequest();
        SetupCandidate(address: "456 King St, Hamilton, ON");
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ApplyFailsWhenTheRequestCityIsBlank(string city)
    {
        SetupRequest(city);
        SetupCandidate();
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
    }

    [Fact]
    public async Task ApplyFailsWhenCandidateAlreadyApplied()
    {
        SetupRequest();
        SetupCandidate();
        var existing = RequestApplicant.CreateWithCandidate(_requestId, _candidateId, "Sigook", string.Empty, RequestApplicantStatus.Pending).Value;
        _requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync(existing);
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.False(result);
        _requestRepository.Verify(r => r.Create(It.IsAny<IEnumerable<RequestApplicant>>()), Times.Never);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ApplyCreatesPendingApplicantWithCandidateAndSkill()
    {
        SetupRequest();
        var candidate = SetupCandidate();
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.True(result);
        _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestApplicant>>(e =>
            e.Single().CandidateId == _candidateId
            && e.Single().WorkerProfileId == null
            && e.Single().RequestId == _requestId
            && e.Single().Status == RequestApplicantStatus.Pending
            && e.Single().CreatedBy == "Sigook")), Times.Once);
        _candidateRepository.Verify(r => r.Create(It.Is<CandidateSkill>(s => s.CandidateId == _candidateId && s.Skill == "Forklift Operator")), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Contains(candidate.Skills, s => s.Skill == "Forklift Operator");
        _sendGridService.Verify(s => s.SendEmail(It.IsAny<Covenant.Common.Models.SendGridModel>()), Times.Never);
    }

    [Fact]
    public async Task ApplySucceedsWhenTheSkillAlreadyExists()
    {
        SetupRequest();
        var candidate = SetupCandidate();
        candidate.AddSkill("Forklift Operator");
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.True(result);
        _candidateRepository.Verify(r => r.Create(It.IsAny<CandidateSkill>()), Times.Never);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ApplyMatchesTheCityIgnoringCaseAndDiacritics()
    {
        SetupRequest("Montreal");
        SetupCandidate(address: "12 Rue Sainte-Catherine, MONTRÉAL");
        var result = await _sut.Apply(_candidateId, _requestId);
        Assert.True(result);
    }
}
