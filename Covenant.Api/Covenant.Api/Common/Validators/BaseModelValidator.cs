using Covenant.Common.Models;
using FluentValidation;

namespace Covenant.Api.Common.Validators
{
    public class BaseModelValidator : AbstractValidator<BaseModel<Guid>>
    {
        public BaseModelValidator()
        {
            RuleFor(m => m.Id).NotEmpty();
        }
    }
}