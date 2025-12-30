using Covenant.Api.Validators.Company;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Accounting;
using Covenant.Common.Repositories.Agency;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Notification;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Core.BL.Services;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Covenant.Tests.Request
{
    public class AgencyBookWorkerServiceTest
    {
        private readonly WorkerProfile _worker;
        private readonly Mock<IWorkerRepository> _workerRepository;
        private readonly Mock<ITimeService> _timeService;
        private readonly DateTime _fakeNow = new DateTime(2019, 01, 01);
        private readonly Covenant.Common.Entities.Request.Request request;
        private readonly Mock<IWorkerRequestRepository> _workerRequestRepository;
        private readonly Mock<IRequestRepository> requestRepository;
        private readonly AgencyService _sut;
        private readonly IValidator<CompanyProfileDetailModel> companyProfileValidator;

        public AgencyBookWorkerServiceTest()
        {

            _worker = new WorkerProfile(new User(CvnEmail.Create("w@mail.com").Value), Guid.NewGuid()) { ApprovedToWork = true };
            var sinInfo = new Mock<ISinInformation<CovenantFile>>();
            sinInfo.SetupGet(g => g.SocialInsurance).Returns("980-897-987");
            sinInfo.SetupGet(g => g.SocialInsuranceFile).Returns(new CovenantFile("sin.pdf"));
            _worker.PatchSinInformation(sinInfo.Object);
            request = Covenant.Common.Entities.Request.Request.AgencyCreateRequest(_worker.AgencyId, Guid.NewGuid(), new Location(), _fakeNow, default, workersQuantity: 1).Value;
            _workerRepository = new Mock<IWorkerRepository>();
            _workerRepository.Setup(r => r.GetProfile(It.IsAny<Expression<Func<WorkerProfile, bool>>>())).ReturnsAsync(_worker);
            _timeService = new Mock<ITimeService>();
            _workerRequestRepository = new Mock<IWorkerRequestRepository>();
            requestRepository = new Mock<IRequestRepository>();
            requestRepository.Setup(r => r.GetRequest(It.IsAny<Expression<Func<Covenant.Common.Entities.Request.Request, bool>>>())).ReturnsAsync(request);
            requestRepository.Setup(r => r.GetRequestShift(It.IsAny<Guid>())).ReturnsAsync(new ShiftModel());
            _sut = new AgencyService(
                Mock.Of<IAgencyRepository>(),
                Mock.Of<ICompanyRepository>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IShiftRepository>(),
                _workerRepository.Object,
                requestRepository.Object,
                _workerRequestRepository.Object,
                Mock.Of<INotificationDataRepository>(),
                Mock.Of<ICatalogRepository>(),
                Mock.Of<ITimeService>(),
                Mock.Of<IIdentityServerService>(),
                Mock.Of<IDocumentService>(),
                Mock.Of<IRazorViewToStringRenderer>(),
                Mock.Of<IEmailService>(),
                Mock.Of<ILogger<AgencyService>>(),
                Mock.Of<IServiceProvider>());
        }

        [Fact]
        public async Task Book()
        {
            var result = await _sut.BookWorker(request.Id, _worker.WorkerId, new AgencyBookWorkerModel { StartWorking = DateTime.Now });
            Assert.True(result);
            requestRepository.Verify(c => c.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>()), Times.Once);
            requestRepository.Verify(c => c.Update(request), Times.Once);
            requestRepository.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Book_IfWorkerWasApplicantItMustBeRemovedFromApplicants()
        {
            var requestApplicant = RequestApplicant.CreateWithWorker(request.Id, _worker.Id, default, default).Value;
            requestRepository.Setup(r => r.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>())).ReturnsAsync(requestApplicant);

            Result result = await _sut.BookWorker(request.Id, _worker.WorkerId, new AgencyBookWorkerModel { StartWorking = DateTime.Now });
            Assert.True(result);

            requestRepository.Verify(c => c.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>()), Times.Once);
            requestRepository.Verify(c => c.Delete(It.IsAny<RequestApplicant>()), Times.Once);
            requestRepository.Verify(c => c.Update(request), Times.Once);
            requestRepository.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task BookRejectedWorker()
        {
            var workerRequest = WorkerRequest.AgencyBook(_worker.WorkerId, request.Id, default);
            workerRequest.Reject();
            _workerRequestRepository.Setup(r => r.GetWorkerRequest(_worker.WorkerId, request.Id)).ReturnsAsync(workerRequest);

            Result result = await _sut.BookWorker(request.Id, _worker.Id, new AgencyBookWorkerModel { StartWorking = DateTime.Now });
            Assert.True(result);

            requestRepository.Verify(c => c.GetRequestApplicant(It.IsAny<Expression<Func<RequestApplicant, bool>>>()), Times.Once);
            requestRepository.Verify(c => c.Update(It.IsAny<Covenant.Common.Entities.Request.Request>()), Times.Once);
            requestRepository.Verify(c => c.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task WorkerCanNotBeBookIfItIsAlreadyInShift()
        {
            requestRepository.Setup(r => r.GetRequestShift(request.Id)).ReturnsAsync(new ShiftModel
            {
                Sunday = true,
                SundayStart = new TimeSpan(16, 59, 0),
                SundayFinish = new TimeSpan(22, 0, 0)
            });
            var shift = new Shift();
            shift.AddSunday(new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));
            request.UpdateShift(shift);
            var workerRequest = WorkerRequest.AgencyBook(_worker.Id, _worker.AgencyId);
            workerRequest.Request = request;
            _workerRequestRepository.Setup(r => r.GetWorkerRequestsByWorkerId(_worker.WorkerId)).ReturnsAsync(new List<WorkerRequest> { workerRequest });
            Result result = await _sut.BookWorker(request.Id, _worker.WorkerId, new AgencyBookWorkerModel { StartWorking = DateTime.Now });
            Assert.False(result);
            Assert.Equal("The worker is associated in other order with the same schedule", result.Errors.Single().Message);
        }
    }
}