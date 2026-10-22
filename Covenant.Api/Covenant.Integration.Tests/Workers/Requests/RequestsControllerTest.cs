using Covenant.Api.Controllers.Workers.Requests;
using Covenant.Common.Entities.Agencies;
using Covenant.Common.Entities.Companies;
using Covenant.Common.Entities.Requests;
using Covenant.Common.Entities.Workers;
using Covenant.Common.Entities;
using Covenant.Common.Enums;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Requests.Workers;
using Covenant.Common.Models.Requests;
using Covenant.Common.Models.Workers;
using Covenant.Common.Models;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using System.Net.Http.Json;
using System.Net;
using Xunit;

namespace Covenant.Integration.Tests.Workers.Requests;

public class RequestsControllerTest : BaseTestOrder, IClassFixture<CustomWebApplicationFactory<RequestsControllerTest.Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _factory;
    private readonly HttpClient _client;
    public RequestsControllerTest(CustomWebApplicationFactory<Startup> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static string RequestUri() => RequestsController.RouteName;

    [Fact]
    public async Task Get()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.NotEmpty(list.Items);
        Request request = Data.FakeRequest;
        WorkerRequestListModel model = list.Items.Single(r => r.Id == request.Id);
        Assert.Equal(request.Id, model.Id);
        Assert.Equal(request.JobTitle, model.JobTitle);
        Assert.Equal(request.NumberId, model.NumberId);
        Assert.Equal(request.WorkersQuantity, model.WorkersQuantity);
        Assert.Equal(request.IsAsap, model.IsAsap);
        Assert.Equal(request.WorkerRate, model.WorkerRate);
        DateAssert.Equal(request.CreatedAt, model.CreatedAt);
        Assert.Equal(request.FinishAt, model.FinishAt);
        Assert.Equal(request.StartAt, model.StartAt);
        Assert.Equal(request.DurationTerm.ToString(), model.DurationTerm);
    }

    [Fact]
    public async Task Get_DefaultSort_NewestFirst()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        var numberIds = list.Items.Select(r => r.NumberId).ToList();
        Assert.Equal(numberIds.OrderByDescending(n => n), numberIds);
        Assert.Contains(list.Items, r => r.Id == Data.FakeRequest.Id);
        Assert.Contains(list.Items, r => r.Id == Data.SecondBookedRequest.Id);
    }

    [Fact]
    public async Task Get_SortByOldest()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?sortBy={GetWorkerRequestsSortBy.NumberId}&isDescending=false");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        var numberIds = list.Items.Select(r => r.NumberId).ToList();
        Assert.Equal(numberIds.OrderBy(n => n), numberIds);
    }

    [Fact]
    public async Task Get_SortByRate()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?sortBy={GetWorkerRequestsSortBy.Rate}&isDescending=true");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.Equal(Data.SecondBookedRequest.Id, list.Items.First().Id);
        Assert.Equal(Data.FakeRequest.Id, list.Items.Last().Id);

        response = await _client.GetAsync($"{RequestUri()}?sortBy={GetWorkerRequestsSortBy.Rate}&isDescending=false");
        response.EnsureSuccessStatusCode();
        list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.Equal(Data.FakeRequest.Id, list.Items.First().Id);
        Assert.Equal(Data.SecondBookedRequest.Id, list.Items.Last().Id);
    }

    [Fact]
    public async Task Get_SearchByNumberId()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?numberId={Data.SecondBookedRequest.NumberId}");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.Equal(Data.SecondBookedRequest.Id, Assert.Single(list.Items).Id);
    }

    [Fact]
    public async Task Get_SearchByJobTitle()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}?jobTitle=driv");
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.Equal(Data.FakeRequest.Id, Assert.Single(list.Items).Id);

        response = await _client.GetAsync($"{RequestUri()}?jobTitle=nothing");
        response.EnsureSuccessStatusCode();
        list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.Empty(list.Items);
    }

    [Fact]
    public async Task Get_BookedWorker_SeesOnlyBookedRequests()
    {
        HttpResponseMessage response = await _client.GetAsync(RequestUri());
        response.EnsureSuccessStatusCode();
        var list = await response.Content.ReadFromJsonAsync<PaginatedList<WorkerRequestListModel>>();
        Assert.Equal(2, list.TotalItems);
        Assert.All(list.Items, r => Assert.Equal(WorkerRequestStatus.Booked.ToString(), r.Status));
        Assert.DoesNotContain(list.Items, r => r.Id == Data.OpenRequest.Id);
    }

    [Fact]
    public async Task GetById()
    {
        HttpResponseMessage response = await _client.GetAsync($"{RequestUri()}/{Data.FakeRequest.Id}");
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<WorkerRequestDetailModel>();
        Assert.NotNull(detail);
        Assert.Equal(Data.FakeRequest.Id, detail.Id);
        Assert.Equal(Data.FakeRequest.JobTitle, detail.JobTitle);
        Assert.Equal(Data.FakeRequest.Description, detail.Description);
        Assert.Equal(Data.FakeRequest.Requirements, detail.Requirements);
        Assert.Equal(Data.FakeRequest.WorkersQuantity, detail.WorkersQuantity);
        Assert.Equal(Data.FakeRequest.JobPositionRate.JobPosition, detail.JobPosition);
        Assert.Equal(Data.FakeRequest.HolidayIsPaid, detail.HolidayIsPaid);
        Assert.Equal(Data.FakeRequest.BreakIsPaid, detail.BreakIsPaid);
        Assert.Equal(WorkerRequestStatus.Booked.ToString(), detail.Status);
        Assert.Equal(Data.FakeRequest.DurationTerm.ToString(), detail.DurationTerm);
        Assert.Equal(Data.FakeRequest.Incentive, detail.Incentive);
        Assert.Equal(Data.FakeRequest.IncentiveDescription, detail.IncentiveDescription);
        Assert.Equal(Data.FakeRequest.DurationBreak, detail.DurationBreak);
        Assert.Equal(Data.FakeRequest.Status.ToString(), detail.RequestStatus);
        Assert.Equal(Data.FakeRequest.WorkerRate, detail.WorkerRate);
        Assert.Equal(Data.FakeRequest.Status.ToString(), detail.RequestStatus);
        DateAssert.Equal(Data.FakeRequest.CreatedAt, detail.CreatedAt);
        Assert.Equal(Data.FakeRequest.StartAt, detail.StartAt);
        Assert.Equal(Data.FakeRequest.FinishAt, detail.FinishAt);
        Assert.Equal(Data.FakeRequest.JobLocation.Address, detail.JobLocation.Address);
        Assert.Equal(Data.FakeRequest.JobLocation.Entrance, detail.JobLocation.Entrance);
    }

    [Fact]
    public async Task Apply2()
    {
        var model = new WorkerRequestApplyModel { Comments = "Hard Worker" };
        var url = $"{RequestUri()}/{Data.OpenRequest.Id}/Apply";
        HttpResponseMessage response = await _client.PostAsJsonAsync(url, model);
        response.EnsureSuccessStatusCode();
        var detail = await response.Content.ReadFromJsonAsync<RequestApplicantDetailModel>();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        RequestApplicant entity = await context.RequestApplicants.SingleAsync(s => s.Id == detail.Id);
        Assert.Equal(detail.WorkerProfileId, entity.WorkerProfileId);
        Assert.Equal(model.Comments, entity.Comments);
        Assert.Null(entity.CandidateId);
        response = await _client.PostAsJsonAsync(url, model);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder()
                .AddTestAuth(o =>
                {
                    o.AddSub(Data.WorkerProfile.Worker.Id);
                    o.AddWorkerRole();
                });
            services.AddTestDatabase();
            services.AddSingleton<TimeProvider>(new FakeTimeProvider(new DateTimeOffset(Data.Now, TimeSpan.Zero)));
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
            context.Requests.AddRange(Data.FakeRequest, Data.OpenRequest, Data.SecondBookedRequest);
            context.CompanyProfiles.Add(Data.CompanyProfile);
            context.CompanyProfileJobPositionRates.Add(Data.FakeRate);
            context.WorkerProfiles.Add(Data.WorkerProfile);
            context.WorkerRequests.AddRange(Data.FakeWorkerRequest, Data.SecondWorkerRequest);
            context.SaveChanges();
        }
    }
    internal static class Data
    {
        public static readonly DateTime Now = new DateTime(2019, 01, 01);

        public static readonly Agency FakeAgency = new Agency
        {
            User = new User(CvnEmail.Create("agency@mail.com").Value)
        }.With(a => a.ChangeLogo(new CovenantFile("logo.png")));
        public static readonly CompanyProfile CompanyProfile = new CompanyProfile { Company = new User(CvnEmail.Create("company@mail.com").Value), Agency = FakeAgency, Logo = new CovenantFile() , Industry = new CompanyProfileIndustry("Test") };
        public static readonly WorkerProfile WorkerProfile;
        public static readonly CompanyProfileJobPositionRate FakeRate = CompanyProfileJobPositionRate.Create(CompanyProfile.Id, "Position", 1, 1, "General", "r@m.com").Value;

        public static readonly Request FakeRequest = Request.AgencyCreateRequest(CompanyProfile.Id, new Location
        {
            Address = "4917 Dundas",
            Entrance = "Back Door",
            City = new City
            {
                Province = new Province
                {
                    Id = Guid.NewGuid(),
                    Value = "Province Test",
                    Code = "ON",
                    Country = new Country
                    {
                        Id = Guid.NewGuid(),
                        Value = "Country Test",
                        Code = "CA"
                    }
                }
            }
        }, Now, FakeRate.Id).Value;
        public static readonly Request OpenRequest = Request.AgencyCreateRequest(CompanyProfile.Id, FakeData.FakeLocation(), Now, FakeRate.Id).Value;
        public static readonly Request SecondBookedRequest = Request.AgencyCreateRequest(CompanyProfile.Id, FakeData.FakeLocation(), Now, FakeRate.Id).Value;
        public static readonly WorkerRequest FakeWorkerRequest;
        public static readonly WorkerRequest SecondWorkerRequest;

        static Data()
        {
            FakeRequest.WorkerRate = 15;
            FakeRequest.UpdateJobTitle("Driver");
            SecondBookedRequest.WorkerRate = 25;
            SecondBookedRequest.UpdateJobTitle("Cook");
            WorkerProfile = new WorkerProfile(new User(CvnEmail.Create("worker_profile@mail.com").Value))
            {
                Agency = FakeAgency,
                ApprovedToWork = true,
                Location = new Location { City = new City { Province = new Province { Country = FakeData.FakeCountry("USA") } } },
                IdentificationType1 = new IdentificationType(),
                IdentificationType1File = new CovenantFile(),
                IdentificationType2 = new IdentificationType(),
                IdentificationType2File = new CovenantFile(),
            };
            WorkerProfile.PatchProfileImage(new CovenantFile("profile.png"));
            WorkerProfile.PatchSinInformation(new FakeSinInfo());
            WorkerProfile.PatchDocuments(new FakeDocuments());
            CompanyProfile.UpdateName("Microsoft");
            FakeWorkerRequest = WorkerRequest.AgencyBook(WorkerProfile.Id, FakeRequest.Id);
            SecondWorkerRequest = WorkerRequest.AgencyBook(WorkerProfile.Id, SecondBookedRequest.Id);
        }

        private class FakeSinInfo : ISinInformation<CovenantFile>
        {
            public string SocialInsurance { get; set; } = "BFF1AAB7E4BD";
            public bool SocialInsuranceExpire { get; set; } = false;
            public DateTime? DueDate { get; set; }
            public CovenantFile SocialInsuranceFile { get; set; } = new CovenantFile("s.pdf");
        }

        private class FakeDocuments : IWorkerDocumentsInformation<IdentificationType, CovenantFile>
        {
            public string IdentificationNumber1 { get; set; } = "5C7292F4D4";
            public IdentificationType IdentificationType1 { get; set; }
            public CovenantFile IdentificationType1File { get; set; } = new CovenantFile("d.pdf");
            public string IdentificationNumber2 { get; set; } = "92EAC1D901";
            public IdentificationType IdentificationType2 { get; set; }
            public CovenantFile IdentificationType2File { get; set; } = new CovenantFile("2.pdf");
            public bool HavePoliceCheckBackground { get; set; }
            public CovenantFile PoliceCheckBackGround { get; set; }
            public CovenantFile Resume { get; set; }
        }
    }
}
