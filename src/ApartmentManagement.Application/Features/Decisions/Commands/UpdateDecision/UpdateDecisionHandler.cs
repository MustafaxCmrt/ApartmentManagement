using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Decisions.Commands.UpdateDecision;

public class UpdateDecisionHandler : IRequestHandler<UpdateDecisionCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateDecisionHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateDecisionCommand request, CancellationToken ct)
    {
        var decision = await _db.Decisions.FirstOrDefaultAsync(k => k.Id == request.Id, ct);
        if (decision is null)
            return Result.Failure(Error.NotFound("Decision"));

        decision.DecisionDate = request.DecisionDate;
        decision.DecisionTitle = request.DecisionTitle.Trim();
        decision.DecisionText = request.DecisionText;
        decision.VotersCount = request.VotersCount;
        decision.ApprovalVotes = request.ApprovalVotes;
        decision.RejectionVotes = request.RejectionVotes;
        decision.AbstentionVotes = request.AbstentionVotes;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
