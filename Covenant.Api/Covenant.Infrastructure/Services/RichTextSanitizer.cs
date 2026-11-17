using Covenant.Common.Interfaces;
using Ganss.Xss;

namespace Covenant.Infrastructure.Services;

public class RichTextSanitizer : IRichTextSanitizer
{
    private static readonly HtmlSanitizer Sanitizer = CreateSanitizer();

    public string Sanitize(string html) =>
        string.IsNullOrEmpty(html) ? html : Sanitizer.Sanitize(html);

    private static HtmlSanitizer CreateSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedSchemes.Add("mailto");
        sanitizer.AllowedSchemes.Add("tel");
        return sanitizer;
    }
}
