using System.Globalization;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.Invoice;
using Covenant.Documents.Pdf;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using static Covenant.Documents.Pdf.PdfLayout;

namespace Covenant.Documents.Services;

public class GenerateInvoicePdf(InvoiceViewModel model) : IRequest<byte[]>
{
    public InvoiceViewModel Model { get; } = model;
}

public class GenerateInvoicePdfHandler(IFilesContainer filesContainer) : IRequestHandler<GenerateInvoicePdf, byte[]>
{
    private static readonly string[] ItemHeaders = ["Item No.", "Description", "Qty", "Unit Price", "Total"];

    public async Task<byte[]> Handle(GenerateInvoicePdf request, CancellationToken cancellationToken)
    {
        var model = request.Model;
        var logo = await PdfLogo.Load(filesContainer, model.AgencyLogoFileName);
        return Generate(column =>
        {
            column.Item().Element(c => ComposeHeader(c, model, logo));
            column.Item().Element(c => ComposeBillTo(c, model));
            column.Item().Element(c => ComposeWeekEnding(c, model));
            column.Item().Element(c => ComposeItems(c, model));
            column.Item().Element(c => ComposeNotesAndTotals(c, model));
            column.Item().PaddingTop(12 * Px).AlignCenter()
                .Text($"If you have any questions about this invoice, please contact: {model.AgencyFullName}. ph: {model.AgencyPhone}{Ext(model.AgencyPhoneExt)}")
                .FontSize(12 * FontPx);
            column.Item().PoweredBy();
        });
    }

    private static void ComposeHeader(IContainer container, InvoiceViewModel model, PdfLogo logo)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                if (logo is not null) column.Item().Element(c => logo.Compose(c, 260 * Px));
                column.Item().PaddingTop(6 * Px).Text(text =>
                {
                    text.DefaultTextStyle(style => style.FontSize(12 * FontPx));
                    text.Line(model.AgencyAddress ?? string.Empty);
                    text.Line($"Phone: {model.AgencyPhone}{Ext(model.AgencyPhoneExt)}");
                    text.Span($"Fax: {model.AgencyFax}{Ext(model.AgencyFaxExt)}");
                });
            });
            row.AutoItem().PaddingTop(10 * Px).PaddingBottom(20 * Px).AlignTop().MinWidth(220 * Px)
                .Border(4 * Px).BorderColor(AccentColor).Padding(8 * Px).Column(box =>
                {
                    box.Item().AlignCenter().Text("INVOICE").FontSize(20 * FontPx).Bold();
                    box.Item().PaddingTop(10 * Px).AlignCenter().Text($"Invoice # {model.InvoiceNumber}").Bold();
                    box.Item().PaddingVertical(10 * Px).AlignCenter().Text(model.CreatedAt.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture)).Bold();
                });
        });
    }

    private static void ComposeBillTo(IContainer container, InvoiceViewModel model)
    {
        container.PaddingVertical(15 * Px).BorderBottom(1 * Px).BorderColor("#D3D3D3").PaddingBottom(20 * Px).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
            });
            LabeledCell(table.Cell(), "Bill to.", model.BillTo);
            LabeledCell(table.Cell(), "Phone:", $"{model.PhonePrincipal}{Ext(model.PhonePrincipalExt)}");
            LabeledCell(table.Cell(), "Address:", model.Address);
            LabeledCell(table.Cell(), "Email:", model.Email);
            LabeledCell(table.Cell(), "Fax:", $"{model.Fax}{Ext(model.FaxExt)}");
            table.Cell();
            if (!string.IsNullOrEmpty(model.ClientSiteAddress)) LabeledCell(table.Cell().ColumnSpan(2), "Site:", model.ClientSiteAddress);
        });
    }

    private static void LabeledCell(IContainer cell, string label, string value) =>
        cell.Padding(CellPadding).Text(text =>
        {
            text.Line(label).Bold();
            text.Span(value ?? string.Empty);
        });

    private static void ComposeWeekEnding(IContainer container, InvoiceViewModel model)
    {
        string weekEnding = model.WeekEnding.HasValue && model.WeekEnding.Value != default
            ? model.WeekEnding.Value.ToString("D", CultureInfo.InvariantCulture)
            : "N/A";
        container.PaddingBottom(20 * Px).Row(row =>
        {
            row.RelativeItem().Padding(CellPadding).Text(text =>
            {
                text.Span("Week Ending: ").Bold();
                text.Span(weekEnding);
            });
            if (string.IsNullOrEmpty(model.HstNumber)) return;
            row.RelativeItem().Padding(CellPadding).AlignRight().Text(text =>
            {
                text.Span("HTS #: ").Bold();
                text.Span(model.HstNumber);
            });
        });
    }

    private static void ComposeItems(IContainer container, InvoiceViewModel model)
    {
        var colors = model.InvoiceColor;
        var items = model.Items;
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(10);
                columns.RelativeColumn(54);
                columns.RelativeColumn(12);
                columns.RelativeColumn(12);
                columns.RelativeColumn(12);
            });
            table.Header(header =>
            {
                for (int i = 0; i < ItemHeaders.Length; i++)
                {
                    var corners = i == 0 ? PdfCorners.TopLeft : i == ItemHeaders.Length - 1 ? PdfCorners.TopRight : PdfCorners.None;
                    header.Cell().TableCell(colors.HeaderTableColor, corners).Text(ItemHeaders[i]).Bold().FontColor(Colors.White);
                }
            });
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool isLast = i == items.Count - 1;
                table.Cell().TableCell(colors.ItemsTableColor, isLast ? PdfCorners.BottomLeft : PdfCorners.None).Text($"{i + 1}");
                table.Cell().TableCell(colors.ItemsTableColor).Text(item.Description ?? string.Empty);
                table.Cell().TableCell(colors.ItemsTableColor).Text(item.Quantity.Equals(0) ? string.Empty : item.Quantity.ToString("F2", EnUs));
                table.Cell().TableCell(colors.ItemsTableColor).Text(item.UnitPrice != 0 ? item.UnitPrice.ToString("C", EnUs) : string.Empty);
                table.Cell().TableCell(colors.ItemsTableColor, isLast ? PdfCorners.BottomRight : PdfCorners.None).Text(item.Total.ToString("C", EnUs));
            }
        });
    }

    private static void ComposeNotesAndTotals(IContainer container, InvoiceViewModel model)
    {
        var colors = model.InvoiceColor;
        container.PreventPageBreak().Row(row =>
        {
            row.RelativeItem(60).PaddingTop(15 * Px).Column(notes =>
            {
                notes.Item().Height(20 * Px).Rounded(PdfCorners.Top).Background(colors.HeaderTableColor);
                notes.Item().MinHeight(100 * Px).Rounded(PdfCorners.Bottom).Background(colors.FooterContentColor).Padding(CellPadding).Column(body =>
                {
                    body.Item().Text("Notes / Comments :").Bold();
                    body.Item().Element(c => HtmlNotes.Compose(c, model.HtmlNotes));
                });
            });
            row.RelativeItem(10);
            row.RelativeItem(30).PaddingTop(10 * Px).AlignTop().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(35);
                    columns.RelativeColumn(65);
                });
                TotalRow(table, "Subtotal", model.SubTotal, colors.FooterContentColor, PdfCorners.Top);
                TotalRow(table, model.TaxName, model.Tax, colors.FooterContentColor, PdfCorners.None);
                TotalRow(table, "Total", model.Total, colors.FooterContentColor, PdfCorners.Bottom);
            });
        });
    }

    private static void TotalRow(TableDescriptor table, string label, decimal value, string background, PdfCorners corners)
    {
        table.Cell().Padding(5 * Px).AlignMiddle().AlignRight().Text(label ?? string.Empty).Bold();
        table.Cell().TableCell(background, corners, 5 * Px).Text(value.ToString("C", EnUs)).Bold();
    }

    private static string Ext(int? ext) => ext.GetValueOrDefault() > 0 ? $" Ext: {ext}" : string.Empty;
}
