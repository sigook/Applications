using Covenant.Common.Models.Location;
using Covenant.Common.Models;
using FluentValidation;

namespace Covenant.Api.Validators.Locations;

public class LocationTaxModelValidator : AbstractValidator<LocationTaxModel>
{
    public LocationTaxModelValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.Tax1)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100);
    }
}
