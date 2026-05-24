using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.RegisterAdmin;

public class RegisterAdminValidator : AbstractValidator<RegisterAdminCommand>
{
    public RegisterAdminValidator()
    {
        RuleFor(x => x.ApartmanAdi).RequiredText(200);
        RuleFor(x => x.ApartmanKisaAd).RequiredText(50)
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Kısa ad sadece harf, rakam, alt çizgi ve tire içerebilir.");
        RuleFor(x => x.ContactEmail).RequiredEmail();
        RuleFor(x => x.ContactPhone).OptionalPhone();
        RuleFor(x => x.AdminAdSoyad).RequiredText(200);
        RuleFor(x => x.AdminEmail).RequiredEmail();
        RuleFor(x => x.AdminTelefon).RequiredPhone();
        RuleFor(x => x.Sifre).StrongPassword();
        RuleFor(x => x.SifreTekrar)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Equal(x => x.Sifre).WithMessage(ValidationMessages.PasswordsDoNotMatch);
    }
}
