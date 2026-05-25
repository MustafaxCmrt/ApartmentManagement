using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkAsReadHandler : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public MarkAsReadHandler(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider dateTime)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(Error.Unauthorized());

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == userId, ct);

        if (notification is null)
            return Result.Failure(Error.NotFound("Notification"));

        notification.IsRead = true;
        notification.ReadAt = _dateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
