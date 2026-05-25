using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email).RequiredEmail();
    }
}
