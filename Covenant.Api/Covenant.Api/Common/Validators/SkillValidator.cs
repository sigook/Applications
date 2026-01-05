using Covenant.Common.Models;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Common.Validators
{
    public class SkillValidator : AbstractValidator<SkillModel>
    {
        public SkillValidator()
        {
            RuleFor(m => m.Skill).NotEmpty().WithName(ApiResources.Skills);
        }
    }
}