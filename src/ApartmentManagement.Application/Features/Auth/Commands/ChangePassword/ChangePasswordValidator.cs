using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator()
    {
        RuleFor(x => x.EskiSifre).NotEmpty();
        RuleFor(x => x.YeniSifre).NotEmpty().MinimumLength(6).MaximumLength(100)
            .NotEqual(x => x.EskiSifre).WithMessage("Yeni şifre eski şifre ile aynı olamaz.");
        RuleFor(x => x.YeniSifreTekrar).NotEmpty().Equal(x => x.YeniSifre)
            .WithMessage("Yeni şifreler eşleşmiyor.");
    }
}
