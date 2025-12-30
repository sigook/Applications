namespace Covenant.Common.Utils.Extensions
{
    public static class BoolExtensions
    {
        public static string YesOrNo(this bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}
