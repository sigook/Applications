using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Xunit;

namespace Covenant.Tests.Common
{
    public class CvnPostalCodeTest
    {
        [Theory]
        [InlineData("M4P1M7")]
        [InlineData("M4P 1M7")]
        [InlineData("M4P 1M7 ")]
        [InlineData(" M4P 1M7")]
        [InlineData(" M4P 1M7 ")]
        [InlineData("m4p1m7")]
        public void Create_ValidCanada(string postalCode)
        {
            Result<CvnPostalCode> result = CvnPostalCode.Create(postalCode);
            Assert.True(result);
        }

        [Theory]
        [InlineData("44P1M7")]
        [InlineData("MMP1M7")]
        [InlineData("MMPAMA")]
        [InlineData("M4P-1M7")]
        [InlineData("444117")]
        public void Create_InValidCanada(string postalCode)
        {
            Result<CvnPostalCode> result = CvnPostalCode.Create(postalCode);
            Assert.False(result);
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("12345-6789")]
        public void Create_ValidUSA(string postalCode)
        {
            Result<CvnPostalCode> result = CvnPostalCode.Create(postalCode);
            Assert.True(result);
        }

        [Theory]
        [InlineData("1234")]
        [InlineData("123456")]
        [InlineData("123456789")]
        [InlineData("1234-56789")]
        public void Create_InValidUSA(string postalCode)
        {
            Result<CvnPostalCode> result = CvnPostalCode.Create(postalCode);
            Assert.False(result);
        }
    }
}