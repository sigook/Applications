using Covenant.Common.Entities.Accounting.PayStub;

namespace Covenant.Common.Models.Accounting.PayStub;

public class TimesheetExtrasResult
{
    public List<PayStubItem> BonusItems { get; init; } = [];
    public List<PayStubItem> ReimbursementItems { get; init; } = [];
    public List<PayStubOtherDeduction> OtherDeductions { get; init; } = [];
}
