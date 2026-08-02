using Covenant.Api.WorkerModule.WorkerRequest.Controllers;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Models.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.WorkerModule.WorkerRequest;

public class WorkerRequestControllerApplyAnonymousTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerRequestControllerApplyAnonymousTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;
    public WorkerRequestControllerApplyAnonymousTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri() => WorkerRequestController.RouteName;

    [Fact]
    public async Task Apply()
    {
        var model = new WorkerRequestApplyModel { Comments = "Hard Worker" };
        var url = $"{RequestUri()}/{Data.WorkerProfile.Worker.Id}/{Data.FakeRequest.Id}/Apply";
        HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<RequestApplicantDetailModel>();
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        var entity = await context.RequestApplicants.SingleAsync(s => s.Id == detail.Id);
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
            context.Requests.Add(Data.FakeRequest);
            context.CompanyProfiles.Add(Data.CompanyProfile);
            context.CompanyProfileJobPositionRates.Add(Data.FakeRate);
            context.WorkerProfiles.Add(Data.WorkerProfile);
            context.SaveChanges();
        }
    }

    internal static class Data
    {
        public static readonly DateTime Now = new DateTime(2019, 01, 01);

        public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency
        {
            User = new User(CvnEmail.Create("agency_apply_anon@mail.com").Value),
            Logo = new CovenantFile("logo.png")
        };

        public static readonly CompanyProfile CompanyProfile = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("company_apply_anon@mail.com").Value),
            Agency = FakeAgency,
            Logo = new CovenantFile()
        };

        public static readonly CompanyProfileJobPositionRate FakeRate = CompanyProfileJobPositionRate.Create(
            CompanyProfile.Id,
            "Position",
            1,
            1,
            "General",
            "r_apply_anon@m.com"
        ).Value;

        public static readonly Request FakeRequest = Request.AgencyCreateRequest(
            CompanyProfile.Id,
            new Location
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
            },
            Now,
            FakeRate.Id
        ).Value;

        public static readonly WorkerProfile WorkerProfile;

        static Data()
        {
            FakeRequest.WorkerRate = 15;
            FakeRequest.UpdateJobTitle("Driver");

            WorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_apply_anon@mail.com").Value))
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
            CompanyProfile.UpdateName("Microsoft Apply Anonymous Test");
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
