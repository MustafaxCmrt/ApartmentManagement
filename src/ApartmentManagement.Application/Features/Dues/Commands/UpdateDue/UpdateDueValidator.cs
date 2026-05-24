using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.UpdateDue;

public class UpdateDueValidator : AbstractValidator<UpdateDueCommand>
{
    public UpdateDueValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
