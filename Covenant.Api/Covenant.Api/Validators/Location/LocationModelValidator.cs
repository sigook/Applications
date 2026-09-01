using Covenant.Api.Common.Models;
using Covenant.Common.Entities;
using Covenant.Common.Models.Location;
using Covenant.Common.Resources;
using FluentValidation;

namespace Covenant.Api.Common.Validators
{
    public class LocationValidator : AbstractValidator<LocationModel>
    {
        public const int MinimumLengthAddress = 5;
        public const int MaximumLengthAddress = 100;

        public LocationValidator()
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
                .SetValidator(new CityValidator())
                .WithName(ApiResources.City);
        }
    }
}