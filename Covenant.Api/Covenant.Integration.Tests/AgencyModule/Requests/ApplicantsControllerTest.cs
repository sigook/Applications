using Covenant.Api.Controllers.Sigook.Agency.Requests;
using Covenant.Api.Authorization;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Candidate;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Request;
using Covenant.Common.Repositories.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Request;
using Covenant.Infrastructure.Services;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Net;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AgencyModule.Requests
{
    public class ApplicantsControllerTest : IClassFixture<CustomWebApplicationFactory<ApplicantsControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public ApplicantsControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => ApplicantsController.RouteName.Replace("{requestId}",
            Startup.FakeRequest.Id.ToString());

        [Fact]
        public async Task PostCandidate()
        {
            var model = new RequestApplicantModel { CandidateId = Startup.FakeCandidatePost.Id, Comments = "Pending documents" };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadFromJsonAsync<RequestApplicantDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicants.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.CandidateId, entity.CandidateId);
            Assert.Equal(model.Comments, entity.Comments);
            Assert.Null(entity.WorkerProfileId);
            Assert.NotNull(entity.CreatedBy);
            Assert.NotEqual(default, entity.CreatedAt);
            Assert.Equal(RequestApplicantStatus.InProgress, entity.Status);
        }

        [Fact]
        public async Task PostWorker()
        {
            var model = new RequestApplicantModel { WorkerProfileId = Startup.FakeWorkerPost.Id, Comments = "Waiting for work permit" };
            HttpResponseMessage response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, RequestUri(), model);
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadFromJsonAsync<RequestApplicantDetailModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicants.SingleAsync(c => c.Id == detail.Id);
            Assert.Equal(model.WorkerProfileId, entity.WorkerProfileId);
            Assert.Equal(model.Comments, entity.Comments);
            Assert.Null(entity.CandidateId);
            Assert.NotNull(entity.CreatedBy);
            Assert.NotEqual(default, entity.CreatedAt);
            Assert.Equal(RequestApplicantStatus.InProgress, entity.Status);
        }

        [Fact]
        public async Task ChangeStatusStartsPendingApplicant()
        {
            Guid id = Startup.FakeApplicantStart.Id;
            var model = new ChangeRequestApplicantStatusModel { Status = RequestApplicantStatus.InProgress };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Status", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicants.SingleAsync(c => c.Id == id);
            Assert.Equal(RequestApplicantStatus.InProgress, entity.Status);
        }

        [Fact]
        public async Task ChangeStatusConfirmsWorkerWithMandatoryCompleted()
        {
            Guid id = Startup.FakeApplicantConfirm.Id;
            var model = new ChangeRequestApplicantStatusModel { Status = RequestApplicantStatus.Confirmed };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Status", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicant entity = await context.RequestApplicants.SingleAsync(c => c.Id == id);
            Assert.Equal(RequestApplicantStatus.Confirmed, entity.Status);
        }

        [Fact]
        public async Task ChangeStatusToConfirmedReturnsBadRequestForCandidate()
        {
            Guid id = Startup.FakeApplicantCandidateConfirm.Id;
            var model = new ChangeRequestApplicantStatusModel { Status = RequestApplicantStatus.Confirmed };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{id}/Status", model);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetComplianceItemsReturnsCompletionState()
        {
            Guid id = Startup.FakeApplicantComplianceGet.Id;
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{id}/ComplianceItems");
            response.EnsureSuccessStatusCode();
            var items = await response.Content.ReadFromJsonAsync<List<ApplicantComplianceItemModel>>();
            Assert.Equal(2, items.Count);
            ApplicantComplianceItemModel w4 = items.Single(i => i.Id == Startup.FakeComplianceItemW4.Id);
            Assert.True(w4.IsCompleted);
            Assert.True(w4.IsMandatory);
            Assert.True(w4.CanUpload);
            Assert.Equal("me@mail.com", w4.CompletedBy);
            ApplicantComplianceItemModel wp = items.Single(i => i.Id == Startup.FakeComplianceItemWP.Id);
            Assert.False(wp.IsCompleted);
            Assert.False(wp.IsMandatory);
            Assert.False(wp.CanUpload);
        }

        [Fact]
        public async Task CompleteComplianceItemCreatesCompletion()
        {
            Guid id = Startup.FakeApplicantCompliancePost.Id;
            Guid itemId = Startup.FakeComplianceItemWP.Id;
            using var content = new MultipartFormDataContent { { new StringContent("{}"), "data" } };
            HttpResponseMessage response = await _client.PostAsync($"{RequestUri()}/{id}/ComplianceItems/{itemId}", content);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            RequestApplicantComplianceItem completion = await context.RequestApplicantComplianceItems
                .SingleAsync(c => c.RequestApplicantId == id && c.RequestComplianceItemId == itemId);
            Assert.NotNull(completion.CompletedBy);
            Assert.NotEqual(default, completion.CompletedAt);
        }

        [Fact]
        public async Task UncompleteComplianceItemDeletesCompletion()
        {
            Guid id = Startup.FakeApplicantUncomplete.Id;
            Guid itemId = Startup.FakeComplianceItemW4.Id;
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.True(await context.RequestApplicantComplianceItems.AnyAsync(c => c.RequestApplicantId == id && c.RequestComplianceItemId == itemId));
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}/ComplianceItems/{itemId}");
            response.EnsureSuccessStatusCode();
            Assert.False(await context.RequestApplicantComplianceItems.AnyAsync(c => c.RequestApplicantId == id && c.RequestComplianceItemId == itemId));
        }

        private static string SinRequestUri() => ApplicantsController.RouteName.Replace("{requestId}",
            Startup.FakeSinRequest.Id.ToString());

        private static MultipartFormDataContent SinIdentificationContent(string identificationNumber) => new()
        {
            {
                new StringContent($"{{\"fileName\":\"sin.pdf\",\"identificationNumber\":\"{identificationNumber}\",\"identificationTypeId\":\"{Startup.FakeSinIdentificationType.Id}\"}}"),
                "data"
            }
        };

        [Fact]
        public async Task CompleteSinTypedIdentificationFillsSocialInsuranceAndCompletesSsnItem()
        {
            Guid id = Startup.FakeApplicantSinFill.Id;
            Guid itemId = Startup.FakeSinIdentificationItem.Id;
            using var content = SinIdentificationContent("222-333-444");
            HttpResponseMessage response = await _client.PostAsync($"{SinRequestUri()}/{id}/ComplianceItems/{itemId}", content);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            WorkerProfile profile = await context.WorkerProfiles.SingleAsync(w => w.Id == Startup.FakeWorkerSinFill.Id);
            Assert.Equal("222-333-444", profile.SocialInsurance);
            Assert.NotNull(profile.SocialInsuranceFileId);
            Assert.Equal("222-333-444", profile.IdentificationNumber1);
            Assert.True(await context.RequestApplicantComplianceItems.AnyAsync(c => c.RequestApplicantId == id && c.RequestComplianceItemId == itemId));
            RequestApplicantComplianceItem ssnCompletion = await context.RequestApplicantComplianceItems
                .SingleAsync(c => c.RequestApplicantId == id && c.RequestComplianceItemId == Startup.FakeSinSsnItem.Id);
            Assert.NotNull(ssnCompletion.CompletedBy);
        }

        [Fact]
        public async Task CompleteSinTypedIdentificationReturnsBadRequestWhenSinIsTaken()
        {
            Guid id = Startup.FakeApplicantSinDuplicate.Id;
            Guid itemId = Startup.FakeSinIdentificationItem.Id;
            using var content = SinIdentificationContent("111-111-111");
            HttpResponseMessage response = await _client.PostAsync($"{SinRequestUri()}/{id}/ComplianceItems/{itemId}", content);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CompleteSinTypedIdentificationReplacesDifferentSinAndCreatesNote()
        {
            Guid id = Startup.FakeApplicantSinConflict.Id;
            Guid itemId = Startup.FakeSinIdentificationItem.Id;
            using var content = SinIdentificationContent("555-666-777");
            HttpResponseMessage response = await _client.PostAsync($"{SinRequestUri()}/{id}/ComplianceItems/{itemId}", content);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            WorkerProfile profile = await context.WorkerProfiles.SingleAsync(w => w.Id == Startup.FakeWorkerSinConflict.Id);
            Assert.Equal("555-666-777", profile.SocialInsurance);
            WorkerProfileNote note = await context.WorkerProfileNotes.SingleAsync(n => n.WorkerProfileId == Startup.FakeWorkerSinConflict.Id);
            Assert.Contains("******-999", note.Note);
            Assert.Contains("******-777", note.Note);
            Assert.NotNull(note.CreatedBy);
        }

        [Fact]
        public async Task GetFiltersByStatuses()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?statuses={(int)RequestApplicantStatus.InProgress}");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<RequestApplicantDetailModel>>();
            Assert.NotEmpty(list.Items);
            Assert.All(list.Items, i => Assert.Equal(RequestApplicantStatus.InProgress, i.Status));
            Assert.Contains(list.Items, i => i.Id == Startup.FakeApplicantCompliancePost.Id);
        }

        [Fact]
        public async Task Get()
        {
            HttpResponseMessage response = await _client.GetAsync(RequestUri());
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadFromJsonAsync<PaginatedList<RequestApplicantDetailModel>>();
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
            RequestApplicant entity = await _factory.Server.Host.Services.GetRequiredService<CovenantContext>().RequestApplicants.SingleAsync(s => s.Id == id);
            Assert.Equal(model.Comments, entity.Comments);
        }

        [Fact]
        public async Task Delete()
        {
            Guid id = Startup.FakeRequestApplicantDelete.Id;
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            Assert.True(await context.RequestApplicants.AnyAsync(c => c.Id == id));
            HttpResponseMessage response = await _client.DeleteAsync($"{RequestUri()}/{id}");
            response.EnsureSuccessStatusCode();
            Assert.False(await context.RequestApplicants.AnyAsync(c => c.Id == id));
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
            public static readonly RequestApplicant FakeRequestApplicantDelete = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateDelete.Id, default, default, RequestApplicantStatus.Pending).Value;
            public static readonly RequestApplicant FakeRequestApplicantList1 = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateList.Id, "me@mail.com", "My Comment", RequestApplicantStatus.Pending).Value;
            public static readonly RequestApplicant FakeRequestApplicantUpdate = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateUpdate.Id, "me@mail.com", "To be changed", RequestApplicantStatus.Pending).Value;
            public static readonly WorkerProfile FakeWorkerPost = new WorkerProfile(new User(CvnEmail.Create("w.post@mail.com").Value), AgencyId);
            public static readonly WorkerProfile FakeWorkerList = new WorkerProfile(new User(CvnEmail.Create("w.list@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeRequestApplicantList2 = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerList.Id, "me@mail.com", "My 2 Comment", RequestApplicantStatus.Pending).Value;

            public static readonly RequestComplianceItem FakeComplianceItemW4 = RequestComplianceItem.Create(FakeRequest.Id, "W4", true, ComplianceDocumentTarget.OtherDocument).Value;
            public static readonly RequestComplianceItem FakeComplianceItemWP = RequestComplianceItem.Create(FakeRequest.Id, "WP", false, ComplianceDocumentTarget.None).Value;

            public static readonly WorkerProfile FakeWorkerStart = new WorkerProfile(new User(CvnEmail.Create("w.start@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeApplicantStart = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerStart.Id, "me@mail.com", null, RequestApplicantStatus.Pending).Value;

            public static readonly WorkerProfile FakeWorkerConfirm = new WorkerProfile(new User(CvnEmail.Create("w.confirm@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeApplicantConfirm = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerConfirm.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
            public static readonly RequestApplicantComplianceItem FakeConfirmCompletion = RequestApplicantComplianceItem.Create(FakeApplicantConfirm.Id, FakeComplianceItemW4.Id, "me@mail.com").Value;

            public static readonly WorkerProfile FakeWorkerComplianceGet = new WorkerProfile(new User(CvnEmail.Create("w.cget@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeApplicantComplianceGet = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerComplianceGet.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
            public static readonly RequestApplicantComplianceItem FakeComplianceGetCompletion = RequestApplicantComplianceItem.Create(FakeApplicantComplianceGet.Id, FakeComplianceItemW4.Id, "me@mail.com").Value;

            public static readonly WorkerProfile FakeWorkerCompliancePost = new WorkerProfile(new User(CvnEmail.Create("w.cpost@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeApplicantCompliancePost = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerCompliancePost.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;

            public static readonly WorkerProfile FakeWorkerUncomplete = new WorkerProfile(new User(CvnEmail.Create("w.cdel@mail.com").Value), AgencyId);
            public static readonly RequestApplicant FakeApplicantUncomplete = RequestApplicant.CreateWithWorker(FakeRequest.Id, FakeWorkerUncomplete.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
            public static readonly RequestApplicantComplianceItem FakeUncompleteCompletion = RequestApplicantComplianceItem.Create(FakeApplicantUncomplete.Id, FakeComplianceItemW4.Id, "me@mail.com").Value;

            public static readonly Candidate FakeCandidateConfirm = new Candidate(AgencyId, "Confirm", CvnEmail.Create("jh.confirm@mail.com").Value);
            public static readonly RequestApplicant FakeApplicantCandidateConfirm = RequestApplicant.CreateWithCandidate(FakeRequest.Id, FakeCandidateConfirm.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
            public static readonly RequestApplicantComplianceItem FakeCandidateConfirmCompletion = RequestApplicantComplianceItem.Create(FakeApplicantCandidateConfirm.Id, FakeComplianceItemW4.Id, "me@mail.com").Value;

            public static readonly Request FakeSinRequest = FakeData.FakeRequest();
            public static readonly IdentificationType FakeSinIdentificationType = new IdentificationType("SIN/SSN") { Code = IdentificationTypeCode.SinSsn };
            public static readonly RequestComplianceItem FakeSinIdentificationItem = RequestComplianceItem.Create(FakeSinRequest.Id, "ID", true, ComplianceDocumentTarget.Identification1).Value;
            public static readonly RequestComplianceItem FakeSinSsnItem = RequestComplianceItem.Create(FakeSinRequest.Id, "SSN", true, ComplianceDocumentTarget.SocialInsurance).Value;
            public static readonly WorkerProfile FakeWorkerSinFill = new WorkerProfile(new User(CvnEmail.Create("w.sinfill@mail.com").Value), AgencyId)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country() } } }
            };
            public static readonly RequestApplicant FakeApplicantSinFill = RequestApplicant.CreateWithWorker(FakeSinRequest.Id, FakeWorkerSinFill.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
            public static readonly WorkerProfile FakeWorkerSinOwner = new WorkerProfile(new User(CvnEmail.Create("w.sinowner@mail.com").Value), AgencyId)
            {
                SocialInsurance = "111-111-111",
                Location = new Location { City = new City { Province = new Province { Country = new Country() } } }
            };
            public static readonly WorkerProfile FakeWorkerSinDuplicate = new WorkerProfile(new User(CvnEmail.Create("w.sindup@mail.com").Value), AgencyId)
            {
                Location = new Location { City = new City { Province = new Province { Country = new Country() } } }
            };
            public static readonly RequestApplicant FakeApplicantSinDuplicate = RequestApplicant.CreateWithWorker(FakeSinRequest.Id, FakeWorkerSinDuplicate.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;
            public static readonly WorkerProfile FakeWorkerSinConflict = new WorkerProfile(new User(CvnEmail.Create("w.sinconflict@mail.com").Value), AgencyId)
            {
                SocialInsurance = "999-999-999",
                Location = new Location { City = new City { Province = new Province { Country = new Country() } } }
            };
            public static readonly RequestApplicant FakeApplicantSinConflict = RequestApplicant.CreateWithWorker(FakeSinRequest.Id, FakeWorkerSinConflict.Id, "me@mail.com", null, RequestApplicantStatus.InProgress).Value;

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
                context.Agencies.Add(FakeData.FakeAgency(FakeRequest.CompanyProfile.AgencyId));
                context.Requests.AddRange(FakeRequest, FakeSinRequest);
                context.IdentificationTypes.Add(FakeSinIdentificationType);
                context.Candidates.AddRange(FakeCandidatePost, FakeCandidateDelete, FakeCandidateList, FakeCandidateUpdate, FakeCandidateConfirm);
                context.WorkerProfiles.AddRange(FakeWorkerPost, FakeWorkerList, FakeWorkerStart, FakeWorkerConfirm, FakeWorkerComplianceGet, FakeWorkerCompliancePost, FakeWorkerUncomplete,
                    FakeWorkerSinFill, FakeWorkerSinOwner, FakeWorkerSinDuplicate, FakeWorkerSinConflict);
                context.RequestApplicants.AddRange(FakeRequestApplicantDelete, FakeRequestApplicantList1, FakeRequestApplicantList2, FakeRequestApplicantUpdate,
                    FakeApplicantStart, FakeApplicantConfirm, FakeApplicantComplianceGet, FakeApplicantCompliancePost, FakeApplicantUncomplete, FakeApplicantCandidateConfirm,
                    FakeApplicantSinFill, FakeApplicantSinDuplicate, FakeApplicantSinConflict);
                context.RequestComplianceItems.AddRange(FakeComplianceItemW4, FakeComplianceItemWP, FakeSinIdentificationItem, FakeSinSsnItem);
                context.RequestApplicantComplianceItems.AddRange(FakeConfirmCompletion, FakeComplianceGetCompletion, FakeUncompleteCompletion, FakeCandidateConfirmCompletion);
                context.SaveChanges();
            }
        }
    }
}
