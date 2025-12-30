using Covenant.Common.Functionals;
using System.Text.RegularExpressions;

namespace Covenant.Common.Entities
{
    public class CvnPostalCode
    {
        private CvnPostalCode(string postalCode) => PostalCode = postalCode;
        public string PostalCode { get; }
        public static Result<CvnPostalCode> Create(string postalCode)
        {
            const string postalCodeDoesNotHaveTheCorrectFormat = "Postal code does not have the correct format";
            if (string.IsNullOrEmpty(postalCode)) return Result.Fail<CvnPostalCode>(postalCodeDoesNotHaveTheCorrectFormat);
            postalCode = postalCode.Replace(" ", string.Empty).ToUpper();
            var isValidPostalCode = Regex.IsMatch(postalCode, "^[ABCEGHJKLMNPRSTVXY]{1}\\d{1}[A-Z]{1} *\\d{1}[A-Z]{1}\\d{1}$");
            isValidPostalCode |= Regex.IsMatch(postalCode, "^[0-9]{5}(?:-[0-9]{4})?$");
            if (isValidPostalCode)
            {
                return Result.Ok(new CvnPostalCode(postalCode));
            }
            return Result.Fail<CvnPostalCode>(postalCodeDoesNotHaveTheCorrectFormat);
        }
    }
}
