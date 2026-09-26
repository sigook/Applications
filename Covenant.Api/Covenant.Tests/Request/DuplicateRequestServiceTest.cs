using Covenant.Common.Configuration;
using Covenant.Infrastructure.Services;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Resources;
using Covenant.Api.Validators.Request;
using Covenant.Core.BL.Adapters;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Tests.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using Moq;
using Xunit;

namespace Covenant.Tests.Request
{
    public class DuplicateRequestServiceTest
    {
        private readonly Mock<IRequestRepository> _requestRepository;
        private readonly IRequestService _sut;
        private readonly Guid _sourceRequestId = Guid.NewGuid();
        private readonly Guid _sourceId = Guid.NewGuid();
        private readonly Guid _requestedById = Guid.NewGuid();
        private readonly Guid _reportToId = Guid.NewGuid();
        private readonly ShiftModel _sourceShift = new() { Monday = true, MondayStart = new TimeSpan(8, 0, 0), MondayFinish = new TimeSpan(17, 0, 0) };
        private readonly RequestCreateModel _model;

        public DuplicateRequestServiceTest()
        {
            _requestRepository = new Mock<IRequestRepository>();
            _requestRepository.Setup(r => r.GetRequest(It.IsAny<System.Linq.Expressions.Expression<Func<Covenant.Common.Entities.Request.Request, bool>>>()))
                .ReturnsAsync(FakeData.FakeRequest());
            _requestRepository.Setup(r => r.GetRequestShift(_sourceRequestId)).ReturnsAsync(_sourceShift);
            _requestRepository.Setup(r => r.GetSkills(_sourceRequestId))
                .ReturnsAsync([new SkillModel { Skill = "Forklift" }, new SkillModel { Skill = "WHMIS" }]);
            _requestRepository.Setup(r => r.GetRequestedByList(_sourceRequestId, It.IsAny<Pagination>()))
                .ReturnsAsync(new PaginatedList<RequestContactPersonModel> { Items = [new RequestContactPersonModel { Id = _requestedById }] });
            _requestRepository.Setup(r => r.GetReportToList(_sourceRequestId, It.IsAny<Pagination>()))
                .ReturnsAsync(new PaginatedList<RequestContactPersonModel> { Items = [new RequestContactPersonModel { Id = _reportToId }] });
            _requestRepository.Setup(r => r.GetRequestSources(_sourceRequestId))
                .ReturnsAsync([new RequestSource { SourceId = _sourceId, PublishedAt = new DateTime(2020, 01, 01), ExternalUrl = "https://old-posting" }]);

            var companyRepository = new Mock<ICompanyRepository>();
            companyRepository.Setup(cr => cr.GetJobPosition(It.IsAny<Guid>())).ReturnsAsync(new CompanyProfileJobPositionRate
            {
                Rate = 1,
                WorkerRate = 1,
            });
            var locationRepository = new Mock<ILocationRepository>();
            locationRepository.Setup(l => l.GetLocationById(It.IsAny<Guid>())).ReturnsAsync(new Location
            {
                City = new City { Province = new Province { Country = new Country { Code = "USA" } } }
            });
            var currentUserService = new Mock<ICurrentUserService>();
            currentUserService.Setup(i => i.GetAgencyId()).Returns(Guid.NewGuid());

            _sut = new RequestService(
                companyRepository.Object,
                Mock.Of<IAgencyRepository>(),
                locationRepository.Object,
                Mock.Of<ITimeService>(),
                _requestRepository.Object,
                Mock.Of<INotificationDataRepository>(),
                Mock.Of<IPushNotifications>(),
                currentUserService.Object,
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IEmailService>(),
                Mock.Of<ISigookBusClient>(),
                Options.Create(new ServiceBusConfiguration()),
                Mock.Of<ILogger<RequestService>>(),
                new RequestCreateModelValidator(),
                new RequestUpdateRequirementsModelValidator(),
                new RequestAdapter(),
                Mock.Of<IMediator>());

            _model = new RequestCreateModel
            {
                JobTitle = "Waiter",
                WorkersQuantity = 1,
                Description = "Description",
                DurationBreak = TimeSpan.FromMinutes(15),
                Requirements = "Requirements",
                JobPositionRateId = Guid.NewGuid(),
                LocationId = Guid.NewGuid(),
            };
        }

        [Fact]
        public async Task DuplicateRequestCopiesTheDataThatIsNotOnTheForm()
        {
            Result<Guid> result = await _sut.DuplicateRequest(_sourceRequestId, _model);

            Assert.True(result);
            Assert.NotEqual(_sourceRequestId, result.Value);
            _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<Covenant.Common.Entities.Request.Request>>(requests =>
                requests.Single().Shift.Monday == true)), Times.Once);
            _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestSkill>>(skills =>
                skills.Single().Skill == "Forklift")), Times.Once);
            _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<RequestSkill>>(skills =>
                skills.Single().Skill == "WHMIS")), Times.Once);
            _requestRepository.Verify(r => r.Create(It.Is<List<RequestRequestedBy>>(items =>
                items.Single().ContactPersonId == _requestedById && items.Single().RequestId == result.Value)), Times.Once);
            _requestRepository.Verify(r => r.Create(It.Is<List<RequestReportTo>>(items =>
                items.Single().ContactPersonId == _reportToId && items.Single().RequestId == result.Value)), Times.Once);
        }

        [Fact]
        public async Task DuplicateRequestCopiesTheJobBoardsWithoutThePostingDataOfTheSource()
        {
            Result<Guid> result = await _sut.DuplicateRequest(_sourceRequestId, _model);

            _requestRepository.Verify(r => r.ReplaceRequestSources(result.Value, It.Is<IEnumerable<CreateRequestSourceModel>>(sources =>
                sources.Single().SourceId == _sourceId
                && sources.Single().PublishedAt == null
                && sources.Single().ExternalUrl == null)), Times.Once);
        }

        [Fact]
        public async Task DuplicateRequestKeepsTheShiftSentOnTheModel()
        {
            _model.Shift = new ShiftModel { Tuesday = true, TuesdayStart = new TimeSpan(9, 0, 0), TuesdayFinish = new TimeSpan(18, 0, 0) };

            Result<Guid> result = await _sut.DuplicateRequest(_sourceRequestId, _model);

            Assert.True(result);
            _requestRepository.Verify(r => r.GetRequestShift(It.IsAny<Guid>()), Times.Never);
            _requestRepository.Verify(r => r.Create(It.Is<IEnumerable<Covenant.Common.Entities.Request.Request>>(requests =>
                requests.Single().Shift.Tuesday == true)), Times.Once);
        }

        [Fact]
        public async Task ErrorWhenTheSourceRequestDoesNotExist()
        {
            _requestRepository.Setup(r => r.GetRequest(It.IsAny<System.Linq.Expressions.Expression<Func<Covenant.Common.Entities.Request.Request, bool>>>()))
                .ReturnsAsync(default(Covenant.Common.Entities.Request.Request));

            Result<Guid> result = await _sut.DuplicateRequest(_sourceRequestId, _model);

            Assert.False(result);
            Assert.Equal(ApiResources.RequestNotAvailable, result.Errors.First().Message);
        }
    }
}
