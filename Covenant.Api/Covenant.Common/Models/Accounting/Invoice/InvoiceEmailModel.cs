namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceEmailModel
{
    public string Subject { get; set; }
    public string Message { get; set; }
    public List<EmailAttachment> Attachments { get; set; } = [];
    public List<string> Cc { get; set; } = [];
}
