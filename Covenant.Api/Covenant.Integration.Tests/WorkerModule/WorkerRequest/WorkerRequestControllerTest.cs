using Covenant.Api.WorkerModule.WorkerRequest.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.WorkerModule.WorkerRequest
{
    public class WorkerRequestControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerRequestControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public WorkerRequestControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => WorkerRequestController.RouteName;

        [Fact, TestOrder(1)]
        public async Task Get()
        {
            var context = _factory.Services.GetService<CovenantContext>();
            context.WorkerRequest.Add(Data.FakeWorkerRequest);
            context.SaveChanges();
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<WorkerRequestListModel>>();
            Assert.NotEmpty(list.Items);
            Request request = Data.FakeRequest;
            WorkerRequestListModel model = list.Items.Single(r => r.Id == request.Id);
            Assert.Equal(request.Id, model.Id);
            Assert.Equal(request.JobTitle, model.JobTitle);
            Assert.Equal(request.NumberId, model.NumberId);
            Assert.Equal(request.WorkersQuantity, model.WorkersQuantity);
            Assert.Equal(request.Agency.FullName, model.AgencyFullName);
            Assert.EndsWith(Data.FakeAgency.Logo.FileName, model.AgencyLogo);
            Assert.Equal(request.IsAsap, model.IsAsap);
            Assert.Equal(request.WorkerRate, model.WorkerRate);
            Assert.Equal(request.CreatedAt, model.CreatedAt);
            Assert.Equal(request.FinishAt, model.FinishAt);
            Assert.Equal(request.StartAt, model.StartAt);
            Assert.Equal(request.DurationTerm.ToString(), model.DurationTerm);
        }

        [Fact, TestOrder(2)]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Data.FakeRequest.Id}");
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<WorkerRequestDetailModel>();
            Assert.NotNull(detail);
            Assert.Equal(Data.FakeRequest.Id, detail.Id);
            Assert.Equal(Data.FakeRequest.JobTitle, detail.JobTitle);
            Assert.Equal(Data.FakeAgency.FullName, detail.AgencyFullName);
            Assert.EndsWith(Data.FakeAgency.Logo.FileName, detail.AgencyLogo);
            Assert.Equal(Data.FakeRequest.Description, detail.Description);
            Assert.Equal(Data.FakeRequest.Requirements, detail.Requirements);
            Assert.Equal(Data.FakeRequest.WorkersQuantity, detail.WorkersQuantity);
            Assert.Equal(Data.FakeRequest.JobPositionRate.OtherJobPosition, detail.JobPosition);
            Assert.Equal(Data.FakeRequest.HolidayIsPaid, detail.HolidayIsPaid);
            Assert.Equal(Data.FakeRequest.BreakIsPaid, detail.BreakIsPaid);
            Assert.Equal(WorkerRequestStatus.Booked.ToString(), detail.Status);
            Assert.Equal(Data.FakeRequest.DurationTerm.ToString(), detail.DurationTerm);
            Assert.Equal(Data.FakeRequest.Incentive, detail.Incentive);
            Assert.Equal(Data.FakeRequest.IncentiveDescription, detail.IncentiveDescription);
            Assert.Equal(Data.FakeRequest.DurationBreak, detail.DurationBreak);
            Assert.Equal(Data.FakeRequest.Status.ToString(), detail.RequestStatus);
            Assert.Equal(Data.FakeRequest.WorkerRate, detail.WorkerRate);
            Assert.Equal(Data.FakeRequest.Status.ToString(), detail.RequestStatus);
            Assert.Equal(Data.FakeRequest.CreatedAt, detail.CreatedAt);
            Assert.Equal(Data.FakeRequest.StartAt, detail.StartAt);
            Assert.Equal(Data.FakeRequest.FinishAt, detail.FinishAt);
            Assert.Equal(Data.FakeRequest.JobLocation.Address, detail.JobLocation.Address);
            Assert.Equal(Data.FakeRequest.JobLocation.Entrance, detail.JobLocation.Entrance);
        }

        [Fact]
        public async Task Apply2()
        {
            var model = new WorkerRequestApplyModel { Comments = "Hard Worker" };
            var url = $"{RequestUri()}/{Data.FakeRequest.Id}/Apply";
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<RequestApplicantDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicant.SingleAsync(s => s.Id == detail.Id);
            Assert.Equal(detail.WorkerProfileId, entity.WorkerProfileId);
            Assert.Equal(model.Comments, entity.Comments);
            Assert.Null(entity.CandidateId);
            response = await _client.PostAsJsonAsync(url, model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.WorkerProfile.Worker.Id);
                        o.AddWorkerRole();
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                var timeService = new Mock<ITimeService>();
                timeService.Setup(s => s.GetCurrentDateTime()).Returns(Data.Now);
                services.AddSingleton(timeService.Object);
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
                context.Request.Add(Data.FakeRequest);
                context.CompanyProfile.Add(Data.CompanyProfile);
                context.CompanyProfileJobPositionRate.Add(Data.FakeRate);
                context.WorkerProfile.Add(Data.WorkerProfile);
                context.SaveChanges();
            }
        }
        internal static class Data
        {
            public static readonly DateTime Now = new DateTime(2019, 01, 01);

            public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency
            {
                User = new User(CvnEmail.Create("agency@mail.com").Value),
                Logo = new CovenantFile("logo.png")
            };
            public static readonly CompanyProfile CompanyProfile = new CompanyProfile { Company = new User(CvnEmail.Create("company@mail.com").Value), Agency = FakeAgency, Logo = new CovenantFile() };
            public static readonly WorkerProfile WorkerProfile;
            public static readonly CompanyProfileJobPositionRate FakeRate = CompanyProfileJobPositionRate.Create(CompanyProfile.Id, "Position", 1, 1, "General", "r@m.com").Value;

            public static readonly Request FakeRequest = Request.AgencyCreateRequest(FakeAgency.Id, CompanyProfile.Company.Id, new Location
            {
                Address = "4917 Dundas",
                Entrance = "Back Door",
                City = new City
                {
                    Province = new Province
                    {
                        Id = Guid.NewGuid(),
                        Value = "Province Test",
                        Code = "ON",
                        Country = new Country
                        {
                            Id = Guid.NewGuid(),
                            Value = "Country Test",
                            Code = "CA"
                        }
                    }
                }
            }, Now, FakeRate.Id).Value;
            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest;

            static Data()
            {
                FakeRequest.WorkerRate = 15;
                FakeRequest.UpdateJobTitle("Driver");
                WorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_profile@mail.com").Value))
                {
                    Agency = FakeAgency,
                    ApprovedToWork = true,
                    Location = new Location { City = new City { Province = new Province { Country = new Country { Code = "USA" } } } },
                    IdentificationType1 = new IdentificationType(),
                    IdentificationType1File = new CovenantFile(),
                    IdentificationType2 = new IdentificationType(),
                    IdentificationType2File = new CovenantFile(),
                };
                WorkerProfile.PatchProfileImage(new CovenantFile("profile.png"));
                WorkerProfile.PatchSinInformation(new FakeSinInfo());
                WorkerProfile.PatchDocuments(new FakeDocuments());
                CompanyProfile.UpdateName("Microsoft");
                FakeWorkerRequest = Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(WorkerProfile.WorkerId, FakeRequest.Id);
            }

            private class FakeSinInfo : ISinInformation<CovenantFile>
            {
                public string SocialInsurance { get; set; } = "BFF1AAB7E4BD";
                public bool SocialInsuranceExpire { get; set; } = false;
                public DateTime? DueDate { get; set; }
                public CovenantFile SocialInsuranceFile { get; set; } = new CovenantFile("s.pdf");
            }

            private class FakeDocuments : IWorkerDocumentsInformation<IdentificationType, CovenantFile>
            {
                public string IdentificationNumber1 { get; set; } = "5C7292F4D4";
                public IdentificationType IdentificationType1 { get; set; }
                public CovenantFile IdentificationType1File { get; set; } = new CovenantFile("d.pdf");
                public string IdentificationNumber2 { get; set; } = "92EAC1D901";
                public IdentificationType IdentificationType2 { get; set; }
                public CovenantFile IdentificationType2File { get; set; } = new CovenantFile("2.pdf");
                public bool HavePoliceCheckBackground { get; set; }
                public CovenantFile PoliceCheckBackGround { get; set; }
                public CovenantFile Resume { get; set; }
            }
        }
    }
}