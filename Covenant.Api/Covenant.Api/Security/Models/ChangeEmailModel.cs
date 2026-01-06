using FluentValidation;

namespace Covenant.Api.Security.Models
{
    public class ChangeEmailModel
    {
        public string NewEmail { get; set; }
        public string ConfirmNewEmail { get; set; }
    }

    public class ChangeEmailValidator : AbstractValidator<ChangeEmailModel>
    {
        public ChangeEmailValidator()
        {
            RuleFor(m => m.NewEmail).NotEmpty().EmailAddress();
            RuleFor(m => m.ConfirmNewEmail).Equal(m => m.NewEmail);
        }
    }
}