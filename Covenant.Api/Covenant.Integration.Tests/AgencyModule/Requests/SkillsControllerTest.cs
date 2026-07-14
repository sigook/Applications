using Covenant.Api.Controllers.Sigook.Agency.Requests;
using Covenant.Api.Authorization;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AgencyModule.Requests
{
    public class SkillsControllerTest : IClassFixture<CustomWebApplicationFactory<SkillsControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public SkillsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => SkillsController.RouteName.Replace("{requestId}",
            Startup.FakeRequest.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new SkillModel("Customer Service");
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadFromJsonAsync<SkillModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestSkill entity = await context.RequestSkill.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(detail.Id, entity.Id);
            Assert.Equal(model.Skill, entity.Skill);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<IEnumerable<SkillModel>>();
            RequestSkill entity = Startup.FakeSkill;
            SkillModel model = list.Single(c => c.Id == entity.Id);
            Assert.Equal(model.Id, entity.Id);
            Assert.Equal(model.Skill, entity.Skill);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeDeleteSkill.Id;
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.False(await context.RequestSkill.AnyAsync(c => c.Id == id));
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
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Request FakeRequest = FakeData.FakeRequest();
            public static readonly RequestSkill FakeSkill = RequestSkill.Create(FakeRequest.Id, "Supervisor").Value;
            public static readonly RequestSkill FakeDeleteSkill = RequestSkill.Create(FakeRequest.Id, "Delete").Value;

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
                context.Request.Add(FakeRequest);
                context.RequestSkill.AddRange(FakeSkill, FakeDeleteSkill);
                context.SaveChanges();
            }
        }
    }
}
