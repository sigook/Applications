using Covenant.Api.Controllers.Workers.Profile;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Models.Workers;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Workers.Profile;

public class JobExperienceControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData>>
{
    private readonly SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData> _factory;
    private readonly WorkerPortalTestData _data;
    private readonly HttpClient _client;

    public JobExperienceControllerTest(SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData> factory)
    {
        _factory = factory;
        _data = factory.Data;
        _client = _data.AsWorker(factory.CreateClient);
    }

    private string Uri => JobExperienceController.RouteName.Replace("{profileId}", _data.Profile.Id.ToString());

    private Task<List<WorkerProfileJobExperience>> Jobs() =>
        _factory.Services.GetRequiredService<CovenantContext>().WorkerProfiles.AsNoTracking()
            .Where(w => w.Id == _data.Profile.Id)
            .SelectMany(w => w.JobExperiences)
            .ToListAsync();

    private static WorkerProfileJobExperienceModel Job(string company) => new()
    {
        Company = company,
        Supervisor = "Sam Supervisor",
        Duties = "Picking and packing",
        StartDate = new DateTime(2022, 01, 01),
        EndDate = new DateTime(2023, 06, 30)
    };

    [Fact]
    public async Task JobExperience_CanBeAddedUpdatedAndDeleted()
    {
        (await _client.PostAsJsonAsync(Uri, Job("Acme Warehouse"))).EnsureSuccessStatusCode();
        WorkerProfileJobExperience created = Assert.Single(await Jobs(), j => j.Company == "Acme Warehouse");

        (await _client.PutAsJsonAsync($"{Uri}/{created.Id}", Job("Globex Warehouse"))).EnsureSuccessStatusCode();
        Assert.Equal("Globex Warehouse", Assert.Single(await Jobs(), j => j.Id == created.Id).Company);

        (await _client.DeleteAsync($"{Uri}/{created.Id}")).EnsureSuccessStatusCode();
        Assert.DoesNotContain(await Jobs(), j => j.Id == created.Id);
    }

    [Fact]
    public async Task Post_WithoutCompany_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri, Job(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PutAndDelete_UnknownJob_ReturnBadRequest()
    {
        HttpResponseMessage put = await _client.PutAsJsonAsync($"{Uri}/{Guid.NewGuid()}", Job("Nowhere Inc"));
        HttpResponseMessage delete = await _client.DeleteAsync($"{Uri}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, put.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, delete.StatusCode);
    }
}
