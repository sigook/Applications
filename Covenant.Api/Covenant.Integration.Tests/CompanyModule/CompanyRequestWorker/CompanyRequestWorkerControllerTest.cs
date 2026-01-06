using Covenant.Api.Authorization;
using Covenant.Api.CompanyModule.CompanyRequestWorker.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyRequestWorker
{
    public class CompanyRequestWorkerControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CompanyRequestWorkerControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public CompanyRequestWorkerControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => CompanyRequestWorkerController.RouteName.Replace("{requestId}", Data.FakeRequest.Id.ToString());

        [Fact]
        public async Task Get()
        {
            var requestUri = $"{RequestUri()}?{nameof(GetWorkersRequestFilter.Statuses)}={Data.FakeWorkerRequest.WorkerRequestStatus}";
            HttpResponseMessage response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
            Assert.NotEmpty(list.Items);
            var item = list.Items[0];
            Assert.NotNull(item);
            Assert.NotNull(item.Name);
            Assert.NotNull(item.ProfileImage);
            Assert.NotNull(item.Status);
            Assert.NotEqual(Guid.Empty, item.WorkerId);
            Assert.NotEqual(Guid.Empty, item.WorkerProfileId);
        }

        [Fact]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Data.FakeWorkerRequest.WorkerId}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<AgencyWorkerRequestModel>();
            Assert.NotNull(model);
            Assert.NotEqual(Guid.Empty, model.WorkerProfileId);
        }

        [Fact]
        public async Task Post()
        {
            var response = await _client.PostAsJsonAsync($"{RequestUri()}/RequestNewWorker", new CommentsModel { Comments = "Some comment" });
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task PutReject()
        {
            var worker = Data.FakeWorkerRequestReject;
            var model = new CommentsModel { Comments = "Worker didn't show up" };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{worker.WorkerId}/Reject", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequest.SingleAsync(s => s.Id == worker.Id);
            Assert.Equal(WorkerRequestStatus.Rejected, entity.WorkerRequestStatus);
            Assert.Equal(model.Comments, entity.RejectComments);

            response = await _client.GetAsync(RequestUri());
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
            Assert.Equal(model.Comments, list.Items.Single(w => w.Id == worker.Id).RejectComments);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.CompanyProfile.Company.Id);
                        o.AddCompanyRole();
                    });
                services.AddHttpClient();
                services.AddDbContext<CovenantContext>(p => p.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestService, RequestService>();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ILocationRepository, LocationRepository>();
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(Data.Now);
                services.AddSingleton(timeService.Object);
                services.AddSingleton<CompanyIdFilter>();
            }

            public void Configure(IApplicationBuilder app, CovenantContext context)
            {
                app.UseRouting();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseResponseCaching();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                });
                context.Availability.Add(Data.FakeAvailability);
                context.Request.Add(Data.FakeRequest);
                context.WorkerRequest.AddRange(Data.FakeWorkerRequest, Data.FakeWorkerRequestReject);
                context.WorkerProfile.AddRange(Data.WorkerProfile, Data.FakeWorkerReject);
                context.CompanyProfile.Add(Data.CompanyProfile);
                context.CompanyProfileJobPositionRate.Add(Data.FakeRate);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            public static readonly DateTime Now = new DateTime(2019, 01, 01);
            private static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency { User = new User(CvnEmail.Create("agency@agency.com").Value) };
            public static readonly CompanyProfile CompanyProfile = new CompanyProfile
            {
                Company = new User(CvnEmail.Create("com_profile@mail.com").Value),
                Agency = Agency,
                Locations = new List<CompanyProfileLocation>
                {
                    new CompanyProfileLocation {Location = new Location {Address = "ABC",City = new City {Province = new Province{Country = new Country()}}}},
                },
                Industry = new CompanyProfileIndustry("Company Industry")
            };
            public static readonly CompanyProfileJobPositionRate FakeRate = CompanyProfileJobPositionRate.Create(CompanyProfile.Id, "Position", 1, 1, "General", "r@m.com").Value;
            public static readonly Request FakeRequest = Request.AgencyCreateRequest(Agency.Id, CompanyProfile.Company.Id, FakeData.FakeLocation(), Now, FakeRate.Id).Value;
            public static readonly Availability FakeAvailability = new Availability();
            private static WorkerProfile _workerProfile;
            public static WorkerProfile WorkerProfile
            {
                get
                {
                    if (_workerProfile != null) return _workerProfile;
                    _workerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_profile@mail.com").Value))
                    {
                        Agency = Agency,
                    };
                    var basicInfo = new Mock<IWorkerBasicInformation<ICatalog<Guid>>>();
                    basicInfo.SetupGet(i => i.FirstName).Returns("Pepe");
                    _workerProfile.PatchBasicInformation(basicInfo.Object);
                    _workerProfile.PatchAvailabilities(new[] { new BaseModel<Guid>(FakeAvailability.Id) });
                    _workerProfile.PatchProfileImage(new CovenantFile("profile.png"));
                    return _workerProfile;
                }
            }

            public static readonly WorkerProfile FakeWorkerReject = new WorkerProfile(new User(CvnEmail.Create("w_reject@mail.com").Value))
            {
                ApprovedToWork = true,
                AgencyId = Agency.Id,
                Location = new Location { City = new City { Province = new Province() } }
            };

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(WorkerProfile.WorkerId, FakeRequest.Id);

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequestReject =
                Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(FakeWorkerReject.WorkerId, FakeRequest.Id, "recruiter@mail.com");
        }
    }
}