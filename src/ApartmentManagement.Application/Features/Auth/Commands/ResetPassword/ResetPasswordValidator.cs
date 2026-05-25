using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Token).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.NewPassword).StrongPassword();
        RuleFor(x => x.NewPasswordConfirm)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Equal(x => x.NewPassword).WithMessage(ValidationMessages.PasswordsDoNotMatch);
    }
}
