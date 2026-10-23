using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Website;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Core.BL.Services.Candidates;
using Covenant.Tests.Utils;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Candidates;

public class CandidateApplicationServiceTest
{
    private const string Topic = "create-applicant";
    private const string Webhook = "https://teams/candidates";

    private readonly Mock<IValidator<CandidateViewModel>> _validator = new();
    private readonly Mock<ICandidateRepository> _candidateRepository = new();
    private readonly Mock<IWorkerRepository> _workerRepository = new();
    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<IAgencyRepository> _agencyRepository = new();
    private readonly Mock<ICatalogRepository> _catalogRepository = new();
    private readonly Mock<ICandidateService> _candidateService = new();
    private readonly Mock<IFilesContainer> _filesContainer = new();
    private readonly Mock<IMessagePublisher> _publisher = new();
    private readonly Mock<ITeamsService> _teams = new();
    private readonly CandidateApplicationService _sut;
    private readonly Agency _masterAgency = new("Master Agency", null);
    private readonly CityModel _city = new(Guid.NewGuid(), "Toronto") { Province = new ProvinceModel { Country = new CountryModel { Id = Guid.Empty } } };
    private readonly Request _request = FakeData.FakeRequest().With(r => r.NumberId = 77);
    private object _body;
    private IReadOnlyDictionary<string, object> _properties;
    private TeamsNotificationModel _notification;

    public CandidateApplicationServiceTest()
    {
        _validator.Setup(v => v.ValidateAsync(It.IsAny<CandidateViewModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        _catalogRepository.Setup(c => c.GetCity(_city.Id)).ReturnsAsync(_city);
        _agencyRepository.Setup(a => a.GetAgencyMasterByCountry(It.IsAny<Guid>())).ReturnsAsync(_masterAgency);
        _publisher.Setup(p => p.SendMessageAsync(It.IsAny<object>(), Topic, It.IsAny<IReadOnlyDictionary<string, object>>()))
            .Callback<object, string, IReadOnlyDictionary<string, object>>((body, _, properties) => { _body = body; _properties = properties; })
            .Returns(Task.CompletedTask);
        _teams.Setup(t => t.SendNotification(Webhook, It.IsAny<TeamsNotificationModel>()))
            .Callback<string, TeamsNotificationModel>((_, n) => _notification = n)
            .ReturnsAsync(Result.Ok());

        _sut = new CandidateApplicationService(
            _validator.Object,
            _candidateRepository.Object,
            _workerRepository.Object,
            _requestRepository.Object,
            _agencyRepository.Object,
            _catalogRepository.Object,
            _candidateService.Object,
            _filesContainer.Object,
            _publisher.Object,
            _teams.Object,
            Options.Create(new ServiceBusConfiguration { CreateApplicantTopic = Topic }),
            Options.Create(new TeamsWebhookConfiguration { CandidateAndWorker = Webhook }));
    }

    private CandidateViewModel Message(string email = "new@mail.com", Guid? requestId = null, string fullName = "Nina New") => new()
    {
        Email = email,
        FullName = fullName,
        Phone = "4165550000",
        CityId = _city.Id,
        Address = "1 King St",
        FileName = "resume.pdf",
        Skills = ["Forklift"],
        RequestId = requestId
    };

    private void RequestExists() =>
        _requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<Request, bool>>>())).ReturnsAsync(_request);

    private void WorkerExists(WorkerProfile worker) =>
        _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(worker);

    private void CandidateExists(Candidate candidate) =>
        _candidateRepository.Setup(r => r.GetCandidate(It.IsAny<Expression<Func<Candidate, bool>>>())).ReturnsAsync(candidate);

    private string Error => _properties.TryGetValue(ServiceBusSqlConstants.ErrorCandidate, out var error) ? error.ToString() : null;

    private bool? IsApplication => _properties.TryGetValue(ServiceBusSqlConstants.RequestApplication, out var value) ? (bool)value : null;

    [Fact]
    public async Task ProcessApplication_InvalidMessage_PublishesErrorAndDeletesResume()
    {
        _validator.Setup(v => v.ValidateAsync(It.IsAny<CandidateViewModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure("Email", "Email is required")]));

        await _sut.ProcessApplication(Message());

        Assert.Null(_body);
        Assert.Contains("Email is required", Error);
        Assert.Null(IsApplication);
        _filesContainer.Verify(f => f.DeleteFileIfExists("resume.pdf"), Times.Once);
    }

    [Fact]
    public async Task ProcessApplication_InvalidEmail_PublishesReadableError()
    {
        await _sut.ProcessApplication(Message("not-an-email"));

        Assert.False(string.IsNullOrWhiteSpace(Error));
        Assert.DoesNotContain(typeof(ResultError).FullName, Error);
    }

    [Fact]
    public async Task ProcessApplication_ExistingWorker_PublishesWorkerApplication()
    {
        var worker = new WorkerProfile(new User(CvnEmail.Create("will@mail.com").Value), Guid.NewGuid());
        WorkerExists(worker);
        RequestExists();

        await _sut.ProcessApplication(Message("Will@Mail.com", _request.Id));

        var applicant = Assert.IsType<RequestApplicant>(_body);
        Assert.Equal(worker.Id, applicant.WorkerProfileId);
        Assert.Null(applicant.CandidateId);
        Assert.Equal(_request.Id, applicant.RequestId);
        Assert.True(IsApplication);
        Assert.Null(Error);
        _candidateService.Verify(s => s.CreateCandidate(It.IsAny<CandidateCreateModel>(), It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ProcessApplication_ExistingCandidate_PublishesCandidateApplication()
    {
        var candidate = new Candidate(Guid.NewGuid(), "Cara", CvnEmail.Create("cara@mail.com").Value);
        CandidateExists(candidate);
        RequestExists();

        await _sut.ProcessApplication(Message("cara@mail.com", _request.Id));

        var applicant = Assert.IsType<RequestApplicant>(_body);
        Assert.Equal(candidate.Id, applicant.CandidateId);
        Assert.True(IsApplication);
    }

    [Fact]
    public async Task ProcessApplication_ExistingPersonWithoutRequest_PublishesAlreadyRegistered()
    {
        CandidateExists(new Candidate(Guid.NewGuid(), "Cara", CvnEmail.Create("cara@mail.com").Value));

        await _sut.ProcessApplication(Message("cara@mail.com"));

        Assert.Equal("Candidate with email: cara@mail.com is already registered", Error);
        _filesContainer.Verify(f => f.DeleteFileIfExists("resume.pdf"), Times.Once);
    }

    [Fact]
    public async Task ProcessApplication_NewPersonWithoutName_PublishesNotRegistered()
    {
        await _sut.ProcessApplication(Message(fullName: " "));

        Assert.Equal("Candidate with email: new@mail.com is not registered", Error);
    }

    [Fact]
    public async Task ProcessApplication_CityWithoutMasterAgency_PublishesError()
    {
        _agencyRepository.Setup(a => a.GetAgencyMasterByCountry(It.IsAny<Guid>())).ReturnsAsync((Agency)null);

        await _sut.ProcessApplication(Message());

        Assert.Contains("without a master agency", Error);
        _candidateService.Verify(s => s.CreateCandidate(It.IsAny<CandidateCreateModel>(), It.IsAny<Guid>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task ProcessApplication_NewPersonWithoutRequest_CreatesCandidateInMasterAgency()
    {
        CandidateCreateModel created = null;
        _candidateService.Setup(s => s.CreateCandidate(It.IsAny<CandidateCreateModel>(), _masterAgency.Id, false))
            .Callback<CandidateCreateModel, Guid, bool>((m, _, _) => created = m)
            .ReturnsAsync(Result.Ok(Guid.NewGuid()));

        await _sut.ProcessApplication(Message());

        Assert.Same(created, _body);
        Assert.False(IsApplication);
        Assert.Null(Error);
        Assert.Equal("Nina New", created.Name);
        Assert.Equal("new@mail.com", created.Email);
        Assert.Equal(_city, created.City);
        Assert.Equal("4165550000", Assert.Single(created.PhoneNumbers).PhoneNumber);
        Assert.Equal(["Forklift"], created.Skills.Select(s => s.Skill));
        _filesContainer.Verify(f => f.DeleteFileIfExists(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ProcessApplication_NewPersonWithRequest_CreatesCandidateAndApplication_AddingJobTitleSkill()
    {
        RequestExists();
        var candidateId = Guid.NewGuid();
        CandidateCreateModel created = null;
        _candidateService.Setup(s => s.CreateCandidate(It.IsAny<CandidateCreateModel>(), _masterAgency.Id, false))
            .Callback<CandidateCreateModel, Guid, bool>((m, _, _) => created = m)
            .ReturnsAsync(Result.Ok(candidateId));

        await _sut.ProcessApplication(Message(requestId: _request.Id));

        var applicant = Assert.IsType<RequestApplicant>(_body);
        Assert.Equal(candidateId, applicant.CandidateId);
        Assert.True(IsApplication);
        Assert.Contains(created.Skills, s => s.Skill == _request.JobTitle);
    }

    [Fact]
    public async Task ProcessApplication_NewPersonWithRequestInAnotherCountry_PublishesError()
    {
        RequestExists();
        _city.Province.Country.Id = Guid.NewGuid();
        _candidateService.Setup(s => s.CreateCandidate(It.IsAny<CandidateCreateModel>(), It.IsAny<Guid>(), false)).ReturnsAsync(Result.Ok(Guid.NewGuid()));

        await _sut.ProcessApplication(Message(requestId: _request.Id));

        Assert.Contains("does not match the one in the request", Error);
    }

    [Fact]
    public async Task RegisterApplicant_New_IsSaved()
    {
        var applicant = RequestApplicant.CreateWithCandidate(_request.Id, Guid.NewGuid(), "Sigook", string.Empty, Covenant.Common.Enums.RequestApplicantStatus.Pending).Value;

        await _sut.RegisterApplicant(applicant);

        _requestRepository.Verify(r => r.Create(applicant), Times.Once);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterApplicant_AlreadyApplied_IsIgnored()
    {
        var applicant = RequestApplicant.CreateWithCandidate(_request.Id, Guid.NewGuid(), "Sigook", string.Empty, Covenant.Common.Enums.RequestApplicantStatus.Pending).Value;
        _requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync(applicant);

        await _sut.RegisterApplicant(applicant);

        _requestRepository.Verify(r => r.Create(It.IsAny<RequestApplicant>()), Times.Never);
    }

    [Fact]
    public async Task NotifyApplication_SendsCandidateNameAndRequestNumber()
    {
        var candidate = new Candidate(Guid.NewGuid(), "Cara Candidate");
        CandidateExists(candidate);
        RequestExists();
        var applicant = RequestApplicant.CreateWithCandidate(_request.Id, candidate.Id, "Sigook", string.Empty, Covenant.Common.Enums.RequestApplicantStatus.Pending).Value;

        await _sut.NotifyApplication(applicant);

        Assert.Equal("Candidate Cara Candidate applied to request number 77", _notification.Text);
    }

    [Fact]
    public async Task NotifyCreation_And_NotifyFailure_UseExpectedSeverity()
    {
        await _sut.NotifyCreation(new CandidateCreateModel { Name = "Nina New" });
        Assert.Equal("Candidate Nina New has been created", _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateSuccess("t", "t").ThemeColor, _notification.ThemeColor);

        await _sut.NotifyFailure("boom");
        Assert.Equal("boom", _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateError("t", "t").ThemeColor, _notification.ThemeColor);
    }
}
