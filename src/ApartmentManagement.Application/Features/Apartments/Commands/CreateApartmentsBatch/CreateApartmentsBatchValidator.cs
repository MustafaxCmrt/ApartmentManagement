using FluentValidation;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartmentsBatch;

public class CreateApartmentsBatchValidator : AbstractValidator<CreateApartmentsBatchCommand>
{
    public CreateApartmentsBatchValidator()
    {
        RuleFor(x => x.BuildingId).NotEmpty();
        RuleFor(x => x.Apartments).NotEmpty().WithMessage("At least one apartment is required.");
        RuleForEach(x => x.Apartments).ChildRules(item =>
        {
            item.RuleFor(d => d.ApartmentNumber).NotEmpty().MaximumLength(20);
            item.RuleFor(d => d.Floor).GreaterThanOrEqualTo(-5).LessThanOrEqualTo(200);
            item.RuleFor(d => d.OccupancyStatus).NotEmpty();
            item.RuleFor(d => d.GrossSquareMeters).GreaterThan(0).When(d => d.GrossSquareMeters.HasValue);
            item.RuleFor(d => d.NetSquareMeters).GreaterThan(0).When(d => d.NetSquareMeters.HasValue);
            item.RuleFor(d => d.DueMultiplier).GreaterThan(0).When(d => d.DueMultiplier.HasValue);
        });
    }
}
