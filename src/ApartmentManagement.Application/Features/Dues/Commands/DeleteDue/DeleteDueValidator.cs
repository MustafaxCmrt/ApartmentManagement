using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Dues.Commands.DeleteDue;

public class DeleteDueValidator : AbstractValidator<DeleteDueCommand>
{
    public DeleteDueValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
