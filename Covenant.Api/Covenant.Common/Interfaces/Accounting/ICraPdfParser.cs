using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Common.Interfaces.Accounting;

public interface ICraPdfParser
{
    IReadOnlyList<CppRow> ParseCpp(Stream pdf);

    IReadOnlyList<TaxRow> ParseTax(Stream pdf);
}
