using Azure.Messaging.ServiceBus;
using Covenant.Api.Consumers;
using Covenant.Common.Configuration;
using Covenant.Common.Constants;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models.Website;
using Covenant.Core.BL.Interfaces.Accounting.PayStubs;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Consumers;

public class ConsumersTest
{
    private static readonly ServiceBusConfiguration Configuration = new()
    {
        BulkPayStubEmailQueue = "bulk-paystub-email",
        InvitationQueue = "invitations",
        ValidateCandidateQueue = "validate-candidate",
        CreateApplicantTopic = "create-applicant"
    };

    private readonly Mock<ISigookBusClient> _bus = new();
    private readonly Mock<ServiceBusReceiver> _receiver = new();
    private readonly Mock<IBulkPayStubEmailService> _bulkPayStubEmailService = new();
    private readonly Mock<IRequestInvitationService> _invitationService = new();
    private readonly Mock<ICandidateApplicationService> _candidateApplicationService = new();
    private readonly IServiceScopeFactory _scopeFactory;
    private Func<ProcessMessageEventArgs, Task> _handler;
    private string _queue;
    private string _subscription;

    public ConsumersTest()
    {
        _scopeFactory = new ServiceCollection()
            .AddScoped(_ => _bulkPayStubEmailService.Object)
            .AddScoped(_ => _invitationService.Object)
            .AddScoped(_ => _candidateApplicationService.Object)
            .BuildServiceProvider()
            .GetRequiredService<IServiceScopeFactory>();
        _bus.Setup(b => b.CreateProcessorAsync(It.IsAny<string>(), It.IsAny<Func<ProcessMessageEventArgs, Task>>(), It.IsAny<Func<ProcessErrorEventArgs, Task>>()))
            .Callback<string, Func<ProcessMessageEventArgs, Task>, Func<ProcessErrorEventArgs, Task>>((queue, handler, _) => { _queue = queue; _handler = handler; })
            .Returns(Task.CompletedTask);
        _bus.Setup(b => b.CreateProcessorAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Func<ProcessMessageEventArgs, Task>>(), It.IsAny<Func<ProcessErrorEventArgs, Task>>()))
            .Callback<string, string, Func<ProcessMessageEventArgs, Task>, Func<ProcessErrorEventArgs, Task>>((topic, subscription, handler, _) => { _queue = topic; _subscription = subscription; _handler = handler; })
            .Returns(Task.CompletedTask);
    }

    private IOptions<ServiceBusConfiguration> Options => Microsoft.Extensions.Options.Options.Create(Configuration);

    private async Task<ServiceBusReceivedMessage> Deliver(IAzureServiceBusConsumer consumer, object body, IDictionary<string, object> properties = null)
    {
        await consumer.OnInit();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(BinaryData.FromObjectAsJson(body), properties: properties);
        await _handler(new ProcessMessageEventArgs(message, _receiver.Object, CancellationToken.None));
        return message;
    }

    private void VerifyCompleted(ServiceBusReceivedMessage message) =>
        _receiver.Verify(r => r.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);

    [Fact]
    public async Task BulkPayStubEmailConsumer_SendsJobAndCompletesMessage()
    {
        var job = new BulkPayStubEmailJob(Guid.NewGuid(), "accounting", [Guid.NewGuid(), Guid.NewGuid()]);
        BulkPayStubEmailJob received = null;
        _bulkPayStubEmailService.Setup(s => s.Send(It.IsAny<BulkPayStubEmailJob>(), It.IsAny<CancellationToken>()))
            .Callback<BulkPayStubEmailJob, CancellationToken>((j, _) => received = j)
            .Returns(Task.CompletedTask);

        var message = await Deliver(new BulkPayStubEmailConsumer(_bus.Object, _scopeFactory, Options), job);

        Assert.Equal(Configuration.BulkPayStubEmailQueue, _queue);
        Assert.Equal(job.AgencyId, received.AgencyId);
        Assert.Equal(job.PayStubIds, received.PayStubIds);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task InvitationConsumer_SendsInvitationsAndCompletesMessage()
    {
        var job = new SendInvitationJob(Guid.NewGuid(), "recruiter");

        var message = await Deliver(new InvitationConsumer(_bus.Object, _scopeFactory, Options), job);

        Assert.Equal(Configuration.InvitationQueue, _queue);
        _invitationService.Verify(s => s.Send(job), Times.Once);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task NewCandidateConsumer_ProcessesApplicationAndCompletesMessage()
    {
        var candidate = new CandidateViewModel { Email = "nina@mail.com", FullName = "Nina New", RequestId = Guid.NewGuid() };
        CandidateViewModel received = null;
        _candidateApplicationService.Setup(s => s.ProcessApplication(It.IsAny<CandidateViewModel>()))
            .Callback<CandidateViewModel>(c => received = c)
            .Returns(Task.CompletedTask);

        var message = await Deliver(new NewCandidateConsumer(_bus.Object, _scopeFactory, Options), candidate);

        Assert.Equal(Configuration.ValidateCandidateQueue, _queue);
        Assert.Equal("nina@mail.com", received.Email);
        Assert.Equal(candidate.RequestId, received.RequestId);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task NewCandidateConsumer_NullBody_OnlyCompletesMessage()
    {
        var message = await Deliver(new NewCandidateConsumer(_bus.Object, _scopeFactory, Options), null);

        _candidateApplicationService.Verify(s => s.ProcessApplication(It.IsAny<CandidateViewModel>()), Times.Never);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task RequestApplicantConsumer_RegistersApplicantFromTopic()
    {
        var applicant = RequestApplicant.CreateWithCandidate(Guid.NewGuid(), Guid.NewGuid(), "Sigook", string.Empty, RequestApplicantStatus.Pending).Value;
        RequestApplicant received = null;
        _candidateApplicationService.Setup(s => s.RegisterApplicant(It.IsAny<RequestApplicant>()))
            .Callback<RequestApplicant>(a => received = a)
            .Returns(Task.CompletedTask);

        var message = await Deliver(new RequestApplicantConsumer(_bus.Object, _scopeFactory, Options), applicant);

        Assert.Equal(Configuration.CreateApplicantTopic, _queue);
        Assert.Equal(TopicSubscription.RequestApplicantNotification.Subscription, _subscription);
        Assert.Equal(applicant.Id, received.Id);
        Assert.Equal(applicant.CandidateId, received.CandidateId);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task TeamsConsumer_Error_NotifiesFailure()
    {
        var message = await Deliver(new TeamsConsumer(_bus.Object, _scopeFactory, Options), null,
            new Dictionary<string, object> { [ServiceBusSqlConstants.ErrorCandidate] = "Invalid email" });

        Assert.Equal(TopicSubscription.TeamsNotification.Subscription, _subscription);
        _candidateApplicationService.Verify(s => s.NotifyFailure("Invalid email"), Times.Once);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task TeamsConsumer_Application_NotifiesApplication()
    {
        var applicant = RequestApplicant.CreateWithCandidate(Guid.NewGuid(), Guid.NewGuid(), "Sigook", string.Empty, RequestApplicantStatus.Pending).Value;

        var message = await Deliver(new TeamsConsumer(_bus.Object, _scopeFactory, Options), applicant,
            new Dictionary<string, object> { [ServiceBusSqlConstants.RequestApplication] = true });

        _candidateApplicationService.Verify(s => s.NotifyApplication(It.Is<RequestApplicant>(a => a.Id == applicant.Id)), Times.Once);
        _candidateApplicationService.Verify(s => s.NotifyCreation(It.IsAny<CandidateCreateModel>()), Times.Never);
        VerifyCompleted(message);
    }

    [Fact]
    public async Task TeamsConsumer_Creation_NotifiesCreation()
    {
        var model = new CandidateCreateModel { Name = "Nina New", Email = "nina@mail.com" };

        var message = await Deliver(new TeamsConsumer(_bus.Object, _scopeFactory, Options), model,
            new Dictionary<string, object> { [ServiceBusSqlConstants.RequestApplication] = false });

        _candidateApplicationService.Verify(s => s.NotifyCreation(It.Is<CandidateCreateModel>(m => m.Name == "Nina New")), Times.Once);
        _candidateApplicationService.Verify(s => s.NotifyApplication(It.IsAny<RequestApplicant>()), Times.Never);
        VerifyCompleted(message);
    }
}
