namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceViewModel
{
    public string BillTo { get; set; }
    public string Address { get; set; }
    public string HtmlNotes { get; set; }
    public string Fax { get; set; }
    public int? FaxExt { get; set; }
    public string Email { get; set; }
    public DateOnly CreatedAt { get; set; }
    public long NumberId { get; set; }
    public string AgencyFullName { get; set; }
    public string PhonePrincipal { get; set; }
    public int? PhonePrincipalExt { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Hst { get; set; }
    public decimal Total { get; set; }
    public string TaxName { get; set; }
    public string HstNumber { get; set; }
    public DateTime? WeekEnding { get; set; }
    public string AgencyLogoFileName { get; set; }
    public string AgencyAddress { get; set; }
    public string AgencyPhone { get; set; }
    public int? AgencyPhoneExt { get; set; }
    public string AgencyFax { get; set; }
    public int? AgencyFaxExt { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid CompanyProfileId { get; set; }
    public Guid Id { get; set; }
    public IReadOnlyList<DescriptionTableViewModel> Items { get; set; } = new List<DescriptionTableViewModel>();
    public InvoiceColor InvoiceColor { get; set; }
}