using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Queries.GetAnnouncementById;

public class GetAnnouncementByIdHandler : IRequestHandler<GetAnnouncementByIdQuery, Result<AnnouncementDetailDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public GetAnnouncementByIdHandler(IApplicationDbContext db, ICurrentUserService currentUser, IDateTimeProvider dateTime)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<Result<AnnouncementDetailDto>> Handle(GetAnnouncementByIdQuery request, CancellationToken ct)
    {
        var today = _dateTime.UtcNow.Date;
        var userId = _currentUser.UserId;

        var dto = await _db.Announcements.AsNoTracking()
            .Where(d => d.Id == request.Id)
            .Select(d => new AnnouncementDetailDto
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
                IsRead = userId != null && d.Reads.Any(o => o.UserId == userId),
                TotalReads = d.Reads.Count()
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            return Result<AnnouncementDetailDto>.Failure(Error.NotFound("Announcement"));

        return Result<AnnouncementDetailDto>.Success(dto);
    }
}
