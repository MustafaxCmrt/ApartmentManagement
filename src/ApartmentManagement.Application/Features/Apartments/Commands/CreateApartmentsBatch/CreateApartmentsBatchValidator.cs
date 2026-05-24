using ApartmentManagement.Application.Common.Validation;
using ApartmentManagement.Domain.Enums;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Apartments.Commands.CreateApartmentsBatch;

public class CreateApartmentsBatchValidator : AbstractValidator<CreateApartmentsBatchCommand>
{
    public CreateApartmentsBatchValidator()
    {
        RuleFor(x => x.BuildingId).RequiredGuid();
        RuleFor(x => x.Apartments).NotEmpty().WithMessage("En az bir daire girilmelidir.");
        RuleForEach(x => x.Apartments).ChildRules(item =>
        {
            item.RuleFor(d => d.ApartmentNumber).RequiredText(20);
            item.RuleFor(d => d.Floor).GreaterThanOrEqualTo(-5).WithMessage("{PropertyName} en az -5 olabilir.")
                .LessThanOrEqualTo(200).WithMessage("{PropertyName} en fazla 200 olabilir.");
            item.RuleFor(d => d.OccupancyStatus).NotEmpty().WithMessage(ValidationMessages.Required)
                .Must(v => Enum.TryParse<OccupancyStatus>(v, true, out _)).WithMessage("Geçersiz doluluk durumu.");
            item.RuleFor(d => d.GrossSquareMeters).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
                .When(d => d.GrossSquareMeters.HasValue);
            item.RuleFor(d => d.NetSquareMeters).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
                .When(d => d.NetSquareMeters.HasValue);
            item.RuleFor(d => d.DueMultiplier).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive)
                .When(d => d.DueMultiplier.HasValue);
        });
    }
}
