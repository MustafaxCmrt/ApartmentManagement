using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Decisions.Queries.GetDecisionById;

public class GetDecisionByIdHandler : IRequestHandler<GetDecisionByIdQuery, Result<DecisionDto>>
{
    private readonly IApplicationDbContext _db;

    public GetDecisionByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DecisionDto>> Handle(GetDecisionByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Decisions.AsNoTracking()
            .Where(k => k.Id == request.Id)
            .Select(k => new DecisionDto
            {
                Id = k.Id,
                TenantId = k.TenantId,
                MeetingId = k.MeetingId,
                MeetingTitle = k.Meeting != null ? k.Meeting.Title : null,
                DecisionNumber = k.DecisionNumber,
                DecisionDate = k.DecisionDate,
                DecisionTitle = k.DecisionTitle,
                DecisionText = k.DecisionText,
                VotersCount = k.VotersCount,
                ApprovalVotes = k.ApprovalVotes,
                RejectionVotes = k.RejectionVotes,
                AbstentionVotes = k.AbstentionVotes
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<DecisionDto>.Failure(Error.NotFound("Decision"));

        return Result<DecisionDto>.Success(dto);
    }
}
