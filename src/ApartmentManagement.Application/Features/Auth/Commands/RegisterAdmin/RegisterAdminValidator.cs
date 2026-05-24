using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.RegisterAdmin;

public class RegisterAdminValidator : AbstractValidator<RegisterAdminCommand>
{
    public RegisterAdminValidator()
    {
        RuleFor(x => x.ApartmanAdi).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ApartmanKisaAd).NotEmpty().MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Kısa ad sadece harf, rakam, alt çizgi ve tire içerebilir.");
        RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.ContactPhone).MaximumLength(30);
        RuleFor(x => x.AdminAdSoyad).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AdminEmail).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.AdminTelefon).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Sifre).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.SifreTekrar).NotEmpty().Equal(x => x.Sifre).WithMessage("Şifreler eşleşmiyor.");
    }
}
