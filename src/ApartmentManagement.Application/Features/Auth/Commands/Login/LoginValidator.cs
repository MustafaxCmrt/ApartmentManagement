using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Sifre).NotEmpty().MinimumLength(6);
    }
}
