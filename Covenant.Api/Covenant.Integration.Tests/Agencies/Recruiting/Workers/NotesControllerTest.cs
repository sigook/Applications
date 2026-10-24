using Covenant.Api.Authorization;
using Covenant.Api.Controllers.Agencies.Recruiting.Workers;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Workers.Agencies;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Workers;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Workers;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agencies.Recruiting.Workers;

public class NotesControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<NotesControllerTest.Startup>>
{
    private readonly HttpClient _client;

    public NotesControllerTest(CustomWebApplicationFactory<Startup> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Post()
    {
        string url = NotesController.RouteName.Replace("{workerProfileId}", Startup.WorkerProfileId.ToString());
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
        string url = NotesController.RouteName.Replace("{workerProfileId}", Startup.WorkerProfileId.ToString());
        HttpResponseMessage response = await _client.GetAsync(url);
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerProfileNoteListModel>>();
        Assert.NotEmpty(list.Items);
    }

    public class Startup
    {
        public static readonly WorkerProfile FakeWorkerProfile = FakeData.FakeWorkerProfile();
        public static readonly Guid WorkerProfileId = FakeWorkerProfile.Id;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddSub(Guid.NewGuid());
                    o.AddAgencyPersonnelRole();
                });
            services.AddTestDatabase();
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
            context.WorkerProfiles.Add(FakeWorkerProfile);
            context.WorkerProfileNotes.Add(
                WorkerProfileNote.Create(WorkerProfileId, "Worker doesn't have documents", "Recruiter").Value);
            context.SaveChanges();
        }
    }
}
