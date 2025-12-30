using Covenant.Common.Utils.Extensions;
using Xunit;

namespace Covenant.Tests.Common
{
    public class ExtensionsTest
    {
        [Fact]
        public void LettersAndDigits()
        {
            string input = "`~1!2@3#4$5%6^7&*8(90)-_=+{[]}\\|:;'\",<.>/?qwertyuiopasdfghjklzxcvbnmñ ń";
            string lettersAndDigits = input.ToLettersAndDigits();
            Assert.Equal("1234567890qwertyuiopasdfghjklzxcvbnmñ ń", lettersAndDigits);
        }
    }
}