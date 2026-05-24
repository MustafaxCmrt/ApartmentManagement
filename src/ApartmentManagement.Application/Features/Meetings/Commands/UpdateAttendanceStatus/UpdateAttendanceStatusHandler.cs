using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateAttendanceStatus;

public class UpdateAttendanceStatusHandler : IRequestHandler<UpdateAttendanceStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateAttendanceStatusHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateAttendanceStatusCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<AttendanceStatus>(request.AttendanceStatus, true, out var status))
            return Result.Failure(Error.Validation("Invalid attendance status."));

        var k = await _db.MeetingParticipants.FirstOrDefaultAsync(x => x.Id == request.ParticipantId, ct);
        if (k is null)
            return Result.Failure(Error.NotFound("Participant"));

        k.AttendanceStatus = status;
        k.ProxyApartmentId = request.ProxyApartmentId;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
