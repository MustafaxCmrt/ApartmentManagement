using ApartmentManagement.Application.Common.Models;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Commands.UpdateAnnouncement;

public record UpdateAnnouncementCommand(
    Guid Id,
    string Title,
    string Content,
    string Severity,
    DateTime PublishedAt,
    DateTime? ExpiresAt,
    string Audience,
    Guid? BuildingId
) : IRequest<Result>;
