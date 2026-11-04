namespace Covenant.Common.Entities.Accounting.Invoices;

public class SkipPayrollNumber(Guid id, string payrollNumber)
{
    public Guid Id { get; private set; } = id;
    public string PayrollNumber { get; private set; } = payrollNumber;
}