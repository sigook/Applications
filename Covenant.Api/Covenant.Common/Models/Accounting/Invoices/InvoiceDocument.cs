namespace Covenant.Common.Models.Accounting.Invoices;

public record InvoiceDocument(byte[] Content, string FileName, InvoiceSummaryModel Model);
