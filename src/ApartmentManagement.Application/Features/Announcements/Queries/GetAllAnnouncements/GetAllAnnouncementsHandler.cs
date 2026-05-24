using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Queries.GetAllAnnouncements;

public class GetAllAnnouncementsHandler : IRequestHandler<GetAllAnnouncementsQuery, Result<PagedResult<AnnouncementDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public GetAllAnnouncementsHandler(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider dateTime)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<PagedResult<AnnouncementDto>>> Handle(GetAllAnnouncementsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var pageNumber = Math.Max(1, request.PageNumber);
        var today = _dateTime.UtcNow.Date;
        var userId = _currentUser.UserId;

        var query = _db.Announcements.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Severity) &&
            Enum.TryParse<AnnouncementSeverity>(request.Severity, true, out var severity))
            query = query.Where(d => d.Severity == severity);

        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                query = query.Where(d => d.ExpiresAt == null || d.ExpiresAt >= today);
            else
                query = query.Where(d => d.ExpiresAt != null && d.ExpiresAt < today);
        }

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(d => d.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new AnnouncementDto
            {
                Id = d.Id,
                TenantId = d.TenantId,
                Title = d.Title,
                Content = d.Content,
                Severity = d.Severity.ToString(),
                PublishedAt = d.PublishedAt,
                ExpiresAt = d.ExpiresAt,
                Audience = d.Audience.ToString(),
                BuildingId = d.BuildingId,
                BuildingName = d.Building != null ? d.Building.Name : null,
                CreatedAt = d.CreatedAt,
                IsActive = d.ExpiresAt == null || d.ExpiresAt >= today,
                IsRead = userId != null && d.Reads.Any(o => o.UserId == userId)
            })
            .ToListAsync(ct);

        return Result<PagedResult<AnnouncementDto>>.Success(new PagedResult<AnnouncementDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total
        });
    }
}
