using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Models.Notifications;
using Covenant.Core.BL.Interfaces.Accounting.PayStubs;
using Covenant.Core.BL.Services.Accounting.PayStubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting.PayStubs;

public class BulkPayStubEmailServiceTest
{
    private const string Webhook = "https://teams/accounting";

    private readonly Mock<IPayStubService> _payStubService = new();
    private readonly Mock<ITeamsService> _teams = new();
    private readonly BulkPayStubEmailService _sut;
    private TeamsNotificationModel _notification;

    public BulkPayStubEmailServiceTest()
    {
        var provider = new ServiceCollection()
            .AddScoped(_ => _payStubService.Object)
            .BuildServiceProvider();
        _teams.Setup(t => t.SendNotification(Webhook, It.IsAny<TeamsNotificationModel>()))
            .Callback<string, TeamsNotificationModel>((_, n) => _notification = n)
            .ReturnsAsync(Result.Ok());

        _sut = new BulkPayStubEmailService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            _teams.Object,
            Options.Create(new TeamsWebhookConfiguration { Accounting = Webhook }),
            NullLogger<BulkPayStubEmailService>.Instance);
    }

    private void Sends(Guid payStubId, string workerFullName) =>
        _payStubService.Setup(s => s.SendPayStubEmail(payStubId))
            .ReturnsAsync(new PayStubEmailResult(payStubId, true, workerFullName, "PS-1"));

    [Fact]
    public async Task Send_AllDelivered_NotifiesSuccess()
    {
        Guid first = Guid.NewGuid(), second = Guid.NewGuid();
        Sends(first, "Jane Doe");
        Sends(second, "John Roe");

        await _sut.Send(new BulkPayStubEmailJob(Guid.NewGuid(), "accounting", [first, second]), CancellationToken.None);

        _payStubService.Verify(s => s.SendPayStubEmail(It.IsAny<Guid>()), Times.Exactly(2));
        Assert.Equal("PayStubs sent by accounting", _notification.Title);
        Assert.Equal("Sent 2 of 2", _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateSuccess("t", "t").ThemeColor, _notification.ThemeColor);
    }

    [Fact]
    public async Task Send_FailuresAndExceptions_AreReportedAsWarning()
    {
        Guid delivered = Guid.NewGuid(), rejected = Guid.NewGuid(), crashed = Guid.NewGuid();
        Sends(delivered, "Jane Doe");
        _payStubService.Setup(s => s.SendPayStubEmail(rejected))
            .ReturnsAsync(new PayStubEmailResult(rejected, false, "John Roe", "PS-2"));
        _payStubService.Setup(s => s.SendPayStubEmail(crashed)).ThrowsAsync(new InvalidOperationException("smtp down"));

        await _sut.Send(new BulkPayStubEmailJob(Guid.NewGuid(), "accounting", [delivered, rejected, crashed]), CancellationToken.None);

        Assert.StartsWith("Sent 1 of 3 - Failed: ", _notification.Text);
        Assert.Contains("John Roe", _notification.Text);
        Assert.Contains(crashed.ToString(), _notification.Text);
        Assert.DoesNotContain("Jane Doe", _notification.Text);
        Assert.Equal(TeamsNotificationModel.CreateWarning("t", "t").ThemeColor, _notification.ThemeColor);
    }

    [Fact]
    public async Task Send_EmptyJob_StillNotifies()
    {
        await _sut.Send(new BulkPayStubEmailJob(Guid.NewGuid(), "accounting", []), CancellationToken.None);

        Assert.Equal("Sent 0 of 0", _notification.Text);
        _payStubService.Verify(s => s.SendPayStubEmail(It.IsAny<Guid>()), Times.Never);
    }
}
