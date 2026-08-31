using Covenant.Common.Utils.Extensions;
using Xunit;

namespace Covenant.Tests.Common;

public class StringExtensionsTest
{
    [Theory]
    [InlineData("Toronto", "toronto")]
    [InlineData("MONTRÉAL", "montreal")]
    [InlineData("Québec City", "quebec city")]
    [InlineData("123 Main St.", "123 main st.")]
    public void NormalizeForComparisonLowercasesAndStripsDiacritics(string value, string expected)
    {
        Assert.Equal(expected, value.NormalizeForComparison());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeForComparisonReturnsEmptyForNullOrWhiteSpace(string value)
    {
        Assert.Equal(string.Empty, value.NormalizeForComparison());
    }
}
