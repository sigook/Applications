using Covenant.Api.Controllers.Workers.Profile;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Models.Locations;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Workers.Profile;

public class ProfileUpdateControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData>>
{
    private readonly SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData> _factory;
    private readonly WorkerPortalTestData _data;
    private readonly HttpClient _client;

    public ProfileUpdateControllerTest(SeededWebApplicationFactory<WorkerPortalTestStartup, WorkerPortalTestData> factory)
    {
        _factory = factory;
        _data = factory.Data;
        _client = _data.AsWorker(factory.CreateClient);
    }

    private string Uri(string section) => $"{ProfileUpdateController.RouteName.Replace("{profileId}", _data.Profile.Id.ToString())}/{section}";

    private Task<WorkerProfile> Reload() =>
        _factory.Services.GetRequiredService<CovenantContext>().WorkerProfiles.AsNoTracking()
            .Include(w => w.Availabilities)
            .Include(w => w.AvailabilityTimes)
            .Include(w => w.AvailabilityDays)
            .Include(w => w.Languages)
            .Include(w => w.LocationPreferences)
            .Include(w => w.Skills)
            .Include(w => w.Location)
            .SingleAsync(w => w.Id == _data.Profile.Id);

    private static List<BaseModel<Guid>> Ids(params Guid[] ids) => [.. ids.Select(id => new BaseModel<Guid>(id))];

    [Fact]
    public async Task Availabilities_ReplaceTheSelection()
    {
        (await _client.PostAsJsonAsync(Uri("Availabilities"), Ids(_data.FullTime.Id))).EnsureSuccessStatusCode();
        (await _client.PostAsJsonAsync(Uri("Availabilities"), Ids(_data.PartTime.Id))).EnsureSuccessStatusCode();

        Assert.Equal([_data.PartTime.Id], (await Reload()).Availabilities.Select(a => a.AvailabilityId));
    }

    [Fact]
    public async Task AvailabilityDaysAndTimes_AreStored()
    {
        (await _client.PostAsJsonAsync(Uri("AvailabilityDays"), Ids(_data.Monday.Id))).EnsureSuccessStatusCode();
        (await _client.PostAsJsonAsync(Uri("AvailabilityTimes"), Ids(_data.Morning.Id))).EnsureSuccessStatusCode();

        WorkerProfile profile = await Reload();
        Assert.Equal([_data.Monday.Id], profile.AvailabilityDays.Select(d => d.DayId));
        Assert.Equal([_data.Morning.Id], profile.AvailabilityTimes.Select(t => t.AvailabilityTimeId));
    }

    [Fact]
    public async Task Languages_And_LocationPreferences_AreStored()
    {
        (await _client.PostAsJsonAsync(Uri("Languages"), Ids(_data.English.Id, _data.French.Id))).EnsureSuccessStatusCode();
        (await _client.PostAsJsonAsync(Uri("LocationPreferences"), Ids(_data.OtherCity.Id))).EnsureSuccessStatusCode();

        WorkerProfile profile = await Reload();
        Assert.Equal(2, profile.Languages.Count);
        Assert.Equal([_data.OtherCity.Id], profile.LocationPreferences.Select(l => l.CityId));
    }

    [Fact]
    public async Task BasicInformation_UpdatesNamesAndVehicle()
    {
        var model = new WorkerProfileBasicInformationModel { FirstName = "Wanda", LastName = "Walker", BirthDay = new DateTime(1990, 1, 1), HasVehicle = true };

        (await _client.PostAsJsonAsync(Uri("BasicInformation"), model)).EnsureSuccessStatusCode();

        WorkerProfile profile = await Reload();
        Assert.Equal("Wanda", profile.FirstName);
        Assert.Equal("Walker", profile.LastName);
        Assert.True(profile.HasVehicle);
    }

    [Fact]
    public async Task BasicInformation_Invalid_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(Uri("BasicInformation"), new WorkerProfileBasicInformationModel { FirstName = "", LastName = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ContactInformation_UpdatesPhoneAndAddress()
    {
        var model = new WorkerProfileContactInformation
        {
            MobileNumber = "4165550199",
            Location = new LocationModel { Address = "77 Queen St", PostalCode = "M5H2N2", City = new CityModel { Id = _data.City.Id } }
        };

        (await _client.PostAsJsonAsync(Uri("ContactInformation"), model)).EnsureSuccessStatusCode();

        WorkerProfile profile = await Reload();
        Assert.Equal("4165550199", profile.MobileNumber);
        Assert.Equal("77 Queen St", profile.Location.Address);
    }

    [Fact]
    public async Task EmergencyInformation_IsStored()
    {
        var model = new EmergencyInformationModel { ContactEmergencyName = "Ed", ContactEmergencyLastName = "Emergency", ContactEmergencyPhone = "4165550100", HaveAnyHealthProblem = true, HealthProblem = "Asthma" };

        (await _client.PostAsJsonAsync(Uri("EmergencyInformation"), model)).EnsureSuccessStatusCode();

        WorkerProfile profile = await Reload();
        Assert.Equal("Ed", profile.ContactEmergencyName);
        Assert.True(profile.HaveAnyHealthProblem);
    }

    [Fact]
    public async Task OtherInformation_StoresTheLift()
    {
        (await _client.PostAsJsonAsync(Uri("OtherInformation"), new WorkerProfileOtherInformationModel { Lift = new BaseModel<Guid>(_data.Lift.Id) })).EnsureSuccessStatusCode();

        Assert.Equal(_data.Lift.Id, (await Reload()).LiftId);
    }

    [Fact]
    public async Task Skills_AreStored()
    {
        (await _client.PostAsJsonAsync(Uri("Skills"), new List<string> { "Forklift Operator" })).EnsureSuccessStatusCode();

        Assert.Single((await Reload()).Skills);
    }

    [Fact]
    public async Task NullBody_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PostAsync(Uri("Languages"), JsonContent.Create<List<BaseModel<Guid>>>(null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UnknownProfile_ReturnsBadRequest()
    {
        string uri = $"{ProfileUpdateController.RouteName.Replace("{profileId}", Guid.NewGuid().ToString())}/Languages";

        HttpResponseMessage response = await _client.PostAsJsonAsync(uri, Ids(_data.English.Id));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("Certificates")]
    [InlineData("Licenses")]
    [InlineData("OtherDocument")]
    public async Task DeleteUnknownDocument_ReturnsBadRequest(string section)
    {
        HttpResponseMessage response = await _client.DeleteAsync($"{Uri(section)}/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
