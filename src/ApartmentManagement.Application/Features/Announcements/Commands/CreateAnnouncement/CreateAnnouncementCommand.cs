using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Commands.CreateAnnouncement;

public record CreateAnnouncementCommand(
    string Title,
    string Content,
    string Severity,
    DateTime PublishedAt,
    DateTime? ExpiresAt,
    string Audience,
    Guid? BuildingId
) : IRequest<Result<AnnouncementDto>>;
