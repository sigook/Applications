using Covenant.Api.Controllers.Agency.Accounting;
using Covenant.Api.Controllers.Shared.Jobs;
using Covenant.Api.Authorization;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Entities.Agency;
using Covenant.Common.Entities.Company;
using Covenant.Common.Enums;
using Covenant.Common.Entities.Request;
using Covenant.Common.Entities.Worker;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Common.Models.Notification;
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

    private CovenantContext Context => _factory.Services.GetRequiredService<CovenantContext>();

    private async Task<Invoice> PostInvoice(Data.BillableCompany company, DateTime? invoiceDate = null)
    {
        var model = new CreateInvoiceModel { CompanyProfileId = company.CompanyProfile.Id, InvoiceDate = invoiceDate };
        HttpResponseMessage response = await _client.PostAsJsonAsync(InvoicesController.RouteName, model);
        response.EnsureSuccessStatusCode();
        return await Context.Invoices.AsNoTracking().SingleAsync(i => i.CompanyProfileId == company.CompanyProfile.Id);
    }

    private Task<HttpResponseMessage> ChangeStatus(Guid invoiceId, InvoiceStatus status, DateTime? paidAt = null) =>
        _client.PutAsJsonAsync($"{InvoicesController.RouteName}/{invoiceId}/status", new ChangeInvoiceStatusModel { Status = status, PaidAt = paidAt });

    private Task<InvoiceListModelWithTotals> GetInvoices(string query = "") =>
        _client.GetFromJsonAsync<InvoiceListModelWithTotals>($"{InvoicesController.RouteName}{query}");

    [Fact]
    public async Task Preview()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyProfileId = _data.ForPreview.CompanyProfile.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync($"{InvoicesController.RouteName}/Preview", model);
        response.EnsureSuccessStatusCode();
        var preview = await response.Content.ReadFromJsonAsync<InvoicePreviewModel>();
        Assert.NotNull(preview);
        Assert.NotEmpty(preview.Items);
        Assert.NotEmpty(preview.Discounts);
        Assert.False(await Context.Invoices.AnyAsync(i => i.CompanyProfileId == _data.ForPreview.CompanyProfile.Id));
    }

    [Fact]
    public async Task Post()
    {
        var model = new CreateInvoiceModel
        {
            AdditionalItems = new[] { new CreateInvoiceItemModel(1, 100, "Item") },
            Discounts = new[] { new CreateInvoiceItemModel(1, 50, "Discount") },
            CompanyProfileId = _data.ForPost.CompanyProfile.Id
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(InvoicesController.RouteName, model);
        response.EnsureSuccessStatusCode();
        var context = Context;
        var invoice = await context.Invoices
            .Include(i => i.InvoiceTotals)
            .Include(i => i.AdditionalItems)
            .Include(i => i.Discounts)
            .SingleAsync(i => i.CompanyProfileId == _data.ForPost.CompanyProfile.Id);
        Guid[] timeSheetIds = _data.ForPost.TimeSheetIds;
        Assert.Equal(timeSheetIds.Length, await context.TimeSheetTotals.CountAsync(t => timeSheetIds.Contains(t.TimeSheetId)));
        Assert.Equal(timeSheetIds.Length, invoice.InvoiceTotals.Count());
        Assert.Equal(model.AdditionalItems.Count(), invoice.AdditionalItems.Count());
        Assert.Equal(model.Discounts.Count(), invoice.Discounts.Count());
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        Assert.StartsWith("US-", invoice.InvoiceNumber);
        Assert.Equal(invoice.InvoiceTotals.Sum(t => t.Total) * 0.06m, invoice.Tax);
    }

    [Fact]
    public async Task ChangeStatus_ToPaid_RecordsAudit()
    {
        Invoice invoice = await PostInvoice(_data.ForPaid);

        HttpResponseMessage response = await ChangeStatus(invoice.Id, InvoiceStatus.Paid);
        response.EnsureSuccessStatusCode();

        var updated = await Context.Invoices.AsNoTracking().SingleAsync(i => i.Id == invoice.Id);
        Assert.Equal(InvoiceStatus.Paid, updated.Status);
        Assert.Equal(Data.CurrentUser.Id, updated.UpdatedBy);
        Assert.NotNull(updated.UpdatedAt);
        Assert.Equal(DateTime.Today, updated.PaidAt?.Date);
    }

    [Fact]
    public async Task ChangeStatus_ToPaid_WithPaymentDate_StoresIt_AndClearsItWhenReverted()
    {
        Invoice invoice = await PostInvoice(_data.ForPaidAt);
        var paidOn = DateTime.Today.AddDays(-3);

        (await ChangeStatus(invoice.Id, InvoiceStatus.Paid, paidOn)).EnsureSuccessStatusCode();
        var paid = await Context.Invoices.AsNoTracking().SingleAsync(i => i.Id == invoice.Id);
        Assert.Equal(paidOn, paid.PaidAt);

        (await ChangeStatus(invoice.Id, InvoiceStatus.Pending)).EnsureSuccessStatusCode();
        var pending = await Context.Invoices.AsNoTracking().SingleAsync(i => i.Id == invoice.Id);
        Assert.Null(pending.PaidAt);
    }

    [Fact]
    public async Task ChangeStatus_ToDue_ReturnsBadRequest()
    {
        HttpResponseMessage response = await ChangeStatus(Guid.NewGuid(), InvoiceStatus.Due);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task MarkOverdueInvoices_MarksOnlyPendingPastPaymentTerm()
    {
        int paymentTermDays = _data.ForOverdue.CompanyProfile.PaymentTermDays;
        Invoice overdue = await PostInvoice(_data.ForOverdue, DateTime.Today.AddDays(-paymentTermDays - 5));
        Invoice current = await PostInvoice(_data.ForNotOverdue, DateTime.Today.AddDays(-paymentTermDays + 1));

        HttpResponseMessage response = await _client.PostAsync($"{ScheduleTasksController.RouteName}/MarkOverdueInvoices", new StringContent(""));
        response.EnsureSuccessStatusCode();

        var context = Context;
        var overdueUpdated = await context.Invoices.AsNoTracking().SingleAsync(i => i.Id == overdue.Id);
        Assert.Equal(InvoiceStatus.Due, overdueUpdated.Status);
        Assert.Null(overdueUpdated.UpdatedBy);
        Assert.NotNull(overdueUpdated.UpdatedAt);
        Assert.Equal(InvoiceStatus.Pending, (await context.Invoices.AsNoTracking().SingleAsync(i => i.Id == current.Id)).Status);
        Startup.Teams.Verify(t => t.SendNotification(
            It.IsAny<string>(),
            It.Is<TeamsNotificationModel>(n => n.Text.Contains(overdue.InvoiceNumber) && !n.Text.Contains(current.InvoiceNumber))), Times.AtLeastOnce);

        var due = await GetInvoices($"?status={(int)InvoiceStatus.Due}");
        InvoiceListModel row = due.Detail.Items.Single(i => i.Id == overdue.Id);
        Assert.Equal(overdue.CreatedAt.Date.AddDays(paymentTermDays), row.DueDate);
        Assert.Equal(5, row.DueDays);
        Assert.Equal(paymentTermDays, row.PaymentTermDays);
    }

    [Fact]
    public async Task ChangeStatus_SameStatus_ReturnsBadRequest()
    {
        Invoice invoice = await PostInvoice(_data.ForSameStatus);
        (await ChangeStatus(invoice.Id, InvoiceStatus.Paid)).EnsureSuccessStatusCode();

        HttpResponseMessage response = await ChangeStatus(invoice.Id, InvoiceStatus.Paid);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_UnknownInvoice_ReturnsBadRequest()
    {
        HttpResponseMessage response = await ChangeStatus(Guid.NewGuid(), InvoiceStatus.Pending);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetInvoices_FiltersByStatus_AndProjectsAudit()
    {
        Invoice invoice = await PostInvoice(_data.ForList);
        (await ChangeStatus(invoice.Id, InvoiceStatus.Paid)).EnsureSuccessStatusCode();

        var paid = await GetInvoices($"?status={(int)InvoiceStatus.Paid}");
        Assert.NotNull(paid);
        Assert.All(paid.Detail.Items, row => Assert.Equal(InvoiceStatus.Paid, row.Status));
        InvoiceListModel row = paid.Detail.Items.Single(i => i.Id == invoice.Id);
        Assert.Equal(Data.CurrentPersonnel.Name, row.UpdatedByName);
        Assert.NotNull(row.UpdatedAt);
        Assert.Equal(DateTime.Today, row.PaidAt?.Date);
        Assert.Equal(0, row.DueDays);

        var pending = await GetInvoices($"?status={(int)InvoiceStatus.Pending}");
        Assert.NotNull(pending);
        Assert.All(pending.Detail.Items, item => Assert.Equal(InvoiceStatus.Pending, item.Status));
        Assert.DoesNotContain(pending.Detail.Items, i => i.Id == invoice.Id);
    }

    [Fact]
    public async Task Delete_ReleasesTimeSheetTotals()
    {
        Invoice invoice = await PostInvoice(_data.ForDelete);
        Guid[] timeSheetIds = _data.ForDelete.TimeSheetIds;
        Assert.Equal(timeSheetIds.Length, await Context.TimeSheetTotals.CountAsync(t => timeSheetIds.Contains(t.TimeSheetId)));

        var request = new HttpRequestMessage(HttpMethod.Delete, $"{InvoicesController.RouteName}/{invoice.Id}")
        {
            Content = JsonContent.Create(new DeleteInvoiceModel())
        };
        HttpResponseMessage response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var context = Context;
        Assert.False(await context.Invoices.AnyAsync(i => i.Id == invoice.Id));
        Assert.False(await context.InvoiceTotals.AnyAsync(t => t.InvoiceId == invoice.Id));
        Assert.False(await context.TimeSheetTotals.AnyAsync(t => timeSheetIds.Contains(t.TimeSheetId)));
    }

    public class Startup
    {
        public static readonly Mock<Covenant.Common.Interfaces.ITeamsService> Teams = new();

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
            services.AddSingleton(Teams.Object);
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

            public override Task<NextNumberModel> GetNextInvoiceNumber(Guid agencyId) => Task.FromResult(new NextNumberModel { NextNumber = _id++ });
        }
    }

    public class Data : ITestData
    {
        public static readonly Guid AgencyId = Guid.NewGuid();
        public static readonly DateTime FakeNow = new(2019, 01, 01);
        public static readonly User CurrentUser = FakeData.FakeUser();
        public static readonly AgencyPersonnel CurrentPersonnel = AgencyPersonnel.CreatePrimary(AgencyId, CurrentUser.Id, "Accounting Admin");

        private readonly City toronto;

        public Covenant.Common.Entities.Agency.Agency Agency { get; }
        public BillableCompany ForPreview { get; }
        public BillableCompany ForPost { get; }
        public BillableCompany ForPaid { get; }
        public BillableCompany ForPaidAt { get; }
        public BillableCompany ForSameStatus { get; }
        public BillableCompany ForList { get; }
        public BillableCompany ForDelete { get; }
        public BillableCompany ForOverdue { get; }
        public BillableCompany ForNotOverdue { get; }

        private BillableCompany[] Companies => [ForPreview, ForPost, ForPaid, ForPaidAt, ForSameStatus, ForList, ForDelete, ForOverdue, ForNotOverdue];

        public Data()
        {
            toronto = FakeData.FakeCity(FakeData.FakeProvince(FakeData.FakeCountry("USA")));
            Agency = FakeData.FakeAgency(AgencyId, toronto);
            ForPreview = new BillableCompany(Agency, toronto, "preview");
            ForPost = new BillableCompany(Agency, toronto, "post");
            ForPaid = new BillableCompany(Agency, toronto, "paid");
            ForPaidAt = new BillableCompany(Agency, toronto, "paid-at");
            ForSameStatus = new BillableCompany(Agency, toronto, "same-status");
            ForList = new BillableCompany(Agency, toronto, "list");
            ForDelete = new BillableCompany(Agency, toronto, "delete");
            ForOverdue = new BillableCompany(Agency, toronto, "overdue");
            ForNotOverdue = new BillableCompany(Agency, toronto, "not-overdue");
        }

        public void Seed(CovenantContext context)
        {
            context.Cities.Add(toronto);
            context.Users.Add(CurrentUser);
            context.AgencyPersonnel.Add(CurrentPersonnel);
            foreach (BillableCompany company in Companies) company.Seed(context);
            context.SaveChanges();
        }

        public class BillableCompany
        {
            private readonly LocationTax locationTax;
            private readonly Request request;
            private readonly WorkerProfile worker;
            private readonly TimeSheet[] timeSheets;

            public CompanyProfile CompanyProfile { get; }
            public Guid[] TimeSheetIds => timeSheets.Select(t => t.Id).ToArray();

            public BillableCompany(Covenant.Common.Entities.Agency.Agency agency, City city, string key)
            {
                CompanyProfile = FakeData.FakeCompanyProfile(agency, city: city, companyEmail: $"{key}@company.com");
                worker = FakeData.FakeWorkerProfile(agency, $"{key}@worker.com", city);
                var jobLocation = FakeData.FakeLocation(city);
                locationTax = new LocationTax { LocationId = jobLocation.Id, Tax1 = 0.06m };
                request = new Request(CompanyProfile, FakeData.FakeJobPositionRate(CompanyProfile))
                {
                    AgencyRate = 2,
                    WorkerRate = 1
                };
                request.UpdateJobLocation(jobLocation, false);
                var workerRequest = WorkerRequest.AgencyBook(worker.Id, request.Id);
                timeSheets = [Approved(workerRequest, FakeNow), Approved(workerRequest, FakeNow.AddDays(1))];
            }

            private static TimeSheet Approved(WorkerRequest workerRequest, DateTime date)
            {
                var timeSheet = TimeSheet.CreateTimeSheet(workerRequest, date, TimeSpan.FromHours(8), now: FakeNow).Value;
                timeSheet.AddApprovedTime(date.AddHours(8), date.AddHours(16));
                return timeSheet;
            }

            public void Seed(CovenantContext context)
            {
                context.LocationTaxes.Add(locationTax);
                context.Requests.Add(request);
                context.TimeSheets.AddRange(timeSheets);
                context.WorkerProfiles.Add(worker);
            }
        }
    }
}
