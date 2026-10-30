using Covenant.Common.Interfaces;
using Covenant.Common.Models.Notifications;
using Covenant.Common.Repositories.Notifications;
using Covenant.Common.Repositories.Requests;
using Covenant.Core.BL.Services.Notifications;
using Moq;
using Xunit;

namespace Covenant.Tests.Notifications;

public class NotificationServiceTest
{
    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Guid _agencyId = Guid.NewGuid();
    private readonly NotificationService _sut;

    public NotificationServiceTest()
    {
        _currentUserService.Setup(c => c.GetAgencyId()).Returns(_agencyId);
        _requestRepository.Setup(r => r.GetPendingReviewRequests(_agencyId)).ReturnsAsync(
            [new PendingReviewRequestNotificationModel { RequestId = Guid.NewGuid(), NumberId = 10, JobTitle = "Picker" }]);
        _sut = new NotificationService(Mock.Of<INotificationRepository>(), _requestRepository.Object, _currentUserService.Object);
    }

    [Fact]
    public async Task RecruitingSeesPendingReviewRequestsOfTheirAgency()
    {
        NotificationsModel result = await _sut.GetNotifications();

        Assert.Single(result.PendingReviewRequests);
    }

    [Fact]
    public async Task SalesDoesNotSeePendingReviewRequests()
    {
        _currentUserService.Setup(c => c.IsSales()).Returns(true);

        NotificationsModel result = await _sut.GetNotifications();

        Assert.Empty(result.PendingReviewRequests);
        _requestRepository.Verify(r => r.GetPendingReviewRequests(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AdminWithSalesRoleSeesPendingReviewRequests()
    {
        _currentUserService.Setup(c => c.IsSales()).Returns(true);
        _currentUserService.Setup(c => c.IsAdmin()).Returns(true);

        NotificationsModel result = await _sut.GetNotifications();

        Assert.Single(result.PendingReviewRequests);
    }
}
