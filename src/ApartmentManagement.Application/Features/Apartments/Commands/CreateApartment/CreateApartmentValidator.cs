using FluentValidation;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartment;

public class CreateApartmentValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentValidator()
    {
        RuleFor(x => x.BuildingId).NotEmpty();
        RuleFor(x => x.ApartmentNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Floor).GreaterThanOrEqualTo(-5).LessThanOrEqualTo(200);
        RuleFor(x => x.GrossSquareMeters).GreaterThan(0).When(x => x.GrossSquareMeters.HasValue);
        RuleFor(x => x.NetSquareMeters).GreaterThan(0).When(x => x.NetSquareMeters.HasValue);
        RuleFor(x => x.OccupancyStatus).NotEmpty();
        RuleFor(x => x.DueMultiplier).GreaterThan(0).When(x => x.DueMultiplier.HasValue);
    }
}
