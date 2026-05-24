using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateDue;

public class CreateDueValidator : AbstractValidator<CreateDueCommand>
{
    public CreateDueValidator()
    {
        RuleFor(x => x.ApartmentId).RequiredGuid();
        RuleFor(x => x.Period).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive);
        RuleFor(x => x.DueDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(ValidationMessages.MaxLength);
    }
}
