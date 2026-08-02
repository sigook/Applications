using Covenant.Common.Enums;

namespace Covenant.Common.Entities.Accounting.Deductions;

public class TaxDeduction
{
    private TaxDeduction() { }

    public TaxDeduction(decimal @from,
        decimal to,
        decimal? cc0,
        decimal? cc1,
        decimal? cc2,
        decimal? cc3,
        decimal? cc4,
        decimal? cc5,
        decimal? cc6,
        decimal? cc7,
        decimal? cc8,
        decimal? cc9,
        decimal? cc10,
        int year,
        PayPeriod payPeriod,
        TaxType taxType,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        From = @from;
        To = to;
        Cc0 = cc0;
        Cc1 = cc1;
        Cc2 = cc2;
        Cc3 = cc3;
        Cc4 = cc4;
        Cc5 = cc5;
        Cc6 = cc6;
        Cc7 = cc7;
        Cc8 = cc8;
        Cc9 = cc9;
        Cc10 = cc10;
        Year = year;
        PayPeriod = payPeriod;
        TaxType = taxType;
    }

    public Guid Id { get; private set; }
    public PayPeriod PayPeriod { get; private set; }
    public TaxType TaxType { get; private set; }
    public decimal From { get; private set; }
    public decimal To { get; private set; }
    public decimal? Cc0 { get; private set; }
    public decimal? Cc1 { get; private set; }
    public decimal? Cc2 { get; private set; }
    public decimal? Cc3 { get; private set; }
    public decimal? Cc4 { get; private set; }
    public decimal? Cc5 { get; private set; }
    public decimal? Cc6 { get; private set; }
    public decimal? Cc7 { get; private set; }
    public decimal? Cc8 { get; private set; }
    public decimal? Cc9 { get; private set; }
    public decimal? Cc10 { get; private set; }
    public int Year { get; private set; }

    public override string ToString() =>
        $"{From} {To} {Cc0} {Cc1} {Cc2} {Cc3} {Cc4} {Cc5} {Cc6} {Cc7} {Cc8} {Cc9} {Cc10} {Year} {PayPeriod} {TaxType}";
}
