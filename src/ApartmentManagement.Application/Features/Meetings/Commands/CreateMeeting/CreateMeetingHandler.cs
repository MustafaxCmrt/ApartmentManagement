using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;

namespace ApartmentManagement.Application.Features.Meetings.Commands.CreateMeeting;

public class CreateMeetingHandler : IRequestHandler<CreateMeetingCommand, Result<MeetingDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;

    public CreateMeetingHandler(IApplicationDbContext db, ICurrentTenantService currentTenant)
    {
        _db = db;
        _currentTenant = currentTenant;
    }

    public async Task<Result<MeetingDto>> Handle(CreateMeetingCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<MeetingDto>.Failure(Error.Unauthorized("Tenant is required."));

        var meeting = new Meeting
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title.Trim(),
            MeetingDate = request.MeetingDate,
            Venue = request.Venue.Trim(),
            Agenda = request.Agenda,
            Status = MeetingStatus.Scheduled
        };

        _db.Meetings.Add(meeting);
        await _db.SaveChangesAsync(ct);

        return Result<MeetingDto>.Success(new MeetingDto
        {
            Id = meeting.Id,
            TenantId = meeting.TenantId,
            Title = meeting.Title,
            MeetingDate = meeting.MeetingDate,
            Venue = meeting.Venue,
            Agenda = meeting.Agenda,
            Status = meeting.Status.ToString(),
            MinutesSummary = null,
            ParticipantCount = 0
        });
    }
}
