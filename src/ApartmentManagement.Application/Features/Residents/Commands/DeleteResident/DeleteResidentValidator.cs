using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResident;

public class DeleteResidentValidator : AbstractValidator<DeleteResidentCommand>
{
    public DeleteResidentValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
