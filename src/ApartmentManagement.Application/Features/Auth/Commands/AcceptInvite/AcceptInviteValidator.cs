using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.AcceptInvite;

public class AcceptInviteValidator : AbstractValidator<AcceptInviteCommand>
{
    public AcceptInviteValidator()
    {
        RuleFor(x => x.InviteToken).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Sifre).StrongPassword();
        RuleFor(x => x.SifreTekrar)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Equal(x => x.Sifre).WithMessage(ValidationMessages.PasswordsDoNotMatch);
    }
}
