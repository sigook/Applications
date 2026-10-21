using System.Net;
using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using static Covenant.Documents.Pdf.PdfLayout;

namespace Covenant.Documents.Pdf;

public static partial class HtmlNotes
{
    private const float ParagraphMargin = 16 * Px;
    private const string LineBreak = "\n";

    private readonly record struct SpanStyle(bool Bold, bool Italic, bool Underline, string Color);

    private sealed record NoteSpan(string Text, SpanStyle Style);

    [GeneratedRegex(@"<(/?)([a-zA-Z0-9]+)([^>]*)>|([^<]+)")]
    private static partial Regex Tokens();

    [GeneratedRegex(@"color:\s*(?:rgb\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)|(#[0-9a-fA-F]{3,6}))", RegexOptions.IgnoreCase)]
    private static partial Regex ColorStyle();

    [GeneratedRegex(@"[ \t\r\n\f]+")]
    private static partial Regex Whitespace();

    public static void Compose(IContainer container, string html)
    {
        var paragraphs = Parse(html);
        container.Column(column =>
        {
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var item = column.Item().PaddingTop(ParagraphMargin);
                if (i == paragraphs.Count - 1) item = item.PaddingBottom(ParagraphMargin);
                var spans = paragraphs[i];
                item.Text(text =>
                {
                    if (spans.Count == 0)
                    {
                        text.Span(" ");
                        return;
                    }
                    foreach (var span in spans)
                    {
                        var descriptor = text.Span(span.Text);
                        if (span.Style.Bold) descriptor.Bold();
                        if (span.Style.Italic) descriptor.Italic();
                        if (span.Style.Underline) descriptor.Underline();
                        if (span.Style.Color is not null) descriptor.FontColor(span.Style.Color);
                    }
                });
            }
        });
    }

    private static List<List<NoteSpan>> Parse(string html)
    {
        var paragraphs = new List<List<NoteSpan>>();
        if (string.IsNullOrWhiteSpace(html)) return paragraphs;

        var openTags = new Stack<(string Tag, SpanStyle Previous)>();
        var style = new SpanStyle(false, false, false, null);
        List<NoteSpan> current = null;

        foreach (Match token in Tokens().Matches(html))
        {
            if (token.Groups[4].Success)
            {
                string text = WebUtility.HtmlDecode(Whitespace().Replace(token.Value, " ")).Replace("﻿", string.Empty).Replace("​", string.Empty);
                bool startsLine = current is null || current.Count == 0 || current[^1].Text.EndsWith(LineBreak);
                if (startsLine) text = text.TrimStart(' ');
                if (text.Length == 0) continue;
                current ??= NewParagraph(paragraphs);
                current.Add(new NoteSpan(text, style));
                continue;
            }

            string tag = token.Groups[2].Value.ToLowerInvariant();
            bool closing = token.Groups[1].Length > 0;
            switch (tag)
            {
                case "p" or "div" or "li":
                    if (current is not null) TrimTrailingLineBreak(current);
                    current = closing ? null : NewParagraph(paragraphs);
                    break;
                case "br":
                    current ??= NewParagraph(paragraphs);
                    current.Add(new NoteSpan(LineBreak, style));
                    break;
                default:
                    if (closing)
                    {
                        while (openTags.Count > 0)
                        {
                            var (openTag, previous) = openTags.Pop();
                            style = previous;
                            if (openTag == tag) break;
                        }
                    }
                    else
                    {
                        openTags.Push((tag, style));
                        style = Apply(style, tag, token.Groups[3].Value);
                    }
                    break;
            }
        }

        if (current is not null) TrimTrailingLineBreak(current);
        return paragraphs;
    }

    private static List<NoteSpan> NewParagraph(List<List<NoteSpan>> paragraphs)
    {
        var paragraph = new List<NoteSpan>();
        paragraphs.Add(paragraph);
        return paragraph;
    }

    private static void TrimTrailingLineBreak(List<NoteSpan> paragraph)
    {
        if (paragraph.Count > 0 && paragraph[^1].Text == LineBreak) paragraph.RemoveAt(paragraph.Count - 1);
    }

    private static SpanStyle Apply(SpanStyle style, string tag, string attributes)
    {
        style = tag switch
        {
            "strong" or "b" => style with { Bold = true },
            "em" or "i" => style with { Italic = true },
            "u" or "a" => style with { Underline = true },
            _ => style
        };
        var color = ColorStyle().Match(attributes);
        if (!color.Success) return style;
        if (color.Groups[4].Success) return style with { Color = color.Groups[4].Value };
        return style with { Color = $"#{Channel(color.Groups[1])}{Channel(color.Groups[2])}{Channel(color.Groups[3])}" };
    }

    private static string Channel(Group group) => Math.Min(int.Parse(group.Value), 255).ToString("X2");
}
