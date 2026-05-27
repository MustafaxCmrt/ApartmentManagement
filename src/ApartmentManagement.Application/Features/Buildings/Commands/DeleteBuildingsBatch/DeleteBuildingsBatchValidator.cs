using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Buildings.Commands.DeleteBuildingsBatch;

public class DeleteBuildingsBatchValidator : AbstractValidator<DeleteBuildingsBatchCommand>
{
    public DeleteBuildingsBatchValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(ids => ids.Count <= 100).WithMessage("En fazla 100 bina aynı anda silinebilir.");

        RuleForEach(x => x.Ids).RequiredGuid();
    }
}
