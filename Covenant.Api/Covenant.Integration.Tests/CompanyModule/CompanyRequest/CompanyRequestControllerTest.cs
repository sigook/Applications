using Covenant.Api.Authorization;
using Covenant.Api.CompanyModule.CompanyRequest.Controllers;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
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
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.CompanyModule.CompanyRequest
{
    public class CompanyRequestControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CompanyRequestControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private const string Url = CompanyRequestController.RouteName;
        private readonly HttpClient _client;
        public CompanyRequestControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync(Url, Data.NewRequest);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            response = await _client.GetAsync(response.Headers.Location);
            response.EnsureSuccessStatusCode();
            CompanyRequestDetailModel detail = await response.Content.ReadAsJsonAsync<CompanyRequestDetailModel>();
            Assert.NotNull(detail);
            Data.AssertModel(Data.NewRequest, detail);
        }

        [Fact]
        public async Task PostWithSpecificWorker()
        {
            var model = Data.NewRequest;
            HttpResponseMessage response = await _client.PostAsJsonAsync(Url, model);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
            response = await _client.GetAsync(response.Headers.Location);
            response.EnsureSuccessStatusCode();
            CompanyRequestDetailModel detail = await response.Content.ReadAsJsonAsync<CompanyRequestDetailModel>();
            Assert.NotNull(detail);
            Data.AssertModel(model, detail);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(Url);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<RequestListModel>>();
            Assert.NotEmpty(list.Items);
            foreach (RequestListModel item in list.Items) Assert.NotEqual(Guid.Empty, item.Id);
        }

        [Fact]
        public async Task Put()
        {
            var model = new RequestUpdateRequirementsModel { Requirements = "New requirements" };
            string updateUrl = $"{Url}/{Data.OldRequest.Id}";
            HttpResponseMessage response = await _client.PutAsJsonAsync(updateUrl, model);
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(updateUrl);
            CompanyRequestDetailModel detail = await response.Content.ReadAsJsonAsync<CompanyRequestDetailModel>();
            Assert.Equal(model.Requirements, detail.Requirements);
        }

        [Fact]
        public async Task PutCancel()
        {
            string updateUrl = $"{Url}/{Data.OldRequest.Id}";
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{updateUrl}/Cancel", new RequestCancellationDetailModel { CancellationReasonId = Data.ReasonCancellationRequest.Id });
            response.EnsureSuccessStatusCode();
            response = await _client.GetAsync(updateUrl);
            CompanyRequestDetailModel detail = await response.Content.ReadAsJsonAsync<CompanyRequestDetailModel>();
            Assert.Equal(RequestStatus.Cancelled.ToString(), detail.Status);
        }

        [Fact]
        public async Task PostRequest_Then_Remove_User_Job_Position_Rate()
        {
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, Url, Data.NewRequest);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            List<CompanyProfileJobPositionRate> rates = await context.CompanyProfileJobPositionRate.ToListAsync();
            foreach (CompanyProfileJobPositionRate rate in rates) rate.Delete(default);
            context.CompanyProfileJobPositionRate.UpdateRange(rates);
            await context.SaveChangesAsync();
            response = await _client.GetAsync(response.Headers.Location);
            var detail = await response.Content.ReadAsJsonAsync<CompanyRequestDetailModel>();
            Assert.NotNull(detail);
            Assert.NotNull(detail.JobPositionRate);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.Sub);
                        o.AddCompanyRole();
                    });
                services.AddHttpClient();
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(Data.Now);
                services.AddSingleton(timeService.Object);
                services.AddSingleton<IRequestService, RequestService>();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
                services.AddSingleton<ILocationRepository, LocationRepository>();
                services.AddSingleton(TimeLimits.DefaultTimeLimits);
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
                context.User.Add(Data.CompanyUser);
                context.Set<CompanyProfileIndustry>().Add(Data.CompanyProfileIndustry);
                context.Location.Add(Data.CompanyLocation);
                context.CompanyProfile.Add(Data.CompanyProfile);
                context.City.Add(Data.Toronto);
                context.Request.Add(Data.OldRequest);
                context.ReasonCancellationRequest.Add(Data.ReasonCancellationRequest);
                context.WorkerProfile.Add(Data.FakeWorkerProfile);
                context.CompanyProfileJobPositionRate.Add(Data.FakeCompanyProfileJobPositionRate);
                context.SaveChanges();
            }
        }

        private static class Data
        {
            private static readonly Covenant.Common.Entities.Agency.Agency Agency = new Covenant.Common.Entities.Agency.Agency { User = new User(CvnEmail.Create("agency@agency.com").Value) };
            public static readonly DateTime Now = new DateTime(2019, 01, 01);
            public static readonly Guid Sub = Guid.NewGuid();
            public static readonly City Toronto = new City
            {
                Value = "Toronto",
                Province = new Province
                {
                    Country = new Country
                    {
                        Code = "CA"
                    }
                }
            };
            public static readonly User CompanyUser = new User(CvnEmail.Create("mi@hot.com").Value, Sub);
            public static readonly CompanyProfileIndustry CompanyProfileIndustry = CompanyProfileIndustry.Create("Industry Test").Value;
            public static readonly Location CompanyLocation = Location.Create(Toronto.Id, "Street False 123", "A1A1A1").Value;
            public static readonly CompanyProfile CompanyProfile = new CompanyProfile
            {
                Company = CompanyUser,
                Agency = Agency,
                Industry = CompanyProfileIndustry
            };
            public static readonly WorkerProfile FakeWorkerProfile = new WorkerProfile(new User(CvnEmail.Create("w_profile@mail.com").Value))
            {
                Agency = Agency,
                ApprovedToWork = true
            };
            public static readonly CompanyProfileJobPositionRate FakeCompanyProfileJobPositionRate = new CompanyProfileJobPositionRate
            {
                CompanyProfileId = CompanyProfile.Id,
                Rate = 8,
                WorkerRate = 5
            };


            public static readonly RequestCreateModel NewRequest = new RequestCreateModel
            {
                JobTitle = "Cut grass",
                WorkersQuantity = 1,
                JobPositionRateId = FakeCompanyProfileJobPositionRate.Id,
                Description = "The job is for cut the grass",
                DurationBreak = TimeSpan.FromMinutes(15),
                BreakIsPaid = true,
                JobIsOnBranchOffice = true,
                LocationId = CompanyLocation.Id,
                Incentive = 100,
                IncentiveDescription = "Extra",
                Requirements = "None",
                Shift = new ShiftModel
                {
                    Wednesday = true,
                    WednesdayStart = TimeSpan.Parse("09:00"),
                    WednesdayFinish = TimeSpan.Parse("16:00")
                }
            };
            public static readonly Request OldRequest = new Request(CompanyProfile.Company, Agency, FakeCompanyProfileJobPositionRate)
            {
                JobLocationId = CompanyLocation.Id,
            };
            public static readonly ReasonCancellationRequest ReasonCancellationRequest = new ReasonCancellationRequest();

            public static void AssertModel(RequestCreateModel model, CompanyRequestDetailModel detail)
            {
                Assert.Equal(model.JobTitle, detail.JobTitle);
                Assert.Equal(model.WorkersQuantity, detail.WorkersQuantity);
                Assert.True(detail.AgencyRate > 0);
                Assert.Equal(model.Description, detail.Description);
                Assert.Equal(model.DurationBreak, detail.DurationBreak);
                Assert.Equal(model.BreakIsPaid, detail.BreakIsPaid);
                Assert.Equal(model.JobIsOnBranchOffice, detail.JobIsOnBranchOffice);
                Assert.Equal(model.LocationId, detail.JobLocation.Id);
                Assert.Equal(model.Incentive, detail.Incentive);
                Assert.Equal(model.IncentiveDescription, detail.IncentiveDescription);
                Assert.Equal(model.Requirements, detail.Requirements);
                Assert.NotNull(detail.Status);
                Assert.NotNull(detail.DurationTerm);
                Assert.NotEqual(default, detail.CreatedAt.Date);
                Assert.NotNull(detail.DisplayShift);
                Assert.Equal(model.StartAt, detail.StartAt);
                Assert.Equal(model.FinishAt, detail.FinishAt);
            }

            static Data()
            {
                CompanyProfile.UpdateName("Microsoft");
                CompanyProfile.AddLocation(CompanyLocation, true);
            }
        }
    }
}