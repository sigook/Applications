using Covenant.Api.AgencyModule.AgencyCandidate.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Candidate;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Core.BL.Interfaces;
using Covenant.Core.BL.Services;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Candidate;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCandidate
{
    public class TestAgencyCandidateController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TestAgencyCandidateController.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private const string RequestUri = AgencyCandidateController.RouteName;

        public TestAgencyCandidateController(CustomWebApplicationFactory<Startup> factory)
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
                    Gender = new BaseModel<Guid>(Startup.Male.Id),
                    HasVehicle = true,
                    Source = "Web page",
                    PhoneNumbers = new[] { new PhoneNumberModel("234 5677") },
                    Skills = new[] { new SkillModel("Cleaning"), new SkillModel("Packing") },
                    ResidencyStatus = "Student"
                };
            }

            HttpResponseMessage response = await _client.PostAsJsonAsync(RequestUri, model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<CandidateDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Candidates.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.Name, entity.Name);
            Assert.Equal(model.Email, entity.Email);
            Assert.Equal(model.Address, entity.Address);
            Assert.Equal(model.PostalCode, entity.PostalCode);
            Assert.Equal(model.Gender?.Id, entity.Gender?.Id);
            Assert.Equal(model.HasVehicle, entity.HasVehicle);
            Assert.Equal(model.Source, entity.Source);
            Assert.Equal(model.ResidencyStatus, entity.ResidencyStatus);
            Assert.All(model.PhoneNumbers, c => Assert.Contains(entity.PhoneNumbers, e => e.PhoneNumber == c.PhoneNumber));
            Assert.All(model.Skills, c => Assert.Contains(entity.Skills, e => e.Skill == c.Skill));
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<CandidateListModel>>();
            Assert.NotEmpty(list.Items);
            CandidateListModel model = list.Items.Single(s => s.Id == Startup.FakeCandidate.Id);
            Assert.Equal(Startup.FakeCandidate.Name, model.Name);
            Assert.Equal(Startup.FakeCandidate.Email, model.Email);
            Assert.Equal(Startup.FakeCandidate.Address, model.Address);
            Assert.Equal(Startup.FakeCandidate.PostalCode, model.PostalCode);
            Assert.Equal(Startup.FakeCandidate.Recruiter, model.Recruiter);
            Assert.Equal(Startup.FakeCandidate.ResidencyStatus, model.ResidencyStatus);
            Assert.Equal(Startup.FakeCandidate.CreatedAt, model.CreatedAt);
        }

        [Fact]
        public async Task GetDetail()
        {
            var entity = Startup.FakeCandidate;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri}/{entity.Id.ToString()}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadAsJsonAsync<CandidateDetailModel>();
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.NumberId, result.NumberId);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Email, result.Email);
        }

        [Fact]
        public async Task Update()
        {
            var model = new CandidateCreateModel
            {
                Name = "Rosa Rojas",
                Address = "4567 Dundas",
                PostalCode = "M2P1T7",
                Gender = new BaseModel<Guid>(Startup.Female.Id),
                HasVehicle = true,
                Email = "r@m.com",
                ResidencyStatus = "Citizen"
            };
            Guid id = Startup.FakeCandidate.Id;
            var response = await _client.PutAsJsonAsync($"{RequestUri}/{id}", model);
            response.EnsureSuccessStatusCode();
            var entity = await _factory.Server.Host.Services.GetRequiredService<CovenantContext>().Candidates.SingleAsync(s => s.Id == id);
            Assert.Equal(model.Name, entity.Name);
            Assert.Equal(model.Address, entity.Address);
            Assert.Equal(model.PostalCode, entity.PostalCode);
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
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Candidates.SingleAsync(c => c.Id == id);
            Assert.Equal(Startup.CurrentUser, entity.Recruiter);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeDeleteCandidate.Id;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri}/{id.ToString()}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.Candidates.FirstOrDefaultAsync(c => c.Id == id);
            Assert.True(entity == null);
        }

        public class Startup
        {
            public const string CurrentUser = "mary@mail.com";
            public static readonly Gender Male = new Gender { Id = Guid.NewGuid(), Value = "Male" };
            public static readonly Gender Female = new Gender { Id = Guid.NewGuid(), Value = "Female" };
            private static readonly User FakeAgencyUser = new User(CvnEmail.Create("FakeAgencyUser@email.com").Value);
            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency { Id = FakeAgencyUser.Id, User = FakeAgencyUser };
            public static readonly Candidate FakeCandidate = new Candidate(FakeAgency.Id, "J Martin")
            {
                Address = "MAIN 123",
                Gender = new Gender { Value = "Female" },
                HasVehicle = true,
                Source = "Web page",
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
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICandidateRepository, CandidateRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<ICandidateService, CandidateService>();
                services.AddSingleton<IIdentityServerService, IdentityServerService>();
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
                context.Gender.AddRange(Male, Female);
                context.Agencies.Add(FakeAgency);
                context.Candidates.Add(FakeCandidate);
                context.Candidates.Add(FakeCandidateEmpty);
                context.Candidates.Add(FakeDeleteCandidate);
                context.SaveChanges();
            }
        }
    }
}