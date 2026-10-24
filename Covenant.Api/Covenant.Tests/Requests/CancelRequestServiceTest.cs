using Covenant.Api.Validators.Requests;
using Covenant.Common.Configuration;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Repositories.Notifications;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Adapters;
using Covenant.Core.BL.Services.Requests;
using Covenant.Core.BL.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Requests;

public class CancelRequestServiceTest
{
    [Fact]
    public async Task Cancel()
    {
        var companyId = Guid.NewGuid();
        var requestRepository = new Mock<IRequestRepository>();
        var request = Request.AgencyCreateRequest(companyId, new Location(), default, Guid.NewGuid()).Value;
        requestRepository.Setup(c => c.GetRequest(It.IsAny<Expression<Func<Request, bool>>>())).ReturnsAsync(request);
        requestRepository.Setup(c => c.GetRequestCancellationDetail(request.Id)).ReturnsAsync(new RequestCancellationDetail());
        var service = new RequestService(
            Mock.Of<ICompanyRepository>(),
            Mock.Of<IAgencyRepository>(),
            Mock.Of<ILocationRepository>(),
            new FakeTimeProvider(),
            requestRepository.Object,
            Mock.Of<INotificationDataRepository>(),
            Mock.Of<ICurrentUserService>(),
            Mock.Of<IRazorViewToStringRenderer>(),
            Mock.Of<IEmailService>(),
            Mock.Of<IMessagePublisher>(),
            Options.Create(new ServiceBusConfiguration()),
            Mock.Of<ILogger<RequestService>>(),
            new RequestCreateModelValidator(),
            new RequestUpdateRequirementsModelValidator(),
            new RequestAdapter(),
            Mock.Of<IMediator>());
        Result result = await service.CancelRequest(request.Id, new RequestCancellationDetailModel());
        Assert.True(result);
        Assert.Equal(RequestStatus.Cancelled, request.Status);
        requestRepository.Verify(c => c.Delete(It.IsAny<RequestCancellationDetail>()), Times.Once);
        requestRepository.Verify(c => c.Create(It.IsAny<RequestNote>()), Times.Once);
        requestRepository.Verify(c => c.Create(It.IsAny<RequestCancellationDetail>()), Times.Once);
        requestRepository.Verify(c => c.Update(request), Times.Once);
        requestRepository.Verify(c => c.SaveChangesAsync(), Times.Once);
    }
}
