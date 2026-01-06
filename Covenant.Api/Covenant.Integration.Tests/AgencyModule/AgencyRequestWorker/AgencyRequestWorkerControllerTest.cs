using Covenant.Api.AgencyModule.AgencyRequestWorker.Controllers;
using Covenant.Api.Authorization;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Context;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Covenant.Integration.Tests.AgencyModule.AgencyRequestWorker
{
    public partial class AgencyRequestWorkerControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<AgencyRequestWorkerControllerTest.Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public AgencyRequestWorkerControllerTest(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private static string RequestUri() => AgencyRequestWorkerController.RouteName.Replace("{requestId}", Data.FakeRequest.Id.ToString());

        [Theory]
        [InlineData("Filter")]
        [InlineData("NoFilter")]
        public async Task Get(string filter)
        {
            string requestUri = RequestUri();
            var workerRequest = Data.FakeWorkerRequestList;
            if (filter.Equals("Filter")) requestUri = $"{requestUri}?status={workerRequest.WorkerRequestStatus}&filter=name";
            HttpResponseMessage response = await _client.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
            Assert.NotEmpty(list.Items);
            AgencyWorkerRequestModel model = list.Items.Single(w => w.Id == workerRequest.Id);
            Assert.Equal(model.Id, workerRequest.Id);
            Assert.Equal(model.NumberId, Data.FakeWorkerForList.NumberId);
            Assert.Equal(model.WorkerId, workerRequest.WorkerId);
            Assert.Equal(model.WorkerProfileId, Data.FakeWorkerForList.Id);
            Assert.Equal(model.Name, Data.FakeWorkerForList.FullName);
            Assert.Equal(model.Status, workerRequest.WorkerRequestStatus.ToString());
            Assert.Equal(model.ProfileImage, Data.FakeWorkerForList.ProfileImage?.FileName);
            Assert.Equal(model.ApprovedToWork, Data.FakeWorkerForList.ApprovedToWork);
            Assert.StartsWith(model.SocialInsurance, Data.FakeWorkerForList.MaskedSocialInsurance);
            Assert.Equal(model.DueDate, Data.FakeWorkerForList.DueDate);
            Assert.Equal(model.SocialInsuranceExpire, Data.FakeWorkerForList.SocialInsuranceExpire);
            Assert.Equal(model.MobileNumber, Data.FakeWorkerForList.MobileNumber);
            Assert.Equal(model.StartWorking, workerRequest.StartWorking);
            Assert.Equal(model.CreatedBy, workerRequest.CreatedBy);
            Assert.Equal(model.CreatedAt, workerRequest.CreatedAt);
            Assert.Equal(model.RejectComments, workerRequest.RejectComments);
            Assert.Equal(model.RejectedAt, workerRequest.RejectedAt);
        }

        [Fact, TestOrder(2)]
        public async Task GetAll()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/All");
            response.EnsureSuccessStatusCode();
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
            List<AgencyWorkerRequestModel> items = list.Items;
            Assert.NotEmpty(items);
            Assert.Equal(Data.FakeWorkers.Count(), items.Count);
            Assert.All(items, i => Assert.Contains(Data.FakeWorkers, w =>
                w.NumberId == i.NumberId &&
                w.Id == i.WorkerProfileId &&
                w.WorkerId == i.WorkerId &&
                w.FullName == i.Name &&
                w.ApprovedToWork == i.ApprovedToWork &&
                w.MaskedSocialInsurance == i.SocialInsurance &&
                w.SocialInsuranceExpire == i.SocialInsuranceExpire &&
                w.DueDate == i.DueDate &&
                w.Location.FormattedAddress == i.Address &&
                w.MobileNumber == i.MobileNumber));
        }

        [Fact, TestOrder(3)]
        public async Task GetById()
        {
            HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Data.FakeWorkerRequestList.Id}");
            response.EnsureSuccessStatusCode();
            AgencyWorkerRequestModel model = await response.Content.ReadAsJsonAsync<AgencyWorkerRequestModel>();
            Assert.Equal(Data.FakeWorkerRequestList.Id, model.Id);
        }

        [Fact]
        public async Task Book()
        {
            WorkerProfile worker = Data.FakeWorkerToBook;
            HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/{worker.WorkerId}/Book", new { });
            response.EnsureSuccessStatusCode();
            var detail = await response.Content.ReadAsJsonAsync<AgencyWorkerRequestModel>();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequest.SingleAsync(s => s.Id == detail.Id);
            Assert.Equal(worker.WorkerId, entity.WorkerId);
            Assert.Equal(WorkerRequestStatus.Booked, entity.WorkerRequestStatus);
        }

        [Fact]
        public async Task Reject()
        {
            var worker = Data.FakeWorkerRequestReject;
            var model = new CommentsModel { Comments = "Worker was hired by the company" };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{worker.WorkerId}/Reject", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequest.SingleAsync(s => s.Id == worker.Id);
            Assert.Equal(WorkerRequestStatus.Rejected, entity.WorkerRequestStatus);
            Assert.Equal(model.Comments, entity.RejectComments);
            Assert.NotNull(entity.RejectedAt);

            response = await _client.GetAsync(RequestUri());
            var list = await response.Content.ReadAsJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
            AgencyWorkerRequestModel detail = list.Items.Single(w => w.Id == worker.Id);
            Assert.Equal(model.Comments, detail.RejectComments);
            Assert.Equal(entity.RejectedAt, detail.RejectedAt);
        }

        [Fact]
        public async Task Put()
        {
            var worker = Data.FakeWorkerRequestList;
            var model = new AgencyBookWorkerModel { StartWorking = new DateTime(2019, 01, 01) };
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{worker.Id}", model);
            response.EnsureSuccessStatusCode();
            var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
            var entity = await context.WorkerRequest.SingleAsync(s => s.Id == worker.Id);
            Assert.Equal(model.StartWorking, entity.StartWorking);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddDefaultTestConfiguration();
                services.AddTestAuthenticationBuilder()
                    .AddTestAuth(o =>
                    {
                        o.AddAgencyPersonnelRole(Data.AgencyId);
                        o.AddName("recruiter@mail.com");
                    });
                services.AddDbContext<CovenantContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()), ServiceLifetime.Singleton);
                var timeService = new Mock<ITimeService>();
                timeService.Setup(c => c.GetCurrentDateTime()).Returns(Data.FakeNow);
                services.AddSingleton(timeService.Object);
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
                Data.Seed(context);
            }
        }
    }
}