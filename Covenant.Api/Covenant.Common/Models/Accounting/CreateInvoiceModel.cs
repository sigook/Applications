namespace Covenant.Common.Models.Accounting;

public class CreateInvoiceModel
{
    public Guid CompanyProfileId { get; set; }
    public Guid CompanyId { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string Email { get; set; }
    public Guid? ProvinceId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public IEnumerable<CreateInvoiceItemModel> Discounts { get; set; } = Array.Empty<CreateInvoiceItemModel>();
    public IEnumerable<CreateInvoiceItemModel> AdditionalItems { get; set; } = Array.Empty<CreateInvoiceItemModel>();
    public bool DirectHiring { get; set; }
}