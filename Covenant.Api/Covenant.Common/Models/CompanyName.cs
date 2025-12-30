using Covenant.Common.Constants;
using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Models
{
    public class CompanyName
    {
        public string Name { get; }
        private CompanyName(string name) => Name = name;
        public static Result<CompanyName> Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Fail<CompanyName>(ValidationMessages.RequiredMsg(ApiResources.BusinessName));

            const int min = CovenantConstants.Validation.BusinessNameMinimumLength;
            if (name.Length <= min)
                return Result.Fail<CompanyName>(ValidationMessages.GreaterThan(ApiResources.BusinessName, min));

            const int max = CovenantConstants.Validation.BusinessNameMaximumLength;
            if (name.Length > max)
                return Result.Fail<CompanyName>(ValidationMessages.LessThanOrEqualMsg(ApiResources.BusinessName, max));
            return Result.Ok(new CompanyName(name));
        }
        public static implicit operator string(CompanyName name) => name.Name;
    }
}