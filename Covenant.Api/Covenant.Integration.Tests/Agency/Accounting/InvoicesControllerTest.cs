using Covenant.Api.Controllers.Agency.Accounting;
using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Request.TimeSheet;
using Covenant.Common.Repositories.Accounting.Invoices;
using Covenant.Common.Utils.Extensions;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting.Invoices;
using Covenant.Integration.Tests.Configuration;
using Covenant.Integration.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net.Http.Json;
using Xunit;

namespace Covenant.Integration.Tests.Agency.Accounting;

public class InvoicesControllerTest : BaseTestOrder, IClassFixture<SeededWebApplicationFactory<InvoicesControllerTest.Startup, InvoicesControllerTest.Data>>
{
    private readonly SeededWebApplicationFactory<Startup, Data> _factory;
    private readonly Data _data;
    private readonly HttpClient _client;

    public InvoicesControllerTest(SeededWebApplicationFactory<Startup, Data> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _data = factory.Data;
    }

    [Fact, TestOrder(1)]
    public async Task Preview()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyProfileId = _data.CompanyProfile.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{InvoicesController.RouteName}/Preview", model);
        response.EnsureSuccessStatusCode();
        var preview = await response.Content.ReadFromJsonAsync<InvoicePreviewModel>();
        Assert.NotNull(preview);
        Assert.NotEmpty(preview.Items);
        Assert.NotEmpty(preview.Discounts);
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        Assert.Empty(await context.InvoicesUSA.ToListAsync());
    }

    [Fact, TestOrder(2)]
    public async Task Post()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyProfileId = _data.CompanyProfile.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(InvoicesController.RouteName, model);
        response.EnsureSuccessStatusCode();
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        Assert.Single(await context.InvoicesUSA.ToListAsync());
        Assert.Equal(_data.TimeSheets.Length, await context.TimeSheetTotals.CountAsync());
        Assert.Equal(3, (await context.InvoicesUSA.SingleAsync()).Items.Count());
        Assert.Equal(model.Discounts.Count(), (await context.InvoicesUSA.SingleAsync()).Discounts.Count());
        Assert.Equal(InvoiceStatus.Pending, (await context.InvoicesUSA.SingleAsync()).Status);
    }

    [Fact, TestOrder(3)]
    public async Task ChangeStatus_ToPaid_RecordsAudit()
    {
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var invoice = await context.InvoicesUSA.AsNoTracking().SingleAsync();

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{InvoicesController.RouteName}/{invoice.Id}/status",
            new ChangeInvoiceStatusModel { Status = InvoiceStatus.Paid });
        response.EnsureSuccessStatusCode();

        var updated = await context.InvoicesUSA.AsNoTracking().SingleAsync(i => i.Id == invoice.Id);
        Assert.Equal(InvoiceStatus.Paid, updated.Status);
        Assert.Equal(Data.CurrentUser.Id, updated.UpdatedBy);
        Assert.NotNull(updated.UpdatedAt);
    }

    [Fact, TestOrder(4)]
    public async Task ChangeStatus_SameStatus_ReturnsBadRequest()
    {
        var context = _factory.Services.GetRequiredService<CovenantContext>();
        var invoice = await context.InvoicesUSA.AsNoTracking().SingleAsync();

        HttpResponseMessage response = await _client.PutAsJsonAsync($"{InvoicesController.RouteName}/{invoice.Id}/status",
            new ChangeInvoiceStatusModel { Status = InvoiceStatus.Paid });
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact, TestOrder(5)]
    public async Task ChangeStatus_UnknownInvoice_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.PutAsJsonAsync($"{InvoicesController.RouteName}/{Guid.NewGuid()}/status",
            new ChangeInvoiceStatusModel { Status = InvoiceStatus.Pending });
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact, TestOrder(6)]
    public async Task GetInvoices_FiltersByStatus_AndProjectsAudit()
    {
        var paid = await _client.GetFromJsonAsync<InvoiceListModelWithTotals>($"{InvoicesController.RouteName}?status={(int)InvoiceStatus.Paid}");
        Assert.NotNull(paid);
        var row = Assert.Single(paid.Detail.Items);
        Assert.Equal(InvoiceStatus.Paid, row.Status);
        Assert.Equal(Data.CurrentPersonnel.Name, row.UpdatedByName);
        Assert.NotNull(row.UpdatedAt);

        var pending = await _client.GetFromJsonAsync<InvoiceListModelWithTotals>($"{InvoicesController.RouteName}?status={(int)InvoiceStatus.Pending}");
        Assert.NotNull(pending);
        Assert.Empty(pending.Detail.Items);
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultTestConfiguration();
            services.AddTestAuthenticationBuilder().AddTestAuth(o =>
            {
                o.AddAdminRole(Data.AgencyId);
            });
            services.AddTestDatabase();
            services.AddSingleton<IInvoiceRepository, InvoiceRepositoryTest>();
            services.AddSingleton(Rates.DefaultRates);
            services.AddSingleton(TimeLimits.DefaultTimeLimits);
            services.AddSingleton<AgencyIdFilter>();
            var invoiceContainer = new Mock<IInvoicesContainer>();
            services.AddSingleton(invoiceContainer.Object);
            var payStubContainer = new Mock<IPayStubsContainer>();
            services.AddSingleton(payStubContainer.Object);
            var currentUserService = new Mock<Covenant.Common.Interfaces.ICurrentUserService>();
            currentUserService.Setup(s => s.GetAgencyId()).Returns(Data.AgencyId);
            currentUserService.Setup(s => s.GetAgencyIds()).Returns(new List<Guid> { Data.AgencyId });
            currentUserService.Setup(s => s.GetUserId()).Returns(Data.CurrentUser.Id);
            services.AddSingleton(currentUserService.Object);
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

        private class InvoiceRepositoryTest : InvoiceRepository
        {
            private static long _id = 1;

            public InvoiceRepositoryTest(CovenantContext context) : base(context)
            {
            }

            public override Task<NextNumberModel> GetNextInvoiceUSANumber() => Task.FromResult(new NextNumberModel { NextNumber = _id++ });
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly Guid CompanyProfileId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);
        public static readonly User CurrentUser = FakeData.FakeUser();
        public static readonly AgencyPersonnel CurrentPersonnel = AgencyPersonnel.CreatePrimary(AgencyId, CurrentUser.Id, "Accounting Admin");

        private readonly City toronto;
        private readonly LocationTax locationTax;
        private readonly Request request;

        public Covenant.Common.Entities.Agency.Agency Agency { get; }
        public CompanyProfile CompanyProfile { get; }
        public WorkerProfile Worker { get; }
        public WorkerRequest WorkerRequest { get; }
        public TimeSheet[] TimeSheets { get; }

        public Data()
        {
            toronto = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("USA")));
            Agency = FakeData.FakeAgency(AgencyId, toronto);
            CompanyProfile = FakeData.FakeCompanyProfile(Agency, city: toronto, id: CompanyProfileId);
            Worker = FakeData.FakeWorkerProfile(Agency, "wor@wor.com", toronto);

            var jobLocation = FakeData.FakeLocation(toronto);
            locationTax = new LocationTax { LocationId = jobLocation.Id, Tax1 = 0.06m };

            request = new Request(CompanyProfile, FakeData.FakeJobPositionRate(CompanyProfile))
            {
                AgencyRate = 2,
                WorkerRate = 1
            };
            request.UpdateJobLocation(jobLocation, false);

            WorkerRequest = WorkerRequest.AgencyBook(Worker.Id, request.Id);
            var timeSheet = TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow, TimeSpan.FromHours(8), now: FakeNow).Value;
            var timeSheet1 = TimeSheet.CreateTimeSheet(WorkerRequest, FakeNow.AddDays(1), TimeSpan.FromHours(8), now: FakeNow).Value;
            timeSheet.AddApprovedTime(FakeNow.AddHours(8), FakeNow.AddHours(16));
            timeSheet1.AddApprovedTime(FakeNow.AddDays(1).AddHours(8), FakeNow.AddDays(1).AddHours(16));
            TimeSheets = [timeSheet, timeSheet1];
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(toronto);
            context.LocationTaxes.Add(locationTax);
            context.Requests.Add(request);
            context.TimeSheets.AddRange(TimeSheets);
            context.WorkerProfiles.Add(Worker);
            context.Users.Add(CurrentUser);
            context.AgencyPersonnel.Add(CurrentPersonnel);
            context.SaveChanges();
        }
    }
}
