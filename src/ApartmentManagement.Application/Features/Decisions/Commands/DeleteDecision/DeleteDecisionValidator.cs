using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Decisions.Commands.DeleteDecision;

public class DeleteDecisionValidator : AbstractValidator<DeleteDecisionCommand>
{
    public DeleteDecisionValidator()
    {
        RuleFor(x => x.Id).RequiredGuid();
    }
}
