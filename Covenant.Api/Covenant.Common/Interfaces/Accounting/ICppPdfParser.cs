using Covenant.Common.Models.Accounting.Deductions;

namespace Covenant.Common.Interfaces.Accounting;

public interface ICppPdfParser
{
    IReadOnlyList<CppRow> Parse(Stream pdf);
}
