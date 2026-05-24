using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMeeting;

public class UpdateMeetingHandler : IRequestHandler<UpdateMeetingCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateMeetingHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateMeetingCommand request, CancellationToken ct)
    {
        var meeting = await _db.Meetings.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (meeting is null)
            return Result.Failure(Error.NotFound("Meeting"));

        if (!Enum.TryParse<MeetingStatus>(request.Status, true, out var status))
            return Result.Failure(Error.Validation("Invalid status."));

        meeting.Title = request.Title.Trim();
        meeting.MeetingDate = request.MeetingDate;
        meeting.Venue = request.Venue.Trim();
        meeting.Agenda = request.Agenda;
        meeting.Status = status;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
