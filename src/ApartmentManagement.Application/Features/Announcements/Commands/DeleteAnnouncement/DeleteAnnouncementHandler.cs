using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Commands.DeleteAnnouncement;

public class DeleteAnnouncementHandler : IRequestHandler<DeleteAnnouncementCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public DeleteAnnouncementHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(DeleteAnnouncementCommand request, CancellationToken ct)
    {
        var announcement = await _db.Announcements.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
        if (announcement is null)
            return Result.Failure(Error.NotFound("Announcement"));

        _db.Announcements.Remove(announcement);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
