namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceModel
{
    private decimal _totalNet;
    public Guid Id { get; set; }

    public decimal TotalNet
    {
        get
        {
            decimal hst = _totalNet * 0.13m;
            return decimal.Subtract(_totalNet, hst);
        }
        set => _totalNet = value;
    }

    public Guid CompanyId { get; set; }
    public long NumberId { get; set; }
    public string CompanyEmail { get; set; }
}