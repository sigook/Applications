using Covenant.Common.Models.Agency;
using FluentValidation;

namespace Covenant.Api.Validators.Agencies;

public class UpdateUserAttendanceModelValidator : AbstractValidator<UpdateUserAttendanceModel>
{
    public UpdateUserAttendanceModelValidator()
    {
        RuleFor(m => m.ClockIn).NotEmpty();
        RuleFor(m => m.ClockOut)
            .GreaterThan(m => m.ClockIn)
            .WithMessage("Clock out must be after clock in.")
            .Must((m, clockOut) => clockOut.Value - m.ClockIn <= TimeSpan.FromHours(24))
            .WithMessage("A shift cannot be longer than 24 hours.")
            .When(m => m.ClockOut.HasValue);
        RuleFor(m => m.LunchMinutes)
            .InclusiveBetween(0, 240)
            .WithMessage("Lunch must be between 0 and 240 minutes.");
        RuleFor(m => m.LunchMinutes)
            .Must((m, lunch) => lunch <= (m.ClockOut.Value - m.ClockIn).TotalMinutes)
            .WithMessage("Lunch cannot be longer than the shift.")
            .When(m => m.ClockOut.HasValue && m.ClockOut > m.ClockIn);
        RuleFor(m => m.Reason).NotEmpty().MaximumLength(500);
    }
}
