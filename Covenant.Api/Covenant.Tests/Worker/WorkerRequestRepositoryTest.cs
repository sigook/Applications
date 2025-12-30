using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Repositories.Request;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Worker
{
    public class WorkerRequestRepositoryTest
    {
        private readonly WorkerRequestRepository _sut;
        private readonly TimeSheetRepository _sut2;
        private readonly CovenantContext _context;
        private static readonly DateTime FakeNow = new DateTime(2019, 10, 02);
        private static readonly User UserAgency = new User(CvnEmail.Create("uAgency@com.com").Value);
        private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency { Id = UserAgency.Id, User = UserAgency };
        private static readonly User UserWorker = new User(CvnEmail.Create("uWorker@com.com").Value);
        private static readonly WorkerProfile Wp = new WorkerProfile(UserWorker) { Agency = FakeAgency, PunchCardId = "ABC" };
        private static readonly Covenant.Common.Entities.Request.Request FakeRequest = new Covenant.Common.Entities.Request.Request(new User(CvnEmail.Create("company@com.com").Value), FakeAgency, new CompanyProfileJobPositionRate());
        private static readonly WorkerRequest Wr = WorkerRequest.AgencyBook(Wp.WorkerId, FakeRequest.Id);

        public WorkerRequestRepositoryTest()
        {
            DbContextOptions<CovenantContext> options = new DbContextOptionsBuilder<CovenantContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new CovenantContext(options);
            var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
            mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration());
            _sut = new WorkerRequestRepository(_context);
            _sut2 = new TimeSheetRepository(_context, Mock.Of<Rates>(), Mock.Of<IRequestRepository>(), Mock.Of<TimeLimits>());
            FakeRequest.UpdateShift(new Shift());
            Wr.UpdateStartWorking(FakeNow);
            _context.WorkerProfile.Add(Wp);
            _context.Request.Add(FakeRequest);
            _context.WorkerRequest.Add(Wr);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CanClockIn()
        {
            Assert.True(await _sut.CanClockIn(w => w.WorkerId == Wp.Worker.Id, FakeNow));
            Assert.True(await _sut.CanClockIn(w => w.PunchCardId.ToLower().Equals(Wp.PunchCardId.ToLower()), FakeNow));
        }

        [Fact]
        public async Task GetLatestTimesheet()
        {
            var timeSheet = TimeSheet.CreateTimeSheet(Wr, FakeNow, TimeSpan.FromHours(1), now: FakeNow).Value;
            await _context.TimeSheet.AddAsync(timeSheet);
            await _context.SaveChangesAsync();
            var latestTimesheet = await _sut2.GetLatestTimesheet(w => w.WorkerId == Wp.Worker.Id);
            AssertModel();
            latestTimesheet = await _sut2.GetLatestTimesheet(w => w.PunchCardId.ToLower().Equals(Wp.PunchCardId.ToLower()));
            AssertModel();
            void AssertModel()
            {
                Assert.NotNull(latestTimesheet);
                Assert.Equal(Wr.Id, latestTimesheet.WorkerRequestId);
                Assert.Equal(timeSheet.Id, latestTimesheet.Id);
                Assert.Equal(timeSheet.TimeIn, latestTimesheet.TimeIn);
                Assert.Equal(timeSheet.TimeOut, latestTimesheet.TimeOut);
            }
        }
    }
}