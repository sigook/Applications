using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Resources;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Tests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Covenant.Tests.Accounting
{
    public class CreateRequestServiceTest
    {
        private readonly Mock<IWorkerRepository> _workerRepository;
        private readonly Mock<IRequestRepository> _requestRepository;
        private readonly Mock<ICompanyRepository> _companyRepository;
        private readonly Mock<ILocationRepository> locationRepository;
        private readonly Mock<IIdentityServerService> identityServerService;
        private readonly IRequestService _sut;
        private readonly Guid _agencyId = Guid.NewGuid();
        private readonly Guid _workerId = Guid.NewGuid();
        private readonly Guid _jobPositionRateId = Guid.NewGuid();
        private readonly CompanyProfile _companyProfile = new CompanyProfile { Company = new User(CvnEmail.Create("company@company.com").Value) };
        private readonly DateTime _now = new DateTime(2018, 01, 01);
        private readonly RequestCreateModel _model;

        public CreateRequestServiceTest()
        {
            _workerRepository = new Mock<IWorkerRepository>();
            _requestRepository = new Mock<IRequestRepository>();
            _companyRepository = new Mock<ICompanyRepository>();
            _companyRepository.Setup(cr => cr.GetJobPosition(It.IsAny<Guid>())).ReturnsAsync(new CompanyProfileJobPositionRate
            {
                Rate = 1,
                WorkerRate = 1,
            });
            _companyRepository.Setup(cr => cr.GetCompanyId(It.IsAny<Guid>())).ReturnsAsync(Guid.NewGuid());
            locationRepository = new Mock<ILocationRepository>();
            locationRepository.Setup(l => l.GetLocationById(It.IsAny<Guid>())).ReturnsAsync(new Location
            {
                City = new City
                {
                    Province = new Province
                    {
                        Country = new Country
                        {
                            Code = "USA"
                        }
                    }
                }
            });
            identityServerService = new Mock<IIdentityServerService>();
            identityServerService.Setup(i => i.GetAgencyId()).Returns(Guid.NewGuid());
            var timeService = new Mock<ITimeService>();
            _sut = new RequestService(
                _companyRepository.Object,
                locationRepository.Object,
                timeService.Object,
                _requestRepository.Object,
                Mock.Of<INotificationDataRepository>(),
                Mock.Of<IPushNotifications>(),
                identityServerService.Object,
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IEmailService>(),
                Mock.Of<AzureStorageConfiguration>(),
                Mock.Of<ILogger<RequestService>>());
            timeService.Setup(s => s.GetCurrentDateTime()).Returns(_now);
            _model = new RequestCreateModel
            {
                JobTitle = "Waiter",
                WorkersQuantity = 1,
                Description = "Description",
                DurationBreak = TimeSpan.FromMinutes(15),
                Incentive = 15,
                IncentiveDescription = "Some Description",
                Requirements = "Requirements",
                JobPositionRateId = _jobPositionRateId,
                LocationId = Guid.NewGuid(),
            };
        }

        [Fact]
        public async Task CreateRequestSuccessfully()
        {
            Result<Guid> result = await _sut.CreateRequest(_model);
            Assert.True(result);
            Assert.Empty(result.Errors);
            _requestRepository.Verify(r => r.Create(It.IsAny<Covenant.Common.Entities.Request.Request>()), Times.Once);
            _requestRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ErrorWhenIsAndInvalidJobPositionRate()
        {
            _companyRepository.Setup(cr => cr.GetJobPosition(It.IsAny<Guid>())).ReturnsAsync(default(CompanyProfileJobPositionRate));
            var request = FakeData.FakeRequest(_agencyId, _companyProfile.Company.Id, _jobPositionRateId);
            Result<Guid> result = await _sut.CreateRequest(_model);
            Assert.False(result);
            Assert.Equal(ApiResources.InvalidJobPosition, result.Errors.First().Message);
        }
    }
}