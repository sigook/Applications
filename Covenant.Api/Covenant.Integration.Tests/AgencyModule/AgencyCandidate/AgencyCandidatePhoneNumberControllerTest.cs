using Covenant.Api.AgencyModule.AgencyCandidatePhoneNumber.Controllers;
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
    public class AgencyCandidatePhoneNumberControllerTest :
        IClassFixture<CustomWebApplicationFactory<AgencyCandidatePhoneNumberControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyCandidatePhoneNumberControllerTest(CustomWebApplicationFactory<AgencyCandidatePhoneNumberControllerTest.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCandidatePhoneNumberController.RouteName.Replace("{candidateId}",
            Startup.FakeCandidate.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new PhoneNumberModel("647897045");
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<PhoneNumberModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidatePhones.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(detail.Id, entity.Id);
            Assert.Equal(model.PhoneNumber, entity.PhoneNumber);
        }

        [Fact]
        public async Task GetById()
        {
            var entity = Startup.FakePhone;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{entity.Id}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<PhoneNumberModel>();
            AssertDetailAndEntity(model, entity);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeDeletePhone.Id;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.CandidatePhones.AnyAsync(c => c.Id == id));
        }

        private static void AssertDetailAndEntity(PhoneNumberModel model, CandidatePhone entity)
        {
            Assert.Equal(model.Id, entity.Id);
            Assert.Equal(model.PhoneNumber, entity.PhoneNumber);
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
            public static CandidatePhone FakePhone = null;
            public static CandidatePhone FakeDeletePhone = null;
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
                FakePhone = FakeCandidate.AddPhone("6547894356").Value;
                FakeDeletePhone = FakeCandidate.AddPhone("647890743").Value;
                context.Candidates.Add(FakeCandidate);
                context.SaveChanges();
            }
        }
    }
}