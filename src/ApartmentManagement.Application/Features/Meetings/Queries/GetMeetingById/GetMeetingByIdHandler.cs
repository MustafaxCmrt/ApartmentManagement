using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Queries.GetMeetingById;

public class GetMeetingByIdHandler : IRequestHandler<GetMeetingByIdQuery, Result<MeetingDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMeetingByIdHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<MeetingDetailDto>> Handle(GetMeetingByIdQuery request, CancellationToken ct)
    {
        var dto = await _db.Meetings.AsNoTracking()
            .Where(t => t.Id == request.Id)
            .Select(t => new MeetingDetailDto
            {
                Id = t.Id,
                TenantId = t.TenantId,
                Title = t.Title,
                MeetingDate = t.MeetingDate,
                Venue = t.Venue,
                Agenda = t.Agenda,
                Status = t.Status.ToString(),
                MinutesSummary = t.MinutesSummary,
                ParticipantCount = t.Participants.Count(),
                Participants = t.Participants
                    .Select(k => new ParticipantDto
                    {
                        Id = k.Id,
                        MeetingId = k.MeetingId,
                        ApartmentId = k.ApartmentId,
                        ApartmentNumber = _db.Apartments.Where(d => d.Id == k.ApartmentId).Select(d => d.ApartmentNumber).FirstOrDefault(),
                        AttendanceStatus = k.AttendanceStatus.ToString(),
                        ProxyApartmentId = k.ProxyApartmentId,
                        ProxyApartmentNumber = k.ProxyApartmentId.HasValue
                            ? _db.Apartments.Where(d => d.Id == k.ProxyApartmentId.Value).Select(d => d.ApartmentNumber).FirstOrDefault()
                            : null
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<MeetingDetailDto>.Failure(Error.NotFound("Meeting"));

        return Result<MeetingDetailDto>.Success(dto);
    }
}
