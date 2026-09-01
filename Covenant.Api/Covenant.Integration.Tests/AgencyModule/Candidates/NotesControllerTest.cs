using Covenant.Api.Controllers.Sigook.Agency.Candidates;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Candidate;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Candidate;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;
using Xunit.Abstractions;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AgencyModule.Candidates
{
    public class NotesControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<NotesControllerTest.Startup, NotesControllerTest.Data>>
    {
        private readonly SeededWebApplicationFactory<Startup, Data> _factory;
        private readonly ITestOutputHelper _output;
        private readonly HttpClient _client;
        private readonly Data _data;

        public NotesControllerTest(SeededWebApplicationFactory<Startup, Data> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
            _client = factory.CreateClient();
            _data = factory.Data;
        }

        private string RequestUri() => NotesController.RouteName.Replace("{candidateId}",
            _data.Candidate.Id.ToString());

        [Fact]
        public async Task PostNote()
        {
            var model = new NoteModel("This is a note", "Blue");
            var response = await _client.PostAsJsonAsync(RequestUri(), model);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var detail = await response.Content.ReadFromJsonAsync<NoteModel>();
            Assert.NotNull(detail.CreatedBy);
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateNotes.SingleAsync(c => c.NoteId == detail.Id);
            Assert.Equal(model.Note, entity.Note.Note);
            Assert.Equal(model.Color, entity.Note.Color);
            Assert.NotNull(entity.Note.CreatedBy);
            DateAssert.Equal(detail.CreatedAt, entity.Note.CreatedAt);
            Assert.True(entity.Note.CreatedAt <= DateTime.Now);
        }

        [Fact]
        public async Task Get()
        {
            var response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<NoteModel>>();
            Assert.NotEmpty(list.Items);
            var entity = _data.Note;
            var model = list.Items.Single(c => c.Id == entity.NoteId);
            Assert.NotNull(model);
            AssertEntityAndModel(entity, model);
        }

        [Fact]
        public async Task GetById()
        {
            var entity = _data.Note;
            var response = await _client.GetAsync($"{RequestUri()}/{entity.NoteId}");
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromJsonAsync<NoteModel>();
            AssertEntityAndModel(entity, model);
        }

        private static void AssertEntityAndModel(CandidateNote entity, NoteModel model)
        {
            Assert.Equal(entity.NoteId, model.Id);
            Assert.Equal(entity.Note.Note, model.Note);
            Assert.Equal(entity.Note.Color, model.Color);
            Assert.Equal(entity.Note.CreatedBy, model.CreatedBy);
            DateAssert.Equal(entity.Note.CreatedAt, model.CreatedAt);
        }

        [Fact]
        public async Task Put()
        {
            var model = new NoteModel("Call next week", "#FFF4444");
            var id = _data.UpdateNote.NoteId;
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
            var id = _data.DeleteNote.NoteId;
            var response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.CandidateNotes.SingleAsync(c => c.NoteId == id);
            Assert.True(entity.Note.IsDeleted);
            Assert.NotNull(entity.Note.UpdatedBy);
            Assert.True(entity.Note.UpdatedAt <= DateTime.Now);
        }

        public class Data : ITestData
        {
            public static readonly Guid AgencyId = Guid.NewGuid();

            public Covenant.Common.Entities.Agency.Agency Agency { get; }
            public Candidate Candidate { get; }
            public CandidateNote Note { get; }
            public CandidateNote UpdateNote { get; }
            public CandidateNote DeleteNote { get; }

            public Data()
            {
                Agency = FakeData.FakeAgency(AgencyId);
                Candidate = new Candidate(Agency.Id, "Mary") { Agency = Agency };
                Note = new CandidateNote(Candidate.Id, CovenantNote.Create("Call Later", "#CCC111", "cn@mail.com").Value);
                UpdateNote = new CandidateNote(Candidate.Id, CovenantNote.Create("Rate 32", "#CCC111", "cn@mail.com").Value);
                DeleteNote = new CandidateNote(Candidate.Id, CovenantNote.Create("Delete", "#BBB111", "delete@mail.com").Value);
                Candidate.Notes.Add(Note);
                Candidate.Notes.Add(UpdateNote);
                Candidate.Notes.Add(DeleteNote);
            }

            public void Seed(CovenantContext context)
            {
                context.Candidates.Add(Candidate);
                context.SaveChanges();
            }
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddSub(Data.AgencyId);
                        o.AddAgencyPersonnelRole();
                        o.AddName("c@mail.com");
                    });
                services.AddTestDatabase();
                services.AddSingleton<ICandidateRepository, CandidateRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
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
