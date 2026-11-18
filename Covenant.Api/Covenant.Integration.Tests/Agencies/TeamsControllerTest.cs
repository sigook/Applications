using Covenant.Api.Controllers.Agencies;
using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Teams;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agencies;

public class TeamsControllerTest(CustomWebApplicationFactory<TeamsControllerTest.Startup> factory)
    : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TeamsControllerTest.Startup>>
{
    private const string FileName = "Quote Detail.xlsx";
    private static readonly byte[] FileBytes = [1, 2, 3];
    private static readonly Mock<ITeamsService> Teams = CreateTeams();

    private static Mock<ITeamsService> CreateTeams()
    {
        var teams = new Mock<ITeamsService>();
        teams.Setup(t => t.FindChannelFiles(It.Is<TeamsChannelFileFilter>(f => f.Channel == "Management")))
            .ReturnsAsync(Result.Ok<IReadOnlyList<TeamsChannelFileModel>>([new TeamsChannelFileModel(FileName, "https://sharepoint/file", 3, DateTimeOffset.UnixEpoch, "Someone")]));
        teams.Setup(t => t.FindChannelFiles(It.Is<TeamsChannelFileFilter>(f => f.Channel == "Missing")))
            .ReturnsAsync(Result.Fail<IReadOnlyList<TeamsChannelFileModel>>(ResultError.NotFound("Channel not found")));
        teams.Setup(t => t.GetChannelFile(It.Is<TeamsChannelFileFilter>(f => f.Name == FileName)))
            .ReturnsAsync(() => Result.Ok<Stream>(new MemoryStream(FileBytes)));
        teams.Setup(t => t.GetChannelFile(It.Is<TeamsChannelFileFilter>(f => f.Name == "nope.xlsx")))
            .ReturnsAsync(Result.Fail<Stream>(ResultError.NotFound("File not found")));
        return teams;
    }

    private HttpClient As(string role)
    {
        HttpClient client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthenticationHandler.RoleHeader, role);
        return client;
    }

    private static string Url(string action, string channel, string name) =>
        $"{TeamsController.RouteName}/{action}?team=Covenant&channel={channel}&name={Uri.EscapeDataString(name)}";

    [Fact]
    public async Task FindFiles_ReturnsTheMatchingFilesOfTheChannel()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Admin).GetAsync(Url("files", "Management", "Quote"));

        response.EnsureSuccessStatusCode();
        var files = await response.Content.ReadFromJsonAsync<List<TeamsChannelFileModel>>();
        Assert.Equal(FileName, Assert.Single(files!).Name);
    }

    [Fact]
    public async Task FindFiles_UnknownChannel_IsNotFound()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Admin).GetAsync(Url("files", "Missing", "Quote"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task FindFiles_ChannelWithPathCharacters_IsBadRequest()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Admin).GetAsync(Url("files", "Management%2F..", "Quote"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetFileContent_ReturnsTheFileBytes()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Admin).GetAsync(Url("files/content", "Management", FileName));

        response.EnsureSuccessStatusCode();
        Assert.Equal(FileBytes, await response.Content.ReadAsByteArrayAsync());
        Assert.Equal(FileName, response.Content.Headers.ContentDisposition?.FileNameStar ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"'));
    }

    [Fact]
    public async Task GetFileContent_UnknownFile_IsNotFound()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Admin).GetAsync(Url("files/content", "Management", "nope.xlsx"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Recruiting_IsForbidden()
    {
        HttpResponseMessage response = await As(CovenantConstants.Role.Recruiting).GetAsync(Url("files", "Management", "Quote"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(_ => { });
            services.AddTestDatabase();
            services.AddSingleton(Teams.Object);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
