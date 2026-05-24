using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Decisions.Commands.CreateDecision;

public record CreateDecisionCommand(
    Guid? MeetingId,
    DateTime DecisionDate,
    string DecisionTitle,
    string DecisionText,
    int? VotersCount,
    int? ApprovalVotes,
    int? RejectionVotes,
    int? AbstentionVotes
) : IRequest<Result<DecisionDto>>;
