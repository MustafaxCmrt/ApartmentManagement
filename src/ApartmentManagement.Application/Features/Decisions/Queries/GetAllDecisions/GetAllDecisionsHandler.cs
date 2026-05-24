using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Decisions.Queries.GetAllDecisions;

public class GetAllDecisionsHandler : IRequestHandler<GetAllDecisionsQuery, Result<PagedResult<DecisionDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllDecisionsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<DecisionDto>>> Handle(GetAllDecisionsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Decisions.AsNoTracking();

        if (request.Year.HasValue)
            query = query.Where(k => k.DecisionDate.Year == request.Year.Value);

        if (request.MeetingId.HasValue)
            query = query.Where(k => k.MeetingId == request.MeetingId.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(k => k.DecisionNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync(ct);

        return Result<PagedResult<DecisionDto>>.Success(new PagedResult<DecisionDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
