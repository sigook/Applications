using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Request;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Covenant.Tests.Worker
{
    public class CreateTimeSheetTest
    {
        private const string createdBy = "payroll@covenantgroupl.com";

        private readonly Mock<IIdentityServerService> identityServerService;
        private readonly Mock<ITimeService> _timeService;
        private readonly Mock<ITimeSheetRepository> _timeSheetRepository;
        private readonly WorkerRequest _workerRequest = WorkerRequest.AgencyBook(Guid.NewGuid(), Guid.NewGuid());
        private readonly ITimesheetService _sut;

        public CreateTimeSheetTest()
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
            _timeService = new Mock<ITimeService>();
            var catalogRepository = new Mock<ICatalogRepository>();
            _timeSheetRepository = new Mock<ITimeSheetRepository>();
            var workerRequestRepository = new Mock<IWorkerRequestRepository>();
            identityServerService = new Mock<IIdentityServerService>();
            _sut = new TimesheetService(
                _timeService.Object,
                workerRequestRepository.Object,
                _timeSheetRepository.Object,
                catalogRepository.Object,
                Mock.Of<IConfiguration>(),
                identityServerService.Object,
                Mock.Of<IMediator>(),
                Mock.Of<ILogger<TimesheetService>>());
            workerRequestRepository.Setup(r => r.GetWorkerRequest(_workerRequest.WorkerId, _workerRequest.RequestId)).ReturnsAsync(_workerRequest);
            identityServerService.Setup(iss => iss.GetNickname()).Returns(createdBy);
        }

        [Fact]
        public async Task Create()
        {
            var now = new DateTime(2021, 01, 01);
            TimeSheet timeSheet = null;
            _timeService.Setup(r => r.GetCurrentDateTime()).Returns(now);
            _timeSheetRepository.Setup(r => r.Create(It.IsAny<TimeSheet>()))
                .Callback<TimeSheet>(t => timeSheet = t).Returns(Task.CompletedTask);
            var model = new TimeSheetModel
            {
                TimeIn = now,
                Hours = TimeSpan.FromHours(8)
            };
            Result<Guid> result = await _sut.CreateTimesheet(_workerRequest.WorkerId, _workerRequest.RequestId, model);
            Assert.True(result);
            Assert.NotEqual(Guid.Empty, result.Value);
            Assert.Equal(model.TimeIn, timeSheet.TimeIn);
            Assert.Equal(model.Hours.TotalHours, timeSheet.TotalHours);
            Assert.NotNull(timeSheet.TimeOut);
            Assert.NotNull(timeSheet.TimeInApproved);
            Assert.NotNull(timeSheet.TimeOutApproved);
            Assert.EndsWith(createdBy, timeSheet.Comment);

            _timeSheetRepository.Verify(r => r.Create(It.IsAny<TimeSheet>()), Times.Once);
            _timeSheetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateWhenWorkerIsRejected()
        {
            var now = new DateTime(2019, 01, 01);
            _timeService.Setup(r => r.GetCurrentDateTime()).Returns(now);
            _workerRequest.Reject("No longer required", now);
            var model = new TimeSheetModel
            {
                TimeIn = new DateTime(2021, 01, 01),
                Hours = TimeSpan.FromHours(8)
            };
            Result<Guid> result = await _sut.CreateTimesheet(_workerRequest.WorkerId, _workerRequest.RequestId, model);
            Assert.True(result);
            _timeSheetRepository.Verify(r => r.Create(It.IsAny<TimeSheet>()), Times.Once);
            _timeSheetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateWhenWorkerIsRejected_IfWorkerWasRejectedOneMonthAgoCreateFails()
        {
            var now = new DateTime(2019, 01, 01);
            _timeService.Setup(r => r.GetCurrentDateTime()).Returns(now);
            _workerRequest.Reject("No longer required", now.AddMonths(-1).AddDays(-1));
            var model = new TimeSheetModel();
            Result<Guid> result = await _sut.CreateTimesheet(_workerRequest.WorkerId, _workerRequest.RequestId, model);
            Assert.False(result);
            _timeSheetRepository.Verify(r => r.Create(It.IsAny<TimeSheet>()), Times.Never);
            _timeSheetRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}