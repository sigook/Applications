using Covenant.Common.Configuration;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Adapters;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Location;
using Covenant.Common.Models.Request.Worker;
using Covenant.Common.Models.Worker;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Agencies;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Repositories.Companies;
using Covenant.Common.Repositories.Notifications;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Repositories;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Core.BL.Interfaces.Requests;
using Covenant.Core.BL.Interfaces.Shared;
using Covenant.Core.BL.Interfaces.Workers;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services.Workers;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Covenant.Tests.Workers;

public class WorkerServiceRequestDetailTest
{
    private readonly Mock<IRequestRepository> _requestRepository = new();
    private readonly Mock<ICurrentUserService> _currentUserService = new();
    private readonly Guid _workerId = Guid.NewGuid();
    private readonly Guid _requestId = Guid.NewGuid();
    private readonly WorkerService _sut;

    public WorkerServiceRequestDetailTest()
    {
        _currentUserService.Setup(s => s.GetUserId()).Returns(_workerId);
        _sut = new WorkerService(
            Mock.Of<IWorkerRepository>(),
            Mock.Of<IAgencyRepository>(),
            Mock.Of<ICompanyRepository>(),
            Mock.Of<IUserRepository>(),
            Mock.Of<INotificationRepository>(),
            _requestRepository.Object,
            Mock.Of<IWorkerRequestRepository>(),
            Mock.Of<IUserAccountService>(),
            _currentUserService.Object,
            Mock.Of<ITeamsService>(),
            Mock.Of<IEmailService>(),
            Mock.Of<IRazorViewToStringRenderer>(),
            Options.Create(new TeamsWebhookConfiguration()),
            Mock.Of<ILogger<WorkerService>>(),
            Mock.Of<IWorkerAdapter>(),
            Mock.Of<IValidator<WorkerProfileCreateModel>>(),
            Mock.Of<IValidator<WorkerProfileLicenseModel>>(),
            Mock.Of<IValidator<CovenantFileModel>>(),
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IFilesContainer>(),
            Mock.Of<IDocumentService>(),
            Mock.Of<ICandidateService>(),
            Mock.Of<ICandidateRepository>(),
            Mock.Of<IUploadedFilesService>(),
            Mock.Of<ICatalogRepository>(),
            Mock.Of<IRequestApplicantNotificationService>(),
            Mock.Of<IWorkerRequirementService>(),
            Options.Create(new PunchConfiguration { CheckpointRadiusMeters = 100 }),
            Mock.Of<ISkillService>());
    }

    private void SetupDetail(double? latitude, double? longitude) =>
        _requestRepository
            .Setup(r => r.GetRequestDetailForWorker(_workerId, _requestId))
            .ReturnsAsync(new WorkerRequestDetailModel
            {
                Id = _requestId,
                JobLocation = new LocationDetailModel { Latitude = latitude, Longitude = longitude }
            });

    [Fact]
    public async Task GetRequestDetail_WithCoordinates_ReturnsCheckpointWithRadius()
    {
        SetupDetail(37.6876, -77.8964);

        var detail = await _sut.GetRequestDetail(_requestId);

        Assert.NotNull(detail.PunchCheckpoint);
        Assert.Equal(37.6876, detail.PunchCheckpoint.Latitude);
        Assert.Equal(-77.8964, detail.PunchCheckpoint.Longitude);
        Assert.Equal(100, detail.PunchCheckpoint.RadiusMeters);
    }

    [Fact]
    public async Task GetRequestDetail_WithoutCoordinates_ReturnsNoCheckpoint()
    {
        SetupDetail(null, null);

        var detail = await _sut.GetRequestDetail(_requestId);

        Assert.Null(detail.PunchCheckpoint);
    }

    [Fact]
    public async Task GetRequestDetail_NotFound_ReturnsNull()
    {
        _requestRepository
            .Setup(r => r.GetRequestDetailForWorker(_workerId, _requestId))
            .ReturnsAsync((WorkerRequestDetailModel)null);

        var detail = await _sut.GetRequestDetail(_requestId);

        Assert.Null(detail);
    }
}
