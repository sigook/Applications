using Covenant.Api.Validators.Request;
using Covenant.Common.Configuration;
using Covenant.Infrastructure.Services;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Adapters;
using Covenant.Core.BL.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Request
{
    public class CancelRequestServiceTest
    {
        [Fact]
        public async Task Cancel()
        {
            var companyId = Guid.NewGuid();
            var requestRepository = new Mock<IRequestRepository>();
            var request = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(companyId, new Location(), default, Guid.NewGuid()).Value;
            requestRepository.Setup(c => c.GetRequest(It.IsAny<Expression<Func<Covenant.Common.Entities.Request.Request, bool>>>())).ReturnsAsync(request);
            requestRepository.Setup(c => c.GetRequestCancellationDetail(request.Id)).ReturnsAsync(new RequestCancellationDetail());
            var service = new RequestService(
                Mock.Of<ICompanyRepository>(),
                Mock.Of<ILocationRepository>(),
                Mock.Of<ITimeService>(),
                requestRepository.Object,
                Mock.Of<INotificationDataRepository>(),
                Mock.Of<IPushNotifications>(),
                Mock.Of<IIdentityServerService>(),
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IEmailService>(),
                Mock.Of<ISigookBusClient>(),
                Options.Create(new ServiceBusConfiguration()),
                Mock.Of<ILogger<RequestService>>(),
                new RequestCreateModelValidator(),
                new RequestUpdateRequirementsModelValidator(),
                new RequestAdapter());
            Result result = await service.CancelRequest(request.Id, new RequestCancellationDetailModel());
            Assert.True(result);
            Assert.Equal(RequestStatus.Cancelled, request.Status);
            requestRepository.Verify(c => c.Delete(It.IsAny<IEnumerable<RequestCancellationDetail>>()), Times.Once);
            requestRepository.Verify(c => c.Create(It.IsAny<IEnumerable<RequestNote>>()), Times.Once);
            requestRepository.Verify(c => c.Create(It.IsAny<IEnumerable<RequestCancellationDetail>>()), Times.Once);
            requestRepository.Verify(c => c.Update(request), Times.Once);
            requestRepository.Verify(c => c.SaveChangesAsync(), Times.Once);
        }
    }
}