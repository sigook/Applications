using Microsoft.AspNetCore.Http;

namespace Covenant.Common.Models.Accounting.Invoice;

public class InvoiceEmailModel
{
    public string Subject { get; set; }
    public string Message { get; set; }
    public IList<IFormFile> Files { get; set; } = new List<IFormFile>();
    public List<string> Cc { get; set; } = new List<string>();
}
