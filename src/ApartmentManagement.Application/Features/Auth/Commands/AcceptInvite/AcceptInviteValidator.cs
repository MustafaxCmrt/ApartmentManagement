using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.AcceptInvite;

public class AcceptInviteValidator : AbstractValidator<AcceptInviteCommand>
{
    public AcceptInviteValidator()
    {
        RuleFor(x => x.InviteToken).NotEmpty();
        RuleFor(x => x.Sifre).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.SifreTekrar).NotEmpty().Equal(x => x.Sifre).WithMessage("Şifreler eşleşmiyor.");
    }
}
