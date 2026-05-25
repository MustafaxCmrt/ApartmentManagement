using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Notifications.Commands.MarkAllAsRead;

public class MarkAllAsReadHandler : IRequestHandler<MarkAllAsReadCommand, Result<MarkAllAsReadResultDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public MarkAllAsReadHandler(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider dateTime)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<MarkAllAsReadResultDto>> Handle(MarkAllAsReadCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<MarkAllAsReadResultDto>.Failure(Error.Unauthorized());

        var now = _dateTime.UtcNow;

        var markedCount = await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.IsRead, true)
                .SetProperty(n => n.ReadAt, now), ct);

        return Result<MarkAllAsReadResultDto>.Success(new MarkAllAsReadResultDto { MarkedCount = markedCount });
    }
}
