using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuilding;

public class DeleteBuildingValidator : AbstractValidator<DeleteBuildingCommand>
{
    public DeleteBuildingValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
