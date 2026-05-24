using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateBulkDues;

public class CreateBulkDuesValidator : AbstractValidator<CreateBulkDuesCommand>
{
    public CreateBulkDuesValidator()
    {
        RuleFor(x => x.Period).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.BaseAmount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive);
        RuleFor(x => x.DueDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(ValidationMessages.MaxLength);
        RuleFor(x => x.BuildingId).NotEqual(Guid.Empty)
            .WithMessage(ValidationMessages.GuidRequired)
            .When(x => x.BuildingId.HasValue);
    }
}
