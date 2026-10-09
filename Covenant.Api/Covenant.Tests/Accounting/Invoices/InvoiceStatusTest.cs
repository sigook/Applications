using Covenant.Common.Entities.Accounting.Invoice;
using Covenant.Common.Enums;
using System;
using Xunit;

namespace Covenant.Tests.Accounting.Invoices;

public class InvoiceStatusTest
{
    private static readonly Guid Actor = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 10, 8, 10, 30, 0);

    [Fact]
    public void Invoice_DefaultsToPending_WithoutAudit()
    {
        var invoice = new Invoice();
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        Assert.Null(invoice.UpdatedAt);
        Assert.Null(invoice.UpdatedBy);
    }

    [Fact]
    public void Invoice_ChangeStatus_SetsStatusAndAudit()
    {
        var invoice = new Invoice();
        var result = invoice.ChangeStatus(InvoiceStatus.Paid, Actor, Now);
        Assert.True(result);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(Now, invoice.UpdatedAt);
        Assert.Equal(Actor, invoice.UpdatedBy);
    }

    [Fact]
    public void Invoice_ChangeStatus_FailsWhenAlreadyInThatStatus()
    {
        var invoice = new Invoice();
        var result = invoice.ChangeStatus(InvoiceStatus.Pending, Actor, Now);
        Assert.False(result);
        Assert.Null(invoice.UpdatedAt);
        Assert.Null(invoice.UpdatedBy);
    }

    [Fact]
    public void Invoice_ChangeStatus_CanRevertToPending()
    {
        var invoice = new Invoice();
        invoice.ChangeStatus(InvoiceStatus.Paid, Actor, Now);
        var later = Now.AddDays(1);
        var otherActor = Guid.NewGuid();
        var result = invoice.ChangeStatus(InvoiceStatus.Pending, otherActor, later);
        Assert.True(result);
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        Assert.Equal(later, invoice.UpdatedAt);
        Assert.Equal(otherActor, invoice.UpdatedBy);
    }

    [Fact]
    public void InvoiceUSA_DefaultsToPending_WithoutAudit()
    {
        var invoice = new InvoiceUSA();
        Assert.Equal(InvoiceStatus.Pending, invoice.Status);
        Assert.Null(invoice.UpdatedAt);
        Assert.Null(invoice.UpdatedBy);
    }

    [Fact]
    public void InvoiceUSA_ChangeStatus_SetsStatusAndAudit()
    {
        var invoice = new InvoiceUSA();
        var result = invoice.ChangeStatus(InvoiceStatus.Paid, Actor, Now);
        Assert.True(result);
        Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        Assert.Equal(Now, invoice.UpdatedAt);
        Assert.Equal(Actor, invoice.UpdatedBy);
    }

    [Fact]
    public void InvoiceUSA_ChangeStatus_FailsWhenAlreadyInThatStatus()
    {
        var invoice = new InvoiceUSA();
        var result = invoice.ChangeStatus(InvoiceStatus.Pending, Actor, Now);
        Assert.False(result);
        Assert.Null(invoice.UpdatedAt);
    }
}
