using Covenant.Common.Constants;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Common.Utils.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(v => CommonValidators.IsValidPhoneNumber(v))
                .WithName(ApiResources.Phone)
                .WithMessage("'{PropertyName}' is not in the correct format.");
        }

        public static IRuleBuilderOptions<T, int?> PhoneExt<T>(this IRuleBuilder<T, int?> ruleBuilder) =>
            ruleBuilder.InclusiveBetween(CovenantConstants.Validation.PhoneExtMinimum, CovenantConstants.Validation.PhoneExtMaximum).WithName(ApiResources.PhoneExt);

        public static IRuleBuilderOptions<T, IEnumerable<TElement>> ListMustContainAtLeastOneElement<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder)
        {
            return ruleBuilder.Must((rootObject, list, context) =>
            {
                if (list is null) return false;
                return list.Any();
            }).WithMessage("{PropertyName} must contain at least one element");
        }
    }
}
