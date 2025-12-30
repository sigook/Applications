using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Request
{
    public class AgencyRejectWorkerServiceTest
    {
        [Fact]
        public async Task Reject()
        {
            var workerId = Guid.NewGuid();
            var request = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(workerId, Guid.NewGuid(), new Location(), default, default, workersQuantity: 1).Value;
            request.AddWorker(workerId, new DateTime(2019, 01, 01));
            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<Covenant.Common.Entities.Request.Request, bool>>>())).ReturnsAsync(request);

            var sut = new RequestService(
                Mock.Of<ICompanyRepository>(),
                Mock.Of<ILocationRepository>(),
                Mock.Of<ITimeService>(),
                requestRepository.Object,
                Mock.Of<INotificationDataRepository>(),
                Mock.Of<IPushNotifications>(),
                Mock.Of<IIdentityServerService>(),
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IEmailService>(),
                Mock.Of<AzureStorageConfiguration>(),
                Mock.Of<ILogger<RequestService>>());

            Result result = await sut.RejectWorker(request.Id, workerId, new CommentsModel { Comments = "This is a test" });
            Assert.True(result);

            requestRepository.Verify(r => r.Update(request), Times.Once);
            requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task IfWorkerHasNotBeenBookedCannotBeRejected()
        {
            var workerId = Guid.NewGuid();
            var request = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(workerId, Guid.NewGuid(), new Location(), default, default, workersQuantity: 1).Value;
            var requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<Covenant.Common.Entities.Request.Request, bool>>>())).ReturnsAsync(request);
            var sut = new RequestService(
                Mock.Of<ICompanyRepository>(),
                Mock.Of<ILocationRepository>(),
                Mock.Of<ITimeService>(),
                requestRepository.Object,
                Mock.Of<INotificationDataRepository>(),
                Mock.Of<IPushNotifications>(),
                Mock.Of<IIdentityServerService>(),
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IEmailService>(),
                Mock.Of<AzureStorageConfiguration>(),
                Mock.Of<ILogger<RequestService>>());
            Result result = await sut.RejectWorker(request.Id, workerId, new CommentsModel { Comments = "This is a test" });
            Assert.False(result);
        }
    }
}