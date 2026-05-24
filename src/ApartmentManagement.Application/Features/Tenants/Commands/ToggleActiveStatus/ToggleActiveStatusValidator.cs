using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Tenants.Commands.ToggleActiveStatus;

public class ToggleActiveStatusValidator : AbstractValidator<ToggleActiveStatusCommand>
{
    public ToggleActiveStatusValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
