using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Common.Utils.Extensions;

public static class RuleBuilderExtensions
{
    private const int PhoneExtMinimum = 0;
    private const int PhoneExtMaximum = 99999999;
    private const int DocumentDescriptionMaxLength = 100;

    public static IRuleBuilderOptions<T, string> DocumentDescription<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(DocumentDescriptionMaxLength).WithMessage($"Description must be at most {DocumentDescriptionMaxLength} characters")
            .Matches("^[-_ a-zA-Z0-9]+$").WithMessage("Description can only contain letters, numbers, spaces, - and _");

    public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(v => CommonValidators.IsValidPhoneNumber(v))
            .WithName(ApiResources.Phone)
            .WithMessage("'{PropertyName}' is not in the correct format.");
    }

    public static IRuleBuilderOptions<T, int?> PhoneExt<T>(this IRuleBuilder<T, int?> ruleBuilder) =>
        ruleBuilder.InclusiveBetween(PhoneExtMinimum, PhoneExtMaximum).WithName(ApiResources.PhoneExt);

    public static IRuleBuilderOptions<T, IEnumerable<TElement>> ListMustContainAtLeastOneElement<T, TElement>(this IRuleBuilder<T, IEnumerable<TElement>> ruleBuilder)
    {
        return ruleBuilder.Must((rootObject, list, context) =>
        {
            if (list is null) return false;
            return list.Any();
        }).WithMessage("{PropertyName} must contain at least one element");
    }
}
