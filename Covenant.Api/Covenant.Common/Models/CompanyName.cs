using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Models
{
    public class CompanyName
    {
        private const int MinimumLength = 2;
        private const int MaximumLength = 50;

        public string Name { get; }
        private CompanyName(string name) => Name = name;
        public static Result<CompanyName> Create(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Fail<CompanyName>(ValidationMessages.RequiredMsg(ApiResources.BusinessName));

            if (name.Length <= MinimumLength)
                return Result.Fail<CompanyName>(ValidationMessages.GreaterThan(ApiResources.BusinessName, MinimumLength));

            if (name.Length > MaximumLength)
                return Result.Fail<CompanyName>(ValidationMessages.LessThanOrEqualMsg(ApiResources.BusinessName, MaximumLength));
            return Result.Ok(new CompanyName(name));
        }
        public static implicit operator string(CompanyName name) => name.Name;
    }
}