using Covenant.Api.AgencyModule.AgencyWorkerProfileNote;
using Covenant.Api.Authorization;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Worker;
using Covenant.Common.Repositories.Worker;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Worker;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyWorkerProfileNote
{
    public class AgencyWorkerProfileNoteControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyWorkerProfileNoteControllerTest.Startup>>
    {
        private readonly HttpClient _client;

        public AgencyWorkerProfileNoteControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

        [Fact]
        public async Task Post()
        {
            string url = AgencyWorkerProfileNoteController.RouteName.Replace("{workerProfileId}", Startup.WorkerProfileId.ToString());
            HttpResponseMessage response = await _client.PostAsJsonAsync(url, new WorkerProfileNoteCreateModel
            {
                Note = "Worker not available",
                CreatedBy = "management@covenantgroupl.com"
            });
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Get()
        {
            string url = AgencyWorkerProfileNoteController.RouteName.Replace("{workerProfileId}", Startup.WorkerProfileId.ToString());
            HttpResponseMessage response = await _client.GetAsync(url);
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<WorkerProfileNoteListModel>>();
            Assert.NotEmpty(list.Items);
        }

        public class Startup
        {
            public static readonly Guid WorkerProfileId = Guid.NewGuid();
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Guid.NewGuid());
                        o.AddAgencyPersonnelRole();
                    });
                services.AddDbContext<CovenantContext>(b =>
                    b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IWorkerRepository, WorkerRepository>();
                services.AddSingleton<ITimeService, TimeService>();
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
                context.WorkerProfileNote.Add(
                    WorkerProfileNote.Create(WorkerProfileId, "Worker doesn't have documents", "Recruiter").Value);
                context.SaveChanges();
            }
        }
    }
}