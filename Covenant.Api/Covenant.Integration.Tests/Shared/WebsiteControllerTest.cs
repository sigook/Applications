using Covenant.Api.Controllers.Shared;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Enums;
using Covenant.Common.Functionals;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Website;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Covenant.Integration.Tests.Shared;

public class WebsiteControllerTest(CustomWebApplicationFactory<WebsiteControllerTest.Startup> factory) : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WebsiteControllerTest.Startup>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetJobsReturnsEmploymentTypeAsDisplayName()
    {
        var response = await _client.GetAsync($"{WebsiteController.RouteName}/jobs?countries=CA");
        response.EnsureSuccessStatusCode();
        var jobs = await response.Content.ReadFromJsonAsync<List<JobViewModel>>();
        var job = Assert.Single(jobs, j => j.RequestId == Data.FakeRequest.Id);
        Assert.Equal("Part Time", job.Type);
    }

    private static MultipartFormDataContent CandidateForm(string claimedBlobName, string uploadedName)
    {
        var form = new MultipartFormDataContent
        {
            { new StringContent(JsonSerializer.Serialize(new { fullName = "Nina New", email = "nina@mail.com", fileName = claimedBlobName })), "data" }
        };
        if (uploadedName is not null)
        {
            form.Add(new ByteArrayContent([1, 2, 3]), claimedBlobName, uploadedName);
        }
        return form;
    }

    [Fact]
    public async Task CreateCandidate_StoresTheResumeUnderAServerGeneratedName()
    {
        Startup.FilesContainer.Invocations.Clear();

        var response = await _client.PostAsync($"{WebsiteController.RouteName}/candidate", CandidateForm("company.png", "cv.pdf"));

        response.EnsureSuccessStatusCode();
        Startup.FilesContainer.Verify(f => f.UploadAsync(It.IsAny<Stream>(), "company.png"), Times.Never);
        Startup.FilesContainer.Verify(f => f.UploadAsync(It.IsAny<Stream>(), It.Is<string>(n => IsServerGeneratedPdfName(n))), Times.Once);
    }

    private static bool IsServerGeneratedPdfName(string name) =>
        name.EndsWith(".pdf") && Guid.TryParse(Path.GetFileNameWithoutExtension(name), out _);

    [Theory]
    [InlineData("login.html", "login.html")]
    [InlineData("login.html", "a.svg")]
    public async Task CreateCandidate_DisallowedResumeFormat_ReturnsBadRequestWithoutUploading(string claimedBlobName, string uploadedName)
    {
        Startup.FilesContainer.Invocations.Clear();

        var response = await _client.PostAsync($"{WebsiteController.RouteName}/candidate", CandidateForm(claimedBlobName, uploadedName));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Startup.FilesContainer.Verify(f => f.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateCandidate_WithoutResume_NeverTouchesTheBlobNamedByTheClient()
    {
        Startup.FilesContainer.Invocations.Clear();

        var response = await _client.PostAsync($"{WebsiteController.RouteName}/candidate", CandidateForm("company.png", null));

        response.EnsureSuccessStatusCode();
        Assert.Empty(Startup.FilesContainer.Invocations);
    }

    public class Startup
    {
        public static readonly Mock<IFilesContainer> FilesContainer = new();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestDatabase();
            services.AddMemoryCache();
            services.AddSingleton(Mock.Of<IRazorViewToStringRenderer>());
            services.AddSingleton(Mock.Of<ITeamsService>());
            FilesContainer.Setup(f => f.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>())).ReturnsAsync(Result.Ok());
            services.AddSingleton(FilesContainer.Object);
        }

        public void Configure(IApplicationBuilder app, CovenantContext context)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            Data.Seed(context);
        }
    }

    private static class Data
    {
        public static readonly Request FakeRequest = FakeData.FakeRequest(startAt: new DateTime(2019, 01, 01));

        public static void Seed(CovenantContext context)
        {
            FakeRequest.CompanyProfile.Active = true;
            FakeRequest.EmploymentType = EmploymentType.PartTime;
            context.Requests.Add(FakeRequest);
            context.SaveChanges();
        }
    }
}
