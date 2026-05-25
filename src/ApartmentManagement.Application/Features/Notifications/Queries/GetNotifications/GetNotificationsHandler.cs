using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationsHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is not { } userId)
            return Result<PagedResult<NotificationDto>>.Failure(Error.Unauthorized());

        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);

        var query = _db.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId);

        if (request.IsRead.HasValue)
            query = query.Where(n => n.IsRead == request.IsRead.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                TenantId = n.TenantId,
                UserId = n.UserId,
                Type = n.Type.ToString(),
                Title = n.Title,
                Message = n.Message,
                Link = n.Link,
                IsRead = n.IsRead,
                ReadAt = n.ReadAt,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(ct);

        return Result<PagedResult<NotificationDto>>.Success(new PagedResult<NotificationDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
