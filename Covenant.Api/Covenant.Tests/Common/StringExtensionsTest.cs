using Covenant.Common.Utils.Extensions;
using Xunit;

namespace Covenant.Tests.Common;

public class StringExtensionsTest
{
    [Theory]
    [InlineData("Québec", "Quebec")]
    [InlineData("Montréal", "Montreal")]
    [InlineData("Toronto", "Toronto")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void RemoveDiacritics(string value, string expected) => Assert.Equal(expected, value.RemoveDiacritics());

    [Theory]
    [InlineData("123 Main St, Montréal QC", "Montreal", true)]
    [InlineData("123 Main St, Montreal QC", "Montréal", true)]
    [InlineData("123 MAIN ST, TORONTO ON", "toronto", true)]
    [InlineData("123 Main St, Toronto ON", "Ottawa", false)]
    [InlineData("", "Toronto", false)]
    [InlineData("123 Main St", "", false)]
    [InlineData(null, "Toronto", false)]
    [InlineData("123 Main St", null, false)]
    public void ContainsNormalized(string source, string value, bool expected) => Assert.Equal(expected, source.ContainsNormalized(value));
}
