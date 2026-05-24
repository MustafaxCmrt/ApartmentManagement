using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Commands.UpdateAnnouncement;

public class UpdateAnnouncementHandler : IRequestHandler<UpdateAnnouncementCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateAnnouncementHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateAnnouncementCommand request, CancellationToken ct)
    {
        var announcement = await _db.Announcements.FirstOrDefaultAsync(d => d.Id == request.Id, ct);
        if (announcement is null)
            return Result.Failure(Error.NotFound("Announcement"));

        if (!Enum.TryParse<AnnouncementSeverity>(request.Severity, true, out var severity))
            return Result.Failure(Error.Validation("Invalid severity."));

        if (!Enum.TryParse<AnnouncementAudience>(request.Audience, true, out var audience))
            return Result.Failure(Error.Validation("Invalid audience."));

        announcement.Title = request.Title.Trim();
        announcement.Content = request.Content;
        announcement.Severity = severity;
        announcement.PublishedAt = request.PublishedAt;
        announcement.ExpiresAt = request.ExpiresAt;
        announcement.Audience = audience;
        announcement.BuildingId = request.BuildingId;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
