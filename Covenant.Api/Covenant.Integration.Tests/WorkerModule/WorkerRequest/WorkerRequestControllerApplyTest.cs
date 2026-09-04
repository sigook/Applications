using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Company;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Models.Worker;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

using Covenant.Api.WorkerModule.WorkerRequest.Controllers;

namespace Covenant.Integration.Tests.WorkerModule.WorkerRequest;

public class WorkerRequestControllerApplyTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkerRequestControllerApplyTest.Startup>>
{
    private static string RequestUri => $"{WorkerRequestController.RouteName}/Apply";
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public WorkerRequestControllerApplyTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private Task<HttpResponseMessage> Apply(int numberId, string email) =>
        _client.PostAsJsonAsync(RequestUri, new WorkerRequestApplyModel { NumberId = numberId, Email = email });

    private CovenantContext GetContext() => _factory.Server.Host.Services.GetRequiredService<CovenantContext>();

    [Fact]
    public async Task ApplyAsWorker()
    {
        var response = await Apply(Data.WorkerRequestNumberId, "worker_applybyemail@mail.com");
        response.EnsureSuccessStatusCode();
        var entity = await GetContext().RequestApplicants.SingleAsync(ra => ra.RequestId == Data.WorkerRequest.Id);
        Assert.Equal(Data.WorkerProfile.Id, entity.WorkerProfileId);
        Assert.Null(entity.CandidateId);
        response = await Apply(Data.WorkerRequestNumberId, "worker_applybyemail@mail.com");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ApplyAsCandidate()
    {
        var response = await Apply(Data.CandidateRequestNumberId, "candidate_applybyemail@mail.com");
        response.EnsureSuccessStatusCode();
        var entity = await GetContext().RequestApplicants.SingleAsync(ra => ra.RequestId == Data.CandidateRequest.Id);
        Assert.Equal(Data.MatchingCandidate.Id, entity.CandidateId);
        Assert.Null(entity.WorkerProfileId);
        Assert.Equal("Sigook", entity.CreatedBy);
        Assert.Equal(RequestApplicantStatus.Pending, entity.Status);
        response = await Apply(Data.CandidateRequestNumberId, "  CANDIDATE_APPLYBYEMAIL@MAIL.COM  ");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ApplyAsCandidateWithAccentedAddress()
    {
        var response = await Apply(Data.MontrealRequestNumberId, "accent_applybyemail@mail.com");
        response.EnsureSuccessStatusCode();
        var entity = await GetContext().RequestApplicants.SingleAsync(ra => ra.RequestId == Data.MontrealRequest.Id);
        Assert.Equal(Data.AccentCandidate.Id, entity.CandidateId);
    }

    [Fact]
    public async Task WorkerWinsOverCandidateWithSameEmail()
    {
        var response = await Apply(Data.PrecedenceRequestNumberId, "worker_applybyemail@mail.com");
        response.EnsureSuccessStatusCode();
        var entity = await GetContext().RequestApplicants.SingleAsync(ra => ra.RequestId == Data.PrecedenceRequest.Id);
        Assert.Equal(Data.WorkerProfile.Id, entity.WorkerProfileId);
        Assert.Null(entity.CandidateId);
    }

    [Fact]
    public async Task ApplyFailsWhenCandidateCityDoesNotMatch()
    {
        var response = await Apply(Data.CandidateRequestNumberId, "mismatch_applybyemail@mail.com");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ApplyFailsWhenCandidateIsDnu()
    {
        var response = await Apply(Data.CandidateRequestNumberId, "dnu_applybyemail@mail.com");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ApplyFailsWhenEmailIsUnknown()
    {
        var response = await Apply(Data.CandidateRequestNumberId, "unknown_applybyemail@mail.com");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ApplyFailsWhenNumberIdIsUnknown()
    {
        var response = await Apply(999999, "candidate_applybyemail@mail.com");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestDatabase();
            services.AddMemoryCache();
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
            context.CompanyProfiles.Add(Data.CompanyProfile);
            context.CompanyProfileJobPositionRates.Add(Data.FakeRate);
            context.Requests.AddRange(Data.WorkerRequest, Data.CandidateRequest, Data.MontrealRequest, Data.PrecedenceRequest);
            context.WorkerProfiles.Add(Data.WorkerProfile);
            context.Candidates.AddRange(Data.MatchingCandidate, Data.AccentCandidate, Data.MismatchCandidate, Data.DnuCandidate, Data.PrecedenceCandidate);
            context.SaveChanges();
        }
    }

    internal static class Data
    {
        public const int WorkerRequestNumberId = 424242;
        public const int CandidateRequestNumberId = 424243;
        public const int MontrealRequestNumberId = 424244;
        public const int PrecedenceRequestNumberId = 424245;

        public static readonly DateTime Now = new DateTime(2019, 01, 01);

        public static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency
        {
            User = new User(CvnEmail.Create("agency_applybyemail@mail.com").Value),
            Logo = new CovenantFile("logo.png")
        };

        public static readonly CompanyProfile CompanyProfile = new CompanyProfile
        {
            Company = new User(CvnEmail.Create("company_applybyemail@mail.com").Value),
            Agency = FakeAgency,
            Logo = new CovenantFile(),
            Industry = new CompanyProfileIndustry("Test")
        };

        public static readonly CompanyProfileJobPositionRate FakeRate = CompanyProfileJobPositionRate.Create(
            CompanyProfile.Id,
            "Position",
            1,
            1,
            "General",
            "r_applybyemail@m.com"
        ).Value;

        public static readonly Request WorkerRequest = CreateRequest(WorkerRequestNumberId, "Toronto");
        public static readonly Request CandidateRequest = CreateRequest(CandidateRequestNumberId, "Toronto");
        public static readonly Request MontrealRequest = CreateRequest(MontrealRequestNumberId, "Montreal");
        public static readonly Request PrecedenceRequest = CreateRequest(PrecedenceRequestNumberId, "Toronto");

        public static readonly WorkerProfile WorkerProfile;

        public static readonly Candidate MatchingCandidate = CreateCandidate("Jane Candidate", "candidate_applybyemail@mail.com", "25 Bay St, Toronto ON");
        public static readonly Candidate AccentCandidate = CreateCandidate("Marie Candidate", "accent_applybyemail@mail.com", "25 Rue Sainte-Catherine, Montréal QC");
        public static readonly Candidate MismatchCandidate = CreateCandidate("Paul Candidate", "mismatch_applybyemail@mail.com", "25 Laurier Ave, Ottawa ON");
        public static readonly Candidate DnuCandidate = CreateCandidate("Dnu Candidate", "dnu_applybyemail@mail.com", "25 Bay St, Toronto ON", dnu: true);
        public static readonly Candidate PrecedenceCandidate = CreateCandidate("Worker Twin", "worker_applybyemail@mail.com", "25 Bay St, Toronto ON");

        static Data()
        {
            WorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_applybyemail@mail.com").Value))
            {
                Agency = FakeAgency,
                ApprovedToWork = true,
                Location = new Location { City = new City { Province = new Province { Country = FakeData.FakeCountry("USA") } } },
                IdentificationType1 = new IdentificationType(),
                IdentificationType1File = new CovenantFile(),
                IdentificationType2 = new IdentificationType(),
                IdentificationType2File = new CovenantFile(),
            };

            WorkerProfile.PatchProfileImage(new CovenantFile("profile.png"));
            WorkerProfile.PatchSinInformation(new FakeSinInfo());
            WorkerProfile.PatchDocuments(new FakeDocuments());
            CompanyProfile.UpdateName("Microsoft Apply By Email Test");
        }

        private static Request CreateRequest(int numberId, string city)
        {
            var request = Request.AgencyCreateRequest(
                CompanyProfile.Id,
                new Location
                {
                    Address = "4917 Dundas",
                    City = new City
                    {
                        Value = city,
                        Province = new Province
                        {
                            Id = Guid.NewGuid(),
                            Value = "Province Test",
                            Code = "ON",
                            Country = FakeData.FakeCountry("CA")
                        }
                    }
                },
                Now,
                FakeRate.Id
            ).Value;
            request.NumberId = numberId;
            request.WorkerRate = 15;
            request.UpdateJobTitle("Driver");
            return request;
        }

        private static Candidate CreateCandidate(string name, string email, string address, bool dnu = false) =>
            new Candidate(FakeAgency.Id, name, CvnEmail.Create(email).Value) { Address = address, Dnu = dnu };

        private class FakeSinInfo : ISinInformation<CovenantFile>
        {
            public string SocialInsurance { get; set; } = "AB12CD34EF56";
            public bool SocialInsuranceExpire { get; set; } = false;
            public DateTime? DueDate { get; set; }
            public CovenantFile SocialInsuranceFile { get; set; } = new CovenantFile("s.pdf");
        }

        private class FakeDocuments : IWorkerDocumentsInformation<IdentificationType, CovenantFile>
        {
            public string IdentificationNumber1 { get; set; } = "1A2B3C4D5E";
            public IdentificationType IdentificationType1 { get; set; }
            public CovenantFile IdentificationType1File { get; set; } = new CovenantFile("d.pdf");
            public string IdentificationNumber2 { get; set; } = "6F7G8H9I0J";
            public IdentificationType IdentificationType2 { get; set; }
            public CovenantFile IdentificationType2File { get; set; } = new CovenantFile("2.pdf");
            public bool HavePoliceCheckBackground { get; set; }
            public CovenantFile PoliceCheckBackGround { get; set; }
            public CovenantFile Resume { get; set; }
        }
    }
}
