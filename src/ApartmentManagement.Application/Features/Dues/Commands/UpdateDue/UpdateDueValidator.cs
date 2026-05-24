using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.UpdateDue;

public class UpdateDueValidator : AbstractValidator<UpdateDueCommand>
{
    public UpdateDueValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage(ValidationMessages.AmountPositive);
        RuleFor(x => x.DueDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(ValidationMessages.MaxLength);
    }
}
