using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateDue;

public class CreateDueValidator : AbstractValidator<CreateDueCommand>
{
    public CreateDueValidator()
    {
        RuleFor(x => x.ApartmentId).NotEmpty();
        RuleFor(x => x.Period).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
