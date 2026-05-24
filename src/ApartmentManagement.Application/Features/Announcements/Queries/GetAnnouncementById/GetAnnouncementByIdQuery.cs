using ApartmentManagement.Application.Common.Models;
using ApartmentManagement.Application.Common.DTOs;
using MediatR;

namespace ApartmentManagement.Application.Features.Announcements.Queries.GetAnnouncementById;

public record GetAnnouncementByIdQuery(Guid Id) : IRequest<Result<AnnouncementDetailDto>>;
