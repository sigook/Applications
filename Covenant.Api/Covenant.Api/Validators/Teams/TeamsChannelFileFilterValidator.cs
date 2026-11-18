using Covenant.Common.Models.Teams;
using FluentValidation;

namespace Covenant.Api.Validators.Teams;

public class TeamsChannelFileFilterValidator : AbstractValidator<TeamsChannelFileFilter>
{
    private static readonly char[] InvalidSiteCharacters = ['/', '\\', ':', '?', '#', '%'];

    public TeamsChannelFileFilterValidator()
    {
        RuleFor(m => m.Team).NotEmpty().Must(BeASiteSegment).WithMessage("Team contains invalid characters");
        RuleFor(m => m.Channel).NotEmpty().Must(BeASiteSegment).WithMessage("Channel contains invalid characters");
        RuleFor(m => m.Name).NotEmpty();
    }

    private static bool BeASiteSegment(string value) => value is null || value.IndexOfAny(InvalidSiteCharacters) < 0;
}
