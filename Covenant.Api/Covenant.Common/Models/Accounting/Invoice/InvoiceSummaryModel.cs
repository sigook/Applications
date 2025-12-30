namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceSummaryModel
{
    public Guid Id { get; set; }
    public long NumberId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateOnly CreatedAt { get; set; }
    public Guid CompanyProfileId { get; set; }
    public string CompanyFullName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string HtmlNotes { get; set; }
    public string Fax { get; set; }
    public int? FaxExt { get; set; }
    public string HstNumber { get; set; }
    public string PhonePrincipal { get; set; }
    public int? PhonePrincipalExt { get; set; }
    public string AgencyFullName { get; set; }
    public string AgencyLogoFileName { get; set; }
    public string AgencyAddress { get; set; }
    public string AgencyPhone { get; set; }
    public int? AgencyPhoneExt { get; set; }
    public string AgencyFax { get; set; }
    public int? AgencyFaxExt { get; set; }
    public string AgencyWebSite { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Hst { get; set; }
    public decimal Total { get; set; }
    public string TaxName { get; set; } = "HST";
    public DateTime? WeedEnding { get; set; }
    public InvoiceColor InvoiceColor { get; set; }
    public InvoicePayroll InvoicePayroll { get; set; }


    public List<InvoiceSummaryDiscountModel> Discounts { get; set; } = new List<InvoiceSummaryDiscountModel>();
    public List<InvoiceSummaryHolidayModel> Holidays { get; set; } = new List<InvoiceSummaryHolidayModel>();
    public List<InvoiceSummaryAdditionalItemModel> AdditionalItems { get; set; } = new List<InvoiceSummaryAdditionalItemModel>();

    public List<InvoiceSummaryItemModel> Items { get; set; } = new List<InvoiceSummaryItemModel>();
}