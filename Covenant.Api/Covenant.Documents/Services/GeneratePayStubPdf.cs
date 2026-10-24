using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models.Accounting.PayStubs;
using Covenant.Common.Utils.Extensions;
using Covenant.Documents.Pdf;
using MediatR;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using static Covenant.Documents.Pdf.PdfLayout;

namespace Covenant.Documents.Services;

public class GeneratePayStubPdf(PayrollViewModel model) : IRequest<byte[]>
{
    public PayrollViewModel Model { get; } = model;
}

public class GeneratePayStubPdfHandler(IFilesContainer filesContainer) : IRequestHandler<GeneratePayStubPdf, byte[]>
{
    private const string HeaderColor = "#577533";
    private const string ItemsColor = "#CFE2AA";
    private const string YtdHeaderColor = "#93B571";
    private const string YtdItemsColor = "#EDF4E1";
    private const string YtdTotalColor = "#D6E8C4";
    private const string SummaryTotalColor = "#A4CB7C";
    private static readonly string[] ItemHeaders = ["Item no.", "Description", "Qty", "Unit price", "Total"];

    public async Task<byte[]> Handle(GeneratePayStubPdf request, CancellationToken cancellationToken)
    {
        var model = request.Model;
        var logo = await PdfLogo.Load(filesContainer, model.AgencyLogoFileName);
        return Generate(column =>
        {
            if (logo is not null) column.Item().Element(c => logo.Compose(c, 200 * Px));
            column.Item().Element(c => ComposeHeader(c, model));
            column.Item().Element(c => ComposeItems(c, model));
            column.Item().PaddingTop(16 * Px).Element(c => ComposeSummary(c, model));
            column.Item().PoweredBy();
        });
    }

    private static void ComposeHeader(IContainer container, PayrollViewModel model)
    {
        container.Row(row =>
        {
            row.RelativeItem(73).PaddingVertical(10 * Px).Padding(15 * Px).Column(info =>
            {
                info.Item().Text(model.AgencyFullName ?? string.Empty).Bold();
                info.Item().Text(model.AgencyLocation ?? string.Empty);
                info.Item().Text($"{model.AgencyPhone}{(model.AgencyPhoneExt.HasValue ? $" Ext: {model.AgencyPhoneExt}" : string.Empty)}");
                info.Item().Text(" ");
                info.Item().PaddingTop(15 * Px).Column(fields =>
                {
                    Field(fields, "Employee Full Name:", model.WorkerFullName);
                    Field(fields, "Position:", model.Position);
                    Field(fields, "Employee ID:", model.EmployeeId.ToString());
                    if (!string.IsNullOrEmpty(model.MaskedSin)) Field(fields, "SIN:", model.MaskedSin);
                    Field(fields, "Dates of work:", $"{model.StartDate.ToString("D", CultureInfo.InvariantCulture)} - {model.EndDate.ToString("D", CultureInfo.InvariantCulture)}");
                    if (model.PaymentDate != default) Field(fields, "Payment date:", model.PaymentDate.ToString("D", CultureInfo.InvariantCulture));
                    Field(fields, "Email:", model.WorkerEmail);
                });
            });
            row.RelativeItem(27).PaddingTop(20 * Px).AlignTop().Border(2 * Px).BorderColor(AccentColor).Padding(8 * Px).Column(box =>
            {
                box.Item().PaddingTop(7 * Px).AlignCenter().Text(model.DocumentName?.ToUpper() ?? string.Empty).Bold().FontColor(AccentColor);
                box.Item().PaddingTop(16 * Px).Text(text =>
                {
                    text.Span($"{model.DocumentName} # ").Bold();
                    text.Span(model.PayrollNumber ?? string.Empty);
                });
                box.Item().PaddingVertical(16 * Px).Text(text =>
                {
                    text.Span("Date: ").Bold();
                    text.Span(model.CreatedAt.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture));
                });
            });
        });
    }

    private static void Field(ColumnDescriptor fields, string label, string value) =>
        fields.Item().PaddingBottom(3 * Px).Text(text =>
        {
            text.Span($"{label} ").Bold();
            text.Span(value ?? string.Empty);
        });

    private static void ComposeItems(IContainer container, PayrollViewModel model)
    {
        var items = model.Table1Items.ToList();
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(10);
                columns.RelativeColumn(30);
                columns.RelativeColumn(10);
                columns.RelativeColumn(25);
                columns.RelativeColumn(25);
            });
            table.Header(header =>
            {
                for (int i = 0; i < ItemHeaders.Length; i++)
                {
                    var corners = i == 0 ? PdfCorners.TopLeft : i == ItemHeaders.Length - 1 ? PdfCorners.TopRight : PdfCorners.None;
                    header.Cell().TableCell(HeaderColor, corners).AlignCenter().Text(ItemHeaders[i]).Bold().FontColor(Colors.White);
                }
            });
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                bool isLast = i == items.Count - 1;
                table.Cell().TableCell(ItemsColor, isLast ? PdfCorners.BottomLeft : PdfCorners.None).AlignRight().Text($"{i + 1}");
                table.Cell().TableCell(ItemsColor).Text(item.Description ?? string.Empty);
                table.Cell().TableCell(ItemsColor).AlignRight().Text(item.Quantity.ToString("F2", EnUs));
                table.Cell().TableCell(ItemsColor).AlignRight().Text(item.UnitPrice.ToCaMoney());
                table.Cell().TableCell(ItemsColor, isLast ? PdfCorners.BottomRight : PdfCorners.None).AlignRight().Text(item.Total.ToCaMoney());
            }
        });
    }

    private static void ComposeSummary(IContainer container, PayrollViewModel model)
    {
        var ytdItems = model.YtdItems.ToList();
        bool hasYtd = ytdItems.Count > 0;
        container.Row(row =>
        {
            if (hasYtd) row.RelativeItem(48).AlignTop().Element(c => ComposeYtd(c, ytdItems));
            row.RelativeItem(hasYtd ? 4 : 50);
            row.RelativeItem(hasYtd ? 48 : 50).AlignTop().Element(c => ComposeTotals(c, model.Table2Items.ToList()));
        });
    }

    private static void ComposeYtd(IContainer container, List<PayrollTable2Item> items)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
            });
            table.Cell().ColumnSpan(2).TableCell(YtdHeaderColor, PdfCorners.Top).AlignCenter().Text("Year To Date").Bold().FontColor(Colors.White);
            for (int i = 0; i < items.Count; i++)
            {
                bool isLast = i == items.Count - 1;
                SummaryRow(table, items[i], isLast ? YtdTotalColor : YtdItemsColor, isLast ? PdfCorners.BottomLeft : PdfCorners.None, isLast ? PdfCorners.BottomRight : PdfCorners.None);
            }
        });
    }

    private static void ComposeTotals(IContainer container, List<PayrollTable2Item> items)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(55);
                columns.RelativeColumn(45);
            });
            for (int i = 0; i < items.Count; i++)
            {
                bool isLast = i == items.Count - 1;
                var (left, right) = i == 0 ? (PdfCorners.TopLeft, PdfCorners.TopRight)
                    : isLast ? (PdfCorners.BottomLeft, PdfCorners.BottomRight)
                    : (PdfCorners.None, PdfCorners.None);
                SummaryRow(table, items[i], isLast ? SummaryTotalColor : ItemsColor, left, right);
            }
        });
    }

    private static void SummaryRow(TableDescriptor table, PayrollTable2Item item, string background, PdfCorners left, PdfCorners right)
    {
        if (string.IsNullOrEmpty(item.Label) && string.IsNullOrEmpty(item.Value))
        {
            table.Cell().EmptyTableCell(background, left);
            table.Cell().EmptyTableCell(background, right);
            return;
        }
        table.Cell().TableCell(background, left).Text(item.Label ?? string.Empty).Bold();
        table.Cell().TableCell(background, right).AlignRight().Text(item.Value ?? string.Empty);
    }
}
