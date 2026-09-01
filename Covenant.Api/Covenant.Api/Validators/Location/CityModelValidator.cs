using Covenant.Common.Models.Location;
using FluentValidation;

namespace Covenant.Api.Common.Validators
{
    public class CityValidator : AbstractValidator<CityModel>
    {
        public CityValidator()
        {
            RuleFor(m => m.Id).NotEmpty();
        }
    }
}