using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Queries.GetAllMeetings;

public class GetAllMeetingsHandler : IRequestHandler<GetAllMeetingsQuery, Result<PagedResult<MeetingDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllMeetingsHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PagedResult<MeetingDto>>> Handle(GetAllMeetingsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Meetings.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<MeetingStatus>(request.Status, true, out var status))
            query = query.Where(t => t.Status == status);

        if (request.Year.HasValue)
            query = query.Where(t => t.MeetingDate.Year == request.Year.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(t => t.MeetingDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new MeetingDto
            {
                Id = t.Id,
                TenantId = t.TenantId,
                Title = t.Title,
                MeetingDate = t.MeetingDate,
                Venue = t.Venue,
                Agenda = t.Agenda,
                Status = t.Status.ToString(),
                MinutesSummary = t.MinutesSummary,
                ParticipantCount = t.Participants.Count()
            })
            .ToListAsync(ct);

        return Result<PagedResult<MeetingDto>>.Success(new PagedResult<MeetingDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
