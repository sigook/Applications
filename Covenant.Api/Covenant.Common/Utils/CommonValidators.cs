using PhoneNumbers;
using System.Text.RegularExpressions;

namespace Covenant.Common.Utils;

public static class CommonValidators
{
    private const string PhonePattern = @"\s*(?:\+?(\d{1,3}))?([-. (]*(\d{3})[-. )]*)?((\d{3})[-. ]*(\d{2,4})(?:[-.x ]*(\d+))?)\s*";
    private const string CanadaCode = "CA";

    public static bool IsValidPhoneNumber(string phoneNumber, bool nullIsValid = true)
    {
        if (string.IsNullOrEmpty(phoneNumber)) return nullIsValid;
        var match = Regex.Match(phoneNumber, PhonePattern);
        if (!(match.Success && match.Value == phoneNumber)) return false;
        try
        {
            var util = PhoneNumberUtil.GetInstance();
            PhoneNumber rPhoneNumber = util.Parse(phoneNumber, CanadaCode);
            return util.IsPossibleNumber(rPhoneNumber);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
