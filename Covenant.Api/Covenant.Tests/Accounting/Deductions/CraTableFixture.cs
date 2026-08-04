namespace Covenant.Tests.Accounting.Deductions;

public static class CraTableFixture
{
    public const string CppWeeklyBlobName = "CPP WEEKLY 2026.pdf";
    public const string TaxMonthlyBlobName = "TAX MONTHLY 2026.pdf";

    public const int CppWeeklyBrackets = 8928;
    public const int TaxMonthlyBrackets = 385;

    public static string CppWeeklyPath => Path("cpp_weekly_2026.pdf");

    public static string TaxMonthlyPath => Path("tax_monthly_2026.pdf");

    private static string Path(string fileName) =>
        System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Accounting", "Deductions", fileName);
}
