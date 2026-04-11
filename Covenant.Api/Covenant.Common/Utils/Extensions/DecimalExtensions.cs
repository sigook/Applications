using System.Globalization;

namespace Covenant.Common.Utils.Extensions;

public static class DecimalExtensions
{
    public static decimal DefaultMoneyRound(this decimal value) => Math.Round(value, 2);
    public static decimal Add(this decimal d1, decimal d2) => decimal.Add(d1, d2);
    public static string ToUsMoney(this decimal value) => string.Format(new CultureInfo("en-US"), "{0:C}", value);
}
