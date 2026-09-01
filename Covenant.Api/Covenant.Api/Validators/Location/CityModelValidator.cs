using Covenant.Common.Models.Location;
using FluentValidation;

namespace Covenant.Api.Validators.Location;

public class CityModelValidator : AbstractValidator<CityModel>
{
    public CityModelValidator()
    {
        RuleFor(m => m.Id).NotEmpty();
    }
}
