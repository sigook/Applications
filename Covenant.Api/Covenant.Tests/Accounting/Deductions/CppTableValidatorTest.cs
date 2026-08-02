using Covenant.Common.Models.Accounting.Deductions;
using Covenant.Infrastructure.Accounting.Deductions;
using Xunit;

namespace Covenant.Tests.Accounting.Deductions;

public class CppTableValidatorTest
{
    [Fact]
    public void A_Contiguous_Table_Is_Valid()
    {
        Assert.True(CppTableValidator.Validate(Table()).IsSuccess);
    }

    [Fact]
    public void An_Empty_Table_Is_Rejected()
    {
        Assert.True(CppTableValidator.Validate([]).IsFailure);
    }

    [Fact]
    public void A_Short_Table_Is_Rejected()
    {
        Assert.True(CppTableValidator.Validate(Table(10)).IsFailure);
    }

    [Fact]
    public void A_Table_That_Does_Not_Start_At_Zero_Is_Rejected()
    {
        var rows = Table();
        rows[0] = rows[0] with { From = 0.01m };

        Assert.True(CppTableValidator.Validate(rows).IsFailure);
    }

    [Fact]
    public void A_Gap_Between_Brackets_Is_Rejected()
    {
        var rows = Table();
        rows.RemoveAt(500);

        Assert.True(CppTableValidator.Validate(rows).IsFailure);
    }

    [Fact]
    public void A_Duplicated_Bracket_Is_Rejected()
    {
        var rows = Table();
        rows.Insert(500, rows[500]);

        Assert.True(CppTableValidator.Validate(rows).IsFailure);
    }

    [Fact]
    public void A_Contribution_That_Goes_Down_Is_Rejected()
    {
        var rows = Table();
        rows[500] = rows[500] with { Cpp = 0m };

        Assert.True(CppTableValidator.Validate(rows).IsFailure);
    }

    private static List<CppRow> Table(int rows = 2000) =>
        [.. Enumerable.Range(0, rows).Select(i => new CppRow(i * 0.10m, i * 0.10m + 0.09m, i * 0.01m))];
}
