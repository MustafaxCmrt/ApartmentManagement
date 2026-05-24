using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Decisions.Commands.CreateDecision;

public class CreateDecisionHandler : IRequestHandler<CreateDecisionCommand, Result<DecisionDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateDecisionHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<DecisionDto>> Handle(CreateDecisionCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<DecisionDto>.Failure(Error.Unauthorized("Tenant is required."));

        if (request.MeetingId.HasValue)
        {
            var meetingExists = await _db.Meetings.AnyAsync(t => t.Id == request.MeetingId.Value, ct);
            if (!meetingExists)
                return Result<DecisionDto>.Failure(Error.NotFound("Meeting"));
        }

        var nextNo = (await _db.Decisions
            .IgnoreQueryFilters()
            .Where(d => d.TenantId == tenantId)
            .MaxAsync(k => (int?)k.DecisionNumber, ct) ?? 0) + 1;

        var decision = new Decision
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MeetingId = request.MeetingId,
            DecisionNumber = nextNo,
            DecisionDate = request.DecisionDate,
            DecisionTitle = request.DecisionTitle.Trim(),
            DecisionText = request.DecisionText,
            VotersCount = request.VotersCount,
            ApprovalVotes = request.ApprovalVotes,
            RejectionVotes = request.RejectionVotes,
            AbstentionVotes = request.AbstentionVotes
        };

        _db.Decisions.Add(decision);
        await _db.SaveChangesAsync(ct);

        return Result<DecisionDto>.Success(new DecisionDto
        {
            Id = decision.Id,
            TenantId = decision.TenantId,
            MeetingId = decision.MeetingId,
            DecisionNumber = decision.DecisionNumber,
            DecisionDate = decision.DecisionDate,
            DecisionTitle = decision.DecisionTitle,
            DecisionText = decision.DecisionText,
            VotersCount = decision.VotersCount,
            ApprovalVotes = decision.ApprovalVotes,
            RejectionVotes = decision.RejectionVotes,
            AbstentionVotes = decision.AbstentionVotes
        });
    }
}
