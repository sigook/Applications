using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Common.Interfaces.Accounting;

public interface ICraPdfParser
{
    IReadOnlyList<CppRow> ParseCpp(byte[] pdf);

    IReadOnlyList<TaxRow> ParseTax(byte[] pdf);
}
