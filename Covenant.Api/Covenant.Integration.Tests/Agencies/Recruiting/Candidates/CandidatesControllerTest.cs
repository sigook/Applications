using ClosedXML.Excel;
using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Recruiting.Candidates;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Candidates;
using Covenant.Common.Entities;
using Covenant.Common.Interfaces.Identity;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Candidates;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidates;
using Covenant.Common.Repositories.Requests;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces.Candidates;
using Covenant.Core.BL.Services.Candidates;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Candidates;
using Covenant.Infrastructure.Repositories.Requests;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Candidates;

public class CandidatesControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<CandidatesControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;
    private const string RequestUri = CandidatesController.RouteName;

    public CandidatesControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("Full")]
    [InlineData("Simple")]
    public async Task Post(string modelType)
    {
        CandidateCreateModel model;
        if (modelType == "Simple") model = new CandidateCreateModel { Name = "Pepe Perez 2", Email = "pepe@e.com", };
        else
        {
            model = new CandidateCreateModel
            {
                Name = "Pepe Perez",
                Email = $"{modelType}pepe@e.com",
                Address = "MAIN 123",
                PostalCode = "M1M2M1",
                City = new CityModel(Startup.FakeCity.Id, Startup.FakeCity.Value),
                Gender = new BaseModel<Guid>(Startup.Male.Id),
                HasVehicle = true,
                SourceId = Startup.WebPageSource.Id,
                PhoneNumbers = new[] { new PhoneNumberModel("234 5677") },
                Skills = new[] { new SkillModel("Cleaning"), new SkillModel("Packing") },
                ResidencyStatus = "Student"
            };
        }

        using var content = new MultipartFormDataContent
        {
            { new StringContent(JsonSerializer.Serialize(model)), "data" }
        };
        HttpResponseMessage response = await _client.PostAsync(RequestUri, content);
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<CandidateDetailModel>();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.Candidates.SingleAsync(c => c.Id == detail.Id);
        Assert.Equal(model.Name, entity.Name);
        Assert.Equal(model.Email, entity.Email);
        Assert.Equal(model.Address, entity.Address);
        Assert.Equal(model.PostalCode, entity.PostalCode);
        Assert.Equal(model.City?.Id, entity.CityId);
        Assert.Equal(model.Gender?.Id, entity.Gender?.Id);
        Assert.Equal(model.HasVehicle, entity.HasVehicle);
        Assert.Equal(model.SourceId, entity.SourceId);
        Assert.Equal(model.ResidencyStatus, entity.ResidencyStatus);
        Assert.All(model.PhoneNumbers, c => Assert.Contains(entity.PhoneNumbers, e => e.PhoneNumber == c.PhoneNumber));
        Assert.All(model.Skills, c => Assert.Contains(entity.Skills, e => e.Skill == c.Skill));
    }

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<CandidateListModel>>();
        Assert.NotEmpty(list.Items);
        CandidateListModel model = list.Items.Single(s => s.Id == Startup.FakeCandidate.Id);
        Assert.Equal(Startup.FakeCandidate.Name, model.Name);
        Assert.Equal(Startup.FakeCandidate.Email, model.Email);
        Assert.Equal(Startup.FakeCandidate.Address, model.Address);
        Assert.Equal(Startup.FakeCandidate.PostalCode, model.PostalCode);
        Assert.Equal(Startup.FakeCity.Value, model.City);
        Assert.Equal(Startup.FakeCity.Province.Code, model.Province);
        Assert.Equal(Startup.FakeCandidate.Recruiter, model.Recruiter);
        Assert.Equal(Startup.FakeCandidate.ResidencyStatus, model.ResidencyStatus);
        DateAssert.Equal(Startup.FakeCandidate.CreatedAt, model.CreatedAt);
    }

    [Fact]
    public async Task GetDetail()
    {
        var entity = Startup.FakeCandidate;
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri}/{entity.Id.ToString()}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CandidateDetailModel>();
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(entity.NumberId, result.NumberId);
        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.Email, result.Email);
        Assert.Equal(Startup.FakeCity.Id, result.City.Id);
        Assert.Equal(Startup.FakeCity.Province.Id, result.City.Province.Id);
        Assert.Equal(Startup.FakeCity.Province.Country.Id, result.City.Province.Country.Id);
    }

    [Fact]
    public async Task Update()
    {
        var model = new CandidateCreateModel
        {
            Name = "Rosa Rojas",
            Address = "4567 Dundas",
            PostalCode = "M2P1T7",
            City = new CityModel(Startup.FakeOtherCity.Id, Startup.FakeOtherCity.Value),
            Gender = new BaseModel<Guid>(Startup.Female.Id),
            HasVehicle = true,
            Email = "r@m.com",
            ResidencyStatus = "Citizen"
        };
        Guid id = Startup.FakeCandidate.Id;
        var response = await _client.PutAsJsonAsync($"{RequestUri}/{id}", model);
        response.EnsureSuccessStatusCode();
        var entity = await _factory.Services.GetRequiredService<CovenantContext>().Candidates.SingleAsync(s => s.Id == id);
        Assert.Equal(model.Name, entity.Name);
        Assert.Equal(model.Address, entity.Address);
        Assert.Equal(model.PostalCode, entity.PostalCode);
        Assert.Equal(model.City.Id, entity.CityId);
        Assert.Equal(model.Gender.Id, entity.Gender.Id);
        Assert.Equal(model.HasVehicle, entity.HasVehicle);
        Assert.Equal(model.Email, entity.Email);
        Assert.Equal(model.ResidencyStatus, entity.ResidencyStatus);
    }

    [Fact]
    public async Task UpdateRecruiter()
    {
        Guid id = Startup.FakeCandidate.Id;
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri}/{id.ToString()}/Recruiter", new { });
        response.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.Candidates.SingleAsync(c => c.Id == id);
        Assert.Equal(Startup.CurrentUser, entity.Recruiter);
    }

    [Fact]
    public async Task Delete()
    {
        Guid id = Startup.FakeDeleteCandidate.Id;
        HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri}/{id.ToString()}");
        response.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var entity = await context.Candidates.FirstOrDefaultAsync(c => c.Id == id);
        Assert.True(entity == null);
    }

    [Fact]
    public async Task GetFile_ExportsAgencyCandidates()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri}/File");
        response.EnsureSuccessStatusCode();
        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        var cells = workbook.Worksheet(1).RowsUsed().Skip(1).SelectMany(r => r.CellsUsed()).Select(c => c.GetString()).ToList();
        Assert.Contains(Startup.FakeCandidate.Email, cells);
    }

    private Task<HttpResponseMessage> PostBulk(string csv)
    {
        var content = new MultipartFormDataContent
        {
            { new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(csv)), "file", "candidates.csv" }
        };
        return _client.PostAsync($"{RequestUri}/bulk/{Startup.AgencyId}", content);
    }

    [Fact]
    public async Task BulkCandidates_ImportsValidRows_AndReturnsErrorsForInvalidOnes()
    {
        const string csv = """
            Full Name,Email,Phone,Address,Skills,Status,Gender,Has Transportation?,Request ID,Source,URL Resume
            Bulk Valid,bulk.valid@mail.com,416 555-0101,1 King St,Forklift,Citizen,Female,Yes,,Web page,
            Bulk Bad Phone,bulk.badphone@mail.com,4165550102,2 King St,Forklift,Citizen,Male,No,,Web page,
            Bulk Foreign Request,bulk.request@mail.com,416 555-0103,3 King St,,Citizen,Male,No,999999,Web page,
            """;

        HttpResponseMessage response = await PostBulk(csv);

        response.EnsureSuccessStatusCode();
        using var workbook = new XLWorkbook(await response.Content.ReadAsStreamAsync());
        var names = workbook.Worksheet(1).RowsUsed().Skip(1).Select(r => r.Cell(1).GetString()).ToList();
        Assert.Equal(["Bulk Bad Phone", "Bulk Foreign Request"], names);
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var imported = await context.Candidates.Include(c => c.Gender).Include(c => c.PhoneNumbers).Include(c => c.Skills).SingleAsync(c => c.Name == "Bulk Valid");
        Assert.Equal(Startup.AgencyId, imported.AgencyId);
        Assert.Equal("Female", imported.Gender.Value);
        Assert.Equal(Startup.WebPageSource.Id, imported.SourceId);
        Assert.Contains(imported.PhoneNumbers, p => p.PhoneNumber == "416 555-0101");
        Assert.Contains(imported.Skills, s => s.Skill == "Forklift");
        Assert.False(await context.Candidates.AnyAsync(c => c.Name == "Bulk Bad Phone"));
    }

    [Fact]
    public async Task BulkCandidates_OnlyValidRows_ReturnsOkWithoutFile()
    {
        const string csv = """
            Full Name,Email,Phone,Address,Skills,Status,Gender,Has Transportation?,Request ID,Source,URL Resume
            Bulk Only Valid,bulk.only@mail.com,416 555-0201,4 King St,Packing,Citizen,Female,Yes,,Web page,
            """;

        HttpResponseMessage response = await PostBulk(csv);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task BulkCandidates_MissingHeaders_ReturnsBadRequest()
    {
        HttpResponseMessage response = await PostBulk("Full Name,Email\nNo Headers,no.headers@mail.com\n");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public const string CurrentUser = "mary@mail.com";
        public static readonly Gender Male = new Gender { Id = Guid.NewGuid(), Value = "Male" };
        public static readonly Gender Female = new Gender { Id = Guid.NewGuid(), Value = "Female" };
        public static readonly Source WebPageSource = new Source { Id = Guid.NewGuid(), Value = "Web page" };
        private static readonly User FakeAgencyUser = new User(CvnEmail.Create("FakeAgencyUser@email.com").Value);
        private static readonly Agency FakeAgency = new Agency { Id = FakeAgencyUser.Id, User = FakeAgencyUser };
        public static Guid AgencyId => FakeAgency.Id;
        public static readonly City FakeCity = FakeData.FakeCity();
        public static readonly City FakeOtherCity = FakeData.FakeCity(FakeCity.Province);
        public static readonly Candidate FakeCandidate = new Candidate(FakeAgency.Id, "J Martin")
        {
            Address = "MAIN 123",
            CityId = FakeCity.Id,
            Gender = new Gender { Value = "Female" },
            HasVehicle = true,
            SourceId = WebPageSource.Id,
            Recruiter = "recruiter",
            AgencyId = FakeAgency.Id,
            ResidencyStatus = "Foreign Worker"
        };

        private static readonly Candidate FakeCandidateEmpty = new Candidate(FakeAgency.Id, "B Martin");
        public static readonly Candidate FakeDeleteCandidate = new Candidate(FakeAgency.Id, "J Martin");

        public Startup()
        {
            FakeCandidate.AddEmail(CvnEmail.Create("g@m.com").Value);
            FakeCandidate.AddPostalCode(CvnPostalCode.Create("M1M2M1").Value);
            FakeCandidate.Documents = new List<CandidateDocument>()
            {
                new CandidateDocument(FakeCandidate.Id, new CovenantFile("resume.png", "resume")),
                new CandidateDocument(FakeCandidate.Id, new CovenantFile("resume2.png", "resume2"))
            };
            FakeCandidate.AddSkill("Computer");
            FakeCandidate.AddSkill("Cleaning");
            FakeCandidate.AddPhone("23456774567");
            FakeCandidate.AddPhone("234568845");
            FakeDeleteCandidate.AddEmail(CvnEmail.Create("e@l.com").Value);
            FakeCandidateEmpty.AddEmail(CvnEmail.Create("f@m.com").Value);
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddAgencyPersonnelRole(FakeAgency.Id);
                    o.AddName(CurrentUser);
                });
            services.AddTestDatabase();
            services.AddSingleton<ICandidateRepository, CandidateRepository>();
            services.AddSingleton<ICandidateService, CandidateService>();
            services.AddSingleton<IUserAccountService, UserAccountService>();
            services.AddSingleton<IRequestRepository, RequestRepository>();
            services.AddSingleton<IWorkerRepository, WorkerRepository>();
            services.AddSingleton<AgencyIdFilter>();
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
            context.Genders.AddRange(Male, Female);
            context.Sources.Add(WebPageSource);
            context.Cities.AddRange(FakeCity, FakeOtherCity);
            context.Agencies.Add(FakeAgency);
            context.Candidates.Add(FakeCandidate);
            context.Candidates.Add(FakeCandidateEmpty);
            context.Candidates.Add(FakeDeleteCandidate);
            context.SaveChanges();
        }
    }
}
