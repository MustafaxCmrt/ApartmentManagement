using FluentValidation;

namespace ApartmentManagement.Application.Features.Buildings.Commands.UpdateBuilding;

public class UpdateBuildingValidator : AbstractValidator<UpdateBuildingCommand>
{
    public UpdateBuildingValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.FloorCount).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.ApartmentCount).GreaterThan(0).LessThanOrEqualTo(1000);
        RuleFor(x => x.ConstructionYear).InclusiveBetween(1800, 2100).When(x => x.ConstructionYear.HasValue);
    }
}
