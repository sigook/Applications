using System.ComponentModel;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using UglyToad.PdfPig;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static class DocumentTools
{
    [McpServerTool(Name = "read_document"), Description("Extracts the text of a local resume or document (pdf, docx, txt, md) so you can read it: skills, certifications, address, experience. Scanned PDFs have no text layer and return empty text. Use the paths returned by read_export.")]
    public static object ReadDocument(
        [Description("Local file path.")] string path,
        [Description("Maximum characters to return. Default 20000.")] int maxChars = 20000)
    {
        if (!File.Exists(path)) throw new McpException($"File not found: {path}");
        var extension = Path.GetExtension(path).ToLowerInvariant();
        var (text, pages) = extension switch
        {
            ".pdf" => ReadPdf(path),
            ".docx" => (ReadDocx(path), 1),
            ".txt" or ".md" or ".csv" => (File.ReadAllText(path), 1),
            _ => throw new McpException($"Unsupported format {extension}. Supported: pdf, docx, txt, md.")
        };
        var normalized = Normalize(text);
        var truncated = normalized.Length > maxChars;
        return new
        {
            path,
            format = extension.TrimStart('.'),
            pages,
            chars = normalized.Length,
            truncated,
            hasText = normalized.Length > 0,
            text = truncated ? normalized[..maxChars] : normalized
        };
    }

    private static (string Text, int Pages) ReadPdf(string path)
    {
        using var document = PdfDocument.Open(path);
        var builder = new StringBuilder();
        var pages = 0;
        foreach (var page in document.GetPages())
        {
            pages++;
            builder.AppendLine(page.Text);
            builder.AppendLine();
        }
        return (builder.ToString(), pages);
    }

    private static string ReadDocx(string path)
    {
        using var document = WordprocessingDocument.Open(path, false);
        var body = document.MainDocumentPart?.Document?.Body;
        if (body is null) return string.Empty;
        var builder = new StringBuilder();
        foreach (var paragraph in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
        {
            builder.AppendLine(paragraph.InnerText);
        }
        return builder.ToString();
    }

    private static string Normalize(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n')
            .Select(l => string.Join(' ', l.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
            .ToList();
        var builder = new StringBuilder();
        var blankRun = 0;
        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                if (++blankRun > 1) continue;
            }
            else blankRun = 0;
            builder.Append(line).Append('\n');
        }
        return builder.ToString().Trim();
    }
}
