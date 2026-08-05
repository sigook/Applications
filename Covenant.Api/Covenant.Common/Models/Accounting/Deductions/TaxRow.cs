using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Deductions;

public record TaxRow(TaxType TaxType, decimal From, decimal To, IReadOnlyList<decimal?> ClaimCodes)
{
    public const int ClaimCodeCount = 11;
}
