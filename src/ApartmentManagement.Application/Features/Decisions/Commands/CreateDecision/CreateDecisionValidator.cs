using ApartmentManagement.Application.Common.Validation;
using FluentValidation;

namespace ApartmentManagement.Application.Features.Decisions.Commands.CreateDecision;

public class CreateDecisionValidator : AbstractValidator<CreateDecisionCommand>
{
    public CreateDecisionValidator()
    {
        RuleFor(x => x.MeetingId).NotEqual(Guid.Empty)
            .WithMessage(ValidationMessages.GuidRequired)
            .When(x => x.MeetingId.HasValue);
        RuleFor(x => x.DecisionDate).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.DecisionTitle).RequiredText(300);
        RuleFor(x => x.DecisionText).NotEmpty().WithMessage(ValidationMessages.Required);
        RuleFor(x => x.VotersCount).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.")
            .When(x => x.VotersCount.HasValue);
        RuleFor(x => x.ApprovalVotes).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.")
            .When(x => x.ApprovalVotes.HasValue);
        RuleFor(x => x.RejectionVotes).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.")
            .When(x => x.RejectionVotes.HasValue);
        RuleFor(x => x.AbstentionVotes).GreaterThanOrEqualTo(0).WithMessage("{PropertyName} negatif olamaz.")
            .When(x => x.AbstentionVotes.HasValue);

        RuleFor(x => x).Must(x =>
                (x.ApprovalVotes ?? 0) + (x.RejectionVotes ?? 0) + (x.AbstentionVotes ?? 0) <= x.VotersCount!.Value)
            .WithMessage("Kabul, Ret ve Çekimser oyların toplamı, Toplam oy sayısından fazla olamaz.")
            .When(x => x.VotersCount.HasValue);
    }
}
