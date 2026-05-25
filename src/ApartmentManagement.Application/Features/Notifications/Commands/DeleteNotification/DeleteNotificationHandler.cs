using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Notifications.Commands.DeleteNotification;

public class DeleteNotificationHandler : IRequestHandler<DeleteNotificationCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteNotificationHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result.Failure(Error.Unauthorized());

        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == userId, ct);

        if (notification is null)
            return Result.Failure(Error.NotFound("Notification"));

        _db.Notifications.Remove(notification);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
