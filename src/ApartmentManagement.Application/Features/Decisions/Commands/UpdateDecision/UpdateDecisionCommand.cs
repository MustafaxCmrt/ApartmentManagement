using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Decisions.Commands.UpdateDecision;

public record UpdateDecisionCommand(
    Guid Id,
    DateTime DecisionDate,
    string DecisionTitle,
    string DecisionText,
    int? VotersCount,
    int? ApprovalVotes,
    int? RejectionVotes,
    int? AbstentionVotes
) : IRequest<Result>;
