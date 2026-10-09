using Covenant.Common.Models.Agency;
using FluentValidation;

namespace Covenant.Api.Validators.Agencies;

public class GetUserAttendanceReportFilterValidator : AbstractValidator<GetUserAttendanceReportFilter>
{
    private const int MaxDays = 366;

    public GetUserAttendanceReportFilterValidator()
    {
        RuleFor(m => m.From).NotEmpty();
        RuleFor(m => m.To)
            .NotEmpty()
            .GreaterThanOrEqualTo(m => m.From)
            .WithMessage("The end date must be on or after the start date.")
            .Must((m, to) => (to.Date - m.From.Date).TotalDays < MaxDays)
            .WithMessage("The date range cannot be longer than one year.");
    }
}
