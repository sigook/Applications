using Sigook.Functions.Models;
using Sigook.Functions.Utils;
using Xunit;

namespace Sigook.Functions.Tests;

public class CraBlobNameTest
{
    [Theory]
    [InlineData("CPP WEEKLY 2026.pdf", PayPeriod.Weekly, 2026)]
    [InlineData("CPP BIWEEKLY 2026.pdf", PayPeriod.BiWeekly, 2026)]
    [InlineData("CPP BI-WEEKLY 2026.pdf", PayPeriod.BiWeekly, 2026)]
    [InlineData("cpp_semi-monthly_2027.pdf", PayPeriod.SemiMonthly, 2027)]
    [InlineData("Cpp-Monthly-2030.PDF", PayPeriod.Monthly, 2030)]
    public void Reads_The_Pay_Period_And_The_Year(string blobName, PayPeriod payPeriod, int year)
    {
        Assert.True(CraBlobName.TryParse(blobName, out var model, out var error));
        Assert.Null(error);
        Assert.Equal(blobName, model.BlobName);
        Assert.Equal(payPeriod, model.PayPeriod);
        Assert.Equal(year, model.Year);
    }

    [Theory]
    [InlineData("")]
    [InlineData("CPP WEEKLY 2026.xlsx")]
    [InlineData("CPP WEEKLY.pdf")]
    [InlineData("CPP ANNUAL 2026.pdf")]
    [InlineData("FEDERAL WEEKLY 2026.pdf")]
    [InlineData("CPP WEEKLY 1999.pdf")]
    [InlineData("tabla.pdf")]
    public void Rejects_Anything_That_Does_Not_Follow_The_Convention(string blobName)
    {
        Assert.False(CraBlobName.TryParse(blobName, out var model, out var error));
        Assert.Null(model);
        Assert.False(string.IsNullOrEmpty(error));
    }
}
