using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Accounting.Deductions;

public class CppDeduction
{
    private CppDeduction() { }

    public CppDeduction(decimal @from, decimal to, decimal cpp, int year, PayPeriod payPeriod, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        From = @from;
        To = to;
        Cpp = cpp;
        Year = year;
        PayPeriod = payPeriod;
    }

    public Guid Id { get; private set; }
    public PayPeriod PayPeriod { get; private set; }
    public decimal From { get; private set; }
    public decimal To { get; private set; }
    public decimal Cpp { get; private set; }
    public int Year { get; private set; }
}
