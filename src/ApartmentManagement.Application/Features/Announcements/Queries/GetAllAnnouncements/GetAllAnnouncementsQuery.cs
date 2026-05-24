using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Queries.GetAllAnnouncements;

public record GetAllAnnouncementsQuery(
    int PageNumber = 1,
    int PageSize = 20,
    string? Severity = null,
    bool? IsActive = null
) : IRequest<Result<PagedResult<AnnouncementDto>>>;
