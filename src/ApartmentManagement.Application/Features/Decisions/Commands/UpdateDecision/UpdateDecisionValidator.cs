using FluentValidation;

namespace ApartmentManagement.Application.Features.Decisions.Commands.UpdateDecision;

public class UpdateDecisionValidator : AbstractValidator<UpdateDecisionCommand>
{
    public UpdateDecisionValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.DecisionDate).NotEmpty();
        RuleFor(x => x.DecisionTitle).NotEmpty().MaximumLength(300);
        RuleFor(x => x.DecisionText).NotEmpty();
    }
}
