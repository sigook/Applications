using Covenant.Common.Entities.Accounting.Deductions;
using Covenant.Common.Enums;
using Covenant.Infrastructure.Contexts;
using Covenant.Infrastructure.Repositories.Accounting;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Covenant.Tests.Accounting.Deductions;

public class DeductionsRepositoryTest
{
    private const int YearsKept = 2;

    private readonly CovenantContext _context;
    private readonly DeductionsRepository _sut;

    public DeductionsRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<CovenantContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new CovenantContext(options);
        _sut = new DeductionsRepository(_context);
    }

    [Fact]
    public async Task ImportCpp_Keeps_The_Previous_Year_And_Drops_The_Older_Ones()
    {
        await GivenStoredYears(PayPeriod.Weekly, 2024, 2025, 2026);

        await _sut.ImportCpp(2027, PayPeriod.Weekly, Table(2027, PayPeriod.Weekly), YearsKept);

        Assert.Equal([2026, 2027], await StoredYears(PayPeriod.Weekly));
    }

    [Fact]
    public async Task ImportCpp_Replaces_The_Year_Being_Imported()
    {
        await GivenStoredYears(PayPeriod.Weekly, 2026);

        await _sut.ImportCpp(2026, PayPeriod.Weekly, Table(2026, PayPeriod.Weekly), YearsKept);

        Assert.Equal([2026], await StoredYears(PayPeriod.Weekly));
        Assert.Equal(Table(2026, PayPeriod.Weekly).Count,
            await _context.CppDeductions.CountAsync(c => c.Year == 2026 && c.PayPeriod == PayPeriod.Weekly));
    }

    [Fact]
    public async Task ImportCpp_Does_Not_Touch_The_Other_Pay_Periods()
    {
        await GivenStoredYears(PayPeriod.Monthly, 2020, 2024, 2025);

        await _sut.ImportCpp(2027, PayPeriod.Weekly, Table(2027, PayPeriod.Weekly), YearsKept);

        Assert.Equal([2020, 2024, 2025], await StoredYears(PayPeriod.Monthly));
    }

    private async Task GivenStoredYears(PayPeriod payPeriod, params int[] years)
    {
        foreach (var year in years)
        {
            await _context.CppDeductions.AddRangeAsync(Table(year, payPeriod));
        }
        await _context.SaveChangesAsync();
    }

    private async Task<List<int>> StoredYears(PayPeriod payPeriod) =>
        await _context.CppDeductions
            .Where(c => c.PayPeriod == payPeriod)
            .Select(c => c.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToListAsync();

    private static List<CppDeduction> Table(int year, PayPeriod payPeriod) =>
    [
        new(0.00m, 67.30m, 0.00m, year, payPeriod),
        new(67.31m, 77.30m, 0.35m, year, payPeriod)
    ];
}
