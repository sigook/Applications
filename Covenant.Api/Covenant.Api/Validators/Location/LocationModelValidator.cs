using Covenant.Common.Entities;
using Covenant.Common.Models.Location;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Validators.Location;

public class LocationModelValidator : AbstractValidator<LocationModel>
{
    public const int MinimumLengthAddress = 5;
    public const int MaximumLengthAddress = 100;

    public LocationModelValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(m => m.Address)
            .NotEmpty()
            .Length(MinimumLengthAddress, MaximumLengthAddress)
            .Matches("^[-.# a-zA-Z0-9]+$")
            .WithName(ApiResources.Address);

        RuleFor(m => m.PostalCode)
            .NotEmpty()
            .Must(postalCode => CvnPostalCode.Create(postalCode))
            .WithName(ApiResources.PostalCode);

        RuleFor(m => m.City)
            .NotNull()
            .SetValidator(new CityModelValidator())
            .WithName(ApiResources.City);
    }
}
