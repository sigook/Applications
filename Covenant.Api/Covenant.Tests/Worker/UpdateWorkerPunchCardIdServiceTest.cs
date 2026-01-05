using Covenant.Api.Validators.Candidate;
using Covenant.Api.Validators.Worker;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Worker
{
    public class UpdateWorkerPunchCardIdServiceTest
    {
        private readonly IWorkerService _sut;
        private readonly Guid _profileId = Guid.NewGuid();
        private readonly Guid _agencyId = Guid.NewGuid();
        private readonly Mock<IWorkerRepository> _workerRepository;
        private const string PunchCardId = "AA22BBB";

        public UpdateWorkerPunchCardIdServiceTest()
        {
            _workerRepository = new Mock<IWorkerRepository>();

            _sut = new WorkerService(
                Mock.Of<IAgencyRepository>(),
                _workerRepository.Object,
                Mock.Of<IUserRepository>(),
                Mock.Of<INotificationRepository>(),
                Mock.Of<IRequestRepository>(),
                Mock.Of<IWorkerRequestRepository>(),
                Mock.Of<IIdentityServerService>(),
                Mock.Of<ITeamsService>(),
                Mock.Of<IEmailService>(),
                Mock.Of<ISendGridService>(),
                Mock.Of<ITimeService>(),
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IOptions<TeamsWebhookConfiguration>>(),
                Mock.Of<ILogger<WorkerService>>(),
                Mock.Of<IWorkerAdapter>(),
                Mock.Of<IValidator<WorkerProfileCreateModel>>(),
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IFilesContainer>(),
                Mock.Of<IDocumentService>());
        }
        [Fact]
        public async Task Invalid_Profile()
        {
            _workerRepository.Setup(r => r.GetProfile(p => p.Id == _profileId && p.AgencyId == _agencyId))
                .ReturnsAsync((WorkerProfile)null);
            Result result = await _sut.UpdateWorkerPunchCardId(_profileId, _agencyId, PunchCardId);
            Assert.False(result);
            _workerRepository.Verify(r => r.UpdateProfile(It.IsAny<WorkerProfile>()), Times.Never);
            _workerRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task PunchCardId_Is_Being_Using_By_Other_Worker()
        {
            _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
                .ReturnsAsync(new WorkerProfile(new User(CvnEmail.Create("w@mail.com").Value)) { Id = _profileId });
            _workerRepository.Setup(r => r.GetWorkerProfilePunchCarId(PunchCardId))
                .ReturnsAsync(new WorkerProfilePunchCardIdModel { Id = Guid.NewGuid(), PunchCardId = PunchCardId });
            Result result = await _sut.UpdateWorkerPunchCardId(_profileId, _agencyId, PunchCardId);
            Assert.False(result);
            Assert.NotEmpty(result.Errors);
            _workerRepository.Verify(r => r.UpdateProfile(It.IsAny<WorkerProfile>()), Times.Never);
            _workerRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData(PunchCardId)]
        [InlineData(null)]
        [InlineData("")]
        public async Task UpdatePunchCard(string punchCardId)
        {
            _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>()))
                .ReturnsAsync(new WorkerProfile(new User(CvnEmail.Create("w@mail.com").Value)) { Id = _profileId });
            _workerRepository.Setup(r => r.GetWorkerProfilePunchCarId(PunchCardId))
                .ReturnsAsync(new WorkerProfilePunchCardIdModel { Id = _profileId, PunchCardId = PunchCardId });
            Result result = await _sut.UpdateWorkerPunchCardId(_profileId, _agencyId, punchCardId);
            Assert.True(result);
            _workerRepository.Verify(r => r.UpdateProfile(It.IsAny<WorkerProfile>()), Times.Once);
            _workerRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}