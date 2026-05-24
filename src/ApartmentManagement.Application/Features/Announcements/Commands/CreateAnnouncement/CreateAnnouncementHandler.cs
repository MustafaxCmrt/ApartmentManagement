using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Application.Features.Announcements.Commands.CreateAnnouncement;

public class CreateAnnouncementHandler : IRequestHandler<CreateAnnouncementCommand, Result<AnnouncementDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenantService _currentTenant;
    private readonly IDateTimeProvider _dateTime;

    public CreateAnnouncementHandler(
        IApplicationDbContext db,
        ICurrentTenantService currentTenant,
        IDateTimeProvider dateTime)
    {
        _db = db;
        _currentTenant = currentTenant;
        _dateTime = dateTime;
    }

    public async Task<Result<AnnouncementDto>> Handle(CreateAnnouncementCommand request, CancellationToken ct)
    {
        if (_currentTenant.TenantId is not { } tenantId)
            return Result<AnnouncementDto>.Failure(Error.Unauthorized("Tenant is required."));

        if (!Enum.TryParse<AnnouncementSeverity>(request.Severity, true, out var severity))
            return Result<AnnouncementDto>.Failure(Error.Validation("Invalid severity."));

        if (!Enum.TryParse<AnnouncementAudience>(request.Audience, true, out var audience))
            return Result<AnnouncementDto>.Failure(Error.Validation("Invalid audience."));

        if (request.BuildingId.HasValue)
        {
            var buildingExists = await _db.Buildings.AnyAsync(b => b.Id == request.BuildingId.Value, ct);
            if (!buildingExists) return Result<AnnouncementDto>.Failure(Error.NotFound("Building"));
        }

        var announcement = new Announcement
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Title = request.Title.Trim(),
            Content = request.Content,
            Severity = severity,
            PublishedAt = request.PublishedAt,
            ExpiresAt = request.ExpiresAt,
            Audience = audience,
            BuildingId = request.BuildingId
        };

        _db.Announcements.Add(announcement);
        await _db.SaveChangesAsync(ct);

        var today = _dateTime.UtcNow.Date;

        return Result<AnnouncementDto>.Success(new AnnouncementDto
        {
            Id = announcement.Id,
            TenantId = announcement.TenantId,
            Title = announcement.Title,
            Content = announcement.Content,
            Severity = announcement.Severity.ToString(),
            PublishedAt = announcement.PublishedAt,
            ExpiresAt = announcement.ExpiresAt,
            Audience = announcement.Audience.ToString(),
            BuildingId = announcement.BuildingId,
            CreatedAt = announcement.CreatedAt,
            IsActive = announcement.ExpiresAt == null || announcement.ExpiresAt >= today,
            IsRead = false
        });
    }
}
