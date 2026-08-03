using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Infrastructure.Accounting.Deductions;
using Xunit;

namespace Covenant.Tests.Accounting.Deductions;

public class CraPdfParserTest
{
    private readonly CraPdfParser _sut = new();

    [Fact]
    public void Reads_Every_Bracket_Of_The_Cra_Weekly_Cpp_Table()
    {
        var rows = ParseCpp();

        Assert.Equal(CraTableFixture.CppWeeklyBrackets, rows.Count);
    }

    [Fact]
    public void Reads_The_First_And_The_Last_Cpp_Bracket()
    {
        var rows = ParseCpp().OrderBy(r => r.From).ToList();

        Assert.Equal(0.00m, rows[0].From);
        Assert.Equal(67.30m, rows[0].To);
        Assert.Equal(0.00m, rows[0].Cpp);

        Assert.Equal(9344.62m, rows[^1].From);
        Assert.Equal(9354.61m, rows[^1].To);
        Assert.Equal(552.30m, rows[^1].Cpp);
    }

    [Fact]
    public void Produces_A_Cpp_Table_That_Passes_Validation()
    {
        var result = CppTableValidator.Validate(ParseCpp());

        Assert.True(result.IsSuccess, result.StringErrors);
    }

    [Theory]
    [InlineData(TaxType.Federal)]
    [InlineData(TaxType.Provincial)]
    public void Reads_Every_Bracket_Of_Both_Cra_Monthly_Tax_Tables(TaxType taxType)
    {
        var rows = ParseTax(taxType);

        Assert.Equal(CraTableFixture.TaxMonthlyBrackets, rows.Count);
    }

    [Fact]
    public void Reads_The_First_And_The_Last_Federal_Bracket()
    {
        var rows = ParseTax(TaxType.Federal);

        Assert.Equal(0m, rows[0].From);
        Assert.Equal(1601m, rows[0].To);
        Assert.Null(rows[0].ClaimCodes[0]);
        Assert.Equal(0.00m, rows[0].ClaimCodes[1]);

        Assert.Equal(24227m, rows[^1].From);
        Assert.Equal(24363m, rows[^1].To);
        Assert.Equal(5746.00m, rows[^1].ClaimCodes[0]);
        Assert.Equal(5292.05m, rows[^1].ClaimCodes[10]);
    }

    [Fact]
    public void Reads_The_First_And_The_Last_Provincial_Bracket()
    {
        var rows = ParseTax(TaxType.Provincial);

        Assert.Equal(0m, rows[0].From);
        Assert.Equal(1682m, rows[0].To);

        Assert.Equal(24308m, rows[^1].From);
        Assert.Equal(24444m, rows[^1].To);
        Assert.Equal(3659.30m, rows[^1].ClaimCodes[0]);
        Assert.Equal(3417.90m, rows[^1].ClaimCodes[10]);
    }

    [Fact]
    public void Leaves_A_Claim_Code_Empty_Until_The_Tax_Starts_Being_Withheld()
    {
        var rows = ParseTax(TaxType.Federal);

        var withheld = rows.First(r => r.ClaimCodes[10] is not null);

        Assert.Equal(3771m, withheld.From);
        Assert.All(rows.TakeWhile(r => r.From < withheld.From), row => Assert.Null(row.ClaimCodes[10]));
    }

    [Fact]
    public void Produces_A_Tax_Table_That_Passes_Validation()
    {
        var result = TaxTableValidator.Validate(Parse(CraTableFixture.TaxMonthlyPath, _sut.ParseTax));

        Assert.True(result.IsSuccess, result.StringErrors);
    }

    private IReadOnlyList<CppRow> ParseCpp() => Parse(CraTableFixture.CppWeeklyPath, _sut.ParseCpp);

    private List<TaxRow> ParseTax(TaxType taxType) =>
        [.. Parse(CraTableFixture.TaxMonthlyPath, _sut.ParseTax).Where(r => r.TaxType == taxType).OrderBy(r => r.From)];

    private static IReadOnlyList<TRow> Parse<TRow>(string fixturePath, Func<Stream, IReadOnlyList<TRow>> parse)
    {
        using var stream = File.OpenRead(fixturePath);
        return parse(stream);
    }
}
