using Covenant.Api.AgencyModule.AgencyCandidateDocument.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Candidate;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCandidate
{
    public class AgencyCandidateDocumentControllerTest :
        IClassFixture<CustomWebApplicationFactory<AgencyCandidateDocumentControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyCandidateDocumentControllerTest(CustomWebApplicationFactory<AgencyCandidateDocumentControllerTest.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCandidateDocumentController.RouteName.Replace("{candidateId}",
            Startup.FakeCandidate.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new CovenantFileModel("resume.pdf", "Resume");
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<Guid>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateDocuments.SingleAsync(c => c.DocumentId == detail);
            Assert.Equal(model.FileName, entity.Document.FileName);
            Assert.Equal(model.Description, entity.Document.Description);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<CovenantFileModel>>();
            Assert.NotEmpty(list.Items);
            var entity = Startup.FakeDocument;
            var model = list.Items.Single(c => c.Id == entity.DocumentId);
            AssertDetailAndEntity(model, entity);
        }

        private static void AssertDetailAndEntity(CovenantFileModel model, CandidateDocument entity)
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
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICandidateRepository, CandidateRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly Candidate FakeCandidate = new Candidate(FakeAgency.Id, "B Martin") { Agency = FakeAgency };

            public static readonly CandidateDocument FakeDocument = new CandidateDocument(FakeCandidate.Id, CovenantFile.Create("doc.pdf").Value);
            public static readonly CandidateDocument FakeDeleteDocument = new CandidateDocument(FakeCandidate.Id, CovenantFile.Create("delete.pdf").Value);

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
                FakeCandidate.Documents.Add(FakeDocument);
                FakeCandidate.Documents.Add(FakeDeleteDocument);
                context.Candidates.Add(FakeCandidate);
                context.SaveChanges();
            }
        }
    }
}