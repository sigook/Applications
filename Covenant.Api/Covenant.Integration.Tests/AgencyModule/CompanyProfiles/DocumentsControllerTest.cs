using Covenant.Api.Controllers.Sigook.Agency.CompanyProfiles;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Company;
using Covenant.Common.Repositories.Company;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Company;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Net.Http.Json;
using System.Text.Json;

namespace Covenant.Integration.Tests.AgencyModule.CompanyProfiles
{
    public class DocumentsControllerTest : IClassFixture<CustomWebApplicationFactory<DocumentsControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public DocumentsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => DocumentsController.RouteName.Replace("{profileId}",
            Startup.FakeCompanyProfile.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new CompanyProfileDocumentModel
            {
                FileName = "contract.pdf",
                Description = "Contract 2021",
                DocumentType = CompanyProfileDocumentType.Contract
            };
            using var content = new MultipartFormDataContent
            {
                { new StringContent(JsonSerializer.Serialize(model)), "data" },
                { new ByteArrayContent("contract content"u8.ToArray()), model.FileName, model.FileName }
            };
            HttpResponseMessage response = await _client.PostAsync(RequestUri(), content);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadFromJsonAsync<Guid>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CompanyProfileDocuments.SingleAsync(c => c.DocumentId == detail);
            Assert.NotNull(entity.CreatedBy);
            Assert.True(entity.CreatedAt <= DateTime.Now);
            Assert.Equal(model.FileName, entity.Document.FileName);
            Assert.Equal(model.Description, entity.Document.Description);
            Assert.Equal(model.DocumentType, entity.DocumentType);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<CompanyProfileDocumentModel>>();
            Assert.NotEmpty(list.Items);
            var entity = Startup.FakeDocument;
            var model = list.Items.Single(c => c.Id == entity.DocumentId);
            AssertDetailAndEntity(model, entity);
        }

        private static void AssertDetailAndEntity(CompanyProfileDocumentModel model, CompanyProfileDocument entity)
        {
            Assert.Equal(model.Id, entity.DocumentId);
            Assert.Equal(model.FileName, entity.Document.FileName);
            Assert.Equal(model.Description, entity.Document.Description);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole();
                        o.AddName("recruiter@mail.com");
                    });
                services.AddTestDatabase();
                services.AddSingleton<ICompanyRepository, CompanyRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency() { User = FakeData.FakeUser() };
            public static readonly CompanyProfile FakeCompanyProfile = new CompanyProfile(new User(CvnEmail.Create("c@mail.com").Value), FakeAgency,
                "", "", new CompanyProfileIndustry("Company Industry"));

            public static readonly CompanyProfileDocument FakeDocument = new CompanyProfileDocument(FakeCompanyProfile.Id, CovenantFile.Create("doc.pdf").Value, "u@mail.com");
            public static readonly CompanyProfileDocument FakeDeleteDocument = new CompanyProfileDocument(FakeCompanyProfile.Id, CovenantFile.Create("delete.pdf").Value, "u@mail.com");


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
                FakeCompanyProfile.Documents.Add(FakeDocument);
                FakeCompanyProfile.Documents.Add(FakeDeleteDocument);
                context.CompanyProfiles.Add(FakeCompanyProfile);
                context.SaveChanges();
            }
        }
    }
}
