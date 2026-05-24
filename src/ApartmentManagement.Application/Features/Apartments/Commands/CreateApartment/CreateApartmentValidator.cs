using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartment;

public class CreateApartmentValidator : AbstractValidator<CreateApartmentCommand>
{
    public CreateApartmentValidator()
    {
        RuleFor(x => x.BuildingId).RequiredGuid();
        RuleFor(x => x.ApartmentNumber).RequiredText(20);
        RuleFor(x => x.Floor).GreaterThanOrEqualTo(-5).WithMessage("{PropertyName} en az -5 olabilir.")
            .LessThanOrEqualTo(200).WithMessage("{PropertyName} en fazla 200 olabilir.");
        RuleFor(x => x.GrossSquareMeters).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .When(x => x.GrossSquareMeters.HasValue);
        RuleFor(x => x.NetSquareMeters).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .When(x => x.NetSquareMeters.HasValue);
        RuleFor(x => x.OccupancyStatus).NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(v => Enum.TryParse<OccupancyStatus>(v, true, out _)).WithMessage("Geçersiz doluluk durumu.");
        RuleFor(x => x.DueMultiplier).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
            .When(x => x.DueMultiplier.HasValue);
    }
}
