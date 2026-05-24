using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.EskiSifre).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.YeniSifre).StrongPassword()
            .NotEqual(x => x.EskiSifre).WithMessage(ValidationMessages.NewPasswordSameAsOld);
        RuleFor(x => x.YeniSifreTekrar)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Equal(x => x.YeniSifre).WithMessage(ValidationMessages.PasswordsDoNotMatch);
    }
}
