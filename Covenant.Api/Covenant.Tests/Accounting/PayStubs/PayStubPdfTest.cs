using Covenant.Common.Enums;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Core.BL.Adapters;
using Covenant.Documents.Services;
using Moq;
using UglyToad.PdfPig;
using Xunit;

namespace Covenant.Tests.Accounting.PayStubs;

public class PayStubPdfTest
{
    private const string LogoFileName = "agency-logo.png";
    private static readonly byte[] Png = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");

    private readonly Mock<IFilesContainer> _filesContainer = new();
    private readonly PayrollDocumentAdapter _adapter = new();

    private static PayStubDetailModel Model(PayStubYtdModel ytd = null, string logo = null) => new()
    {
        NumberId = 42,
        PayrollNumber = "PS-0042",
        AgencyFullName = "Covenant Staffing",
        AgencyPhone = "4165550000",
        AgencyPhoneExt = 12,
        AgencyLocation = "100 King St, Toronto",
        AgencyLogoFileName = logo,
        WorkerFullName = "Jane Worker",
        WorkerEmail = "jane@worker.com",
        Position = "General Labour",
        SinNumber = "123456789",
        EmployeeId = 7,
        StartDate = new DateTime(2026, 3, 2),
        EndDate = new DateTime(2026, 3, 8),
        CreatedAt = new DateTime(2026, 3, 9),
        PaymentDate = new DateTime(2026, 3, 13),
        Gross = 800m,
        Vacations = 32m,
        Earnings = 832m,
        DeductionCpp = 40m,
        DeductionEi = 13m,
        DeductionTax = 70m,
        DeductionProvincialTax = 30m,
        DeductionTotal = 163m,
        TotalNet = 669m,
        FederalCategory = TaxCategory.Cc1,
        ProvincialCategory = TaxCategory.Cc1,
        Items = [new PayStubDetailItemModel("Regular Hours", 40, 20m, 800m, PayStubItemType.Regular)],
        OtherDeductions =
        [
            new PayStubDetailItemModel("Safety boots", 1, 10m, 10m, PayStubItemType.Other),
            new PayStubDetailItemModel(null, 1, 5m, 5m, PayStubItemType.Other)
        ],
        Ytd = ytd
    };

    private static PayStubYtdModel Ytd() => new()
    {
        Gross = 4000m,
        Vacations = 160m,
        Earnings = 4160m,
        Cpp = 200m,
        Ei = 65m,
        FederalTax = 350m,
        ProvincialTax = 150m,
        TotalDeductions = 765m,
        TotalPaid = 3395m
    };

    private async Task<string> RenderText(PayStubDetailModel model)
    {
        var handler = new GeneratePayStubPdfHandler(_filesContainer.Object);
        byte[] pdf = await handler.Handle(new GeneratePayStubPdf(_adapter.MapToPayrollViewModel(model)), CancellationToken.None);
        using var document = PdfDocument.Open(pdf);
        return string.Join(" ", document.GetPages().Select(p => p.Text));
    }

    [Fact]
    public void MapToPayrollViewModel_MapsHeaderItemsAndTotals()
    {
        var model = Model();

        var result = _adapter.MapToPayrollViewModel(model);

        Assert.Equal("PS-0042", result.PayrollNumber);
        Assert.Equal("******6789", result.MaskedSin);
        Assert.Equal(12, result.AgencyPhoneExt);
        var item = Assert.Single(result.Table1Items);
        Assert.Equal("Regular Hours", item.Description);
        Assert.Equal(800m, item.Total);
        var labels = result.Table2Items.Select(i => i.Label).ToList();
        Assert.Contains("Federal TAX CC1 (-)", labels);
        Assert.Contains("Safety boots", labels);
        Assert.Contains("Others Deductions (-)", labels);
        Assert.Equal("Total Net Paid", labels[^1]);
        Assert.Empty(result.YtdItems);
    }

    [Fact]
    public void MapToPayrollViewModel_WithYtd_MapsYtdItems()
    {
        var result = _adapter.MapToPayrollViewModel(Model(Ytd()));

        Assert.Equal(11, result.YtdItems.Count());
        Assert.Equal("Gross Payment", result.YtdItems.First().Label);
        Assert.Equal("Total Net Paid", result.YtdItems.Last().Label);
    }

    [Fact]
    public void MapToPayrollViewModel_WithoutOtherDeductions_SkipsThem()
    {
        var model = Model();
        model.OtherDeductions = [];

        var result = _adapter.MapToPayrollViewModel(model);

        Assert.DoesNotContain(result.Table2Items, i => i.Label == "Safety boots");
    }

    [Fact]
    public void MapToPayrollEmailViewModel_MapsFields()
    {
        var result = _adapter.MapToPayrollEmailViewModel(Model());

        Assert.Equal("Jane Worker", result.WorkerFullName);
        Assert.Equal(669m, result.TotalNet);
        Assert.Equal("jane@worker.com", result.WorkerEmail);
        Assert.Equal("PS-0042", result.PayrollNumber);
    }

    [Fact]
    public async Task Handle_WithoutYtdAndLogo_RendersPayStub()
    {
        string text = await RenderText(Model());

        Assert.Contains("Jane Worker", text);
        Assert.Contains("PS-0042", text);
        Assert.Contains("******6789", text);
        Assert.Contains("Regular Hours", text);
        Assert.Contains("Total Net Paid", text);
        Assert.DoesNotContain("Year To Date", text);
        _filesContainer.Verify(f => f.Download(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithYtdAndLogo_RendersYearToDate()
    {
        _filesContainer.Setup(f => f.Download(LogoFileName)).ReturnsAsync(Png);

        string text = await RenderText(Model(Ytd(), LogoFileName));

        Assert.Contains("Year To Date", text);
        Assert.Contains("Ext: 12", text);
        _filesContainer.Verify(f => f.Download(LogoFileName), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidLogo_RendersWithoutLogo()
    {
        _filesContainer.Setup(f => f.Download(LogoFileName)).ReturnsAsync([1, 2, 3]);
        var model = Model(logo: LogoFileName);
        model.SinNumber = null;
        model.AgencyPhoneExt = null;
        model.PaymentDate = default;

        string text = await RenderText(model);

        Assert.Contains("Jane Worker", text);
        Assert.DoesNotContain("SIN:", text);
        Assert.DoesNotContain("Payment date:", text);
    }
}
