using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Covenant.Tests.Company
{
    public class CompanyAddClockInTest
    {
        private readonly ITimesheetService _sut;
        private readonly DateTime _fakeNow = new DateTime(2019, 01, 01, 08, 00, 00, 00);
        private readonly Covenant.Common.Entities.Request.WorkerRequest _workerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(Guid.NewGuid(), Guid.NewGuid());
        private readonly Mock<ITimeSheetRepository> _timeSheetRepository;

        public CompanyAddClockInTest()
        {
            var user = new User(CvnEmail.Create("test@test.com").Value, Guid.NewGuid());
            var agency = new Covenant.Common.Entities.Agency.Agency();
            var jobPositionRate = new CompanyProfileJobPositionRate();
            var location = new Location
            {
                City = new City
                {
                    Province = new Province
                    {
                        Country = new Country { Code = "CA" }
                    }
                }
            };
            var request = new Covenant.Common.Entities.Request.Request(user, agency, jobPositionRate);
            request.UpdateJobLocation(location, false);
            _workerRequest.Request = request;
            var timService = new Mock<ITimeService>();
            timService.Setup(t => t.GetCurrentDateTime()).Returns(_fakeNow);
            var workerRequestRepository = new Mock<IWorkerRequestRepository>();
            workerRequestRepository.Setup(w => w.GetWorkerRequest(_workerRequest.WorkerId, _workerRequest.RequestId)).ReturnsAsync(_workerRequest);

            _timeSheetRepository = new Mock<ITimeSheetRepository>();
            var catalogRepository = new Mock<ICatalogRepository>();
            _sut = new TimesheetService(
                timService.Object,
                workerRequestRepository.Object,
                _timeSheetRepository.Object,
                catalogRepository.Object,
                Mock.Of<IConfiguration>(),
                Mock.Of<IIdentityServerService>(),
                Mock.Of<IMediator>(),
                Mock.Of<ILogger<TimesheetService>>());
        }

        [Fact]
        public async Task AddClockIn()
        {
            TimeSpan clockIn = TimeSpan.Parse("08:00");
            var result = await _sut.AddClockIn(_workerRequest.RequestId, _workerRequest.WorkerId, clockIn);
            Assert.True(result);

            _timeSheetRepository.Verify(v => v.Create(It.IsAny<TimeSheet>()), Times.Once);
            _timeSheetRepository.Verify(v => v.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddClockIn_TimeMustBeLessOrEqualThanNow()
        {
            TimeSpan clockIn = _fakeNow.TimeOfDay.Add(TimeSpan.FromMinutes(1));
            var result = await _sut.AddClockIn(_workerRequest.RequestId, _workerRequest.WorkerId, clockIn);
            Assert.False(result);
            Assert.StartsWith("Clock in must be less than", result.Errors.First().Message);
            _timeSheetRepository.Verify(v => v.Create(It.IsAny<TimeSheet>()), Times.Never);
            _timeSheetRepository.Verify(v => v.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AddClockIn_ItIsOnlyAllowOneClockInPerDay()
        {
            _timeSheetRepository.Setup(s => s.Create(It.IsAny<TimeSheet>()))
                .Callback<TimeSheet>(tst => _workerRequest.AddTimeSheet(tst))
                .Returns(Task.CompletedTask);

            TimeSpan clockIn = _fakeNow.TimeOfDay;
            var result = await _sut.AddClockIn(_workerRequest.RequestId, _workerRequest.WorkerId, clockIn);
            Assert.True(result);

            result = await _sut.AddClockIn(_workerRequest.RequestId, _workerRequest.WorkerId, clockIn);
            Assert.False(result);

            Assert.Equal("Worker already clock in", result.Errors.First().Message);
        }

        [Fact]
        public async Task AddClockIn_WorkerMustBeBooked()
        {
            _workerRequest.Reject();
            TimeSpan clockIn = _fakeNow.TimeOfDay;
            var result = await _sut.AddClockIn(_workerRequest.RequestId, _workerRequest.WorkerId, clockIn);
            Assert.False(result);

            Assert.Equal("Worker is rejected", result.Errors.First().Message);
        }
    }
}