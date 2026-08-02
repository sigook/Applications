using Covenant.Common.Enums;

namespace Covenant.Common.Models.Accounting.Deductions;

public class TaxModel
{
    public Guid Id { get; set; }

    public PayPeriod PayPeriod { get; set; }

    public TaxType TaxType { get; set; }

    public decimal From { get; set; }

    public decimal To { get; set; }

    public decimal? Cc0 { get; set; }

    public decimal? Cc1 { get; set; }

    public decimal? Cc2 { get; set; }

    public decimal? Cc3 { get; set; }

    public decimal? Cc4 { get; set; }

    public decimal? Cc5 { get; set; }

    public decimal? Cc6 { get; set; }

    public decimal? Cc7 { get; set; }

    public decimal? Cc8 { get; set; }

    public decimal? Cc9 { get; set; }

    public decimal? Cc10 { get; set; }

    public int Year { get; set; }
}
