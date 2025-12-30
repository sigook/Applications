using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Request
{
    public class RequestRepositoryTest
    {
        private readonly RequestRepository _sut;
        private readonly Guid _workerId1 = Guid.NewGuid();
        private readonly Guid _agencyId = Guid.NewGuid();
        private readonly Guid _requestId = Guid.NewGuid();
        private readonly Guid _workerId = Guid.NewGuid();
        private readonly Guid _workerProfileId = Guid.NewGuid();
        private readonly Guid _workerProfileId1 = Guid.NewGuid();
        private readonly CovenantContext _context;
        private readonly Mock<ITimeService> _timeService;
        private class BasicInfo : IWorkerBasicInformation<ICatalog<Guid>>
        {
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string SecondLastName { get; set; }
            public DateTime BirthDay { get; set; }
            public ICatalog<Guid> Gender { get; set; }
            public bool HasVehicle { get; set; }
        }
        public RequestRepositoryTest()
        {
            DbContextOptions<CovenantContext> options = new DbContextOptionsBuilder<CovenantContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new CovenantContext(options);
            _timeService = new Mock<ITimeService>();
            var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
            mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration());
            _sut = new RequestRepository(_context, mockFilesConfiguration.Object, _timeService.Object);
        }

        [Fact]
        public async Task GetAllWorkersThatCanApplyToRequestByAgencyId()
        {
            await _context.Agencies.AddAsync(new Covenant.Common.Entities.Agency.Agency { Id = _agencyId });
            var workerProfile1 = new WorkerProfile(new User(CvnEmail.Create("a@a.com").Value, _workerId))
            {
                Id = _workerProfileId,
                Location = new Location { City = new City { Province = new Province() } },
                AgencyId = _agencyId
            };
            workerProfile1.PatchBasicInformation(new BasicInfo { FirstName = "John" });
            workerProfile1.PatchProfileImage(new CovenantFile("profile.png"));
            await _context.WorkerProfile.AddAsync(workerProfile1);
            var workerProfile2 = new WorkerProfile(new User(CvnEmail.Create("e@e.com").Value, _workerId1))
            {
                Id = _workerProfileId1,
                Location = new Location { City = new City { Province = new Province() } },
                AgencyId = _agencyId
            };
            workerProfile2.PatchBasicInformation(new BasicInfo { FirstName = "Juan" });
            workerProfile2.PatchProfileImage(new CovenantFile("profile.png"));
            await _context.WorkerProfile.AddAsync(workerProfile2);
            await _context.WorkerRequest.AddAsync(Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(_workerId, _requestId, default));
            await _context.SaveChangesAsync();

            var list = await _sut.GetAllWorkersThatCanApplyToRequest(_agencyId, _requestId, new Pagination(), null);
            Assert.Equal(2, list.Items.Count);
            Assert.Equal(WorkerRequestStatus.Booked.ToString(), list.Items.First(c => c.WorkerId == _workerId).Status);
            Assert.Equal(string.Empty, list.Items.First(c => c.WorkerId == _workerId1).Status);
        }
    }
}