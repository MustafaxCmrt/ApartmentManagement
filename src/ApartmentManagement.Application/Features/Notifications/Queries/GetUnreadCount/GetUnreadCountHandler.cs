using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, Result<UnreadCountDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<UnreadCountDto>> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<UnreadCountDto>.Failure(Error.Unauthorized());

        var count = await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

        return Result<UnreadCountDto>.Success(new UnreadCountDto { Count = count });
    }
}
