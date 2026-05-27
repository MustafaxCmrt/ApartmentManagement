using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Residents.Commands.DeleteResidentsBatch;

public class DeleteResidentsBatchValidator : AbstractValidator<DeleteResidentsBatchCommand>
{
    public DeleteResidentsBatchValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage(ValidationMessages.Required)
            .Must(ids => ids.Count <= 100).WithMessage("En fazla 100 sakin aynı anda silinebilir.");

        RuleForEach(x => x.Ids).RequiredGuid();
    }
}
