using Covenant.Api.Controllers.Agency.Recruiting.Clients;
using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Repositories.Companies;
using Covenant.Core.BL.Interfaces.Companies;
using Covenant.Core.BL.Services.Companies;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Companies;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Recruiting.Clients;

public class LogoControllerTest : IClassFixture<CustomWebApplicationFactory<LogoControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;

    public LogoControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri() => LogoController.RouteName.Replace("{profileId}",
        Startup.FakeCompanyProfile.Id.ToString());

    private static MultipartFormDataContent LogoContent(string fileName)
    {
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent([1, 2, 3]), fileName, fileName);
        return content;
    }

    [Fact]
    public async Task Put()
    {
        Guid id = Startup.FakeCompanyProfile.Id;
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        CompanyProfile entity = await context.CompanyProfiles.Include(c => c.Logo).SingleAsync(c => c.Id == id);
        Assert.Equal(CompanyProfile.DefaultImageLogo, entity.Logo.FileName);
        Guid? defaultLogoId = entity.LogoId;

        using var first = LogoContent("myLogo.png");
        HttpResponseMessage response = await _client.PutAsync(RequestUri(), first);
        response.EnsureSuccessStatusCode();

        entity = await context.CompanyProfiles.Include(c => c.Logo).SingleAsync(c => c.Id == id);
        Assert.NotNull(entity.LogoId);
        Assert.NotEqual(defaultLogoId, entity.LogoId);
        Assert.Equal("myLogo.png", entity.Logo.FileName);
        Startup.FilesContainer.Verify(fc => fc.UploadAsync(It.IsAny<Stream>(), "myLogo.png"), Times.Once);
        Startup.DocumentService.Verify(ds => ds.DeleteFile(defaultLogoId.Value), Times.Once);

        Guid? firstLogoId = entity.LogoId;
        using var second = LogoContent("myNewLogo.png");
        response = await _client.PutAsync(RequestUri(), second);
        response.EnsureSuccessStatusCode();

        entity = await context.CompanyProfiles.Include(c => c.Logo).SingleAsync(c => c.Id == id);
        Assert.NotEqual(firstLogoId, entity.LogoId);
        Assert.Equal("myNewLogo.png", entity.Logo.FileName);
        Startup.DocumentService.Verify(ds => ds.DeleteFile(firstLogoId.Value), Times.Once);
    }

    [Fact]
    public async Task Put_WithoutFile_ReturnsBadRequest()
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("{}"), "data");
        HttpResponseMessage response = await _client.PutAsync(RequestUri(), content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_WithUnsupportedFormat_ReturnsBadRequest()
    {
        using var content = LogoContent("myLogo.exe");
        HttpResponseMessage response = await _client.PutAsync(RequestUri(), content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public static readonly Mock<IFilesContainer> FilesContainer = new();
        public static readonly Mock<IDocumentService> DocumentService = new();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddAgencyPersonnelRole(FakeAgency.Id);
                });
            services.AddTestDatabase();
            services.AddSingleton<ICompanyRepository, CompanyRepository>();
            services.AddSingleton<ICompanyService, CompanyService>();
            services.AddSingleton(FilesContainer.Object);
            services.AddSingleton(DocumentService.Object);
            var filesConfiguration = new Mock<IOptions<FilesConfiguration>>();
            filesConfiguration.Setup(m => m.Value).Returns(new FilesConfiguration
            {
                MaximumFileSize = 1000000,
                DocumentFormats = ["png"]
            });
            services.AddSingleton(filesConfiguration.Object);
            services.AddSingleton<AgencyIdFilter>();
        }

        private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency("Test", "Test") { User = FakeData.FakeUser() };
        public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
            "", "", new CompanyProfileIndustry("Company Industry"));

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
            FakeAgency.User = new User(CvnEmail.Create("agency@mail.com").Value);
            context.CompanyProfiles.Add(FakeCompanyProfile);
            context.SaveChanges();
        }
    }
}
