using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Buildings.Commands.UpdateBuilding;

public class UpdateBuildingValidator : AbstractValidator<UpdateBuildingCommand>
{
    public UpdateBuildingValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Name).RequiredText(100);
        RuleFor(x => x.Address).RequiredText(500);
        RuleFor(x => x.FloorCount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .LessThanOrEqualTo(100).WithMessage("{PropertyName} en fazla 100 olabilir.");
        RuleFor(x => x.ApartmentCount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .LessThanOrEqualTo(1000).WithMessage("{PropertyName} en fazla 1000 olabilir.");
        RuleFor(x => x.ConstructionYear).InclusiveBetween(1800, 2100)
            .WithMessage("{PropertyName} 1800 ile 2100 arasında olmalıdır.")
            .When(x => x.ConstructionYear.HasValue);
    }
}
