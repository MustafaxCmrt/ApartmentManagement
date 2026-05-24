using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.DeleteMeeting;

public class DeleteMeetingHandler : IRequestHandler<DeleteMeetingCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteMeetingHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteMeetingCommand request, CancellationToken ct)
    {
        var meeting = await _db.Meetings.FirstOrDefaultAsync(t => t.Id == request.Id, ct);
        if (meeting is null)
            return Result.Failure(Error.NotFound("Meeting"));

        _db.Meetings.Remove(meeting);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
