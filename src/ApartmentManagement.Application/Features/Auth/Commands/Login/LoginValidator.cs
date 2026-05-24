using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Telefon).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Sifre).NotEmpty().MinimumLength(6);
    }
}
