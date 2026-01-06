using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Worker
{
    public class WorkerRepositoryTest
    {
        private readonly WorkerRepository _sut;
        private readonly CovenantContext _context;

        public WorkerRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<CovenantContext>()
                .UseInMemoryDatabase("WorkerRepository").Options;
            _context = new CovenantContext(options);
            var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
            mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration());
            _sut = new WorkerRepository(_context, mockFilesConfiguration.Object);
        }

        [Theory]
        [InlineData("123456789", "true")]
        [InlineData("987654321", "false")]
        public async Task IdentificationNumberIsAlreadyTaken(string identificationNumber, string taken)
        {
            _context.WorkerProfile.Add(new WorkerProfile(new User(CvnEmail.Create("other@gmail.com").Value)) { IdentificationNumber1 = "123456789" });
            await _context.SaveChangesAsync();

            bool result = await _sut.InfoIsAlreadyTaken(x => x.IdentificationNumber1 == identificationNumber || x.IdentificationNumber2 == identificationNumber);
            if (bool.Parse(taken)) Assert.True(result);
            else Assert.False(result);
        }

        [Theory]
        [InlineData("SO123456789", "true")]
        [InlineData("SO987654321", "false")]
        public async Task SocialInsuranceIsAlreadyTaken(string socialInsurance, string taken)
        {
            var sinInfo = new Mock<ISinInformation<ICovenantFile>>();
            sinInfo.SetupGet(i => i.SocialInsurance).Returns("SO123456789");
            sinInfo.SetupGet(i => i.SocialInsuranceFile).Returns(new CovenantFile("soc.pdf"));
            var workerProfile = new WorkerProfile(new User(CvnEmail.Create("other@gmail.com").Value));
            workerProfile.PatchSinInformation(sinInfo.Object);
            _context.WorkerProfile.Add(workerProfile);
            await _context.SaveChangesAsync();

            bool result = await _sut.InfoIsAlreadyTaken(x => x.SocialInsurance == socialInsurance);
            if (bool.Parse(taken)) Assert.True(result);
            else Assert.False(result);
        }
    }
}