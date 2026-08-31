using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using CandidateEntity = Covenant.Common.Entities.Candidate.Candidate;

namespace Covenant.Integration.Tests.WebSite;

public class WebSiteControllerCandidateApplyAnonymousTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WebSiteControllerCandidateApplyAnonymousTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public WebSiteControllerCandidateApplyAnonymousTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Apply()
    {
        var url = $"api/WebSite/candidate/{Data.FakeCandidate.Id}/{Data.FakeRequest.Id}/apply";
        HttpResponseMessage response = await _client.PostAsync(url, null);
        response.EnsureSuccessStatusCode();
        var applicantId = await response.Content.ReadFromJsonAsync<Guid>();
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        var entity = await context.RequestApplicants.SingleAsync(s => s.Id == applicantId);
        Assert.Equal(Data.FakeCandidate.Id, entity.CandidateId);
        Assert.Null(entity.WorkerProfileId);
        Assert.Equal(RequestApplicantStatus.Pending, entity.Status);
        Assert.Equal("Sigook", entity.CreatedBy);
        var skill = await context.Set<CandidateSkill>().SingleAsync(s => s.CandidateId == Data.FakeCandidate.Id);
        Assert.Equal(Data.FakeRequest.JobTitle, skill.Skill);
        response = await _client.PostAsync(url, null);
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
            context.Candidates.Add(Data.FakeCandidate);
            context.SaveChanges();
        }
    }

    internal static class Data
    {
        public static readonly DateTime Now = new DateTime(2019, 01, 01);

        public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency
        {
            User = new User(CvnEmail.Create("agency_candidate_apply@mail.com").Value),
            Logo = new CovenantFile("logo.png")
        };

        public static readonly CompanyProfile CompanyProfile = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("company_candidate_apply@mail.com").Value),
            Agency = FakeAgency,
            Logo = new CovenantFile()
        };

        public static readonly CompanyProfileJobPositionRate FakeRate = CompanyProfileJobPositionRate.Create(
            CompanyProfile.Id,
            "Position",
            1,
            1,
            "General",
            "r_candidate_apply@m.com"
        ).Value;

        public static readonly Request FakeRequest = Request.AgencyCreateRequest(
            CompanyProfile.Id,
            new Location
            {
                Address = "4917 Dundas",
                City = new City
                {
                    Value = "Toronto",
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

        public static readonly CandidateEntity FakeCandidate;

        static Data()
        {
            FakeRequest.WorkerRate = 15;
            FakeRequest.UpdateJobTitle("Driver");
            CompanyProfile.UpdateName("Microsoft Candidate Apply Test");

            FakeCandidate = new CandidateEntity(FakeAgency.Id, "Invited Candidate")
            {
                Agency = FakeAgency,
                Email = "candidate_apply@mail.com",
                Address = "1 Main St, Toronto, ON"
            };
        }
    }
}
