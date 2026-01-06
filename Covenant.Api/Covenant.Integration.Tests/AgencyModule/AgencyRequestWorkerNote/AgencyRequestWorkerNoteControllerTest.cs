using Covenant.Api.AgencyModule.AgencyRequestWorkerNote.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Request;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestWorkerNote
{
    public class AgencyRequestWorkerNoteControllerTest :
        IClassFixture<CustomWebApplicationFactory<AgencyRequestWorkerNoteControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyRequestWorkerNoteControllerTest(CustomWebApplicationFactory<AgencyRequestWorkerNoteControllerTest.Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestWorkerNoteController.RouteName
            .Replace("{requestId}", Startup.FakeWorkerRequest.RequestId.ToString())
            .Replace("{workerRequestId}", Startup.FakeWorkerRequest.Id.ToString());

        [Fact]
        public async Task Post()
        {
            var model = new NoteModel("Add new note", "#FFF000");
            var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<NoteModel>();
            Assert.NotNull(detail.CreatedBy);
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequestNote.SingleAsync(c => c.NoteId == detail.Id);
            Assert.Equal(model.Note, entity.Note.Note);
            Assert.Equal(model.Color, entity.Note.Color);
            Assert.NotNull(entity.Note.CreatedBy);
            Assert.Equal(detail.CreatedAt, entity.Note.CreatedAt);
            Assert.True(entity.Note.CreatedAt <= DateTime.Now);
        }

        [Fact]
        public async Task Put()
        {
            var model = new NoteModel("Update note", "#FFF111");
            var id = Startup.FakeUpdateNote.NoteId;
            var response = await HttpClientJsonExtensions.PutAsJsonAsync(_client, $"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequestNote.SingleAsync(c => c.NoteId == id);
            Assert.Equal(model.Note, entity.Note.Note);
            Assert.Equal(model.Color, entity.Note.Color);
            Assert.NotNull(entity.Note.UpdatedBy);
            Assert.True(entity.Note.UpdatedAt <= DateTime.Now);
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

        private static void AssertEntityAndModel(WorkerRequestNote entity, NoteModel model)
        {
            Assert.Equal(entity.NoteId, model.Id);
            Assert.Equal(entity.Note.Note, model.Note);
            Assert.Equal(entity.Note.Color, model.Color);
            Assert.Equal(entity.Note.CreatedBy, model.CreatedBy);
            Assert.Equal(entity.Note.CreatedAt, model.CreatedAt);
        }

        [Fact]
        public async Task Delete()
        {
            var id = Startup.FakeDeleteNote.NoteId;
            var response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequestNote.SingleAsync(c => c.NoteId == id);
            Assert.True(entity.Note.IsDeleted);
            Assert.NotNull(entity.Note.UpdatedBy);
            Assert.True(entity.Note.UpdatedAt <= DateTime.Now);
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
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Covenant.Common.Entities.Request.WorkerRequest FakeWorkerRequest =
                Covenant.Common.Entities.Request.WorkerRequest.AgencyBook(Guid.NewGuid(), Guid.NewGuid());

            public static readonly WorkerRequestNote FakeNote = new WorkerRequestNote(FakeWorkerRequest.Id,
                CovenantNote.Create("Fake request note", "#CCC111", "a@mail.com").Value);

            public static readonly WorkerRequestNote FakeUpdateNote = new WorkerRequestNote(FakeWorkerRequest.Id,
                CovenantNote.Create("My request note", "#AAA111", "r@mail.com").Value);

            public static readonly WorkerRequestNote FakeDeleteNote = new WorkerRequestNote(FakeWorkerRequest.Id,
                CovenantNote.Create("Delete request note", "#DDD3333", "d@mail.com").Value);

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
                context.WorkerRequest.Add(FakeWorkerRequest);
                context.WorkerRequestNote.AddRange(FakeNote, FakeUpdateNote, FakeDeleteNote);
                context.SaveChanges();
            }
        }
    }
}