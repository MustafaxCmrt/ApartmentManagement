using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Meetings.Commands.UpdateMinutes;

public class UpdateMinutesHandler : IRequestHandler<UpdateMinutesCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateMinutesHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateMinutesCommand request, CancellationToken ct)
    {
        var t = await _db.Meetings.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (t is null)
            return Result.Failure(Error.NotFound("Meeting"));

        t.MinutesSummary = request.MinutesSummary;
        if (t.Status == MeetingStatus.Scheduled)
            t.Status = MeetingStatus.Held;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
