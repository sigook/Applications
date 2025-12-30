using Covenant.Api.AgencyModule.AgencyCandidateNote.Controllers;
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
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace Covenant.Integration.Tests.AgencyModule.AgencyCandidate
{
    public class TestAgencyCandidateNoteController : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<TestAgencyCandidateNoteController.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;
        private readonly HttpClient _client;

        public TestAgencyCandidateNoteController(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyCandidateNoteController.RouteName.Replace("{candidateId}",
            Startup.FakeCandidate.Id.ToString());

        [Fact]
        public async Task PostNote()
        {
            var model = new NoteModel("This is a note", "Blue");
            var response = await _client.PostAsJsonAsync(RequestUri(), model);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var detail = await response.Content.ReadAsJsonAsync<NoteModel>();
            Assert.NotNull(detail.CreatedBy);
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateNotes.SingleAsync(c => c.NoteId == detail.Id);
            Assert.Equal(model.Note, entity.Note.Note);
            Assert.Equal(model.Color, entity.Note.Color);
            Assert.NotNull(entity.Note.CreatedBy);
            Assert.Equal(detail.CreatedAt, entity.Note.CreatedAt);
            Assert.True(entity.Note.CreatedAt <= DateTime.Now);
        }

        [Fact]
        public async Task Get()
        {
            var response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<NoteModel>>();
            Assert.NotEmpty(list.Items);
            var entity = Startup.FakeNote;
            var model = list.Items.Single(c => c.Id == entity.NoteId);
            Assert.NotNull(model);
            AssertEntityAndModel(entity, model);
        }

        [Fact]
        public async Task GetById()
        {
            var entity = Startup.FakeNote;
            var response = await _client.GetAsync($"{RequestUri()}/{entity.NoteId}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadAsJsonAsync<NoteModel>();
            AssertEntityAndModel(entity, model);
        }

        private static void AssertEntityAndModel(CandidateNote entity, NoteModel model)
        {
            Assert.Equal(entity.NoteId, model.Id);
            Assert.Equal(entity.Note.Note, model.Note);
            Assert.Equal(entity.Note.Color, model.Color);
            Assert.Equal(entity.Note.CreatedBy, model.CreatedBy);
            Assert.Equal(entity.Note.CreatedAt, model.CreatedAt);
        }

        [Fact]
        public async Task Put()
        {
            var model = new NoteModel("Call next week", "#FFF4444");
            var id = Startup.FakeUpdateNote.NoteId;
            var response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateNotes.SingleAsync(c => c.NoteId == id);
            Assert.Equal(model.Note, entity.Note.Note);
            Assert.Equal(model.Color, entity.Note.Color);
            Assert.NotNull(entity.Note.UpdatedBy);
            Assert.True(entity.Note.UpdatedAt <= DateTime.Now);
        }

        [Fact]
        public async Task Delete()
        {
            var id = Startup.FakeDeleteNote.NoteId;
            var response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateNotes.SingleAsync(c => c.NoteId == id);
            Assert.True(entity.Note.IsDeleted);
            Assert.NotNull(entity.Note.UpdatedBy);
            Assert.True(entity.Note.UpdatedAt <= DateTime.Now);
        }

        public class Startup
        {
            private static readonly Covenant.Common.Entities.Agency.Agency FakeAgency = new Covenant.Common.Entities.Agency.Agency();
            public static readonly Candidate FakeCandidate = new Candidate(FakeAgency.Id, "Mary") { Agency = FakeAgency };
            public static readonly CandidateNote FakeNote = new CandidateNote(FakeCandidate.Id, CovenantNote.Create("Call Later", "#CCC111", "cn@mail.com").Value);
            public static readonly CandidateNote FakeUpdateNote = new CandidateNote(FakeCandidate.Id, CovenantNote.Create("Rate 32", "#CCC111", "cn@mail.com").Value);
            public static readonly CandidateNote FakeDeleteNote = new CandidateNote(FakeCandidate.Id, CovenantNote.Create("Delete", "#BBB111", "delete@mail.com").Value);

            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(FakeAgency.Id);
                        o.AddAgencyPersonnelRole();
                        o.AddName("c@mail.com");
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<ICandidateRepository, CandidateRepository>();
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
                FakeCandidate.Notes.Add(FakeNote);
                FakeCandidate.Notes.Add(FakeUpdateNote);
                FakeCandidate.Notes.Add(FakeDeleteNote);
                context.Candidates.Add(FakeCandidate);
                context.SaveChanges();
            }
        }
    }
}