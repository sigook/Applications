using Covenant.Infrastructure.Services;
using Xunit;

namespace Covenant.Tests.Common;

public class RichTextSanitizerTest
{
    private readonly RichTextSanitizer _sut = new();

    [Theory]
    [InlineData("<p>Hello</p><script>alert(1)</script>", "script")]
    [InlineData("<img src=x onerror=\"fetch('https://evil')\">", "onerror")]
    [InlineData("<a href=\"javascript:alert(1)\">link</a>", "javascript")]
    [InlineData("<p onclick=\"steal()\">text</p>", "onclick")]
    [InlineData("<iframe src=\"https://evil\"></iframe>", "iframe")]
    [InlineData("<svg onload=\"alert(1)\"></svg>", "onload")]
    public void Sanitize_RemovesActiveContent(string html, string forbidden)
    {
        var result = _sut.Sanitize(html);

        Assert.DoesNotContain(forbidden, result, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("<p><strong>Bold</strong> <em>italic</em> <u>underline</u></p>")]
    [InlineData("<ol><li>One</li><li class=\"ql-indent-1\">Two</li></ol>")]
    [InlineData("<p class=\"ql-align-center\">Centered</p>")]
    [InlineData("<h2>Title</h2><ul><li>Item</li></ul>")]
    [InlineData("<p><a href=\"https://sigook.com\" target=\"_blank\">Site</a></p>")]
    [InlineData("<p><a href=\"mailto:jobs@sigook.com\">Mail</a></p>")]
    public void Sanitize_KeepsEditorMarkup(string html)
    {
        Assert.Equal(html, _sut.Sanitize(html));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Sanitize_EmptyInput_ReturnsItUnchanged(string html)
    {
        Assert.Equal(html, _sut.Sanitize(html));
    }

    [Fact]
    public void Sanitize_PlainText_KeepsTheText()
    {
        Assert.Equal("Forklift operator, night shift", _sut.Sanitize("Forklift operator, night shift"));
    }
}
