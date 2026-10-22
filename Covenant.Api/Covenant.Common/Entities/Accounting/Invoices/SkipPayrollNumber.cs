namespace Covenant.Common.Entities.Accounting.Invoices;

public class SkipPayrollNumber
{
    public SkipPayrollNumber(Guid id, string payrollNumber)
    {
        Id = id;
        PayrollNumber = payrollNumber;
    }

    public Guid Id { get; private set; }
    public string PayrollNumber { get; private set; }
}