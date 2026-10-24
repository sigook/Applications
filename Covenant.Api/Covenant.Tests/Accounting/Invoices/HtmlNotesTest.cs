using Covenant.Documents.Pdf;
using QuestPDF.Fluent;
using UglyToad.PdfPig;
using Xunit;

namespace Covenant.Tests.Accounting.Invoices;

public class HtmlNotesTest
{
    private static string Render(string html)
    {
        byte[] pdf = PdfLayout.Generate(column => column.Item().Element(c => HtmlNotes.Compose(c, html)));
        using var document = PdfDocument.Open(pdf);
        return string.Join(" ", document.GetPages().Select(p => p.Text));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Compose_EmptyHtml_RendersNothing(string html)
    {
        Assert.Equal(string.Empty, Render(html).Trim());
    }

    [Fact]
    public void Compose_FormattedParagraphs_RendersTextWithoutTags()
    {
        const string html = "<p><strong>Payment</strong> due in <em>30</em> days</p>" +
                            "<div><u>Bank</u>: <a href=\"#\">Covenant</a> &amp; Co</div>" +
                            "<ul><li><span style=\"color: rgb(300, 0, 10)\">Red</span></li><li><span style=\"color:#00ff00\">Green</span></li></ul>" +
                            "<p></p>" +
                            "<p>Line one<br>Line two<br></p>" +
                            "loose <b>bold <i>italic</b> text";

        string text = Render(html);

        Assert.Contains("Payment", text);
        Assert.Contains("30", text);
        Assert.Contains("Covenant & Co", text);
        Assert.Contains("Red", text);
        Assert.Contains("Green", text);
        Assert.Contains("Line one", text);
        Assert.Contains("Line two", text);
        Assert.Contains("italic", text);
        Assert.DoesNotContain("<", text);
        Assert.DoesNotContain("&amp;", text);
    }

    [Fact]
    public void Compose_LineBreakWithoutParagraph_StartsParagraph()
    {
        string text = Render("<br>after break");

        Assert.Contains("after break", text);
    }
}
