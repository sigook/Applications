using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;
using Covenant.Integration.Tests.Configuration;
using Covenant.Test.Utils.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Common
{
    public class FileControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<FileControllerTest.Startup>>
    {
        private readonly HttpClient _client;
        private const string Url = "api/File";
        public static readonly string PathTestFiles = Path.Combine(Directory.GetCurrentDirectory(), "Common");

        public FileControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.WithWebHostBuilder(b =>
                    b.ConfigureAppConfiguration((context, config) => config.AddJsonFile(Path.Combine(PathTestFiles, "appsettings.test.json"), false)))
                .CreateClient();
        }

        [Fact]
        public async Task PostImageProfile_Valid()
        {
            await PostImage("imageprofile.jpg", response => response.EnsureSuccessStatusCode());
        }

        [Fact]
        public async Task PostImageProfile_TooLarge()
        {
            await PostImage("imageprofilelarge.jpg", response => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode));
        }

        [Fact]
        public async Task PostImageProfile_InvalidFormat()
        {
            await PostImage("imageprofile.invalidformat", response => Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode));
        }

        private async Task PostImage(string image, Action<HttpResponseMessage> assert)
        {
            string imagePath = Path.Combine(PathTestFiles, image);
            using (var content = new MultipartFormDataContent())
            {
                byte[] bytes = File.ReadAllBytes(imagePath);
                content.Add(new StreamContent(new MemoryStream(bytes)), "files", image);
                HttpResponseMessage response = await _client.PostAsync($"{Url}/imageProfile", content);
                assert(response);
            }
            await _client.DeleteAsync($"{Url}/{imagePath}");
        }

        [Fact]
        public async Task PostDocument_ValidDocument()
        {
            string resumePath = Path.Combine(PathTestFiles, "resume_test.pdf");
            string socialInsurancePath = Path.Combine(PathTestFiles, "social_insurance_test.pdf");
            using (var content = new MultipartFormDataContent())
            {
                byte[] bytesResume = File.ReadAllBytes(resumePath);
                byte[] bytesSocialInsurance = File.ReadAllBytes(socialInsurancePath);
                content.Add(new StreamContent(new MemoryStream(bytesResume)), "files", "resume_test.pdf");
                content.Add(new StreamContent(new MemoryStream(bytesSocialInsurance)), "files", "social_insurance_test.pdf");
                var response = await _client.PostAsync($"{Url}/document", content);
                response.EnsureSuccessStatusCode();
            }
            await _client.DeleteAsync($"{Url}/{resumePath}");
            await _client.DeleteAsync($"{Url}/{socialInsurancePath}");
        }


        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                var fileContainer = new Mock<IFilesContainer>();
                services.AddSingleton(fileContainer.Object);
                var mockFilesConfiguration = new Mock<IOptions<FilesConfiguration>>();
                mockFilesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration
                {
                    FilesPath = PathTestFiles,
                    FilesUrl = PathTestFiles,
                    MaximumFileSize = 1100000
                });
                services.AddSingleton(mockFilesConfiguration.Object);
            }

            public void Configure(IApplicationBuilder app)
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
            }
        }
    }
}