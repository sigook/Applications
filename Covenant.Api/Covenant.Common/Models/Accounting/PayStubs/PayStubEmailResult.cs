namespace Covenant.Common.Models.Accounting.PayStub;

public record PayStubEmailResult(Guid PayStubId, bool Success, string WorkerFullName, string PayrollNumber)
{
    public static PayStubEmailResult Failed(Guid payStubId) => new(payStubId, false, string.Empty, string.Empty);
}
