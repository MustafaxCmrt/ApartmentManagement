using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.CreateBulkDues;

public class CreateBulkDuesValidator : AbstractValidator<CreateBulkDuesCommand>
{
    public CreateBulkDuesValidator()
    {
        RuleFor(x => x.Period).NotEmpty();
        RuleFor(x => x.BaseAmount).GreaterThan(0);
        RuleFor(x => x.DueDate).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
