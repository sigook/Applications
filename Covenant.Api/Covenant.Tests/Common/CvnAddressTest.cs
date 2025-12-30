using Covenant.Common.Entities;
using Covenant.Common.Functionals;
using Xunit;

namespace Covenant.Tests.Common
{
    public class CvnAddressTest
    {
        [Theory]
        [InlineData("4917 Dundas Ave.", true)]
        [InlineData("4917 Dundas Ave #1234", true)]
        [InlineData("220 Woolner - avenue Apt# 906", true)]
        [InlineData("1608-25 San Romanoway ", true)]
        [InlineData("1-Fountainhead road", true)]
        [InlineData("316- 37 Eastbourne Dr. ", true)]
        [InlineData("6-35 PROVENCE TRAIL", true)]
        [InlineData("30 NORMANDY PLACE, APARTMENT 317", true)]
        [InlineData("30 NORMANDY PLACE? APARTMENT 317", false)]
        [InlineData("++ NORMANDY PLACE? APARTMENT 317", false)]
        [InlineData("@ NORMANDY PLACE? APARTMENT 317", false)]
        [InlineData("! NORMANDY PLACE? APARTMENT 317", false)]
        [InlineData("% NORMANDY PLACE? APARTMENT 317", false)]
        [InlineData("$ NORMANDY PLACE? APARTMENT 317", false)]
        public void Create(string address, bool valid)
        {
            Result<CvnAddress> result = CvnAddress.Create(address);
            if (valid)
            {
                Assert.True(result);
            }
            else
            {
                Assert.False(result);
            }
        }
    }
}