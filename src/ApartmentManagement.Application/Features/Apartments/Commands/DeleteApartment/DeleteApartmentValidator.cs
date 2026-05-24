using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Apartments.Commands.DeleteApartment;

public class DeleteApartmentValidator : AbstractValidator<DeleteApartmentCommand>
{
    public DeleteApartmentValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
