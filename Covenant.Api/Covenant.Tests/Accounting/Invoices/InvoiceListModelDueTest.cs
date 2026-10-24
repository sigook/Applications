using Covenant.Common.Enums;
using Covenant.Common.Models.Accounting.Invoices;
using System;
using Xunit;

namespace Covenant.Tests.Accounting.Invoices;

public class InvoiceListModelDueTest
{
    private static readonly DateTime InvoiceDate = new(2026, 9, 1, 10, 0, 0);

    private static InvoiceListModel Invoice(InvoiceStatus status, int paymentTermDays = 30) => new()
    {
        CreatedAt = InvoiceDate,
        PaymentTermDays = paymentTermDays,
        Status = status
    };

    [Fact]
    public void DueDate_IsInvoiceDatePlusPaymentTerm()
    {
        var invoice = Invoice(InvoiceStatus.Pending, 14);
        Assert.Equal(new DateTime(2026, 9, 15), invoice.DueDate);
    }

    [Fact]
    public void DueDays_IsZero_BeforeDueDate()
    {
        var invoice = Invoice(InvoiceStatus.Pending);
        invoice.SetDueDays(new DateTime(2026, 9, 20));
        Assert.Equal(0, invoice.DueDays);
    }

    [Fact]
    public void DueDays_CountsDaysPastDueDate()
    {
        var invoice = Invoice(InvoiceStatus.Due);
        invoice.SetDueDays(new DateTime(2026, 10, 13, 23, 0, 0));
        Assert.Equal(12, invoice.DueDays);
    }

    [Fact]
    public void DueDays_IsZero_WhenPaid()
    {
        var invoice = Invoice(InvoiceStatus.Paid);
        invoice.SetDueDays(new DateTime(2026, 12, 1));
        Assert.Equal(0, invoice.DueDays);
    }
}
