using FluentValidation;

namespace ApartmentManagement.Application.Features.Decisions.Commands.CreateDecision;

public class CreateDecisionValidator : AbstractValidator<CreateDecisionCommand>
{
    public CreateDecisionValidator()
    {
        RuleFor(x => x.DecisionDate).NotEmpty();
        RuleFor(x => x.DecisionTitle).NotEmpty().MaximumLength(300);
        RuleFor(x => x.DecisionText).NotEmpty();
        RuleFor(x => x.VotersCount).GreaterThanOrEqualTo(0).When(x => x.VotersCount.HasValue);
        RuleFor(x => x.ApprovalVotes).GreaterThanOrEqualTo(0).When(x => x.ApprovalVotes.HasValue);
        RuleFor(x => x.RejectionVotes).GreaterThanOrEqualTo(0).When(x => x.RejectionVotes.HasValue);
        RuleFor(x => x.AbstentionVotes).GreaterThanOrEqualTo(0).When(x => x.AbstentionVotes.HasValue);
    }
}
