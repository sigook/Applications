using Covenant.Common.Constants;
using PhoneNumbers;
using System.Text.RegularExpressions;

namespace Covenant.Common.Utils
{
    public static class CommonValidators
    {
        public static bool IsValidPhoneNumber(string phoneNumber, bool nullIsValid = true)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return nullIsValid;
            var match = Regex.Match(phoneNumber, CovenantConstants.Validation.PhonePattern);
            if (!(match.Success && match.Value == phoneNumber)) return false;
            try
            {
                var util = PhoneNumberUtil.GetInstance();
                PhoneNumber rPhoneNumber = util.Parse(phoneNumber, CovenantConstants.DefaultRegion);
                return util.IsPossibleNumber(rPhoneNumber);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumberByRegion(string phoneNumber, string region = CovenantConstants.DefaultRegion)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;
            var match = Regex.Match(phoneNumber, CovenantConstants.Validation.PhonePattern);
            if (!(match.Success && match.Value == phoneNumber)) return false;
            try
            {
                var util = PhoneNumberUtil.GetInstance();
                var phoneResult = util.ParseAndKeepRawInput(phoneNumber, region);
                return util.IsValidNumberForRegion(phoneResult, region);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
