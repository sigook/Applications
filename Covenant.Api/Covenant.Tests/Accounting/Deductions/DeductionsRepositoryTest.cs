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
        await GivenStoredCppYears(PayPeriod.Weekly, 2024, 2025, 2026);

        await _sut.ImportCpp(2027, PayPeriod.Weekly, CppTable(2027, PayPeriod.Weekly), YearsKept);

        Assert.Equal([2026, 2027], await StoredCppYears(PayPeriod.Weekly));
    }

    [Fact]
    public async Task ImportCpp_Replaces_The_Year_Being_Imported()
    {
        await GivenStoredCppYears(PayPeriod.Weekly, 2026);

        await _sut.ImportCpp(2026, PayPeriod.Weekly, CppTable(2026, PayPeriod.Weekly), YearsKept);

        Assert.Equal([2026], await StoredCppYears(PayPeriod.Weekly));
        Assert.Equal(CppTable(2026, PayPeriod.Weekly).Count,
            await _context.CppDeductions.CountAsync(c => c.Year == 2026 && c.PayPeriod == PayPeriod.Weekly));
    }

    [Fact]
    public async Task ImportCpp_Does_Not_Touch_The_Other_Pay_Periods()
    {
        await GivenStoredCppYears(PayPeriod.Monthly, 2020, 2024, 2025);

        await _sut.ImportCpp(2027, PayPeriod.Weekly, CppTable(2027, PayPeriod.Weekly), YearsKept);

        Assert.Equal([2020, 2024, 2025], await StoredCppYears(PayPeriod.Monthly));
    }

    [Fact]
    public async Task ImportTax_Keeps_The_Previous_Year_And_Drops_The_Older_Ones()
    {
        await GivenStoredTaxYears(PayPeriod.Monthly, 2024, 2025, 2026);

        await _sut.ImportTax(2027, PayPeriod.Monthly, TaxTable(2027, PayPeriod.Monthly), YearsKept);

        Assert.Equal([2026, 2027], await StoredTaxYears(PayPeriod.Monthly));
    }

    [Fact]
    public async Task ImportTax_Replaces_Both_Tax_Types_Of_The_Year_Being_Imported()
    {
        await GivenStoredTaxYears(PayPeriod.Monthly, 2026);

        await _sut.ImportTax(2026, PayPeriod.Monthly, TaxTable(2026, PayPeriod.Monthly), YearsKept);

        Assert.Equal([2026], await StoredTaxYears(PayPeriod.Monthly));
        Assert.Equal(TaxTable(2026, PayPeriod.Monthly).Count,
            await _context.TaxDeductions.CountAsync(t => t.Year == 2026 && t.PayPeriod == PayPeriod.Monthly));
    }

    [Fact]
    public async Task ImportTax_Does_Not_Touch_The_Other_Pay_Periods()
    {
        await GivenStoredTaxYears(PayPeriod.Weekly, 2020, 2024, 2025);

        await _sut.ImportTax(2027, PayPeriod.Monthly, TaxTable(2027, PayPeriod.Monthly), YearsKept);

        Assert.Equal([2020, 2024, 2025], await StoredTaxYears(PayPeriod.Weekly));
    }

    private async Task GivenStoredCppYears(PayPeriod payPeriod, params int[] years)
    {
        foreach (var year in years)
        {
            await _context.CppDeductions.AddRangeAsync(CppTable(year, payPeriod));
        }
        await _context.SaveChangesAsync();
    }

    private async Task GivenStoredTaxYears(PayPeriod payPeriod, params int[] years)
    {
        foreach (var year in years)
        {
            await _context.TaxDeductions.AddRangeAsync(TaxTable(year, payPeriod));
        }
        await _context.SaveChangesAsync();
    }

    private async Task<List<int>> StoredCppYears(PayPeriod payPeriod) =>
        await _context.CppDeductions
            .Where(c => c.PayPeriod == payPeriod)
            .Select(c => c.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToListAsync();

    private async Task<List<int>> StoredTaxYears(PayPeriod payPeriod) =>
        await _context.TaxDeductions
            .Where(t => t.PayPeriod == payPeriod)
            .Select(t => t.Year)
            .Distinct()
            .OrderBy(y => y)
            .ToListAsync();

    private static List<CppDeduction> CppTable(int year, PayPeriod payPeriod) =>
    [
        new(0.00m, 67.30m, 0.00m, year, payPeriod),
        new(67.31m, 77.30m, 0.35m, year, payPeriod)
    ];

    private static List<TaxDeduction> TaxTable(int year, PayPeriod payPeriod) =>
    [
        .. Enum.GetValues<TaxType>().Select(taxType => new TaxDeduction(0m, 1601m,
            null, 0.00m, null, null, null, null, null, null, null, null, null, year, payPeriod, taxType))
    ];
}
