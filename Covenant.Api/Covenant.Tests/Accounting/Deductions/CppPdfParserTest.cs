using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Infrastructure.Accounting.Deductions;
using Xunit;

namespace Covenant.Tests.Accounting.Deductions;

public class CppPdfParserTest
{
    private const int WeeklyBrackets2026 = 8928;
    private static readonly string FixturePath =
        Path.Combine(Directory.GetCurrentDirectory(), "Accounting", "Deductions", "cpp_weekly_2026.pdf");

    private readonly CppPdfParser _sut = new();

    [Fact]
    public void Reads_Every_Bracket_Of_The_Cra_Weekly_Table()
    {
        var rows = Parse();

        Assert.Equal(WeeklyBrackets2026, rows.Count);
    }

    [Fact]
    public void Reads_The_First_And_The_Last_Bracket()
    {
        var rows = Parse().OrderBy(r => r.From).ToList();

        Assert.Equal(0.00m, rows[0].From);
        Assert.Equal(67.30m, rows[0].To);
        Assert.Equal(0.00m, rows[0].Cpp);

        Assert.Equal(9344.62m, rows[^1].From);
        Assert.Equal(9354.61m, rows[^1].To);
        Assert.Equal(552.30m, rows[^1].Cpp);
    }

    [Fact]
    public void Produces_A_Table_That_Passes_Validation()
    {
        var result = CppTableValidator.Validate(Parse());

        Assert.True(result.IsSuccess, result.StringErrors);
    }

    private IReadOnlyList<CppRow> Parse()
    {
        using var stream = File.OpenRead(FixturePath);
        return _sut.Parse(stream);
    }
}
