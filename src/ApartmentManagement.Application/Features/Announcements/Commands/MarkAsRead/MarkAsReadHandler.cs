using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Commands.MarkAsRead;

public class MarkAsReadHandler : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantService _currentTenant;
    private readonly IDateTimeProvider _dateTime;

    public MarkAsReadHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        ICurrentTenantService currentTenant,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId || _currentTenant.TenantId is not { } tenantId)
            return Result.Failure(Error.Unauthorized());

        var announcement = await _db.Announcements.FirstOrDefaultAsync(d => d.Id == request.AnnouncementId, ct);
        if (announcement is null)
            return Result.Failure(Error.NotFound("Announcement"));

        var existing = await _db.AnnouncementReads
            .AnyAsync(o => o.AnnouncementId == request.AnnouncementId && o.UserId == userId, ct);

        if (existing)
            return Result.Success();

        _db.AnnouncementReads.Add(new AnnouncementRead
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AnnouncementId = request.AnnouncementId,
            UserId = userId,
            ReadAt = _dateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
