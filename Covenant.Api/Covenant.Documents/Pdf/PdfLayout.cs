using System.Globalization;
using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Covenant.Documents.Pdf;

[Flags]
public enum PdfCorners
{
    None = 0,
    TopLeft = 1,
    TopRight = 2,
    BottomLeft = 4,
    BottomRight = 8,
    Top = TopLeft | TopRight,
    Bottom = BottomLeft | BottomRight
}

public static class PdfLayout
{
    public const float Px = 0.61f;
    public const float FontPx = 0.59f;
    private const float CellGap = 0.75f;
    public const float CellPadding = 8 * Px;
    public const float CornerRadius = 14 * Px;
    public const string AccentColor = "#21B7FF";
    public static readonly CultureInfo EnUs = CreateEnUs();
    private const string FontFamily = "DejaVu Sans";

    static PdfLayout()
    {
        Settings.License = LicenseType.Community;
        Settings.UseSystemFonts = false;
        var assembly = typeof(PdfLayout).Assembly;
        foreach (string resource in assembly.GetManifestResourceNames().Where(r => r.EndsWith(".ttf")))
            FontManager.RegisterFontFromEmbeddedResource(assembly, resource);
    }

    private static CultureInfo CreateEnUs()
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-US").Clone();
        culture.NumberFormat.CurrencyNegativePattern = 1;
        return culture;
    }

    public static byte[] Generate(Action<ColumnDescriptor> composeContent) =>
        Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(10, Unit.Millimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(style => style.FontFamily(FontFamily).FontSize(16 * FontPx).FontColor(Colors.Black));
            page.Content().Padding(13 * Px).Column(composeContent);
        })).GeneratePdf();

    public static IContainer Rounded(this IContainer container, PdfCorners corners)
    {
        if (corners.HasFlag(PdfCorners.TopLeft)) container = container.CornerRadiusTopLeft(CornerRadius);
        if (corners.HasFlag(PdfCorners.TopRight)) container = container.CornerRadiusTopRight(CornerRadius);
        if (corners.HasFlag(PdfCorners.BottomLeft)) container = container.CornerRadiusBottomLeft(CornerRadius);
        if (corners.HasFlag(PdfCorners.BottomRight)) container = container.CornerRadiusBottomRight(CornerRadius);
        return container;
    }

    public static IContainer TableCell(this IContainer container, Color background, PdfCorners corners = PdfCorners.None, float padding = CellPadding) =>
        container.Padding(CellGap).Rounded(corners).Background(background).Padding(padding).AlignMiddle();

    public static void EmptyTableCell(this IContainer container, Color background, PdfCorners corners = PdfCorners.None) =>
        container.Padding(CellGap).Rounded(corners).Background(background).Height(2 * CellPadding);

    public static void PoweredBy(this IContainer container) =>
        container.PaddingTop(20 * Px).Text("Powered by SIGOOK™ .").FontSize(14 * FontPx).Italic();
}
