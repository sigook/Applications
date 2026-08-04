using Sigook.Functions.Models;
using Sigook.Functions.Utils;
using Xunit;

namespace Sigook.Functions.Tests;

public class CraBlobNameTest
{
    [Theory]
    [InlineData("CPP WEEKLY 2026.pdf", CraTableKind.Cpp, PayPeriod.Weekly, 2026)]
    [InlineData("CPP BIWEEKLY 2026.pdf", CraTableKind.Cpp, PayPeriod.BiWeekly, 2026)]
    [InlineData("CPP BI-WEEKLY 2026.pdf", CraTableKind.Cpp, PayPeriod.BiWeekly, 2026)]
    [InlineData("cpp_semi-monthly_2027.pdf", CraTableKind.Cpp, PayPeriod.SemiMonthly, 2027)]
    [InlineData("Cpp-Monthly-2030.PDF", CraTableKind.Cpp, PayPeriod.Monthly, 2030)]
    [InlineData("TAX MONTHLY 2026.pdf", CraTableKind.Tax, PayPeriod.Monthly, 2026)]
    [InlineData("tax_semi-monthly_2027.pdf", CraTableKind.Tax, PayPeriod.SemiMonthly, 2027)]
    [InlineData("Tax-Bi-Weekly-2030.PDF", CraTableKind.Tax, PayPeriod.BiWeekly, 2030)]
    public void Reads_The_Table_The_Pay_Period_And_The_Year(string blobName, CraTableKind kind, PayPeriod payPeriod, int year)
    {
        Assert.True(CraBlobName.TryParse(blobName, out var table, out var error));
        Assert.Null(error);
        Assert.Equal(kind, table.Kind);
        Assert.Equal(blobName, table.Import.BlobName);
        Assert.Equal(payPeriod, table.Import.PayPeriod);
        Assert.Equal(year, table.Import.Year);
    }

    [Theory]
    [InlineData("")]
    [InlineData("CPP WEEKLY 2026.xlsx")]
    [InlineData("CPP WEEKLY.pdf")]
    [InlineData("CPP ANNUAL 2026.pdf")]
    [InlineData("FEDERAL WEEKLY 2026.pdf")]
    [InlineData("CPP WEEKLY 1999.pdf")]
    [InlineData("TAX WEEKLY 1999.pdf")]
    [InlineData("tabla.pdf")]
    public void Rejects_Anything_That_Does_Not_Follow_The_Convention(string blobName)
    {
        Assert.False(CraBlobName.TryParse(blobName, out var table, out var error));
        Assert.Null(table);
        Assert.False(string.IsNullOrEmpty(error));
    }
}
