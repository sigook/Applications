using Covenant.Api.Controllers.Sigook.Agency.Requests;
using Covenant.Api.Authorization;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Agency;
using Covenant.Common.Models.Request;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Covenant.Test.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Net.Http.Json;

namespace Covenant.Integration.Tests.AgencyModule.Requests;

public partial class WorkersControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<WorkersControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;
    public WorkersControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri() => WorkersController.RouteName.Replace("{requestId}", Data.FakeRequest.Id.ToString());

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
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
        Assert.NotEmpty(list.Items);
        AgencyWorkerRequestModel model = list.Items.Single(w => w.Id == workerRequest.Id);
        Assert.Equal(model.Id, workerRequest.Id);
        Assert.Equal(model.NumberId, Data.FakeWorkerForList.NumberId);
        Assert.Equal(model.WorkerId, Data.FakeWorkerForList.WorkerId);
        Assert.Equal(model.WorkerProfileId, Data.FakeWorkerForList.Id);
        Assert.Equal(model.Name, Data.FakeWorkerForList.FullName);
        Assert.Equal(model.WorkerRequestStatus, workerRequest.WorkerRequestStatus);
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

    [Fact]
    public async Task GetById()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Data.FakeWorkerRequestList.Id}");
        response.EnsureSuccessStatusCode();
        AgencyWorkerRequestModel model = await response.Content.ReadFromJsonAsync<AgencyWorkerRequestModel>();
        Assert.Equal(Data.FakeWorkerRequestList.Id, model.Id);
    }

    [Fact]
    public async Task Book()
    {
        WorkerProfile worker = Data.FakeWorkerToBook;
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{RequestUri()}/{worker.Id}/Book", new { });
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<AgencyWorkerRequestModel>();
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        var entity = await context.WorkerRequests.SingleAsync(s => s.Id == detail.Id);
        Assert.Equal(worker.Id, entity.WorkerProfileId);
        Assert.Equal(WorkerRequestStatus.Booked, entity.WorkerRequestStatus);
    }

    [Fact]
    public async Task Reject()
    {
        var worker = Data.FakeWorkerRequestReject;
        var model = new CommentsModel { Comments = "Worker was hired by the company" };
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{RequestUri()}/{worker.WorkerProfileId}/Reject", model);
        response.EnsureSuccessStatusCode();
        var context = _factory.Server.Host.Services.GetRequiredService<CovenantContext>();
        var entity = await context.WorkerRequests.SingleAsync(s => s.Id == worker.Id);
        Assert.Equal(WorkerRequestStatus.Rejected, entity.WorkerRequestStatus);
        Assert.Equal(model.Comments, entity.RejectComments);
        Assert.NotNull(entity.RejectedAt);

        response = await _client.GetAsync(RequestUri());
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<AgencyWorkerRequestModel>>();
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
        var entity = await context.WorkerRequests.SingleAsync(s => s.Id == worker.Id);
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
