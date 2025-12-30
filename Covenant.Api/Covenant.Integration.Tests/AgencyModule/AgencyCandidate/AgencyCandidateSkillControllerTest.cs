using Covenant.Api.AgencyModule.AgencyCandidateSkill.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Candidate;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCandidate
{
    public class AgencyCandidateSkillControllerTest : IClassFixture<CustomWebApplicationFactory<AgencyCandidateSkillControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyCandidateSkillControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCandidateSkillController.RouteName.Replace("{candidateId}",
            Startup.FakeCandidate.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new SkillModel("AZ Driver");
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<SkillModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateSkills.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(detail.Id, entity.Id);
            Assert.Equal(model.Skill, entity.Skill);
        }

        [Fact]
        public async Task GetById()
        {
            var entity = Startup.FakeSkill;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<SkillModel>();
            AssertDetailAndEntity(model, entity);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeDeleteSkill.Id;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.CandidateSkills.AnyAsync(c => c.Id == id));
        }

        private static void AssertDetailAndEntity(SkillModel model, CandidateSkill entity)
        {
            Assert.Equal(model.Id, entity.Id);
            Assert.Equal(model.Skill, entity.Skill);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o => o.AddAgencyPersonnelRole());
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICandidateRepository, CandidateRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly Candidate FakeCandidate = new Candidate(FakeAgency.Id, "B Martin") { Agency = FakeAgency };
            public static readonly CandidateSkill FakeSkill = CandidateSkill.Create(FakeCandidate.Id, "Supervisor").Value;
            public static readonly CandidateSkill FakeDeleteSkill = CandidateSkill.Create(FakeCandidate.Id, "Delete").Value;

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
                FakeCandidate.Skills.Add(FakeSkill);
                FakeCandidate.Skills.Add(FakeDeleteSkill);
                context.Candidates.Add(FakeCandidate);
                context.SaveChanges();
            }
        }
    }
}