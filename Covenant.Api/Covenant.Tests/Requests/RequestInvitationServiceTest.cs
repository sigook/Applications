using Covenant.Common.Configuration;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Resources;
using Covenant.Core.BL.Services.Requests;
using Covenant.Tests.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Requests;

public class RequestInvitationServiceTest
{
    private const string Webhook = "https://teams/candidates";
    private static readonly DateTime Now = new(2026, 10, 23, 9, 0, 0);

    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<IWorkerRepository> _workerRepository = new();
    private readonly Mock<ICandidateRepository> _candidateRepository = new();
    private readonly Mock<ISendGridService> _sendGrid = new();
    private readonly Mock<ITeamsService> _teams = new();
    private readonly RequestInvitationService _sut;
    private readonly City _toronto = new() { Value = "Toronto", Province = new Province { Code = "ON", Country = new Country { Code = "CA" } } };
    private readonly Request _request;
    private TeamsNotificationModel _notification;
    private IReadOnlyCollection<TemplateRecipient> _recipients;
    private string _from;

    public RequestInvitationServiceTest()
    {
        var agency = new Agency("Test Agency", null) { RecruitmentEmail = "recruit@agency.com" };
        var company = new CompanyProfile(new User(CvnEmail.Create("company@mail.com").Value), agency, "Acme", "4165551234", new CompanyProfileIndustry("Test"));
        var location = new Location { City = _toronto, CityId = _toronto.Id };
        _request = FakeData.FakeRequest(location: location).With(r =>
        {
            r.CompanyProfile = company;
            r.WorkerRate = 21.5m;
            r.NumberId = 42;
        });

        _requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<Request, bool>>>())).ReturnsAsync(_request);
        _workerRepository.Setup(r => r.GetWorkersAvailableToInvite(It.IsAny<Guid>(), _toronto.Id)).ReturnsAsync([]);
        _candidateRepository.Setup(r => r.GetCandidatesAvailableToInvite(It.IsAny<Guid>(), _request.Id)).ReturnsAsync([]);
        _sendGrid.Setup(s => s.SendTemplateBatch(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<TemplateRecipient>>()))
            .Callback<string, IReadOnlyCollection<TemplateRecipient>>((from, recipients) => { _from = from; _recipients = recipients; })
            .ReturnsAsync(Result.Ok());
        _teams.Setup(t => t.SendNotification(Webhook, It.IsAny<TeamsNotificationModel>()))
            .Callback<string, TeamsNotificationModel>((_, n) => _notification = n)
            .ReturnsAsync(Result.Ok());

        _sut = new RequestInvitationService(
            _requestRepository.Object,
            _workerRepository.Object,
            _candidateRepository.Object,
            new FakeTimeProvider(new DateTimeOffset(Now, TimeSpan.Zero)),
            _sendGrid.Object,
            _teams.Object,
            Options.Create(new SendGridConfiguration
            {
                UnsubscribeUrl = "https://sigook/unsubscribe?email={{email}}",
                ApplyOnlineUrl = "https://sigook/apply/{{numberId}}?email={{email}}"
            }),
            Options.Create(new TeamsWebhookConfiguration { CandidateAndWorker = Webhook }),
            NullLogger<RequestInvitationService>.Instance);
    }

    private static WorkerContactInfoModel Worker(string email, string first = "Will", string last = "Worker") =>
        new() { WorkerId = Guid.NewGuid(), Email = email, FirstName = first, LastName = last };

    private static CandidateContactInfoModel Candidate(string email, Guid? cityId = null, string address = null, string name = "Cara Candidate") =>
        new() { Id = Guid.NewGuid(), Email = email, Name = name, CityId = cityId, Address = address };

    private void Workers(params WorkerContactInfoModel[] workers) =>
        _workerRepository.Setup(r => r.GetWorkersAvailableToInvite(It.IsAny<Guid>(), _toronto.Id)).ReturnsAsync([.. workers]);

    private void Candidates(params CandidateContactInfoModel[] candidates) =>
        _candidateRepository.Setup(r => r.GetCandidatesAvailableToInvite(It.IsAny<Guid>(), _request.Id)).ReturnsAsync([.. candidates]);

    private static string Field(TemplateRecipient recipient, string name) =>
        recipient.Data.GetType().GetProperty(name)!.GetValue(recipient.Data)?.ToString();

    [Fact]
    public async Task Send_InvitesWorkersAndSameCityCandidates_AndMarksRequest()
    {
        Workers(Worker("will@mail.com"));
        Candidates(
            Candidate("cara@mail.com", cityId: _toronto.Id),
            Candidate("far@mail.com", cityId: Guid.NewGuid()),
            Candidate("addr@mail.com", address: "12 King St, Toronto ON"));

        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        Assert.Equal("recruit@agency.com", _from);
        Assert.Equal(["will@mail.com", "cara@mail.com", "addr@mail.com"], _recipients.Select(r => r.Email));
        Assert.Equal(Now, _request.InvitationSentItAt);
        _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.Equal("Invitations sent by recruiter", _notification.Title);
        Assert.Equal($"Sent 1 worker and 2 candidate invitations for request #42 ({_request.JobTitle})", _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateSuccess("t", "t").ThemeColor, _notification.ThemeColor);
    }

    [Fact]
    public async Task Send_SameEmailAsWorkerAndCandidate_IsInvitedOnce()
    {
        Workers(Worker("Dup@Mail.com"), Worker("dup@mail.com"));
        Candidates(Candidate("DUP@mail.com", cityId: _toronto.Id));

        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        Assert.Single(_recipients);
        Assert.Contains("Sent 1 worker and 0 candidate", _notification.Text);
    }

    [Fact]
    public async Task Send_BuildsTemplateDataWithEscapedLinksAndCanadianRate()
    {
        Workers(Worker("w+1@mail.com", "Ana", "Lee"));

        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        TemplateRecipient recipient = Assert.Single(_recipients);
        Assert.Equal("Ana Lee", recipient.FullName);
        Assert.Equal("Ana Lee", Field(recipient, "worker_name"));
        Assert.Equal("https://sigook/apply/42?email=w%2B1%40mail.com", Field(recipient, "apply"));
        Assert.Equal("https://sigook/unsubscribe?email=w%2B1%40mail.com", Field(recipient, "unsubscribe"));
        Assert.Equal("Toronto", Field(recipient, "city"));
        Assert.Equal(21.5m.ToString("C", new System.Globalization.CultureInfo("en-CA")), Field(recipient, "rate"));
    }

    [Fact]
    public async Task Send_WithoutRecipients_DoesNotEmailNorMarkRequest()
    {
        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        _sendGrid.Verify(s => s.SendTemplateBatch(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<TemplateRecipient>>()), Times.Never);
        Assert.Null(_request.InvitationSentItAt);
        Assert.Contains("Sent 0 worker and 0 candidate", _notification.Text);
    }

    [Fact]
    public async Task Send_UnknownRequest_NotifiesWarning()
    {
        _requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<Request, bool>>>())).ReturnsAsync((Request)null);

        await _sut.Send(new SendInvitationJob(Guid.NewGuid(), "recruiter"));

        Assert.Equal(ApiResources.RequestNotAvailable, _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateWarning("t", "t").ThemeColor, _notification.ThemeColor);
        _sendGrid.Verify(s => s.SendTemplateBatch(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<TemplateRecipient>>()), Times.Never);
    }

    [Fact]
    public async Task Send_CancelledRequest_NotifiesWarning()
    {
        Assert.True(_request.Cancel(Now));
        Workers(Worker("will@mail.com"));

        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        Assert.Equal(ApiResources.RequestNotAvailable, _notification.Text);
        _sendGrid.Verify(s => s.SendTemplateBatch(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<TemplateRecipient>>()), Times.Never);
    }

    [Fact]
    public async Task Send_InvitationSentRecently_NotifiesWarning()
    {
        _request.MarkInvitationSent(Now.AddDays(-1));
        Workers(Worker("will@mail.com"));

        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        Assert.StartsWith("The invitation must be sent it only once every", _notification.Text);
        _sendGrid.Verify(s => s.SendTemplateBatch(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<TemplateRecipient>>()), Times.Never);
    }

    [Fact]
    public async Task Send_Exception_NotifiesError()
    {
        _workerRepository.Setup(r => r.GetWorkersAvailableToInvite(It.IsAny<Guid>(), It.IsAny<Guid>())).ThrowsAsync(new InvalidOperationException("db down"));

        await _sut.Send(new SendInvitationJob(_request.Id, "recruiter"));

        Assert.Equal("db down", _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateError("t", "t").ThemeColor, _notification.ThemeColor);
    }
}
