using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.FullName).RequiredText(200);
        RuleFor(x => x.Email).RequiredEmail();
        RuleFor(x => x.Phone).RequiredPhone();
    }
}
