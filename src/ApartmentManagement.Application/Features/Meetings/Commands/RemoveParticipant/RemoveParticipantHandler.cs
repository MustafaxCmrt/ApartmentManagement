using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.RemoveParticipant;

public class RemoveParticipantHandler : IRequestHandler<RemoveParticipantCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public RemoveParticipantHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RemoveParticipantCommand request, CancellationToken ct)
    {
        var k = await _db.MeetingParticipants.FirstOrDefaultAsync(x => x.Id == request.ParticipantId, ct);
        if (k is null)
            return Result.Failure(Error.NotFound("Participant"));

        _db.MeetingParticipants.Remove(k);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
