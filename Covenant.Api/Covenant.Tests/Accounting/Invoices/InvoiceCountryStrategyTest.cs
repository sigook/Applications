using Covenant.Common.Configuration;
using Covenant.Common.Models.Accounting.Invoices;
using Covenant.Common.Models.Accounting;
using Covenant.Core.BL.Interfaces.Accounting.Invoices;
using Covenant.Core.BL.Services.Accounting.Invoices;
using Xunit;

namespace Covenant.Tests.Accounting.Invoices;

public class InvoiceCountryStrategyTest
{
    private static readonly IInvoiceCountryStrategy Canada = new CanadaInvoiceStrategy(Rates.DefaultRates);
    private static readonly IInvoiceCountryStrategy Usa = new UsaInvoiceStrategy();

    [Fact]
    public void Canada_Tax_IsGlobalHstOnSubTotal()
    {
        var tax = Canada.CalculateTax(1000m, 999m, new CreateInvoiceModel());
        Assert.Equal(1000m * Rates.DefaultRates.Hst, tax);
    }

    [Fact]
    public void Usa_Tax_ComesFromTimesheets()
    {
        var tax = Usa.CalculateTax(1000m, 60m, new CreateInvoiceModel());
        Assert.Equal(60m, tax);
    }

    [Fact]
    public void Usa_Tax_DirectHiring_UsesTaxPercentage()
    {
        var tax = Usa.CalculateTax(1000m, 60m, new CreateInvoiceModel { DirectHiring = true, TaxPercentage = 7m });
        Assert.Equal(70m, tax);
    }

    [Fact]
    public void Usa_Tax_DirectHiring_WithoutPercentage_IsZero()
    {
        var tax = Usa.CalculateTax(1000m, 60m, new CreateInvoiceModel { DirectHiring = true });
        Assert.Equal(0m, tax);
    }

    [Fact]
    public void BuildInvoiceNumber_UsesPrefixAndTwoDigitYear()
    {
        Assert.Equal("AI-0333-19", Canada.BuildInvoiceNumber(333, new DateTime(2019, 5, 27)));
        Assert.Equal("US-1001-23", Usa.BuildInvoiceNumber(1001, new DateTime(2023, 4, 17)));
    }

    [Fact]
    public void Canada_Decorate_KeepsHstNumberAndCovenantBranding()
    {
        var summary = new InvoiceSummaryModel { HstNumber = "826842692RT0001" };
        Canada.Decorate(summary);
        Assert.Equal("HST", summary.TaxName);
        Assert.Equal("826842692RT0001", summary.HstNumber);
        Assert.Equal(InvoiceColor.Covenant.HeaderTableColor, summary.InvoiceColor.HeaderTableColor);
        Assert.Equal(InvoicePayroll.Covenant.EmailSettingName, summary.InvoicePayroll.EmailSettingName);
        Assert.True(Canada.BillsUnworkedHolidays);
    }

    [Fact]
    public void Usa_Decorate_ClearsHstNumberAndUsesSigookBranding()
    {
        var summary = new InvoiceSummaryModel { HstNumber = "826842692RT0001" };
        Usa.Decorate(summary);
        Assert.Equal("Tax", summary.TaxName);
        Assert.Null(summary.HstNumber);
        Assert.Equal(InvoiceColor.Sigook.HeaderTableColor, summary.InvoiceColor.HeaderTableColor);
        Assert.Equal(InvoicePayroll.Sigook.EmailSettingName, summary.InvoicePayroll.EmailSettingName);
        Assert.False(Usa.BillsUnworkedHolidays);
    }
}
