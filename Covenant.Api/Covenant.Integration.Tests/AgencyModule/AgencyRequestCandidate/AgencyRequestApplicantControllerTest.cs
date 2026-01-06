using Covenant.Api.AgencyModule.AgencyRequestApplicant.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestCandidate
{
    public class AgencyRequestApplicantControllerTest : IClassFixture<CustomWebApplicationFactory<AgencyRequestApplicantControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public AgencyRequestApplicantControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestApplicantController.RouteName.Replace("{requestId}",
            Startup.FakeRequest.Id.ToString());

        [Fact]
        public async Task PostCandidate()
        {
            var model = new RequestApplicantModel { CandidateId = Startup.FakeCandidatePost.Id, Comments = "Pending documents" };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<RequestApplicantDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicant.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.CandidateId, entity.CandidateId);
            Assert.Equal(model.Comments, entity.Comments);
            Assert.Null(entity.WorkerProfileId);
            Assert.NotNull(entity.CreatedBy);
            Assert.NotEqual(default, entity.CreatedAt);

        }

        [Fact]
        public async Task PostWorker()
        {
            var model = new RequestApplicantModel { WorkerProfileId = Startup.FakeWorkerPost.Id, Comments = "Waiting for work permit" };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<RequestApplicantDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicant.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.WorkerProfileId, entity.WorkerProfileId);
            Assert.Equal(model.Comments, entity.Comments);
            Assert.Null(entity.CandidateId);
            Assert.NotNull(entity.CreatedBy);
            Assert.NotEqual(default, entity.CreatedAt);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<RequestApplicantDetailModel>>();
            Assert.NotEmpty(list.Items);
            RequestApplicant entity = Startup.FakeRequestApplicantList1;
            RequestApplicantDetailModel model = list.Items.Single(c => c.Id == entity.Id);
            Assert.NotNull(model);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.CandidateId, model.CandidateId);
            Assert.Null(entity.WorkerProfileId);
            Assert.Equal(Startup.FakeCandidateList.Email, model.Email);
            Assert.Equal(entity.Comments, model.Comments);
            Assert.Equal(entity.CreatedBy, model.CreatedBy);

            entity = Startup.FakeRequestApplicantList2;
            model = list.Items.Single(c => c.Id == entity.Id);
            Assert.NotNull(model);
            Assert.Equal(entity.Id, model.Id);
            Assert.Equal(entity.WorkerProfileId, model.WorkerProfileId);
            Assert.Null(entity.CandidateId);
            Assert.Equal(Startup.FakeWorkerList.Worker.Email, model.Email);
            Assert.Equal(entity.Comments, model.Comments);
            Assert.Equal(entity.CreatedBy, model.CreatedBy);
        }

        [Fact]
        public async Task Update()
        {
            Guid id = Startup.FakeRequestApplicantUpdate.Id;
            var model = new CommentsModel { Comments = "Worker didn't show up" };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}", model);
            response.EnsureSuccessStatusCode();
            RequestApplicant entity = await _factory.Server.Host.Services.GetRequiredService<CovenantContext>().RequestApplicant.SingleAsync(s => s.Id == id);
            Assert.Equal(model.Comments, entity.Comments);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeRequestApplicantDelete.Id;
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.True(await context.RequestApplicant.AnyAsync(c => c.Id == id));
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            Assert.False(await context.RequestApplicant.AnyAsync(c => c.Id == id));
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
                        o.AddName("r@mail.com");
                    });
                services.AddDbContext<CovenantContext>(b
                    => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                services.AddSingleton<IRequestRepository, RequestRepository>();
                services.AddSingleton<ITimeService, TimeService>();
                services.AddSingleton<IWorkerRequestRepository, WorkerRequestRepository>();
                services.AddSingleton<AgencyIdFilter>();
            }

            public static readonly Request FakeRequest = FakeData.FakeRequest();

            private static readonly Guid AgencyId = Guid.NewGuid();

            public static readonly Candidate FakeCandidatePost = new Candidate(AgencyId, "Jhon", CvnEmail.Create("jh.post@mail.com").Value);
            private static readonly Candidate FakeCandidateDelete = new Candidate(AgencyId, "Delete", CvnEmail.Create("jh.delete@mail.com").Value);
            public static readonly Candidate FakeCandidateList = new Candidate(AgencyId, "List", CvnEmail.Create("jh.list@mail.com").Value);
            private static readonly Candidate FakeCandidateUpdate = new Candidate(AgencyId, "Update", CvnEmail.Create("jh.update@mail.com").Value);
            public static readonly RequestApplicant FakeRequestApplicantDelete = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateDelete.Id, default, default).Value;
            public static readonly RequestApplicant FakeRequestApplicantList1 = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateList.Id, "me@mail.com", "My Comment").Value;
            public static readonly RequestApplicant FakeRequestApplicantUpdate = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateUpdate.Id, "me@mail.com", "To be changed").Value;
            public static readonly WorkerProfile FakeWorkerPost = new WorkerProfile(new User(CvnEmail.Create("w.post@mail.com").Value), AgencyId);
            public static readonly WorkerProfile FakeWorkerList = new WorkerProfile(new User(CvnEmail.Create("w.list@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeRequestApplicantList2 = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerList.Id, "me@mail.com", "My 2 Comment").Value;

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
                context.Request.Add(FakeRequest);
                context.Candidates.AddRange(FakeCandidatePost, FakeCandidateDelete, FakeCandidateList, FakeCandidateUpdate);
                context.WorkerProfile.AddRange(FakeWorkerPost, FakeWorkerList);
                context.RequestApplicant.AddRange(FakeRequestApplicantDelete, FakeRequestApplicantList1, FakeRequestApplicantList2, FakeRequestApplicantUpdate);
                context.SaveChanges();
            }
        }
    }
}